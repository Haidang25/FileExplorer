using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileExplorerApp.Helpers;
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

        /// <summary>
        /// Khoa dong bo hoa MOI LAN GHI vao file log - xem "GHI LOG AN TOAN KHI
        /// NHIEU THAO TAC LIEN TIEP" o remarks tren dau lop de biet ly do can
        /// khoa va tai sao phai la STATIC (khong phai field rieng cua tung
        /// instance LogService).
        /// </summary>
        private static readonly object WriteLock = new object();

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

            // Ton trong tuy chon "Bat/tat ghi nhat ky thao tac" cua nguoi dung
            // (Settings.Default.LogEnabled, da co san va duoc SettingsForm cho
            // sua qua chkEnableLog) - kiem tra O DAY (diem vao duy nhat cua moi
            // luot ghi log, ca WriteLog truc tiep lan qua LogOperation) de nguoi
            // goi (VD: MainForm) khong can tu kiem tra setting nay truoc moi lan
            // goi LogOperation/WriteLog.
            if (!Settings.Default.LogEnabled)
                return;

            try
            {
                // KHOA toan bo phan doc-kiem tra-roi-ghi ben duoi bang mot khoa
                // STATIC dung chung cho MOI instance LogService (khong phai
                // "lock (this)") - xem "GHI LOG AN TOAN KHI NHIEU THAO TAC LIEN
                // TIEP" o remarks tren dau lop de biet day du ly do. Tom tat: neu
                // khong khoa, 2 lenh WriteLog goi gan nhu dong thoi (VD: dan nhieu
                // file lien tiep, hoac 2 luong nen doc lap cung ghi log) co the
                // CUNG THAY file chua ton tai (File.Exists) roi CA HAI cung ghi
                // header - lam file CSV co 2 dong header giua chung, phá vo gia
                // dinh "dong dau la header" ma GetLogs se dua vao de parse.
                lock (WriteLock)
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
                    // lai header o giua file moi lan ung dung khoi dong lai. An toan
                    // truoc race-condition vi ca kiem tra lan ghi deu nam trong CUNG
                    // MOT pham vi lock - khong con luong nao khac co the xen vao giua.
                    bool isNewFile = !File.Exists(logFilePath);

                    // Dung FileMode.Append (khong phai File.AppendAllText goi lai tu
                    // dau moi lan) ket hop 1 StreamWriter duy nhat cho ca header (neu
                    // can) va dong du lieu - dam bao ca 2 dong (neu co) duoc ghi
                    // atomically hon la 2 lan mo/dong file rieng biet, giam nguy co
                    // chi ghi duoc header ma dong du lieu bi loi giua chung.
                    //
                    // FileShare.Read (khong phai FileShare.None) van CHO PHEP mot
                    // luong DOC file log (VD: LogForm dang mo xem log qua GetLogs)
                    // trong luc WriteLog dang ghi - chi loai tru cac WriteLog KHAC
                    // ghi dong thoi, dieu ma "lock" o day da tu dam bao trong pham
                    // vi ung dung nay, nen FileShare.Read la du (khong can
                    // FileShare.ReadWrite, vi khong co noi nao khac trong ung dung
                    // GHI vao file nay ngoai WriteLog).
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
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // Nuot loi - xem <remarks> tren: ghi log khong duoc phep lam gian
                // doan thao tac chinh cua nguoi dung. Khong co noi "hien thi loi"
                // phu hop o day (WriteLog thuong duoc goi ngam sau moi thao tac
                // file, khong phai tu hanh dong truc tiep cua nguoi dung), nen
                // don gian la bo qua dong log nay va tiep tuc. Voi khoa "lock" o
                // tren, day gio chi con xay ra vi ly do BEN NGOAI ung dung (VD:
                // mot chuong trinh KHAC - VD Excel dang mo file log - dang giu
                // khoa doc/ghi rieng cua no), khong con do 2 luong noi bo ung dung
                // tranh chap voi nhau.
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
            var entry = new LogEntryModel(operation, source, destination, result, message, itemCount, duration);
            WriteLog(entry);
        }

        /// <summary>
        /// Lay toan bo danh sach log hien co, sap xep theo thoi gian gan nhat truoc.
        /// Doc TOAN BO file (khong khoa WriteLock khi doc - xem ghi chu "FileShare.Read"
        /// tai WriteLog: doc dong thoi voi ghi la an toan vi FileStream mo o che do
        /// FileShare.Read cho phep mot luong khac doc trong luc ghi).
        /// </summary>
        /// <remarks>
        /// Neu file log khong ton tai (chua ghi lan nao) hoac khong doc duoc (VD:
        /// dang bi khoa boi chuong trinh khac, mat quyen truy cap), tra ve danh
        /// sach RONG thay vi nem loi - man hinh xem log (LogForm) chi nen hien
        /// "chua co lich su" thay vi crash/bao loi kho hieu cho nguoi dung.
        /// Dong nao khong parse duoc (VD: file log bi hong/sua tay sai dinh dang)
        /// se duoc BO QUA (khong lam hong ca danh sach) - xem ParseCsvLine/TryParseLogLine.
        /// </remarks>
        public List<LogEntryModel> GetLogs()
        {
            var result = new List<LogEntryModel>();
            string logFilePath = GetLogFilePath();

            if (!File.Exists(logFilePath))
                return result;

            try
            {
                using (var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line = reader.ReadLine();
                    bool isFirstLine = true;

                    while (line != null)
                    {
                        // Dong dau tien la header ten cot (LogFileHeader) - bo qua,
                        // khong phai du lieu. So sanh chinh xac header thay vi luon
                        // bo qua "dong dau tien bat ky" de neu file bi hong/thieu
                        // header thi van co co hoi parse dong dau nhu du lieu binh
                        // thuong thay vi mat luon 1 dong log hop le.
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            if (string.Equals(line, LogFileHeader, StringComparison.Ordinal))
                            {
                                line = reader.ReadLine();
                                continue;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            LogEntryModel entry = TryParseLogLine(line);
                            if (entry != null)
                            {
                                result.Add(entry);
                            }
                        }

                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // Khong doc duoc file log (VD: dang bi khoa boi chuong trinh khac) -
                // tra ve nhung gi da doc duoc (co the la danh sach rong) thay vi nem
                // loi, giu dung nguyen tac "log la tinh nang phu tro, khong lam gian
                // doan/that bai man hinh chinh" ap dung tuong tu nhu WriteLog.
            }

            // Gan nhat truoc - thuan tien nhat cho nguoi dung xem lich su (thao tac
            // vua roi luon nam tren cung danh sach, khong can cuon xuong cuoi).
            result.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return result;
        }

        /// <summary>
        /// Lay danh sach log trong mot khoang thoi gian (ca hai dau bao gom).
        /// </summary>
        /// <param name="fromDate">Tu ngay.</param>
        /// <param name="toDate">Den ngay.</param>
        public List<LogEntryModel> GetLogs(DateTime fromDate, DateTime toDate)
        {
            return GetLogs().FindAll(entry => entry.Timestamp >= fromDate && entry.Timestamp <= toDate);
        }

        /// <summary>
        /// Lay danh sach log theo ket qua thao tac (VD: chi xem cac thao tac Failed).
        /// </summary>
        /// <param name="result">Ket qua can loc.</param>
        public List<LogEntryModel> GetLogsByResult(OperationResult result)
        {
            return GetLogs().FindAll(entry => entry.Result == result);
        }

        /// <summary>
        /// Lay danh sach log theo loai thao tac (VD: chi xem lich su Delete).
        /// </summary>
        /// <param name="operation">Loai thao tac can loc.</param>
        public List<LogEntryModel> GetLogsByOperation(FileOperationType operation)
        {
            return GetLogs().FindAll(entry => entry.Operation == operation);
        }

        /// <summary>
        /// Xoa toan bo lich su log hien co (xoa han file log - lan WriteLog tiep
        /// theo se tu tao lai file moi kem dong header, xem WriteLog).
        /// </summary>
        /// <remarks>
        /// Dung "lock (WriteLock)" giong WriteLog de tranh xoa file dung luc mot
        /// thao tac khac dang ghi log (VD: nguoi dung bam "Xoa lich su" tren
        /// LogForm dung luc MainForm dang ghi 1 dong log khac o luong background) -
        /// neu khong khoa, co the xay ra ghi vao file vua bi xoa giua chung, gay
        /// loi kho luong hoac mot file log moi voi noi dung khong nhu mong doi.
        /// </remarks>
        public OperationResult ClearLogs()
        {
            try
            {
                lock (WriteLock)
                {
                    string logFilePath = GetLogFilePath();
                    if (File.Exists(logFilePath))
                    {
                        File.Delete(logFilePath);
                    }
                }
                return OperationResult.Success;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // File dang bi khoa (VD: dang duoc mo boi Excel/trinh xem log
                // khac) hoac mat quyen truy cap - bao that bai de noi goi (LogForm)
                // hien thong bao loi cu the cho nguoi dung, thay vi nuot am tham
                // nhu WriteLog (day la thao tac NGUOI DUNG CHU DONG bam, ho can
                // biet ket qua, khac voi ghi log ngam sau moi thao tac khac).
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Parse MOT dong CSV du lieu (khong phai dong header) thanh LogEntryModel.
        /// Tra ve null (thay vi nem loi) neu dong khong dung dinh dang - de GetLogs
        /// co the bo qua dong hong ma khong lam mat toan bo danh sach.
        /// </summary>
        private static LogEntryModel TryParseLogLine(string line)
        {
            try
            {
                string[] fields = ParseCsvLine(line);

                // Phai co dung 9 cot theo LogFileHeader - neu khac (VD: dong bi cat
                // ngang do ghi do do, hoac file bi sua tay sai) thi coi la khong
                // hop le, khong co gang doan mo thieu.
                if (fields.Length != 9)
                    return null;

                var entry = new LogEntryModel
                {
                    Id = Guid.Parse(fields[0]),
                    Timestamp = DateTime.Parse(fields[1], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    Operation = (FileOperationType)Enum.Parse(typeof(FileOperationType), fields[2]),
                    Source = fields[3],
                    Destination = fields[4],
                    Result = (OperationResult)Enum.Parse(typeof(OperationResult), fields[5]),
                    Message = fields[6],
                    ItemCount = int.Parse(fields[7], CultureInfo.InvariantCulture),
                    Duration = string.IsNullOrEmpty(fields[8])
                        ? (TimeSpan?)null
                        : TimeSpan.FromSeconds(double.Parse(fields[8], CultureInfo.InvariantCulture))
                };
                return entry;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException || ex is OverflowException)
            {
                // Dong khong dung dinh dang mong doi (VD: enum khong ton tai, so
                // khong parse duoc, thieu/thua cot) - bo qua dong nay, GetLogs se
                // tiep tuc voi cac dong con lai.
                return null;
            }
        }

        /// <summary>
        /// Tach MOT dong CSV thanh mang cac truong, XU LY dung dau ngoac kep bao
        /// quanh truong theo RFC 4180 (nguoc lai voi EscapeCsvField) - KHONG the
        /// dung don gian line.Split(',') vi truong Message co the tu chua dau
        /// phay/xuong dong da duoc bao trong dau ngoac kep luc ghi (xem
        /// EscapeCsvField/FormatCsvRow).
        /// </summary>
        private static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (inQuotes)
                {
                    if (c == '"')
                    {
                        // Hai dau ngoac kep lien tiep ben trong vung quote la MOT
                        // dau ngoac kep thuc su trong du lieu (da duoc nhan doi
                        // luc ghi - xem EscapeCsvField), khong phai dau dong quote.
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            current.Append('"');
                            i++;
                        }
                        else
                        {
                            inQuotes = false;
                        }
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else
                {
                    if (c == '"')
                    {
                        inQuotes = true;
                    }
                    else if (c == ',')
                    {
                        fields.Add(current.ToString());
                        current.Clear();
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
            }

            fields.Add(current.ToString());
            return fields.ToArray();
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

        // ================================================================
        // MO RONG: BAO CAO DIEU TRA TOAN VEN (integrity investigation report)
        // ================================================================
        //
        // YEU CAU: "Mo rong LogService xuat bao cao dieu tra: thoi gian,
        // duong dan, hash truoc/sau, Environment.UserName" - tuc la GHI LAI
        // moi vi pham toan ven ma IntegrityService phat hien (xem
        // IntegrityViolationEventArgs/IntegrityViolationDetected tai
        // Services\IntegrityService.cs) thanh mot BAO CAO co the XEM LAI/XUAT
        // RA sau nay, phuc vu dieu tra khi nghi ngo co truy cap/sua doi trai
        // phep.
        //
        // QUYET DINH THIET KE: dung MOT FILE/SCHEMA CSV RIENG (khong tai su
        // dung log.csv/LogFileHeader/LogEntryModel hien co) - xem <remarks>
        // day du tai Models\IntegrityInvestigationEntry.cs de biet ly do chi
        // tiet (tom tat: LogFileHeader dang CO DINH 9 cot va TryParseLogLine
        // TU CHOI moi dong khac 9 cot, nhoi them cot se pha vo tinh tuong
        // thich nguoc voi file log.csv da co san tren may nguoi dung; hai
        // khai niem - "nhat ky thao tac cua nguoi dung" va "bao cao dieu tra
        // vi pham he thong tu phat hien" - cung khac ban chat nhau). Cac
        // nguyen tac con lai (CSV, append-only luc ghi, escape RFC 4180, khoa
        // tinh dong bo, dat trong AppData) giu NGUYEN VE NHU voi log.csv -
        // xem "QUYET DINH THIET KE"/"QUYET DINH THIET KE THU HAI" o remarks
        // dau lop de biet ly do goc, khong nhac lai o day.

        /// <summary>
        /// Ten file rieng cho bao cao dieu tra toan ven (KHAC voi "log" +
        /// LogFileExtension cua nhat ky thao tac thong thuong) - dung CHUNG
        /// thu muc voi log.csv (Settings.Default.LogPath) vi ca hai deu la
        /// "du lieu ung dung sinh ra luc chay" theo dung tinh chat cua AppData
        /// (xem "QUYET DINH THIET KE THU HAI" o remarks dau lop), khong can
        /// mot Settings.Default.XxxPath rieng chi de doi thu muc.
        /// </summary>
        public const string InvestigationReportFileName = "integrity_investigation" + LogFileExtension;

        /// <summary>
        /// Dong header dau tien cua file bao cao dieu tra CSV - PHAI khop
        /// chinh xac thu tu voi FormatInvestigationCsvRow (ghi) va
        /// TryParseInvestigationLine (doc lai).
        /// </summary>
        public const string InvestigationReportFileHeader = "Timestamp,FilePath,ViolationType,HashBefore,HashAfter,UserName";

        /// <summary>
        /// Khoa dong bo hoa RIENG cho file bao cao dieu tra - TACH BIET voi
        /// WriteLock (log.csv) vi day la HAI FILE KHAC NHAU, khoa chung mot
        /// object se lam moi lan ghi bao cao dieu tra CHO KHONG CAN THIET moi
        /// lan co mot thao tac Copy/Move/Delete... (va nguoc lai) dang duoc
        /// ghi log dong thoi, trong khi hai thao tac ghi nay hoan toan khong
        /// lien quan/khong dung chung file.
        /// </summary>
        private static readonly object InvestigationWriteLock = new object();

        /// <summary>
        /// Lay duong dan file bao cao dieu tra toan ven hien dang su dung -
        /// xem InvestigationReportFileName. Dung CHUNG logic doc/mo rong
        /// Settings.Default.LogPath voi GetLogFilePath (chi khac ten file o
        /// buoc cuoi), giu dung nguyen tac "khong hardcode duong dan rieng,
        /// tu Settings ma ra" ap dung cho log.csv.
        /// </summary>
        public string GetInvestigationReportFilePath()
        {
            string configuredPath = Settings.Default.LogPath;
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                configuredPath = @"%AppData%\SFileManager\logs";
            }

            string expandedDirectory = Environment.ExpandEnvironmentVariables(configuredPath);
            return Path.Combine(expandedDirectory, InvestigationReportFileName);
        }

        /// <summary>
        /// Ghi MOT dong bao cao dieu tra tu mot vi pham toan ven da phat hien
        /// (IntegrityViolationEventArgs - xem IntegrityService). Day la ham
        /// TIEN LOI danh cho noi goi (VD MainForm.IntegrityService_IntegrityViolationDetected)
        /// truyen thang doi tuong su kien nhan duoc, khong can tu tay anh xa
        /// tung truong sang IntegrityInvestigationEntry.
        /// </summary>
        /// <remarks>
        /// UserName duoc lay TU BEN TRONG ham nay bang Environment.UserName
        /// (KHONG nhan tu tham so ben ngoai) - dung dung yeu cau "xuat...
        /// Environment.UserName", va dam bao gia tri LUON la nguoi dang dang
        /// nhap TREN MAY DANG CHAY ham nay (may dang giam sat), bat ke
        /// IntegrityService duoc goi tu dau.
        ///
        /// Cung nhu WriteLog, loi ghi (file dang bi khoa, mat quyen...) duoc
        /// NUOT (khong nem ra ngoai) - ghi bao cao dieu tra la tinh nang PHU
        /// TRO chay ngam khi phat hien vi pham, khong duoc phep lam gian doan
        /// hoac lam crash luong xu ly su kien IntegrityViolationDetected (VD
        /// dang chay tren luong threadpool cua FileSystemWatcher - xem
        /// IntegrityService remarks) chi vi khong ghi duoc bao cao.
        /// </remarks>
        /// <param name="violation">Vi pham can ghi lai.</param>
        public void LogIntegrityViolation(IntegrityViolationEventArgs violation)
        {
            if (violation == null)
                return;

            var entry = new IntegrityInvestigationEntry(
                violation.DetectedAtUtc,
                violation.FilePath,
                violation.ViolationType.ToString(),
                violation.ExpectedHash,
                violation.ActualHash,
                Environment.UserName);

            WriteInvestigationEntry(entry);
        }

        /// <summary>
        /// Ghi MOT dong IntegrityInvestigationEntry da co san vao file bao cao
        /// dieu tra CSV - cau truc/ly do y het WriteLog (tao file + ghi header
        /// neu la lan dau, sau do APPEND mot dong moi), chi khac file dich va
        /// khoa dung (InvestigationWriteLock thay vi WriteLock).
        /// </summary>
        /// <param name="entry">Dong bao cao can ghi.</param>
        public void WriteInvestigationEntry(IntegrityInvestigationEntry entry)
        {
            if (entry == null)
                return;

            if (!Settings.Default.LogEnabled)
                return;

            try
            {
                lock (InvestigationWriteLock)
                {
                    string reportFilePath = GetInvestigationReportFilePath();

                    string reportDirectory = Path.GetDirectoryName(reportFilePath);
                    if (!string.IsNullOrEmpty(reportDirectory))
                    {
                        Directory.CreateDirectory(reportDirectory);
                    }

                    bool isNewFile = !File.Exists(reportFilePath);

                    using (var stream = new FileStream(reportFilePath, FileMode.Append, FileAccess.Write, FileShare.Read))
                    using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
                    {
                        if (isNewFile)
                        {
                            writer.WriteLine(InvestigationReportFileHeader);
                        }

                        writer.WriteLine(FormatInvestigationCsvRow(entry));
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // Nuot loi - xem <remarks> tai LogIntegrityViolation.
            }
        }

        /// <summary>
        /// Chuyen mot IntegrityInvestigationEntry thanh MOT DONG CSV (chua
        /// bao gom ky tu xuong dong cuoi dong) dung thu tu cot cua
        /// InvestigationReportFileHeader. FilePath/ViolationType/HashBefore/
        /// HashAfter/UserName deu qua EscapeCsvField (co san, dung chung voi
        /// WriteLog) de an toan truoc gia tri hiem gap chua dau phay/ngoac
        /// kep (VD duong dan file tren mot so he thong tep cho phep dau phay
        /// trong ten file).
        /// </summary>
        private static string FormatInvestigationCsvRow(IntegrityInvestigationEntry entry)
        {
            string[] fields =
            {
                entry.Timestamp.ToString("o", CultureInfo.InvariantCulture),
                EscapeCsvField(entry.FilePath),
                EscapeCsvField(entry.ViolationType),
                EscapeCsvField(entry.HashBefore),
                EscapeCsvField(entry.HashAfter),
                EscapeCsvField(entry.UserName)
            };

            return string.Join(",", fields);
        }

        /// <summary>
        /// Lay toan bo bao cao dieu tra hien co, sap xep giam dan theo
        /// Timestamp (gan nhat truoc) - cau truc/ly do y het GetLogs (tra ve
        /// danh sach rong neu file chua ton tai/khong doc duoc, bo qua dong
        /// khong parse duoc thay vi lam hong ca danh sach).
        /// </summary>
        public List<IntegrityInvestigationEntry> GetInvestigationEntries()
        {
            var result = new List<IntegrityInvestigationEntry>();
            string reportFilePath = GetInvestigationReportFilePath();

            if (!File.Exists(reportFilePath))
                return result;

            try
            {
                using (var stream = new FileStream(reportFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string line = reader.ReadLine();
                    bool isFirstLine = true;

                    while (line != null)
                    {
                        if (isFirstLine)
                        {
                            isFirstLine = false;
                            if (string.Equals(line, InvestigationReportFileHeader, StringComparison.Ordinal))
                            {
                                line = reader.ReadLine();
                                continue;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            IntegrityInvestigationEntry entry = TryParseInvestigationLine(line);
                            if (entry != null)
                            {
                                result.Add(entry);
                            }
                        }

                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                // Xem ghi chu tuong tu tai GetLogs.
            }

            result.Sort((a, b) => b.Timestamp.CompareTo(a.Timestamp));
            return result;
        }

        /// <summary>
        /// Parse MOT dong CSV du lieu (khong phai dong header) thanh
        /// IntegrityInvestigationEntry. Tra ve null (thay vi nem loi) neu
        /// dong khong dung dinh dang - de GetInvestigationEntries bo qua dong
        /// hong ma khong mat toan bo danh sach, y het TryParseLogLine.
        /// </summary>
        private static IntegrityInvestigationEntry TryParseInvestigationLine(string line)
        {
            try
            {
                string[] fields = ParseCsvLine(line);

                // Dung 6 cot theo InvestigationReportFileHeader.
                if (fields.Length != 6)
                    return null;

                return new IntegrityInvestigationEntry
                {
                    Timestamp = DateTime.Parse(fields[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    FilePath = fields[1],
                    ViolationType = fields[2],
                    HashBefore = fields[3],
                    HashAfter = fields[4],
                    UserName = fields[5]
                };
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException || ex is OverflowException)
            {
                return null;
            }
        }

        /// <summary>
        /// Dong header HIEN THI (tieng Viet, danh cho FILE XUAT RA doc bang
        /// mat nguoi/Excel) - KHAC voi InvestigationReportFileHeader (tieng
        /// Anh, danh cho file LUU TRU NOI BO ma TryParseInvestigationLine can
        /// doc lai chinh xac tung chu). Tach rieng 2 header vi 2 muc dich khac
        /// nhau - xem "QUYET DINH THIET KE - XUAT BAO CAO DANG BAO CAO DIEU
        /// TRA" o remarks tai ExportInvestigationReport.
        /// </summary>
        public const string InvestigationReportDisplayHeader = "Thời gian,Đường dẫn,Loại vi phạm,Hash trước,Hash sau,Người dùng";

        /// <summary>Tieu de o dong dau file bao cao xuat ra - xem ExportInvestigationReport.</summary>
        public const string InvestigationReportTitle = "BÁO CÁO ĐIỀU TRA TOÀN VẸN TỆP";

        /// <summary>
        /// XUAT bao cao dieu tra hien co RA MOT FILE do nguoi dung chi dinh
        /// (dung yeu cau "xuat bao cao dieu tra"), DUNG DINH DANG DE DOC danh
        /// cho nguoi dieu tra (KHONG con la copy nguyen si file luu tru noi
        /// bo nua - xem <remarks> ben duoi de biet ly do thay doi).
        /// </summary>
        /// <remarks>
        /// QUYET DINH THIET KE - XUAT BAO CAO DANG BAO CAO DIEU TRA (khac
        /// phien ban truoc CHI File.Copy file noi bo): file LUU TRU NOI BO
        /// (integrity_investigation.csv, xem InvestigationReportFileHeader)
        /// duoc toi uu de MAY doc lai chinh xac (header tieng Anh co dinh,
        /// Timestamp dang "o" UTC round-trip, ViolationType giu nguyen ten
        /// enum) - phu hop de LogService tu doc lai (GetInvestigationEntries),
        /// nhung KHO DOC voi con nguoi neu mo truc tiep bang Excel (VD:
        /// "2026-08-23T10:15:30.1234567Z" thay vi "23/08/2026 17:15:30",
        /// "ContentModified" thay vi "Nội dung bị sửa"). Vi day la BAO CAO
        /// XUAT RA (khac voi file luu tru noi bo), uu tien "ro rang, dung
        /// phong cach bao cao dieu tra" cho nguoi doc hon la de may parse lai
        /// - nen ham nay GIO DAY tu dung GetInvestigationEntries() (KHONG
        /// File.Copy) roi tu VIET LAI toan bo noi dung file xuat theo dinh
        /// dang moi:
        /// - Dong 1: tieu de bao cao (InvestigationReportTitle).
        /// - Dong 2-3: sieu du lieu bao cao (thoi diem xuat, tong so vi pham) -
        ///   giup nguoi doc biet NGAY bao cao nay xuat luc nao, gom bao nhieu
        ///   dong, khong can tu dem dong.
        /// - Dong 4: dong trong (phan cach sieu du lieu voi bang du lieu, de
        ///   Excel/nguoi doc de phan biet 2 phan).
        /// - Dong 5: header tieng Viet (InvestigationReportDisplayHeader).
        /// - Cac dong sau: MOI dong MOT vi pham, Thoi gian da doi sang GIO
        ///   DIA PHUONG (ToLocalTime, xem ben duoi), Loai vi pham da dich
        ///   sang tieng Viet (TranslateViolationType), Hash rong hien "-"
        ///   thay vi de trong (ro rang hon la mot o trong KHONG BIET la
        ///   "khong ap dung" hay "loi xuat").
        ///
        /// QUYET DINH THIET KE - HASH CUA BAO CAO (tinh hash CHINH file bao
        /// cao vua xuat, luu kem de doi chieu sau nay): muc dich la de PHAT
        /// HIEN neu file bao cao CSV nay (mot khi da ra khoi may/duoc gui di
        /// noi khac de luu tru/trinh bay) co bi SUA DOI sau khi xuat hay
        /// khong - CUNG logic "toan ven" nhu IntegrityService ap dung cho
        /// file nguoi dung dang giam sat, gio ap dung cho CHINH bao cao dieu
        /// tra nay (mot bao cao dieu tra ma khong the tu chung minh no CHUA
        /// bi sua sau khi xuat thi gia tri lam bang chung se giam di rat
        /// nhieu). Dung lai HashHelper.ComputeSha256 (thuat toan GIONG HET
        /// BaselineService/IntegrityService - xem HashHelper.cs, ly do chon
        /// SHA-256 thay vi MD5) de nguoi dieu tra doi chieu bang CUNG mot
        /// thuat toan quen thuoc voi phan con lai cua tinh nang giam sat toan
        /// ven.
        ///
        /// Hash duoc ghi vao MOT FILE .sha256 RIENG, DAT CANH file bao cao
        /// (VD "baocao.csv" -> "baocao.csv.sha256"), KHONG nhoi vao BEN TRONG
        /// noi dung file bao cao (VD them 1 dong "Hash file nay:..." o cuoi) -
        /// vi neu lam vay, hash se phai TU BAO GOM CHINH NO trong du lieu
        /// dau vao tinh hash (nghich ly "hash cua X phu thuoc vao X"), buoc
        /// phai dung mot quy uoc phuc tap hon (VD tinh hash TRUOC roi chen
        /// vao, nhung nhu vay hash ghi trong file se KHONG con la hash CUA
        /// FILE HOAN CHINH nua ma la hash cua file THIEU dong cuoi) - tach
        /// hash ra file .sha256 rieng, tinh SAU KHI file bao cao da ghi xong
        /// hoan toan, don gian va chinh xac hon nhieu.
        ///
        /// Dinh dang file .sha256 tuan theo QUY UOC CHUAN cua cac cong cu
        /// checksum pho bien (VD lenh "sha256sum" tren Linux/WSL, hoac
        /// "certutil -hashfile ... SHA256" tren Windows co the doi chieu thu
        /// cong bang mat) - "&lt;hash hex chu thuong&gt;  &lt;ten file&gt;" (CHINH XAC
        /// hai khoang trang) - de nguoi dieu tra (hoac mot cong cu ben ngoai
        /// ung dung) co the doi chieu bang lenh "sha256sum -c baocao.csv.sha256"
        /// tieu chuan MA KHONG can phu thuoc vao ung dung nay, thay vi tu
        /// dat mot dinh dang rieng chi ung dung nay hieu duoc.
        ///
        /// XEM THEM: VerifyExportedReportHash ben duoi - ham DOI CHIEU LAI
        /// (doc file .sha256 da luu, tinh lai hash HIEN TAI cua file bao cao,
        /// so sanh) - day chinh la "doi chieu sau nay" nhu yeu cau, thuc hien
        /// NGAY TRONG ung dung nay ma khong bat buoc phai ra ngoai dong lenh.
        ///
        /// GIO DIA PHUONG (ToLocalTime) CHI o BAN XUAT: file luu tru noi bo
        /// (WriteInvestigationEntry) VAN giu UTC (DetectedAtUtc nguyen ven) -
        /// dung cho MOI doi chieu/phan tich noi bo sau nay (VD so sanh voi
        /// log tren mot may khac o mui gio khac) can mot moc thoi gian TUYET
        /// DOI, KHONG phu thuoc mui gio may dang xem; con BAO CAO XUAT la de
        /// NGUOI DIEU TRA doc truc tiep, ho quen voi gio dia phuong hon la
        /// UTC (giong FormatHelper.FormatDate/LogEntryModel.Timestamp =
        /// DateTime.Now dang dung cho hien thi trong toan ung dung).
        ///
        /// BOM (encoderShouldEmitUTF8Identifier: TRUE, khac voi WriteLock/
        /// InvestigationWriteLock ben tren dung FALSE): file luu tru noi bo
        /// KHONG duoc co BOM vi TryParseInvestigationLine/TryParseLogLine so
        /// sanh CHUOI HEADER CHINH XAC (string.Equals) - mot BOM vo hinh o
        /// dau file se lam dong dau tien KHONG con khop header nua. File XUAT
        /// RA nguoc lai NEN co BOM vi day la file de MO BANG EXCEL - thieu
        /// BOM, Excel (dac biet ban cu/mac dinh tieng Anh) co the doan sai
        /// bang ma va hien sai dau tieng Viet (VD "Ni dung b sa" thay vi "Nội
        /// dung bị sửa").
        /// </remarks>
        /// <param name="destinationFilePath">Duong dan file dich nguoi dung muon luu bao cao ra (VD tu SaveFileDialog).</param>
        /// <returns>Success neu xuat duoc, Failed neu chua co vi pham nao duoc ghi nhan hoac khong ghi duoc file dich (VD mat quyen, duong dan khong hop le, o dia day).</returns>
        public OperationResult ExportInvestigationReport(string destinationFilePath)
        {
            if (string.IsNullOrWhiteSpace(destinationFilePath))
                return OperationResult.Failed;

            try
            {
                // GetInvestigationEntries() da tu doc file luu tru noi bo va
                // sap xep giam dan theo Timestamp (gan nhat truoc) - giu
                // nguyen thu tu nay cho ban xuat, phu hop voi cach nguoi dieu
                // tra thuong muon xem VI PHAM GAN DAY NHAT truoc tien.
                List<IntegrityInvestigationEntry> entries = GetInvestigationEntries();
                if (entries.Count == 0)
                    return OperationResult.Failed; // Chua tung ghi nhan vi pham nao - khong co gi de xuat.

                string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
                if (!string.IsNullOrEmpty(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                using (var writer = new StreamWriter(destinationFilePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)))
                {
                    writer.WriteLine(InvestigationReportTitle);
                    writer.WriteLine("Xuất lúc:," + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                    writer.WriteLine("Tổng số vi phạm:," + entries.Count.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();
                    writer.WriteLine(InvestigationReportDisplayHeader);

                    foreach (IntegrityInvestigationEntry entry in entries)
                    {
                        writer.WriteLine(FormatInvestigationDisplayRow(entry));
                    }
                }

                // Tinh VA LUU KEM hash cua CHINH file bao cao VUA XUAT - xem
                // "QUYET DINH THIET KE - HASH CUA BAO CAO" o remarks tren dau
                // ham nay VA WriteReportHashFile de biet day du ly do/dinh
                // dang. Lam NGAY SAU KHI file bao cao da GHI XONG va DONG
                // (StreamWriter da Dispose o tren, thoat khoi khoi using) -
                // BAT BUOC phai the, vi HashHelper.ComputeSha256 can MO LAI
                // file de doc (FileShare.Read) - neu con StreamWriter dang mo
                // ghi file nay o che do doc quyen (FileShare mac dinh cua
                // StreamWriter constructor la khong chia se ghi), ComputeSha256
                // se that bai voi IOException "file dang duoc su dung boi mot
                // tien trinh khac" (that ra la CHINH tien trinh nay, vi con
                // StreamWriter chua dong).
                string reportHash = HashHelper.ComputeSha256(destinationFilePath);
                WriteReportHashFile(destinationFilePath, reportHash, entries.Count);

                return OperationResult.Success;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException || ex is ArgumentException || ex is NotSupportedException)
            {
                // Duong dan dich khong hop le, mat quyen ghi, file dang bi
                // khoa boi chuong trinh khac... - day la thao tac NGUOI DUNG
                // CHU DONG yeu cau (bam "Xuat bao cao"), nen bao Failed de UI
                // hien thong bao loi cu the, khac voi WriteLog/LogIntegrityViolation
                // (ghi ngam, nuot loi am tham).
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Chuyen mot IntegrityInvestigationEntry thanh MOT DONG CSV theo dinh
        /// dang HIEN THI (InvestigationReportDisplayHeader) - xem <remarks>
        /// tai ExportInvestigationReport de biet day du ly do khac voi
        /// FormatInvestigationCsvRow (dinh dang luu tru noi bo).
        /// </summary>
        private static string FormatInvestigationDisplayRow(IntegrityInvestigationEntry entry)
        {
            string[] fields =
            {
                entry.Timestamp.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture),
                EscapeCsvField(entry.FilePath),
                EscapeCsvField(TranslateViolationType(entry.ViolationType)),
                EscapeCsvField(string.IsNullOrEmpty(entry.HashBefore) ? "-" : entry.HashBefore),
                EscapeCsvField(string.IsNullOrEmpty(entry.HashAfter) ? "-" : entry.HashAfter),
                EscapeCsvField(entry.UserName)
            };

            return string.Join(",", fields);
        }

        /// <summary>
        /// Dich ten enum IntegrityViolationType (luu duoi dang chuoi trong
        /// IntegrityInvestigationEntry.ViolationType - xem ghi chu tai model)
        /// sang nhan tieng Viet, DUNG Y HET cach dat ten trong yeu cau phan
        /// loai truoc do ("Nội dung bị sửa / Tệp bị xóa / Tệp mới xuất hiện")
        /// de nguoi doc bao cao thay nhan quen thuoc, dong nhat voi phan con
        /// lai cua ung dung (VD tsslIntegrityAlert, toast canh bao).
        /// </summary>
        /// <remarks>
        /// Nhan tham so dang string (khong phai enum IntegrityViolationType
        /// truc tiep) vi LogService khong (va khong nen) phu thuoc nguoc lai
        /// Services\IntegrityService.cs chi de doi 1 ham dich nhan - ca hai
        /// deu la lop trong CUNG namespace FileExplorerApp.Services nen VE
        /// MAT KY THUAT co the tham chieu duoc, nhung giu tham so string giup
        /// ham nay (va IntegrityInvestigationEntry) hoan toan doc lap, khong
        /// vo tinh gay loi bien dich day chuyen neu IntegrityViolationType
        /// thay doi ten/vi tri sau nay. Gia tri KHONG khop enum nao da biet
        /// (VD du lieu cu/bi sua tay) tra ve NGUYEN VAN chuoi goc thay vi nem
        /// loi hoac hien "khong xac dinh" - an toan hon, khong lam mat thong
        /// tin dieu tra chi vi mot nhan hien thi khong dich duoc.
        /// </remarks>
        private static string TranslateViolationType(string violationType)
        {
            switch (violationType)
            {
                case "ContentModified":
                    return "Nội dung bị sửa";
                case "FileMissing":
                    return "Tệp bị xóa";
                case "UnexpectedNewFile":
                    return "Tệp mới xuất hiện";
                default:
                    return violationType;
            }
        }

        /// <summary>
        /// Hau to file luu hash cua mot bao cao dieu tra da xuat - xem "QUYET
        /// DINH THIET KE - HASH CUA BAO CAO" o remarks tai ExportInvestigationReport.
        /// </summary>
        public const string ReportHashFileSuffix = ".sha256";

        /// <summary>
        /// Lay duong dan file .sha256 tuong ung voi MOT file bao cao dieu tra
        /// da xuat (VD "C:\bc\baocao.csv" -> "C:\bc\baocao.csv.sha256") - dung
        /// chung boi ca WriteReportHashFile (ghi) va VerifyExportedReportHash
        /// (doc lai) de dam bao LUON khop nhau MOT quy uoc dat ten duy nhat.
        /// </summary>
        public static string GetReportHashFilePath(string reportFilePath)
        {
            return reportFilePath + ReportHashFileSuffix;
        }

        /// <summary>
        /// Ghi file .sha256 di kem MOT bao cao dieu tra da xuat - xem "QUYET
        /// DINH THIET KE - HASH CUA BAO CAO" o remarks tai ExportInvestigationReport
        /// de biet day du ly do/dinh dang. Duoc goi TU DONG ngay sau khi
        /// ExportInvestigationReport ghi xong file CSV - noi goi (VD LogForm)
        /// KHONG can tu goi rieng ham nay trong luong xuat bao cao binh
        /// thuong.
        /// </summary>
        /// <remarks>
        /// Them mot dong tieu de tieng Viet bat dau bang "#" NGAY TRUOC dong
        /// checksum chuan - cac cong cu checksum pho bien (VD sha256sum) BO
        /// QUA cac dong bat dau bang "#" khi doi chieu (coi la chu thich),
        /// nen dong nay KHONG lam hong kha nang doi chieu bang cong cu ngoai,
        /// ma van giup nguoi tu mo file .sha256 bang mat hieu ngay day la gi
        /// (thay vi chi thay mot dong hash + ten file kho hieu doi voi nguoi
        /// khong quen dinh dang checksum).
        /// </remarks>
        private static void WriteReportHashFile(string reportFilePath, string reportHash, int violationCount)
        {
            string hashFilePath = GetReportHashFilePath(reportFilePath);
            string reportFileName = Path.GetFileName(reportFilePath);

            using (var writer = new StreamWriter(hashFilePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)))
            {
                writer.WriteLine("# Hash SHA-256 cua báo cáo điều tra \"" + reportFileName + "\" (" + violationCount.ToString(CultureInfo.InvariantCulture) + " vi phạm), tính lúc xuất: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                writer.WriteLine("# Đối chiếu bằng lệnh: sha256sum -c \"" + reportFileName + ReportHashFileSuffix + "\" (hoặc dùng LogService.VerifyExportedReportHash trong ứng dụng).");
                writer.WriteLine(reportHash + "  " + reportFileName);
            }
        }

        /// <summary>
        /// DOI CHIEU LAI một bao cao dieu tra da xuat truoc do voi hash da
        /// luu kem (file .sha256 - xem WriteReportHashFile) - day chinh la
        /// chuc nang "doi chieu sau nay" nhu yeu cau: tinh lai hash HIEN TAI
        /// cua reportFilePath tren dia va so sanh voi hash da ghi nhan LUC
        /// XUAT, phat hien neu file bao cao da bi SUA DOI (vo tinh hoac co
        /// chu dich) ke tu luc xuat.
        /// </summary>
        /// <param name="reportFilePath">Duong dan file bao cao CSV da xuat truoc do (KHONG phai duong dan file .sha256).</param>
        /// <returns>
        /// Match: hash hien tai KHOP voi hash da luu - bao cao con NGUYEN VEN.
        /// Mismatch: hash KHAC nhau - bao cao DA BI THAY DOI ke tu luc xuat.
        /// ReportFileNotFound: khong tim thay file bao cao tai reportFilePath.
        /// HashFileNotFound: tim thay file bao cao nhung KHONG tim thay file
        ///   .sha256 di kem (VD: bao cao duoc xuat boi phien ban ung dung CU
        ///   truoc khi co tinh nang nay, hoac file .sha256 da bi xoa/di
        ///   chuyen rieng).
        /// Error: khong doc/tinh hash duoc vi ly do khac (mat quyen, file
        ///   dang bi khoa boi chuong trinh khac...).
        /// </returns>
        public ReportHashVerificationResult VerifyExportedReportHash(string reportFilePath)
        {
            if (string.IsNullOrWhiteSpace(reportFilePath) || !File.Exists(reportFilePath))
                return ReportHashVerificationResult.ReportFileNotFound;

            string hashFilePath = GetReportHashFilePath(reportFilePath);
            if (!File.Exists(hashFilePath))
                return ReportHashVerificationResult.HashFileNotFound;

            try
            {
                string savedHash = ReadSavedReportHash(hashFilePath);
                if (savedHash == null)
                    return ReportHashVerificationResult.Error; // File .sha256 ton tai nhung khong doc duoc dong hash hop le (VD bi sua tay sai dinh dang).

                string currentHash = HashHelper.ComputeSha256(reportFilePath);
                return string.Equals(savedHash, currentHash, StringComparison.OrdinalIgnoreCase)
                    ? ReportHashVerificationResult.Match
                    : ReportHashVerificationResult.Mismatch;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                return ReportHashVerificationResult.Error;
            }
        }

        /// <summary>
        /// Doc gia tri hash tu file .sha256 (bo qua cac dong chu thich bat
        /// dau bang "#" va dong trong do WriteReportHashFile ghi them) - tra
        /// ve null neu khong tim thay dong nao dung dinh dang checksum hop le
        /// ("&lt;hash&gt;  &lt;ten file&gt;").
        /// </summary>
        private static string ReadSavedReportHash(string hashFilePath)
        {
            foreach (string line in File.ReadAllLines(hashFilePath, Encoding.UTF8))
            {
                string trimmed = line.Trim();
                if (trimmed.Length == 0 || trimmed.StartsWith("#", StringComparison.Ordinal))
                    continue;

                // Dinh dang chuan "sha256sum": "<hash hex>  <ten file>" (hai
                // khoang trang, nhung chi tach theo KHOANG TRANG DAU TIEN de
                // an toan neu ten file sau do co chua khoang trang).
                int separatorIndex = trimmed.IndexOf(' ');
                string candidateHash = separatorIndex > 0 ? trimmed.Substring(0, separatorIndex) : trimmed;

                // Hash SHA-256 luon la CHUOI HEX 64 KY TU - kiem tra so bo de
                // tranh nham mot dong khac dinh dang thanh hash.
                if (candidateHash.Length == 64)
                {
                    return candidateHash;
                }
            }

            return null;
        }
    }

    /// <summary>
    /// Ket qua doi chieu hash cua mot bao cao dieu tra da xuat voi hash da
    /// luu kem luc xuat - xem LogService.VerifyExportedReportHash.
    /// </summary>
    public enum ReportHashVerificationResult
    {
        /// <summary>Hash hien tai khop voi hash da luu - bao cao con nguyen ven.</summary>
        Match,

        /// <summary>Hash khac nhau - bao cao da bi thay doi ke tu luc xuat.</summary>
        Mismatch,

        /// <summary>Khong tim thay file bao cao.</summary>
        ReportFileNotFound,

        /// <summary>Tim thay file bao cao nhung khong co file .sha256 di kem.</summary>
        HashFileNotFound,

        /// <summary>Loi khac khi doi chieu (mat quyen, file dang bi khoa...).</summary>
        Error
    }
}
