using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Phan loai mot vi pham toan ven duoc IntegrityService phat hien - xem
    /// <see cref="IntegrityViolationEventArgs"/>.
    /// </summary>
    public enum IntegrityViolationType
    {
        /// <summary>
        /// Noi dung file DA THAY DOI so voi baseline (hash SHA-256 hien tai
        /// khac hash da luu trong baseline) - vi pham NGHIEM TRONG NHAT, y
        /// nghia truc tiep cua tinh nang giam sat toan ven.
        /// </summary>
        ContentModified,

        /// <summary>
        /// File CO trong baseline nhung khong con tim thay tren dia (bi xoa,
        /// hoac bi doi ten sang duong dan khac ma IntegrityService coi duong
        /// dan CU la "mat" - xem OnFileRenamed).
        /// </summary>
        FileMissing,

        /// <summary>
        /// Phat hien mot file KHONG CO trong baseline (moi duoc tao, hoac
        /// xuat hien tu mot thao tac doi ten) SAU khi baseline da duoc chup -
        /// co the la hoat dong binh thuong (nguoi dung tao file moi hop le)
        /// hoac dang ngo (file la vi tri that xa la nao do bi dat vao thu muc
        /// dang giam sat) - IntegrityService chi BAO CAO, KHONG tu phan xet
        /// dung/sai, de noi nhan (UI) va nguoi dung tu quyet dinh.
        /// </summary>
        UnexpectedNewFile
    }

    /// <summary>
    /// Du lieu di kem su kien <see cref="IntegrityService.IntegrityViolationDetected"/>.
    /// </summary>
    public class IntegrityViolationEventArgs : EventArgs
    {
        /// <summary>Duong dan file lien quan den vi pham.</summary>
        public string FilePath { get; }

        /// <summary>Loai vi pham - xem <see cref="IntegrityViolationType"/>.</summary>
        public IntegrityViolationType ViolationType { get; }

        /// <summary>
        /// Hash SHA-256 mong doi (theo baseline) - null neu khong ap dung
        /// (VD UnexpectedNewFile: chua co gia tri "mong doi" nao ca).
        /// </summary>
        public string ExpectedHash { get; }

        /// <summary>
        /// Hash SHA-256 tinh duoc TAI THOI DIEM phat hien - null neu khong
        /// tinh duoc (VD FileMissing: file khong con de hash).
        /// </summary>
        public string ActualHash { get; }

        /// <summary>Thoi diem (UTC) IntegrityService phat hien vi pham nay.</summary>
        public DateTime DetectedAtUtc { get; }

        public IntegrityViolationEventArgs(string filePath, IntegrityViolationType violationType, string expectedHash, string actualHash)
        {
            FilePath = filePath;
            ViolationType = violationType;
            ExpectedHash = expectedHash;
            ActualHash = actualHash;
            DetectedAtUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Giam sat TOAN VEN (integrity) mot thu muc: ket hop theo doi thay doi
    /// THOI GIAN THUC cua FileMonitorService (lop cha) voi hash SHA-256 cua
    /// HashHelper de PHAT HIEN va BAO CAO khi noi dung file khac voi baseline
    /// da chup luc bat dau giam sat (xem BaselineService/FolderBaselineModel,
    /// yeu cau truoc do da thiet ke co che luu baseline).
    /// </summary>
    /// <remarks>
    /// VI SAO KE THUA (khong phai composition nhu DuplicateService/
    /// BaselineService dung voi SearchService): FileMonitorService duoc
    /// CHINH TAC GIA cua no thiet ke SAN cho muc dich mo rong nay - 5 phuong
    /// thuc OnFileCreated/OnFileDeleted/OnFileChanged/OnFileRenamed/
    /// OnMonitorError deu la "protected virtual" voi doc ro rang "Cho phep
    /// lop con (neu co) tuy bien" (xem FileMonitorService.cs). Day CHINH LA
    /// diem mo rong danh cho IntegrityService: thay vi dang ky lai 5 event
    /// cong khai (FileCreated/FileDeleted/...) nhu MainForm dang lam (xem
    /// MainForm.RegisterFileMonitorEvents), IntegrityService OVERRIDE truc
    /// tiep 4 hook On*(khong ke OnMonitorError, xem ghi chu ben duoi) de
    /// "chen" logic kiem tra hash NGAY TAI NGUON, truoc khi su kien tho duoc
    /// raise tiep ra ngoai (goi base.OnFileXxx(e) DAU TIEN trong moi override
    /// de KHONG lam mat hanh vi cu - noi dang ky FileCreated/FileDeleted/...
    /// hien co, VD MainForm.OnExternalChangeDetected, van hoat dong BINH
    /// THUONG KHONG DOI, ke ca khi dang dung mot IntegrityService thay vi
    /// mot FileMonitorService thuong).
    ///
    /// KHONG override OnMonitorError: loi giam sat (VD mat quyen, o dia bi
    /// rut) khong lien quan gi den toan ven noi dung file, xu ly y het lop
    /// cha la du, khong can them logic gi o day.
    ///
    /// StartMonitoring/StopMonitoring/Dispose CUA LOP CHA KHONG PHAI virtual
    /// (xem FileMonitorService.cs) nen KHONG THE (va KHONG NEN co gang)
    /// override - IntegrityService thay vao do THEM cac phuong thuc MOI voi
    /// TEN KHAC (StartIntegrityMonitoringAsync/ResumeIntegrityMonitoring/
    /// StopIntegrityMonitoring) goi lai StartMonitoring/StopMonitoring KE
    /// THUA ben trong, dong thoi quan ly them phan baseline. Noi goi VAN CO
    /// THE goi truc tiep StartMonitoring/StopMonitoring ke thua (VD ep kieu
    /// ve FileMonitorService) - khi do CHI theo doi thay doi tho, KHONG kiem
    /// tra toan ven (baseline se rong, moi Created/Changed deu bi coi la
    /// UnexpectedNewFile) - nen LUON dung StartIntegrityMonitoringAsync/
    /// ResumeIntegrityMonitoring thay vi goi thang StartMonitoring khi da
    /// khai bao la IntegrityService.
    ///
    /// LUONG (THREAD): giu nguyen canh bao cua FileMonitorService - cac
    /// override On* ben duoi (va vi vay CA event IntegrityViolationDetected)
    /// deu chay tren LUONG THREADPOOL cua FileSystemWatcher, KHONG PHAI luong
    /// UI. Noi dang ky IntegrityViolationDetected (VD mot Form hien canh bao)
    /// BAT BUOC phai tu Invoke/BeginInvoke truoc khi dung control WinForms,
    /// y het nguyen tac da ap dung voi FileCreated/FileDeleted/... cua lop
    /// cha. Gia dinh THEM (dac thu IntegrityService): FileSystemWatcher raise
    /// su kien CHO MOT INSTANCE watcher LAN LUOT (khong song song) theo tai
    /// lieu .NET, nen _baselineByPath duoc doc/ghi TUAN TU trong cac
    /// handler nay KHONG can khoa (lock) rieng - neu sau nay co nhu cau goi
    /// CurrentBaseline/CacViPhamGanDay tu luong UI SONG SONG voi luc dang
    /// giam sat, se can xem xet lai gia dinh nay.
    ///
    /// HIEU NANG: hash lai MOT file khi co Changed event duoc thuc hien
    /// DONG BO (HashHelper.ComputeSha256, khong phai ban Async) NGAY TRONG
    /// handler - CO Y, vi handler nay von da chay tren luong threadpool rieng
    /// cua FileSystemWatcher (khong phai luong UI, xem tren), nen "chan"
    /// luong nay trong luc hash KHONG lam giat giao dien. Danh doi: neu file
    /// vua sua RAT LON (VD video hang GB) hoac co RAT NHIEU thay doi don don
    /// xay ra gan nhu dong thoi, cac Changed event tiep theo se phai CHO hash
    /// hien tai xong moi duoc xu ly (FileSystemWatcher van xep hang cac su
    /// kien noi bo trong luc do, co nguy co InternalBufferOverflowException
    /// - xem OnMonitorError - neu bo dem day truoc khi kip xu ly). Chap nhan
    /// duoc cho pham vi tinh nang hien tai (thu muc can giam sat toan ven
    /// thuong khong phai noi dien ra thay doi dung luong lon lien tuc); hang
    /// doi xu ly nen (background queue + Task) cho truong hop nay se lam o
    /// mot yeu cau khac neu can.
    /// </remarks>
    public class IntegrityService : FileMonitorService
    {
        // Composition voi BaselineService (KHONG ke thua no - chi
        // FileMonitorService moi duoc thiet ke de ke thua, xem <remarks>) de
        // tai su dung nguyen ven co che tao/luu/doc baseline da co san.
        private readonly BaselineService _baselineService;

        // Tra cuu NHANH mot FileBaselineEntry theo duong dan day du - xay lai
        // MOI LAN ApplyBaseline duoc goi (StartIntegrityMonitoringAsync/
        // ResumeIntegrityMonitoring). OrdinalIgnoreCase vi duong dan Windows
        // khong phan biet hoa/thuong ("C:\A.txt" va "c:\a.txt" la CUNG mot file).
        private readonly Dictionary<string, FileBaselineEntry> _baselineByPath =
            new Dictionary<string, FileBaselineEntry>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Baseline dang duoc dung de so sanh - null neu chua bat dau giam
        /// sat toan ven (da goi StartMonitoring ke thua truc tiep, hoac chua
        /// goi StartIntegrityMonitoringAsync/ResumeIntegrityMonitoring lan nao).
        /// </summary>
        public FolderBaselineModel CurrentBaseline { get; private set; }

        /// <summary>
        /// Phat sinh khi phat hien MOT vi pham toan ven (noi dung file thay
        /// doi/file bi mat/file moi khong ro nguon goc) - xem "LUONG (THREAD)"
        /// o remarks dau lop VE VIEC handler CUA su kien nay chay tren luong
        /// threadpool, khong phai luong UI.
        /// </summary>
        public event EventHandler<IntegrityViolationEventArgs> IntegrityViolationDetected;

        public IntegrityService() : this(new BaselineService())
        {
        }

        /// <summary>Cho phep truyen BaselineService tu ben ngoai (dependency injection/unit test).</summary>
        public IntegrityService(BaselineService baselineService)
        {
            _baselineService = baselineService ?? throw new ArgumentNullException(nameof(baselineService));
        }

        /// <summary>
        /// Bat dau giam sat toan ven MOT thu muc TU DAU: chup mot baseline
        /// MOI (HashHelper.ComputeSha256Async cho tung file), luu ra dia
        /// (BaselineService.SaveBaseline, GHI DE baseline cu cua CUNG thu
        /// muc neu co - xem "QUYET DINH THIET KE" tai BaselineService), roi
        /// bat dau theo doi thay doi thoi gian thuc (StartMonitoring ke
        /// thua tu FileMonitorService).
        /// </summary>
        /// <param name="path">Thu muc can giam sat.</param>
        /// <param name="includeSubdirectories">True: giam sat ca file trong thu muc con (de quy) - PHAI khop voi gia tri se truyen cho StartMonitoring, xem <see cref="FolderBaselineModel.IncludeSubdirectories"/>.</param>
        /// <param name="cancellationToken">Cho phep huy giua luc dang chup baseline (truoc khi StartMonitoring duoc goi - huy sau thoi diem do khong con y nghia, giam sat da bat dau).</param>
        /// <param name="progress">Bao cao tien do chup baseline - xem BaselineService.CreateBaselineAsync.</param>
        /// <returns>Baseline vua duoc tao va luu.</returns>
        public async Task<FolderBaselineModel> StartIntegrityMonitoringAsync(
            string path, bool includeSubdirectories = false,
            CancellationToken cancellationToken = default, IProgress<int> progress = null)
        {
            FolderBaselineModel baseline = await _baselineService
                .CreateBaselineAsync(path, includeSubdirectories, cancellationToken, progress)
                .ConfigureAwait(false);

            _baselineService.SaveBaseline(baseline);
            ApplyBaseline(baseline);

            // StartMonitoring KE THUA tu FileMonitorService - xem "VI SAO KE
            // THUA" o remarks dau lop. Goi SAU CUNG (sau khi baseline da san
            // sang trong _baselineByPath) de tranh mot Changed/Created event
            // hiem gap lot vao DUNG luc giua ApplyBaseline va StartMonitoring
            // bi xu ly voi baseline CHUA day du.
            StartMonitoring(path, includeSubdirectories);

            return baseline;
        }

        /// <summary>
        /// Tiep tuc giam sat toan ven mot thu muc bang baseline DA LUU TU
        /// TRUOC (BaselineService.LoadBaseline) thay vi chup baseline moi -
        /// dung khi nguoi dung mo lai ung dung va muon tiep tuc giam sat MOT
        /// thu muc da tung bat dau giam sat o phien lam viec truoc, ma khong
        /// muon mat cong chup+hash lai toan bo file (co the rat lau voi thu
        /// muc lon) chi vi khoi dong lai ung dung.
        /// </summary>
        /// <returns>True neu tim thay va nap duoc baseline da luu, false neu thu muc nay CHUA TUNG co baseline (noi goi nen goi StartIntegrityMonitoringAsync de tao moi thay vi ResumeIntegrityMonitoring).</returns>
        public bool ResumeIntegrityMonitoring(string path, bool includeSubdirectories = false)
        {
            FolderBaselineModel baseline = _baselineService.LoadBaseline(path);
            if (baseline == null)
                return false;

            ApplyBaseline(baseline);
            StartMonitoring(path, includeSubdirectories);
            return true;
        }

        /// <summary>
        /// Dung giam sat toan ven: dung theo doi thay doi (StopMonitoring ke
        /// thua) VA xoa baseline dang giu trong bo nho (CurrentBaseline,
        /// _baselineByPath) - KHONG xoa file baseline da luu tren dia (goi
        /// BaselineService.DeleteBaseline rieng cho viec do, neu nguoi dung
        /// muon xoa han, se lam o mot yeu cau khac neu can).
        /// </summary>
        public void StopIntegrityMonitoring()
        {
            StopMonitoring();
            CurrentBaseline = null;
            _baselineByPath.Clear();
        }

        private void ApplyBaseline(FolderBaselineModel baseline)
        {
            CurrentBaseline = baseline;
            _baselineByPath.Clear();

            foreach (FileBaselineEntry entry in baseline.Entries)
            {
                _baselineByPath[entry.FilePath] = entry;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Goi base.OnFileCreated(e) TRUOC TIEN de KHONG lam mat hanh vi cu
        /// (su kien FileCreated cong khai van duoc raise binh thuong cho noi
        /// dang ky khac, VD MainForm) - xem "VI SAO KE THUA" o remarks dau lop.
        /// </remarks>
        protected override void OnFileCreated(FileSystemEventArgs e)
        {
            base.OnFileCreated(e);
            HandleCreatedOrChanged(e.FullPath, isNewlyCreated: true);
        }

        /// <inheritdoc/>
        protected override void OnFileChanged(FileSystemEventArgs e)
        {
            base.OnFileChanged(e);
            HandleCreatedOrChanged(e.FullPath, isNewlyCreated: false);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// KHONG the kiem tra "day co phai thu muc khong" o day (Directory.Exists
        /// se tra ve false vi muc do da bi xoa) - nhung khong sao, vi
        /// _baselineByPath TU BAN CHAT khong bao gio chua duong dan thu muc
        /// (BaselineService.CreateBaselineAsync chi dua FILE vao Entries),
        /// nen TryGetValue duoi day tu dong tra ve false voi moi thu muc bi
        /// xoa - khong can loc rieng.
        /// </remarks>
        protected override void OnFileDeleted(FileSystemEventArgs e)
        {
            base.OnFileDeleted(e);

            if (_baselineByPath.TryGetValue(e.FullPath, out FileBaselineEntry entry))
            {
                _baselineByPath.Remove(e.FullPath);
                RaiseViolation(e.FullPath, IntegrityViolationType.FileMissing, entry.Hash, null);
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Doi ten duoc coi la MAT file CU (neu duong dan cu co trong
        /// baseline - bao FileMissing) CONG VOI mot file MOI xuat hien o
        /// duong dan moi (bao UnexpectedNewFile qua HandleCreatedOrChanged,
        /// vi duong dan moi chac chan CHUA co trong baseline). Lua chon nay
        /// DON GIAN HOA logic (khong co nhanh rieng "day la doi ten hop le
        /// cua chinh file da biet") - danh doi la mot thao tac doi ten hop le
        /// (VD nguoi dung tu doi ten trong luc dang giam sat) se bi bao CA
        /// HAI loai vi pham thay vi mot canh bao "da doi ten" rieng, chap
        /// nhan duoc vi day la tinh nang GIAM SAT (thien ve bao ĐỦ hon la bao
        /// GỌN), noi nhan (UI) van co the tu ghep 2 su kien FileMissing +
        /// UnexpectedNewFile xay ra gan nhau thanh "co the la doi ten" neu
        /// muon, se lam o mot yeu cau khac neu can.
        /// </remarks>
        protected override void OnFileRenamed(RenamedEventArgs e)
        {
            base.OnFileRenamed(e);

            if (_baselineByPath.TryGetValue(e.OldFullPath, out FileBaselineEntry oldEntry))
            {
                _baselineByPath.Remove(e.OldFullPath);
                RaiseViolation(e.OldFullPath, IntegrityViolationType.FileMissing, oldEntry.Hash, null);
            }

            HandleCreatedOrChanged(e.FullPath, isNewlyCreated: true);
        }

        /// <summary>
        /// Xu ly chung cho Created VA Changed: neu duong dan KHONG co trong
        /// baseline, bao UnexpectedNewFile (chi khi isNewlyCreated - xem ben
        /// duoi); neu CO trong baseline, hash lai va so sanh, bao
        /// ContentModified khi khac, CAP NHAT LAI entry trong baseline voi
        /// hash/Size/LastWriteTimeUtc MOI sau khi da bao (xem <remarks>).
        /// </summary>
        /// <param name="isNewlyCreated">
        /// True khi goi tu OnFileCreated/OnFileRenamed (duong dan vua XUAT
        /// HIEN). False khi goi tu OnFileChanged (duong dan da ton tai tu
        /// truoc) - dung de TRANH bao UnexpectedNewFile LAP LAI cho MOI lan
        /// Changed tiep theo tren mot file da tung duoc bao la "moi" (Created
        /// event da bao UnexpectedNewFile mot lan roi). Cac Changed sau do tren
        /// CUNG file (van chua co trong baseline) se bi HandleCreatedOrChanged
        /// AM THAM bo qua (khong bao lai) o nhanh isNewlyCreated == false.
        /// </param>
        /// <remarks>
        /// QUYET DINH THIET KE - cap nhat baseline SAU KHI da bao
        /// ContentModified: tranh bao LAP LAI vi pham CHO CUNG mot lan thay
        /// doi (mot so chuong trinh ghi file qua NHIEU buoc ghi noi bo, khien
        /// FileSystemWatcher ban ra NHIEU Changed event lien tiep cho MOT lan
        /// nguoi dung nhan Save). Danh doi: neu ke tan cong THUC SU sua file
        /// nhieu lan lien tiep that (khong phai do 1 lan Save ban nhieu
        /// event), CHI lan dau tien duoc bao - cac lan sau se so sanh voi
        /// hash MOI (da cap nhat), khong con la hash baseline GOC nua. Day la
        /// mot gioi han CHAP NHAN DUOC cho pham vi tinh nang nay (phat hien
        /// THAY DOI so voi trang thai GAN NHAT da biet, khong phai luu lich
        /// su TOAN BO cac lan thay doi) - luu lich su day du se can LogService
        /// ghi lai TUNG lan ContentModified thay vi chi cap nhat baseline
        /// tai cho, co the bo sung o mot yeu cau khac neu can.
        /// </remarks>
        private void HandleCreatedOrChanged(string filePath, bool isNewlyCreated)
        {
            if (Directory.Exists(filePath))
                return; // Baseline chi quan tam FILE - xem BaselineService.CreateBaselineAsync.

            if (!_baselineByPath.TryGetValue(filePath, out FileBaselineEntry entry))
            {
                if (isNewlyCreated)
                {
                    RaiseViolation(filePath, IntegrityViolationType.UnexpectedNewFile, null, null);
                }

                // isNewlyCreated == false: day la Changed event tren mot file
                // KHONG co trong baseline - chac chan da tung duoc bao
                // UnexpectedNewFile tu Created/Renamed truoc do roi, khong
                // bao lai (xem <param name="isNewlyCreated">).
                return;
            }

            try
            {
                if (!File.Exists(filePath))
                {
                    // Bi xoa NGAY GIUA luc dang xu ly (race condition hiem -
                    // VD file bi xoa dung khoang thoi gian giua luc
                    // FileSystemWatcher ban Changed va luc handler nay chay
                    // toi day) - OnFileDeleted rieng CO THE se KHONG con bat
                    // duoc entry nay nua (da bi remove o day) nen phai tu bao
                    // FileMissing NGAY TAI DAY, tranh bo sot vi pham.
                    _baselineByPath.Remove(filePath);
                    RaiseViolation(filePath, IntegrityViolationType.FileMissing, entry.Hash, null);
                    return;
                }

                // Hash DONG BO (khong phai *Async) - xem "HIEU NANG" o remarks dau lop.
                string actualHash = HashHelper.ComputeSha256(filePath);
                if (!string.Equals(actualHash, entry.Hash, StringComparison.OrdinalIgnoreCase))
                {
                    RaiseViolation(filePath, IntegrityViolationType.ContentModified, entry.Hash, actualHash);

                    // Cap nhat baseline TAI CHO sau khi da bao - xem <remarks>.
                    entry.Hash = actualHash;
                    entry.Size = new FileInfo(filePath).Length;
                    entry.LastWriteTimeUtc = File.GetLastWriteTimeUtc(filePath);
                }
            }
            catch (UnauthorizedAccessException) { } // Mat quyen doc file dung luc kiem tra - bo qua LAN NAY, se thu lai o su kien tiep theo.
            catch (FileNotFoundException) { } // File vua bi xoa giua luc hash - da xu ly o nhanh File.Exists tren, day la phong hiem gap Race condition sau hon.
            catch (IOException) { } // VD: file dang bi khoa boi chuong trinh khac dung luc kiem tra.
        }

        private void RaiseViolation(string filePath, IntegrityViolationType violationType, string expectedHash, string actualHash)
        {
            IntegrityViolationDetected?.Invoke(this, new IntegrityViolationEventArgs(filePath, violationType, expectedHash, actualHash));
        }
    }
}
