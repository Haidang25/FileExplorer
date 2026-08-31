using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Properties;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Tao, luu va doc lai "baseline" (anh chup trang thai goc) cua mot thu
    /// muc - buoc dau tien cua tinh nang giam sat toan ven thu muc: KHI BAT
    /// DAU GIAM SAT mot thu muc, ung dung goi CreateBaselineAsync roi
    /// SaveBaseline MOT LAN de ghi lai hash SHA-256 cua tat ca file hien co,
    /// dung lam moc so sanh cho CAC LAN KIEM TRA SAU qua CompareWithBaselineAsync
    /// (nhan mot baseline vua tao hoac vua doc lai qua LoadBaseline, tra ve
    /// phan loai Unchanged/ContentModified/Deleted/NewFile cho tung file -
    /// xem <see cref="FileIntegrityStatus"/>).
    /// </summary>
    /// <remarks>
    /// TACH RIENG khoi FileMonitorService CO CHU DICH: FileMonitorService chi
    /// co trach nhiem "theo doi VA BAO su kien tho" (Created/Deleted/Changed/
    /// Renamed tu FileSystemWatcher, dung cho tu dong lam moi giao dien - xem
    /// remarks tai do), hoan toan KHONG biet gi ve hash/noi dung file.
    /// BaselineService la mot khai niem KHAC ve ban chat (chup + luu tru mot
    /// snapshot CO CAU TRUC de so sanh sau nay, khong phai phan ung tuc thi
    /// voi tung su kien rieng le) nen duoc tach thanh mot Service doc lap,
    /// giu dung nguyen tac "mot Service - mot trach nhiem" da ap dung xuyen
    /// suot du an (vi du DuplicateService tach rieng khoi SearchService du
    /// dung chung logic duyet thu muc, xem remarks tai DuplicateService).
    /// Noi goi (VD: mot handler "Bat dau giam sat" trong MainForm, se lam o
    /// mot yeu cau khac) co the goi CA HAI: FileMonitorService.StartMonitoring
    /// (theo doi thay doi thoi gian thuc) VA BaselineService (chup baseline
    /// mot lan) mot cach doc lap, khong Service nao phu thuoc Service kia.
    ///
    /// QUYET DINH THIET KE - DINH DANG LUU: CSV, cung ly do da neu chi tiet
    /// tai LogService (xem "QUYET DINH THIET KE" o remarks dau lop do): khong
    /// can them NuGet package (du an .NET Framework 4.7.2 khong co san
    /// System.Text.Json), nguoi dung cuoi co the tu mo file .csv bang Excel
    /// de xem thu neu muon. Khac voi LogService (moi dong la MOT thao tac,
    /// APPEND lien tuc theo thoi gian), baseline la MOT SNAPSHOT DUY NHAT tai
    /// mot thoi diem - moi lan SaveBaseline GHI DE HOAN TOAN file cu (khong
    /// append), vi bat dau giam sat lai MOT thu muc dong nghia "lam lai tu
    /// dau", baseline cu khong con y nghia gi nua.
    ///
    /// Cau truc file (moi dong ket thuc bang xuong dong, escape RFC 4180
    /// giong EscapeCsvField/ParseCsvLine cua LogService, CHU Y day la ban SAO
    /// CO CHU DICH - khong dung chung code voi LogService de BaselineService
    /// khong phu thuoc LogService, xem "TACH RIENG" o tren):
    ///   Dong 1 (METADATA - CHI MOT DONG DUY NHAT, KHONG PHAI header cot):
    ///     MetadataRowMarker,FolderPath,IncludeSubdirectories,CreatedAtUtc
    ///     VD: #BASELINE,"D:\Du an\Anh",True,2026-08-31T10:00:00.0000000Z
    ///     (MetadataRowMarker = "#BASELINE" - AN TOAN lam sentinel de phan
    ///     biet dong nay voi dong du lieu file, vi mot duong dan Windows
    ///     TUYET DOI khong bao gio bat dau bang "#").
    ///   Dong 2 (HEADER ten cot, cho DU LIEU FILE tu dong 3 tro di):
    ///     FilePath,Size,LastWriteTimeUtc,Hash
    ///   Dong 3+: mot dong = mot FileBaselineEntry, escape FilePath theo RFC
    ///     4180 (duong dan Windows CO THE chua dau phay - VD "C:\Bao cao, thang 8\a.txt"),
    ///     Size la so nguyen, LastWriteTimeUtc dinh dang "o" (round-trip, xem
    ///     ly do tai LogService.FormatCsvRow), Hash la chuoi hex SHA-256.
    ///
    /// QUYET DINH THIET KE - VI TRI LUU VA TEN FILE: mot file baseline RIENG
    /// cho MOI thu muc duoc giam sat (khong phai MOT file baseline duy nhat
    /// dung chung), dat trong AppData (Settings.Default.BaselinePath, mac
    /// dinh "%AppData%\SFileManager\baselines" - cung ly do chon AppData nhu
    /// LogService.LogPath: luon ghi duoc khong can quyen Administrator, rieng
    /// theo tung tai khoan Windows, khong mat khi go cai dat/cap nhat ung
    /// dung). Ten file duoc tao tu MD5 cua duong dan thu muc DA CHUAN HOA
    /// (Path.GetFullPath + ToLowerInvariant, xem GetBaselineFilePath) ket hop
    /// tien to la ten thu muc (de nguoi dung mo thu muc AppData\baselines
    /// bang tay van nhan ra file nao ung voi thu muc nao) - dung HASH (khong
    /// phai tu duong dan goc thay ky tu khong hop le thanh "_") vi cach thay
    /// the truc tiep DE GAY TRUNG TEN file giua 2 thu muc KHAC NHAU nhung
    /// giong nhau sau khi thay the (VD "C:\Báo cáo" va "C:\Báo:cáo" cung ra
    /// "C__Báo_cáo" neu chi thay the don gian) - hash dam bao MOI duong dan
    /// KHAC NHAU (kem ca chi khac hoa/thuong, da duoc ToLowerInvariant truoc
    /// khi hash de tranh truong hop Windows coi 2 duong dan chi khac hoa/
    /// thuong la CUNG mot thu muc nhung lai ra 2 file baseline khac nhau) deu
    /// ra ten file RIENG BIET, khong the trung.
    /// </remarks>
    public enum FileIntegrityStatus
    {
        /// <summary>Hash SHA-256 hien tai KHOP voi baseline - file khong doi.</summary>
        Unchanged,

        /// <summary>File CO trong baseline VA van con tren dia, nhung hash SHA-256 hien tai KHAC baseline - noi dung DA BI SUA.</summary>
        ContentModified,

        /// <summary>File CO trong baseline nhung khong con tim thay tren dia - da bi XOA (hoac o dia/thu muc chua no khong con truy cap duoc).</summary>
        Deleted,

        /// <summary>File dang co tren dia nhung KHONG CO trong baseline - MOI XUAT HIEN sau thoi diem baseline duoc chup.</summary>
        NewFile
    }

    /// <summary>
    /// Ket qua phan loai MOT file sau khi so sanh voi baseline - xem
    /// <see cref="BaselineService.CompareWithBaselineAsync"/>.
    /// </summary>
    public class FileIntegrityCheckResult
    {
        /// <summary>Duong dan file duoc phan loai.</summary>
        public string FilePath { get; set; }

        /// <summary>Phan loai ket qua - xem <see cref="FileIntegrityStatus"/>.</summary>
        public FileIntegrityStatus Status { get; set; }

        /// <summary>
        /// Hash SHA-256 theo baseline (GOC) - null voi <see cref="FileIntegrityStatus.NewFile"/>
        /// (file nay chua tung co trong baseline nen khong co gia tri "mong doi" nao ca).
        /// </summary>
        public string ExpectedHash { get; set; }

        /// <summary>
        /// Hash SHA-256 tinh duoc TAI THOI DIEM so sanh - null voi
        /// <see cref="FileIntegrityStatus.Deleted"/> (file khong con de hash).
        /// </summary>
        public string ActualHash { get; set; }
    }

    public class BaselineService
    {
        /// <summary>Phan mo rong file baseline - xem "QUYET DINH THIET KE - DINH DANG LUU" o remarks tren dau lop.</summary>
        public const string BaselineFileExtension = ".csv";

        /// <summary>Gia tri cot dau tien cua dong METADATA (dong 1) - xem "Cau truc file" o remarks tren dau lop.</summary>
        private const string MetadataRowMarker = "#BASELINE";

        /// <summary>Dong header ten cot cho phan DU LIEU FILE (tu dong 3 tro di) - xem "Cau truc file" o remarks tren dau lop.</summary>
        public const string BaselineFileHeader = "FilePath,Size,LastWriteTimeUtc,Hash";

        /// <summary>
        /// Khoang cach toi thieu giua 2 lan goi progress.Report trong
        /// CreateBaselineAsync - giong DuplicateService.ProgressReportInterval,
        /// tranh goi Report qua day dac voi thu muc nhieu file nho.
        /// </summary>
        private static readonly TimeSpan ProgressReportInterval = TimeSpan.FromMilliseconds(150);

        // Composition voi SearchService, cung ly do DuplicateService da giai
        // thich trong remarks cua no: tai su dung logic duyet thu muc (bo qua
        // loi quyen/IO tren tung nhanh rieng le) da co san, khong tu viet lai.
        private readonly SearchService _searchService;

        public BaselineService() : this(new SearchService())
        {
        }

        /// <summary>Cho phep truyen SearchService tu ben ngoai (dependency injection/unit test) - xem DuplicateService(SearchService).</summary>
        public BaselineService(SearchService searchService)
        {
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        /// <summary>
        /// Quet toan bo file trong mot thu muc va tinh hash SHA-256 cho tung
        /// file, tra ve MOT FolderBaselineModel moi (CHUA duoc luu ra dia -
        /// goi SaveBaseline rieng de luu, xem <see cref="SaveBaseline"/>) -
        /// day la buoc "chup anh" khi BAT DAU giam sat mot thu muc.
        /// </summary>
        /// <param name="folderPath">Thu muc can chup baseline.</param>
        /// <param name="includeSubdirectories">True: bao gom ca file trong thu muc con (de quy).</param>
        /// <param name="cancellationToken">
        /// Cho phep huy giua chung (VD: nguoi dung dong hop thoai "Dang tao
        /// baseline..." truoc khi quet xong mot thu muc rat lon).
        /// </param>
        /// <param name="progress">
        /// Bao cao so file DA XU LY XONG (ke ca file loi/bi bo qua rieng, xem
        /// remarks) - tuy chon, thuong la Progress&lt;int&gt; tao tren luong UI
        /// giong quy uoc cua DuplicateService.FindDuplicateFiles.
        /// </param>
        /// <exception cref="ArgumentException">folderPath rong/null hoac khong ton tai.</exception>
        /// <remarks>
        /// Loi hash MOT file rieng le (mat quyen doc, file bi khoa, file vua
        /// bi xoa giua luc quet) chi LOAI FILE DO khoi baseline (khong co
        /// nghia la "khong doi" - don gian la KHONG THE xac nhan, nen tot hon
        /// la BO QUA thay vi ghi mot hash sai/gia) va KHONG lam hong toan bo
        /// qua trinh chup baseline cho CA thu muc - giong nguyen tac loi tren
        /// tung nhanh khong lam dung ca qua trinh da ap dung xuyen suot du an
        /// (xem DuplicateService.FindDuplicateFiles).
        /// </remarks>
        public async Task<FolderBaselineModel> CreateBaselineAsync(
            string folderPath, bool includeSubdirectories,
            CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                throw new ArgumentException($"Thư mục không tồn tại: {folderPath}", nameof(folderPath));

            var baseline = new FolderBaselineModel
            {
                FolderPath = folderPath,
                IncludeSubdirectories = includeSubdirectories,
                CreatedAtUtc = DateTime.UtcNow
            };

            int processedCount = 0;
            Stopwatch progressStopwatch = progress != null ? Stopwatch.StartNew() : null;

            foreach (FileItemModel item in _searchService.Search(
                folderPath, "*", includeSubdirectories, includeHidden: true, cancellationToken: cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (item.IsDirectory)
                    continue; // Baseline chi quan tam NOI DUNG file - thu muc khong co gi de hash (xem FolderBaselineModel.Entries).

                try
                {
                    string hash = await HashHelper.ComputeSha256Async(item.FullPath, cancellationToken).ConfigureAwait(false);
                    baseline.Entries.Add(new FileBaselineEntry
                    {
                        FilePath = item.FullPath,
                        Size = item.Size,
                        LastWriteTimeUtc = File.GetLastWriteTimeUtc(item.FullPath),
                        Hash = hash
                    });
                }
                catch (UnauthorizedAccessException) { } // Mat quyen doc file nay - bo qua rieng, xem <remarks>.
                catch (FileNotFoundException) { } // File vua bi xoa giua luc quet - phai dat TRUOC IOException vi la lop con cua no.
                catch (IOException) { } // VD: file dang bi khoa boi chuong trinh khac.
                finally
                {
                    processedCount++;

                    if (progressStopwatch != null && progressStopwatch.Elapsed >= ProgressReportInterval)
                    {
                        progress.Report(processedCount);
                        progressStopwatch.Restart();
                    }
                }
            }

            // Bao cao MOC HOAN TAT (khong throttle) - dam bao lan Report cuoi
            // cung luon phan anh dung tong so file da xu ly, giong quy uoc cua
            // DuplicateService.FindDuplicateFiles.
            progress?.Report(processedCount);

            return baseline;
        }

        /// <summary>
        /// So sanh HIEN TRANG THUC TE cua thu muc (baseline.FolderPath) VOI
        /// mot baseline da co (vua tao boi CreateBaselineAsync, hoac doc lai
        /// tu dia boi LoadBaseline) - day la buoc "KIEM TRA SAU" ma phan
        /// <summary> dau lop da nhac se lam o mot yeu cau khac; gio da co.
        /// Tra ve MOT FileIntegrityCheckResult CHO MOI file lien quan (ca
        /// file trong baseline LAN file moi phat sinh), phan loai theo dung
        /// 4 nhom <see cref="FileIntegrityStatus"/>: Unchanged/ContentModified/
        /// Deleted/NewFile.
        /// </summary>
        /// <param name="baseline">Baseline can so sanh (baseline.FolderPath xac dinh thu muc can quet lai, baseline.IncludeSubdirectories xac dinh pham vi - PHAI khop voi luc tao baseline).</param>
        /// <param name="cancellationToken">Cho phep huy giua chung (VD: nguoi dung dong man hinh "Dang kiem tra..." truoc khi quet xong thu muc rat lon).</param>
        /// <param name="progress">Bao cao so file DA XU LY XONG (ca file trong baseline LAN file moi phat hien) - tuy chon.</param>
        /// <exception cref="ArgumentNullException">baseline la null.</exception>
        /// <remarks>
        /// THUAT TOAN (2 buoc, tuong tu cach CreateBaselineAsync liet ke file):
        ///
        /// - Buoc 1: liet ke TOAN BO file HIEN TAI trong baseline.FolderPath
        ///   (dung SearchService, cung pham vi IncludeSubdirectories nhu luc
        ///   tao baseline) vao mot HashSet de tra cuu nhanh "file nay CO CON
        ///   TON TAI khong" o Buoc 2, VA de biet file nao la MOI (khong co
        ///   trong baseline) cho Buoc 3.
        ///
        /// - Buoc 2: VOI TUNG entry trong baseline.Entries - neu file khong
        ///   con tren dia (File.Exists false) => Deleted (ActualHash = null).
        ///   Neu con, hash lai (HashHelper.ComputeSha256Async) va so sanh voi
        ///   entry.Hash => Unchanged (khop) hoac ContentModified (khac).
        ///
        /// - Buoc 3: VOI TUNG file HIEN TAI o Buoc 1 nhung KHONG co trong
        ///   baseline.Entries => NewFile. KHONG can hash file nay (khong co
        ///   gi de so sanh - ExpectedHash/ActualHash deu null), tranh chi phi
        ///   hash khong can thiet cho file von da chac chan la "moi".
        ///
        /// Loi hash MOT file rieng le o Buoc 2 (mat quyen doc, file bi khoa)
        /// khien file do bi BO QUA HOAN TOAN (KHONG them vao ket qua voi mot
        /// trang thai "doan mo" nao ca) thay vi doan sai thanh Unchanged hay
        /// ContentModified - giong nguyen tac loi tren tung nhanh khong lam
        /// dung ca qua trinh, ap dung xuyen suot du an (xem
        /// DuplicateService.FindDuplicateFiles, CreateBaselineAsync o tren).
        /// Neu ca thu muc goc (baseline.FolderPath) khong con ton tai, TOAN
        /// BO entry trong baseline duoc bao Deleted NGAY (khong can quet gi
        /// them - thu muc da mat thi moi file trong do chac chan cung mat).
        /// </remarks>
        public async Task<List<FileIntegrityCheckResult>> CompareWithBaselineAsync(
            FolderBaselineModel baseline, CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            if (baseline == null)
                throw new ArgumentNullException(nameof(baseline));

            var results = new List<FileIntegrityCheckResult>();

            if (string.IsNullOrWhiteSpace(baseline.FolderPath) || !Directory.Exists(baseline.FolderPath))
            {
                // Ca thu muc goc da mat - TOAN BO file trong baseline chac chan cung mat, xem <remarks>.
                foreach (FileBaselineEntry missingEntry in baseline.Entries)
                {
                    results.Add(new FileIntegrityCheckResult
                    {
                        FilePath = missingEntry.FilePath,
                        Status = FileIntegrityStatus.Deleted,
                        ExpectedHash = missingEntry.Hash,
                        ActualHash = null
                    });
                }
                return results;
            }

            // Buoc 1: liet ke TOAN BO file HIEN TAI - xem <remarks>.
            var currentFilePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (FileItemModel item in _searchService.Search(
                baseline.FolderPath, "*", baseline.IncludeSubdirectories, includeHidden: true, cancellationToken: cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (!item.IsDirectory)
                {
                    currentFilePaths.Add(item.FullPath);
                }
            }

            var baselinePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (FileBaselineEntry entry in baseline.Entries)
            {
                baselinePaths.Add(entry.FilePath);
            }

            int processedCount = 0;
            Stopwatch progressStopwatch = progress != null ? Stopwatch.StartNew() : null;

            // Buoc 2: kiem tra tung file DA CO trong baseline - Deleted/Unchanged/ContentModified.
            foreach (FileBaselineEntry entry in baseline.Entries)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    if (!currentFilePaths.Contains(entry.FilePath))
                    {
                        results.Add(new FileIntegrityCheckResult
                        {
                            FilePath = entry.FilePath,
                            Status = FileIntegrityStatus.Deleted,
                            ExpectedHash = entry.Hash,
                            ActualHash = null
                        });
                        continue;
                    }

                    string actualHash = await HashHelper.ComputeSha256Async(entry.FilePath, cancellationToken).ConfigureAwait(false);
                    bool isUnchanged = string.Equals(actualHash, entry.Hash, StringComparison.OrdinalIgnoreCase);

                    results.Add(new FileIntegrityCheckResult
                    {
                        FilePath = entry.FilePath,
                        Status = isUnchanged ? FileIntegrityStatus.Unchanged : FileIntegrityStatus.ContentModified,
                        ExpectedHash = entry.Hash,
                        ActualHash = actualHash
                    });
                }
                catch (UnauthorizedAccessException) { } // Mat quyen doc file nay - bo qua RIENG file nay, xem <remarks>.
                catch (FileNotFoundException) { } // File vua bi xoa giua luc so sanh (sau khi da qua kiem tra currentFilePaths.Contains o tren) - phai dat TRUOC IOException vi la lop con cua no.
                catch (IOException) { } // VD: file dang bi khoa boi chuong trinh khac dung luc hash.
                finally
                {
                    processedCount++;

                    if (progressStopwatch != null && progressStopwatch.Elapsed >= ProgressReportInterval)
                    {
                        progress.Report(processedCount);
                        progressStopwatch.Restart();
                    }
                }
            }

            // Buoc 3: file HIEN TAI nhung KHONG co trong baseline - NewFile, xem <remarks>.
            foreach (string currentPath in currentFilePaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (baselinePaths.Contains(currentPath))
                    continue;

                results.Add(new FileIntegrityCheckResult
                {
                    FilePath = currentPath,
                    Status = FileIntegrityStatus.NewFile,
                    ExpectedHash = null,
                    ActualHash = null
                });

                processedCount++;

                if (progressStopwatch != null && progressStopwatch.Elapsed >= ProgressReportInterval)
                {
                    progress.Report(processedCount);
                    progressStopwatch.Restart();
                }
            }

            progress?.Report(processedCount);

            return results;
        }

        /// <summary>
        /// Ghi mot FolderBaselineModel (thuong vua duoc tao boi
        /// CreateBaselineAsync) ra file CSV tren dia, GHI DE HOAN TOAN baseline
        /// cu cua CUNG thu muc do neu co - xem "QUYET DINH THIET KE - DINH
        /// DANG LUU" o remarks dau lop de biet ly do (khac voi LogService,
        /// baseline la MOT SNAPSHOT, khong phai nhat ky append-only).
        /// </summary>
        /// <exception cref="ArgumentNullException">baseline la null.</exception>
        /// <exception cref="ArgumentException">baseline.FolderPath rong/null.</exception>
        public void SaveBaseline(FolderBaselineModel baseline)
        {
            if (baseline == null)
                throw new ArgumentNullException(nameof(baseline));

            if (string.IsNullOrWhiteSpace(baseline.FolderPath))
                throw new ArgumentException("FolderBaselineModel.FolderPath không được để trống.", nameof(baseline));

            string filePath = GetBaselineFilePath(baseline.FolderPath);
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // FileMode.Create (khong phai Append nhu LogService.WriteLog) - CO
            // CHU DICH ghi de toan bo file cu, xem ly do o remarks dau lop.
            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            using (var writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                writer.WriteLine(FormatMetadataRow(baseline));
                writer.WriteLine(BaselineFileHeader);

                foreach (FileBaselineEntry entry in baseline.Entries)
                {
                    writer.WriteLine(FormatEntryRow(entry));
                }
            }
        }

        /// <summary>
        /// Doc lai baseline da luu cua mot thu muc (neu co) - dung boi cac lan
        /// kiem tra sau nay de so sanh voi hien trang thu muc (chua trien
        /// khai trong buoc "thiet ke" nay, xem <summary> dau lop).
        /// </summary>
        /// <returns>
        /// FolderBaselineModel doc duoc, hoac null neu CHUA TUNG luu baseline
        /// cho thu muc nay, hoac file baseline bi hong/khong doc duoc (loi
        /// duoc NUOT va coi nhu chua co baseline - an toan hon nem ngoai va
        /// lam sap noi goi chi vi mot file baseline cu bi hong).
        /// </returns>
        public FolderBaselineModel LoadBaseline(string folderPath)
        {
            string filePath = GetBaselineFilePath(folderPath);
            if (!File.Exists(filePath))
                return null;

            try
            {
                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                if (lines.Length < 2)
                    return null; // Thieu it nhat dong metadata + dong header - file hong/rong bat thuong.

                string[] metaFields = ParseCsvLine(lines[0]);
                if (metaFields.Length < 4 || metaFields[0] != MetadataRowMarker)
                    return null; // Dong dau khong dung dinh dang metadata mong doi - coi nhu khong doc duoc.

                var baseline = new FolderBaselineModel
                {
                    FolderPath = metaFields[1],
                    IncludeSubdirectories = bool.Parse(metaFields[2]),
                    CreatedAtUtc = DateTime.Parse(metaFields[3], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                };

                // Bat dau tu dong INDEX 2 (dong thu 3) - bo qua dong 0 (metadata)
                // VA dong 1 (header ten cot).
                for (int i = 2; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i]))
                        continue;

                    string[] fields = ParseCsvLine(lines[i]);
                    if (fields.Length < 4)
                        continue; // Dong hong/thieu cot - bo qua RIENG dong nay, khong lam hong ca baseline.

                    baseline.Entries.Add(new FileBaselineEntry
                    {
                        FilePath = fields[0],
                        Size = long.Parse(fields[1], CultureInfo.InvariantCulture),
                        LastWriteTimeUtc = DateTime.Parse(fields[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                        Hash = fields[3]
                    });
                }

                return baseline;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException
                || ex is FormatException || ex is OverflowException)
            {
                return null; // File baseline hong/khong doc duoc - xem <returns>.
            }
        }

        /// <summary>
        /// Xoa baseline da luu cua mot thu muc (neu co) - dung khi nguoi dung
        /// chu dong "Dung giam sat va xoa baseline" (se lam o mot yeu cau
        /// khac). An toan khi goi voi thu muc CHUA TUNG co baseline (khong
        /// lam gi ca, khong nem loi).
        /// </summary>
        /// <returns>True neu da xoa (hoac khong co gi de xoa), false neu xoa that bai (VD: file dang bi khoa).</returns>
        public bool DeleteBaseline(string folderPath)
        {
            try
            {
                string filePath = GetBaselineFilePath(folderPath);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                return false;
            }
        }

        /// <summary>
        /// Xac dinh duong dan file baseline TUONG UNG voi mot thu muc - xem
        /// "QUYET DINH THIET KE - VI TRI LUU VA TEN FILE" o remarks dau lop
        /// de biet day du ly do chon cach dat ten nay (hash cua duong dan da
        /// chuan hoa, khong phai thay the ky tu truc tiep).
        /// </summary>
        public string GetBaselineFilePath(string folderPath)
        {
            // Path.GetFullPath: chuan hoa duong dan tuong doi/co ".."/dau "\"
            // thua thanh dang tuyet doi day du, dam bao 2 cach viet KHAC NHAU
            // cua CUNG mot thu muc (VD "D:\A\..\A\Anh" va "D:\A\Anh") ra CUNG
            // mot ten file baseline. TrimEnd dau phan cach: tranh "D:\Anh" va
            // "D:\Anh\" (ve ban chat la MOT thu muc) bi tinh la 2 duong dan
            // khac nhau chi vi dau "\" cuoi. ToLowerInvariant: Windows KHONG
            // phan biet hoa/thuong trong duong dan, "D:\Anh" va "d:\anh" phai
            // ra CUNG mot file baseline.
            string normalizedPath = Path.GetFullPath(folderPath)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                .ToLowerInvariant();

            string pathHash;
            using (var pathStream = new MemoryStream(Encoding.UTF8.GetBytes(normalizedPath)))
            {
                // Dung lai HashHelper.ComputeMd5(Stream,...) (MD5 la du, chi can
                // NHAN DIEN duy nhat mot chuoi duong dan lam ten file - KHONG
                // phai boi canh chong gia mao noi dung nhu HashHelper.ComputeSha256
                // dung cho tung FileBaselineEntry ben tren) qua MemoryStream bao
                // boc chuoi duong dan, tranh viet lai mot ham hash chuoi rieng.
                pathHash = HashHelper.ComputeMd5(pathStream, CancellationToken.None);
            }

            // Tien to de nguoi dung mo thu muc AppData\baselines bang tay van
            // nhan ra so bo file nao ung voi thu muc nao - CHI mang tinh tham
            // khao/de doc, KHONG phai phan dam bao duy nhat (do la trach nhiem
            // cua pathHash o tren, xem remarks dau lop).
            string folderName = new DirectoryInfo(folderPath).Name;
            if (string.IsNullOrEmpty(folderName))
            {
                folderName = "root"; // VD thu muc goc mot o dia (DirectoryInfo("C:\\").Name tra ve "C:\\").
            }

            string fileName = $"{SanitizeFileNameComponent(folderName)}_{pathHash.Substring(0, 12)}{BaselineFileExtension}";
            return Path.Combine(GetBaselineDirectory(), fileName);
        }

        /// <summary>
        /// Thay TUNG ky tu KHONG HOP LE trong ten file Windows (xem
        /// Path.GetInvalidFileNameChars, VD ':' '\\' '/' '*' '?') bang '_' -
        /// dung cho phan TIEN TO de-doc cua ten file baseline (xem
        /// GetBaselineFilePath), KHONG anh huong tinh duy nhat cua ten file
        /// (da duoc dam bao boi pathHash di kem, xem remarks dau lop).
        /// </summary>
        private static string SanitizeFileNameComponent(string component)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            var builder = new StringBuilder(component.Length);
            foreach (char c in component)
            {
                builder.Append(Array.IndexOf(invalidChars, c) >= 0 ? '_' : c);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Thu muc chua cac file baseline - doc tu Settings.Default.BaselinePath
        /// (co the chua bien moi truong nhu "%AppData%\SFileManager\baselines")
        /// roi Environment.ExpandEnvironmentVariables, giong het cach
        /// LogService.GetLogFilePath doc Settings.Default.LogPath.
        /// </summary>
        private static string GetBaselineDirectory()
        {
            string configuredPath = Settings.Default.BaselinePath;
            if (string.IsNullOrWhiteSpace(configuredPath))
            {
                configuredPath = @"%AppData%\SFileManager\baselines";
            }

            return Environment.ExpandEnvironmentVariables(configuredPath);
        }

        /// <summary>Dinh dang dong METADATA (dong 1 cua file) - xem "Cau truc file" o remarks dau lop.</summary>
        private static string FormatMetadataRow(FolderBaselineModel baseline)
        {
            string[] fields =
            {
                MetadataRowMarker,
                EscapeCsvField(baseline.FolderPath),
                baseline.IncludeSubdirectories.ToString(CultureInfo.InvariantCulture),
                baseline.CreatedAtUtc.ToString("o", CultureInfo.InvariantCulture)
            };
            return string.Join(",", fields);
        }

        /// <summary>Dinh dang MOT dong du lieu file (tu dong 3 tro di) - xem "Cau truc file" o remarks dau lop.</summary>
        private static string FormatEntryRow(FileBaselineEntry entry)
        {
            string[] fields =
            {
                EscapeCsvField(entry.FilePath),
                entry.Size.ToString(CultureInfo.InvariantCulture),
                entry.LastWriteTimeUtc.ToString("o", CultureInfo.InvariantCulture),
                entry.Hash
            };
            return string.Join(",", fields);
        }

        /// <summary>
        /// Escape MOT truong CSV theo RFC 4180 - BAN SAO CO CHU DICH cua
        /// LogService.EscapeCsvField (xem ly do khong dung chung code giua 2
        /// Service tai "TACH RIENG" o remarks dau lop). Chi FilePath thuc su
        /// can qua ham nay (duong dan Windows co the chua dau phay), cac cot
        /// con lai (Size/LastWriteTimeUtc/Hash) khong the chua ky tu can
        /// escape nhung goi qua day van an toan/khong thay doi gi.
        /// </summary>
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
        /// Tach MOT dong CSV thanh mang cac truong, hieu dung phan bao quanh
        /// truong theo RFC 4180 (nguoc lai voi EscapeCsvField) - BAN SAO CO
        /// CHU DICH cua LogService.ParseCsvLine, xem ly do tai EscapeCsvField
        /// o tren. KHONG the dung don gian line.Split(',') vi FilePath co the
        /// tu chua dau phay da duoc bao trong dau ngoac kep luc ghi.
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
    }
}
