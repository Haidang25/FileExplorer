using System;
using System.IO;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Lop theo doi thay doi trong mot thu muc (tao/xoa/doi ten/sua doi file
    /// hoac thu muc con) de tu dong cap nhat giao dien (VD: ListView/TreeView) khi
    /// co thay doi tu ben ngoai ung dung (nguoi dung dung Explorer, chuong trinh khac...).
    /// Dung <see cref="FileSystemWatcher"/> lam nen tang ben trong.
    /// </summary>
    /// <remarks>
    /// QUAN TRONG ve luong (thread): FileSystemWatcher raise cac su kien
    /// (Created/Deleted/Changed/Renamed/Error) tren MOT LUONG THREADPOOL RIENG
    /// cua he thong, KHONG PHAI luong UI. Cac su kien cong khai cua lop nay
    /// (FileCreated/FileDeleted/FileChanged/FileRenamed/MonitorError) vi vay
    /// CUNG duoc raise tren luong threadpool do, KHONG duoc tu dong marshal ve
    /// luong UI. Noi dang ky (VD: MainForm) BAT BUOC phai tu goi
    /// this.Invoke/BeginInvoke ben trong handler cua minh truoc khi dung bat
    /// ky control WinForms nao (VD: cap nhat lvwFiles) - goi truc tiep tu
    /// luong threadpool se nem InvalidOperationException ("Cross-thread
    /// operation not valid"). Lop nay CHU Y KHONG tu lam viec do (khong biet
    /// Form/Control nao dang lang nghe), giu dung trach nhiem "theo doi va bao
    /// su kien tho", de linh hoat cho nhieu noi dang ky (khong chi mot Form).
    /// </remarks>
    public class FileMonitorService : IDisposable
    {
        private FileSystemWatcher _watcher;
        private bool _disposed;

        /// <summary>Duong dan thu muc dang duoc theo doi. Null neu chua bat dau theo doi.</summary>
        public string MonitoredPath { get; private set; }

        /// <summary>True neu dang theo doi thay doi (da goi StartMonitoring va chua Stop/Dispose).</summary>
        public bool IsMonitoring { get; private set; }

        /// <summary>Phat sinh khi co file/thu muc moi duoc tao trong pham vi theo doi.</summary>
        public event EventHandler<FileSystemEventArgs> FileCreated;

        /// <summary>Phat sinh khi co file/thu muc bi xoa trong pham vi theo doi.</summary>
        public event EventHandler<FileSystemEventArgs> FileDeleted;

        /// <summary>Phat sinh khi noi dung/thuoc tinh cua file/thu muc bi thay doi.</summary>
        public event EventHandler<FileSystemEventArgs> FileChanged;

        /// <summary>Phat sinh khi file/thu muc bi doi ten.</summary>
        public event EventHandler<RenamedEventArgs> FileRenamed;

        /// <summary>Phat sinh khi co loi xay ra trong qua trinh theo doi (VD: mat quyen truy cap, o dia bi rut).</summary>
        public event EventHandler<ErrorEventArgs> MonitorError;

        public FileMonitorService()
        {
            // Chua tao FileSystemWatcher ngay - chi tao khi StartMonitoring duoc
            // goi, vi FileSystemWatcher can Path hop le ngay luc khoi tao
            // (constructor cua no nem ArgumentException neu Path rong), trong
            // khi FileMonitorService co the duoc tao truoc ma chua biet theo
            // doi thu muc nao (VD: MainForm tao _fileMonitorService mot lan luc
            // Load, roi goi StartMonitoring moi lan nguoi dung doi thu muc).
        }

        /// <summary>
        /// Bat dau theo doi thay doi trong mot thu muc. Neu dang theo doi mot
        /// thu muc khac, tu dong dung theo doi thu muc cu truoc (mot
        /// FileMonitorService chi theo doi MOT thu muc tai mot thoi diem - goi
        /// StartMonitoring nhieu lan lien tiep se CHUYEN sang thu muc moi thay
        /// vi cong don nhieu watcher).
        /// </summary>
        /// <param name="path">Duong dan thu muc can theo doi.</param>
        /// <param name="includeSubdirectories">True: theo doi ca thu muc con (de quy).</param>
        /// <exception cref="ArgumentException">path rong/null hoac khong ton tai.</exception>
        public void StartMonitoring(string path, bool includeSubdirectories = false)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(FileMonitorService));

            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Đường dẫn thư mục không được để trống.", nameof(path));

            if (!Directory.Exists(path))
                throw new ArgumentException($"Thư mục không tồn tại: {path}", nameof(path));

            // Dang theo doi thu muc khac (hoac chinh thu muc nay) - dung watcher
            // cu truoc khi tao watcher moi, tranh ro ri FileSystemWatcher (handle
            // he thong) neu StartMonitoring duoc goi lien tiep nhieu lan khi
            // nguoi dung doi qua lai giua cac thu muc.
            if (IsMonitoring)
            {
                StopMonitoring();
            }

            var watcher = new FileSystemWatcher(path)
            {
                IncludeSubdirectories = includeSubdirectories,

                // Filter (mau ten file can theo doi) - CO Y dat la "*" thay vi
                // dung mac dinh "*.*" cua FileSystemWatcher. Day la MOT CAM BAY
                // KINH DIEN: .NET (ke thua tu quy uoc DOS 8.3 cu) coi "*.*"
                // nghia la "co dau cham trong ten", nen mac dinh SE BO SOT moi
                // file/thu muc KHONG CO PHAN MO RONG (VD: "README", "Makefile",
                // hoac mot thu muc bat ky ten "Reports" khong co dau cham) -
                // nhung muc nay van la du lieu hop le nguoi dung can thay ngay
                // tren ListView khi duoc tao/xoa/doi ten tu ben ngoai. "*" (khong
                // co dau cham) khop VOI TAT CA ten, khong bi gioi han boi quy uoc
                // 8.3 nay - day la ly do vi sao KHONG the dung "*.*" ("khop moi
                // thu") nhu truc giac ten goi cua no.
                Filter = "*",

                // FileName/DirectoryName: bat Created/Deleted/Renamed cho ca file
                // lan thu muc con. LastWrite: bat Changed khi noi dung file duoc
                // ghi lai (thoi gian sua doi thay doi). Size: bat Changed them
                // khi kich thuoc file thay doi ma co the LastWrite chua kip cap
                // nhat (mot so trinh ghi file cap nhat Size truoc). Khong bat
                // Attributes/Security/CreationTime/LastAccess vi day la cac thay
                // doi ung dung KHONG can phan ung (VD: LastAccess doi lien tuc
                // moi lan doc file, se lam Changed bi goi qua nhieu khong can thiet).
                NotifyFilter = NotifyFilters.FileName
                    | NotifyFilters.DirectoryName
                    | NotifyFilters.LastWrite
                    | NotifyFilters.Size
            };

            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
            watcher.Error += Watcher_Error;

            // EnableRaisingEvents = true BAT DAU nhan su kien ngay lap tuc - phai
            // dat SAU CUNG, sau khi da gan het handler o tren, de tranh lo mat
            // su kien xay ra dung luc dang gan dang ky (hiem nhung ve nguyen tac
            // an toan hon).
            watcher.EnableRaisingEvents = true;

            _watcher = watcher;
            MonitoredPath = path;
            IsMonitoring = true;
        }

        /// <summary>
        /// Dung theo doi thay doi (neu dang theo doi). An toan khi goi nhieu
        /// lan lien tiep hoac khi chua tung StartMonitoring (khong lam gi ca).
        /// </summary>
        public void StopMonitoring()
        {
            if (_watcher == null)
                return;

            // Tat EnableRaisingEvents TRUOC khi huy dang ky/Dispose - dam bao
            // khong con su kien moi nao duoc dua vao hang doi ngay khi bat dau
            // don dep, giam nguy co mot su kien "lot luoi" gap dung watcher vua
            // Dispose xong (van co the xay ra rat hiem do ban chat da luong cua
            // FileSystemWatcher, nhung cac handler Watcher_* ben duoi da tu
            // kiem tra _disposed/watcher con hop le truoc khi raise event ra ngoai).
            _watcher.EnableRaisingEvents = false;

            _watcher.Created -= Watcher_Created;
            _watcher.Deleted -= Watcher_Deleted;
            _watcher.Changed -= Watcher_Changed;
            _watcher.Renamed -= Watcher_Renamed;
            _watcher.Error -= Watcher_Error;

            _watcher.Dispose();
            _watcher = null;

            MonitoredPath = null;
            IsMonitoring = false;
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e) => OnFileCreated(e);

        private void Watcher_Deleted(object sender, FileSystemEventArgs e) => OnFileDeleted(e);

        private void Watcher_Changed(object sender, FileSystemEventArgs e) => OnFileChanged(e);

        private void Watcher_Renamed(object sender, RenamedEventArgs e) => OnFileRenamed(e);

        private void Watcher_Error(object sender, ErrorEventArgs e) => OnMonitorError(e);

        /// <summary>Raise event FileCreated. Cho phep lop con (neu co) tuy bien.</summary>
        protected virtual void OnFileCreated(FileSystemEventArgs e)
        {
            FileCreated?.Invoke(this, e);
        }

        /// <summary>Raise event FileDeleted.</summary>
        protected virtual void OnFileDeleted(FileSystemEventArgs e)
        {
            FileDeleted?.Invoke(this, e);
        }

        /// <summary>Raise event FileChanged.</summary>
        protected virtual void OnFileChanged(FileSystemEventArgs e)
        {
            FileChanged?.Invoke(this, e);
        }

        /// <summary>Raise event FileRenamed.</summary>
        protected virtual void OnFileRenamed(RenamedEventArgs e)
        {
            FileRenamed?.Invoke(this, e);
        }

        /// <summary>
        /// Raise event MonitorError. Cac loi thuong gap tu FileSystemWatcher: bo
        /// dem noi bo (InternalBufferOverflowException) bi tran khi qua nhieu
        /// thay doi xay ra don don trong thoi gian ngan (VD: sao chep hang ngan
        /// file cung luc vao thu muc dang theo doi), hoac thu muc dang theo doi
        /// bi xoa/o dia bi rut giua chung.
        /// </summary>
        protected virtual void OnMonitorError(ErrorEventArgs e)
        {
            MonitorError?.Invoke(this, e);
        }

        /// <summary>
        /// Giai phong tai nguyen (dung theo doi va Dispose FileSystemWatcher ben trong).
        /// An toan khi goi nhieu lan (theo dung khuyen nghi cua IDisposable).
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            StopMonitoring();
            _disposed = true;

            // Khong can GC.SuppressFinalize(this) vi lop nay khong co finalizer
            // (khong nam giu tai nguyen khong quan ly truc tiep - FileSystemWatcher
            // tu no da la mot IDisposable duoc quan ly, da duoc Dispose o
            // StopMonitoring() o tren).
        }
    }
}
