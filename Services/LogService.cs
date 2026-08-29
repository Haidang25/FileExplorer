using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileExplorerApp.Models;
using FileExplorerApp.Properties;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop xu ly ghi/doc nhat ky (log) cac thao tac file/thu muc,
    /// dua tren <see cref="LogEntryModel"/>. Cac phuong thuc hien tai chi la
    /// khai bao (signature) + TODO, can trien khai logic thuc te ben trong.
    /// </summary>
    /// <remarks>
    /// Cau truc LogEntryModel da duoc thiet ke lai de dung chung FileOperationType/
    /// OperationResult voi phan con lai cua ung dung (xem ghi chu chi tiet tai
    /// LogEntryModel) - LogService chi con can trien khai phan luu tru/doc lai.
    ///
    /// QUYET DINH THIET KE: dinh dang luu la CSV (LogFileExtension = ".csv"), MOT
    /// dong = MOT LogEntryModel, dong dau la header ten cot. Da can nhac 3 lua
    /// chon va chon CSV vi ly do sau:
    ///
    /// Lua chon 1: Text tu do (dang LogEntryModel.ToString() cu, VD "[2026-08-21 14:30:00]
    ///    Copy | Nguon: ... | ..."): DE VIET (chi can WriteLine) nhung RAT KHO
    ///    doc lai co cau truc - GetLogsByResult/GetLogsByOperation/GetLogs(from,to)
    ///    se phai parse bang regex/tach chuoi mong manh, de vo neu Message chua
    ///    ky tu "|" hoac dinh dang thay doi nho. Loai vi ung dung CAN doc/loc lai
    ///    (khong chi de nguoi xem), khong chi de ghi mot chieu.
    ///
    /// Lua chon 2: JSON (mot mang lon hoac JSON Lines - moi dong mot object JSON): cau
    ///    truc day du nhat, de doi tuong hoa (Duration/Message long nhau ro
    ///    rang), nhung co 2 bat loi cho du an nay: (a) .NET Framework 4.7.2
    ///    KHONG co san System.Text.Json (chi co tu .NET Core 3.0+/duoc dong goi
    ///    rieng tu .NET Standard 2.0 ban System.Text.Json qua NuGet) - se phai
    ///    them mot NuGet package (System.Text.Json hoac Newtonsoft.Json) CHI DE
    ///    ghi log, trong khi CSV khong can them dependency nao (String.Join/
    ///    String.Split cua BCL la du, du lieu log deu la kieu don gian: string/
    ///    enum/DateTime/int/TimeSpan?, khong co object long nhau); (b) neu dung
    ///    dang MOT MANG JSON duy nhat, moi lan ghi them 1 dong phai doc lai TOAN
    ///    BO file, chen phan tu, ghi lai TOAN BO file (khong append-friendly),
    ///    cang cham khi file log cang lon theo thoi gian - trong khi CSV chi can
    ///    File.AppendAllText/AppendText mot dong moi, khong dong den phan con lai
    ///    cua file. (JSON Lines tranh duoc van de (b) nhung van con van de (a),
    ///    va la dinh dang it quen thuoc hon voi nguoi dung cuoi neu ho muon tu mo
    ///    file len xem/loc thu cong.)
    ///
    /// Lua chon 3: CSV (DA CHON): append-friendly (chi ghi them 1 dong moi lan, khong doc
    ///    lai file), khong can NuGet package (String.Join/String.Split la du vi
    ///    du lieu log don gian, khong long nhau), VA nguoi dung cuoi co the tu mo
    ///    file .csv bang Excel/Google Sheets de xem/loc/sap xep thu cong ma
    ///    khong can cong cu gi khac - phu hop voi mot ung dung quan ly file de
    ///    ban, noi nguoi dung co the muon tu kiem tra lich su thao tac ngoai
    ///    ung dung. Danh doi duy nhat la phai tu escape/un-escape dau phay va
    ///    xuong dong trong Message (RFC 4180: bao Message trong dau ngoac kep
    ///    "..." va nhan doi dau ngoac kep ben trong neu co) - chap nhan duoc vi
    ///    chi anh huong 1 truong duy nhat (Message), khong anh huong cau truc
    ///    tong the.
    ///
    /// Cau truc dong CSV (theo dung thu tu cot header, escape Message theo RFC
    /// 4180 nhu tren, cac truong con lai khong can escape vi khong the chua dau
    /// phay/xuong dong theo ban chat kieu du lieu cua chung):
    ///   Id,Timestamp,Operation,Source,Destination,Result,Message,ItemCount,Duration
    /// - Id: Guid.ToString() (VD: "3fa85f64-5717-4562-b3fc-2c963f66afa6").
    /// - Timestamp: dinh dang "o" (round-trip ISO 8601, VD "2026-08-21T14:30:00.0000000")
    ///   de Timestamp.Parse/ParseExact khong bi anh huong boi CultureInfo cua may
    ///   nguoi dung (khac voi dinh dang hien thi "dd/MM/yyyy HH:mm" cua FormatHelper
    ///   - do la dinh dang DE HIEN THI, khac muc dich voi dinh dang DE LUU TRU).
    /// - Operation/Result: ten enum dang chuoi (VD "Copy", "Success") - doc lai
    ///   bang Enum.Parse&lt;FileOperationType&gt;/Enum.Parse&lt;OperationResult&gt;.
    /// - Source/Destination: nguyen duong dan (co the rong voi Destination).
    /// - Message: co the rong; escape RFC 4180 khi ghi, un-escape khi doc.
    /// - ItemCount: so nguyen dang chuoi (VD "1", "20").
    /// - Duration: tong so giay dang so thap phan bat dong (VD "0.42"), de RONG
    ///   (khong phai "0") neu Duration null - phan biet "khong do duoc thoi gian"
    ///   voi "do duoc va bang 0 giay".
    ///
    /// QUYET DINH THIET KE THU HAI: file log dat trong AppData cua nguoi dung
    /// (%AppData%\SFileManager\logs\log.csv, lay tu Settings.Default.LogPath -
    /// da co san setting nay, xem Properties/Settings.settings), KHONG dat trong
    /// thu muc cai dat ung dung (VD thu muc chua FileExplorerApp.exe). Ly do:
    ///
    /// - Quyen ghi: thu muc cai dat mac dinh tren Windows (VD "C:\Program Files\...")
    ///   YEU CAU quyen Administrator de ghi doi voi nguoi dung thuong (UAC/
    ///   Windows Resource Protection) - neu dat file log o do, LogService se
    ///   thuong xuyen gap UnauthorizedAccessException khi ghi tren may that cua
    ///   nguoi dung cuoi (khac voi may dev dang chay bang quyen cao). AppData
    ///   (%AppData% = C:\Users\{ten}\AppData\Roaming) LUON ghi duoc boi chinh
    ///   nguoi dung dang dang nhap, khong can quyen dac biet nao.
    ///
    /// - Nhieu nguoi dung cung may: neu nhieu tai khoan Windows cung dung chung
    ///   mot ban cai dat ung dung (thu muc cai dat dung chung), ghi log vao thu
    ///   muc cai dat se tron lich su thao tac cua TAT CA nguoi dung vao 1 file,
    ///   hoac gay xung dot ghi dong thoi. AppData la thu muc RIENG cho tung tai
    ///   khoan Windows, nen moi nguoi dung tu nhien co lich su log RIENG cua minh
    ///   ma khong can LogService tu quan ly phan quyen/tach biet gi them.
    ///
    /// - Go cai dat/cap nhat phien ban: trinh go cai dat (uninstaller) thuong xoa
    ///   sach thu muc cai dat - neu log nam trong do, lich su thao tac cua nguoi
    ///   dung se mat theo khi go cai dat hoac khi cap nhat len phien ban moi ghi
    ///   de thu muc cai dat. AppData thuong duoc GIU LAI qua cac lan cai
    ///   dat/go cai dat/cap nhat (tru khi nguoi dung chu dong xoa), phu hop voi
    ///   ban chat cua log: du lieu NGUOI DUNG tao ra, khong phai mot phan cua
    ///   ban than ung dung.
    ///
    /// - Quy uoc chuan cua Windows: Microsoft khuyen dung AppData (hoac
    ///   LocalApplicationData) cho du lieu ung dung sinh ra trong luc chay (cau
    ///   hinh, cache, log...), con thu muc cai dat CHI nen chua file thuc thi/tai
    ///   nguyen tinh cua ung dung (khong doi sau khi cai) - dat log dung noi giup
    ///   ung dung "cu xu dung chuan" tren Windows, tranh bi phan mem diet virus/
    ///   Windows Defender Application Control gan co gang ghi vao thu muc
    ///   Program Files.
    ///
    /// GetLogFilePath() se doc Settings.Default.LogPath (chuoi co the chua bien
    /// moi truong nhu "%AppData%\SFileManager\logs", giong cach SettingsForm da
    /// hien thi/cho sua) roi Environment.ExpandEnvironmentVariables de ra duong
    /// dan tuyet doi, ket hop voi "log" + LogFileExtension lam ten file. Nguoi
    /// dung van co the tuy chinh vi tri nay qua SettingsForm (txtLogPath) neu
    /// muon doi khoi mac dinh AppData - KHONG hardcode cung mot duong dan trong
    /// LogService, tranh lech voi gia tri nguoi dung da cau hinh trong Settings.
    ///
    /// Cac buoc trien khai con lai (chua lam trong buoc "thiet ke" nay - vi tri
    /// file (GetLogFilePath) va dam bao thu muc ton tai (constructor) DA xong):
    /// - WriteLog: neu file chua ton tai, ghi dong header truoc; sau do
    ///   File.AppendAllText mot dong moi (bat try/catch, loi ghi log khong duoc
    ///   lam gian doan thao tac chinh cua nguoi dung).
    /// - GetLogs(): doc toan bo file, bo qua dong header, parse tung dong (chu y
    ///   tach dung khi Message co dau phay/xuong dong da duoc bao trong ngoac
    ///   kep - khong the dung String.Split(',') don gian cho toan dong), sap xep
    ///   giam dan theo Timestamp.
    /// - Nen ghi log bat dong bo (append, khong khoa UI luc ghi).
    /// </remarks>
    public class LogService
    {
        /// <summary>
        /// Dinh dang luu log da chon: CSV - xem "QUYET DINH THIET KE" o remarks
        /// tren dau lop de biet ly do (append-friendly, khong can them NuGet
        /// package, nguoi dung tu mo duoc bang Excel) so voi text tu do va JSON.
        /// </summary>
        public const string LogFileExtension = ".csv";

        /// <summary>
        /// Dong header dau tien cua file log CSV - PHAI khop chinh xac thu tu voi
        /// cach WriteLog ghi tung dong va cach GetLogs parse lai tung dong.
        /// </summary>
        public const string LogFileHeader = "Id,Timestamp,Operation,Source,Destination,Result,Message,ItemCount,Duration";

        // TODO: co the cho phep truyen duong dan file log tuy chinh qua constructor,
        // hien tai dung mac dinh.
        // private readonly string _logFilePath;

        public LogService()
        {
            // Dam bao thu muc chua file log (AppData\SFileManager\logs, hoac vi
            // tri nguoi dung da tuy chinh qua Settings.Default.LogPath) TON TAI
            // NGAY TU LUC KHOI TAO LogService, thay vi doi den lan WriteLog dau
            // tien moi kiem tra - tranh truong hop lan ghi log dau tien that bai
            // vi Directory.CreateDirectory chua duoc goi. Directory.CreateDirectory
            // tu no da AN TOAN khi thu muc da ton tai san (khong nem loi, chi tra
            // ve DirectoryInfo hien co) nen khong can kiem tra Directory.Exists truoc.
            try
            {
                string logDirectory = Path.GetDirectoryName(GetLogFilePath());
                if (!string.IsNullOrEmpty(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // Khong the tao thu muc log (hiem - VD AppData bi gioi han quyen
                // bat thuong) - khong nem loi tu constructor de tranh lam sap ca
                // ung dung chi vi tinh nang log (khong thiet yeu) khong hoat dong
                // duoc. WriteLog sau nay se tu gap loi tuong tu va tu xu ly rieng.
            }
        }

        /// <summary>
        /// Ghi mot dong log da co san (LogEntryModel) vao file CSV (xem "QUYET
        /// DINH THIET KE" o remarks tren dau lop) - tao file + ghi dong header
        /// truoc neu day la lan ghi dau tien, sau do APPEND (khong ghi de) mot
        /// dong moi ung voi entry.
        /// </summary>
        /// <remarks>
        /// Loi ghi log (VD: file dang bi khoa boi trinh xem log khac, o dia day,
        /// mat quyen truy cap AppData) se duoc NUOT (khong nem ra ngoai) - ghi
        /// log la mot tinh nang PHU TRO, khong duoc phep lam gian doan hoac lam
        /// that bai thao tac file/thu muc CHINH cua nguoi dung (VD: Copy/Delete)
        /// chi vi khong ghi duoc 1 dong log cho chinh thao tac do. Day la ly do
        /// WriteLog tra ve void (khong phai OperationResult) - noi goi khong can
        /// va khong nen re nhanh xu ly theo ket qua ghi log.
        /// </remarks>
        /// <param name="entry">Dong log can ghi.</param>
        public void WriteLog(LogEntryModel entry)
        {
            if (entry == null)
                return;

            try
            {
                string logFilePath = GetLogFilePath();

                // Dam bao thu muc ton tai ngay ca khi constructor truoc do da
                // gap loi (VD AppData tam thoi khong truy cap duoc luc khoi tao
                // LogService nhung da phuc hoi) - CreateDirectory an toan khi thu
                // muc da co san (xem ghi chu tai constructor).
                string logDirectory = Path.GetDirectoryName(logFilePath);
                if (!string.IsNullOrEmpty(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                // Chi ghi dong header MOT LAN DUY NHAT, luc file log CHUA TON TAI
                // (lan ghi dau tien sau khi cai dat, hoac sau khi ClearLogs da xoa
                // han file) - kiem tra File.Exists TRUOC khi mo file de tranh ghi
                // lai header o giua file moi lan ung dung khoi dong lai.
                bool isNewFile = !File.Exists(logFilePath);

                // Dung FileMode.Append (khong phai File.AppendAllText goi lai tu
                // dau moi lan) ket hop 1 StreamWriter duy nhat cho ca header (neu
                // can) va dong du lieu - dam bao ca 2 dong (neu co) duoc ghi
                // atomically hon la 2 lan mo/dong file rieng biet, giam nguy co
                // chi ghi duoc header ma dong du lieu bi loi giua chung.
                using (var stream = new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
                {
                    if (isNewFile)
                    {
                        writer.WriteLine(LogFileHeader);
                    }

                    writer.WriteLine(FormatCsvRow(entry));
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // Nuot loi - xem <remarks> tren: ghi log khong duoc phep lam gian
                // doan thao tac chinh cua nguoi dung. Khong co noi "hien thi loi"
                // phu hop o day (WriteLog thuong duoc goi ngam sau moi thao tac
                // file, khong phai tu hanh dong truc tiep cua nguoi dung), nen
                // don gian la bo qua dong log nay va tiep tuc.
            }
        }

        /// <summary>
        /// Chuyen mot LogEntryModel thanh MOT DONG CSV (chua bao gom ky tu xuong
        /// dong cuoi dong) dung thu tu cot cua LogFileHeader. Xem "Cau truc dong
        /// CSV" o remarks tren dau lop de biet dinh dang chinh xac tung cot.
        /// </summary>
        private static string FormatCsvRow(LogEntryModel entry)
        {
            string[] fields =
            {
                entry.Id.ToString(),
                entry.Timestamp.ToString("o", CultureInfo.InvariantCulture),
                entry.Operation.ToString(),
                EscapeCsvField(entry.Source),
                EscapeCsvField(entry.Destination),
                entry.Result.ToString(),
                EscapeCsvField(entry.Message),
                entry.ItemCount.ToString(CultureInfo.InvariantCulture),
                entry.Duration.HasValue ? entry.Duration.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture) : string.Empty
            };

            return string.Join(",", fields);
        }

        /// <summary>
        /// Escape MOT truong CSV theo RFC 4180: bao trong dau ngoac kep neu
        /// truong chua dau phay, dau ngoac kep, hoac ky tu xuong dong (CR/LF) -
        /// vi day la 3 ky tu co the lam sai lech cach doc lai file CSV neu khong
        /// escape. Ben trong dau ngoac kep, moi dau ngoac kep phai duoc nhan doi
        /// ("" thay vi ") de phan biet voi dau ngoac kep dong truong.
        /// </summary>
        /// <remarks>
        /// Chi Source/Destination/Message thuc su can qua ham nay (xem "Cau truc
        /// dong CSV" - cac cot con lai la Guid/enum/so nguyen/so thap phan, ve
        /// ban chat khong the chua dau phay/ngoac kep/xuong dong nen khong can
        /// escape, nhung goi ham nay cho chung van an toan/khong thay doi gi vi
        /// khong khop dieu kien needsQuoting).
        /// </remarks>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            bool needsQuoting = field.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
            if (!needsQuoting)
                return field;

            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }

        /// <summary>
        /// Tao va ghi mot dong log moi tu cac thong tin thao tac - cach dung nhanh,
        /// khong can tu tay tao LogEntryModel truoc. Nhan thang FileOperationType/
        /// OperationResult (cung 2 enum FileService/FolderService da tra ve/nhan
        /// vao) nen co the goi truc tiep tu ket qua mot loi goi Service, khong can
        /// anh xa qua enum rieng cho log - xem ghi chu thiet ke tai LogEntryModel.
        /// </summary>
        /// <param name="operation">Loai thao tac.</param>
        /// <param name="source">Duong dan nguon.</param>
        /// <param name="destination">Duong dan dich (co the null/rong).</param>
        /// <param name="result">Ket qua thao tac.</param>
        /// <param name="message">Thong tin bo sung (tuy chon).</param>
        /// <param name="itemCount">So luong muc lien quan (mac dinh 1 - xem LogEntryModel.ItemCount).</param>
        /// <param name="duration">Thoi gian thuc hien (tuy chon - xem LogEntryModel.Duration).</param>
        public void LogOperation(FileOperationType operation, string source, string destination, OperationResult result, string message = null, int itemCount = 1, TimeSpan? duration = null)
        {
            // TODO: tao new LogEntryModel(operation, source, destination, result, message, itemCount, duration)
            // roi goi WriteLog(entry).
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay toan bo danh sach log hien co, sap xep theo thoi gian gan nhat truoc.
        /// </summary>
        public List<LogEntryModel> GetLogs()
        {
            // TODO: doc file log, parse tung dong thanh LogEntryModel, sap xep theo Timestamp giam dan.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach log trong mot khoang thoi gian.
        /// </summary>
        /// <param name="fromDate">Tu ngay.</param>
        /// <param name="toDate">Den ngay.</param>
        public List<LogEntryModel> GetLogs(DateTime fromDate, DateTime toDate)
        {
            // TODO: goi GetLogs() roi loc theo Timestamp trong khoang [fromDate, toDate].
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach log theo ket qua thao tac (VD: chi xem cac thao tac Failed).
        /// </summary>
        /// <param name="result">Ket qua can loc.</param>
        public List<LogEntryModel> GetLogsByResult(OperationResult result)
        {
            // TODO: goi GetLogs() roi loc theo Result == result.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach log theo loai thao tac (VD: chi xem lich su Delete).
        /// </summary>
        /// <param name="operation">Loai thao tac can loc.</param>
        public List<LogEntryModel> GetLogsByOperation(FileOperationType operation)
        {
            // TODO: goi GetLogs() roi loc theo Operation == operation.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Xoa toan bo lich su log hien co.
        /// </summary>
        public OperationResult ClearLogs()
        {
            // TODO: xoa/ghi rong file log. Bat try/catch cho truong hop file dang bi
            // khoa (VD: dang duoc mo boi trinh xem log khac).
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay duong dan file log hien dang su dung (dinh dang CSV - LogFileExtension).
        /// </summary>
        public string GetLogFilePath()
        {
            // Settings.Default.LogPath da co san (Properties/Settings.settings),
            // mac dinh "%AppData%\SFileManager\logs" - xem "QUYET DINH THIET KE
            // THU HAI" o remarks tren dau lop de biet ly do chon AppData thay vi
            // thu muc cai dat. Environment.ExpandEnvironmentVariables chuyen
            // "%AppData%" thanh duong dan tuyet doi thuc te (VD
            // "C:\Users\ten\AppData\Roaming"), giong dung cach SettingsForm dang
            // hien thi gia tri nay cho nguoi dung xem (txtLogPath.Text).
            string configuredPath = Settings.Default.LogPath;

            // Phong truong hop nguoi dung/mot ban cai dat cu de trong LogPath
            // (VD nang cap tu phien ban truoc chua co setting nay) - dung lai
            // dung gia tri mac dinh da khai bao trong Settings.settings thay vi
            // de Path.Combine ben duoi nem ArgumentException voi chuoi rong.
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                configuredPath = @"%AppData%\SFileManager\logs";
            }

            string expandedDirectory = Environment.ExpandEnvironmentVariables(configuredPath);
            return Path.Combine(expandedDirectory, "log" + LogFileExtension);
        }
    }
}
