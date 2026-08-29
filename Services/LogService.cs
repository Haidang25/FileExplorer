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
    }
}
