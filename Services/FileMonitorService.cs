using System;
using System.IO;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop theo doi thay doi trong mot thu muc (tao/xoa/doi ten/sua doi file
    /// hoac thu muc con) de tu dong cap nhat giao dien (VD: ListView/TreeView) khi
    /// co thay doi tu ben ngoai ung dung (nguoi dung dung Explorer, chuong trinh khac...).
    /// Du kien dung <see cref="FileSystemWatcher"/> lam nen tang ben trong.
    /// Cac phuong thuc/thuoc tinh hien tai chi la khai bao (signature) + TODO,
    /// can trien khai logic thuc te ben trong.
    /// </summary>
    public class FileMonitorService : IDisposable
    {
        // TODO: private FileSystemWatcher _watcher;

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
            // TODO: khoi tao trang thai ban dau, chua tao FileSystemWatcher ngay
            // (chi tao khi StartMonitoring duoc goi).
        }

        /// <summary>
        /// Bat dau theo doi thay doi trong mot thu muc.
        /// </summary>
        /// <param name="path">Duong dan thu muc can theo doi.</param>
        /// <param name="includeSubdirectories">True: theo doi ca thu muc con (de quy).</param>
        public void StartMonitoring(string path, bool includeSubdirectories = false)
        {
            // TODO:
            // 1. Neu dang theo doi thu muc khac, goi StopMonitoring() truoc.
            // 2. Tao FileSystemWatcher moi voi Path = path, IncludeSubdirectories = includeSubdirectories,
            //    NotifyFilter phu hop (FileName | DirectoryName | LastWrite | Size).
            // 3. Gan handler cho Created/Deleted/Changed/Renamed/Error -> goi cac
            //    OnFileCreated/OnFileDeleted/... tuong ung (raise event).
            // 4. EnableRaisingEvents = true; cap nhat MonitoredPath, IsMonitoring.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dung theo doi thay doi (neu dang theo doi).
        /// </summary>
        public void StopMonitoring()
        {
            // TODO: EnableRaisingEvents = false, huy dang ky handler, Dispose watcher cu,
            // dat MonitoredPath = null, IsMonitoring = false.
            throw new NotImplementedException();
        }

        /// <summary>Raise event FileCreated. Cho phep lop con (neu co) tuy bien.</summary>
        protected virtual void OnFileCreated(FileSystemEventArgs e)
        {
            // TODO: FileCreated?.Invoke(this, e);
            throw new NotImplementedException();
        }

        /// <summary>Raise event FileDeleted.</summary>
        protected virtual void OnFileDeleted(FileSystemEventArgs e)
        {
            // TODO: FileDeleted?.Invoke(this, e);
            throw new NotImplementedException();
        }

        /// <summary>Raise event FileChanged.</summary>
        protected virtual void OnFileChanged(FileSystemEventArgs e)
        {
            // TODO: FileChanged?.Invoke(this, e);
            throw new NotImplementedException();
        }

        /// <summary>Raise event FileRenamed.</summary>
        protected virtual void OnFileRenamed(RenamedEventArgs e)
        {
            // TODO: FileRenamed?.Invoke(this, e);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Giai phong tai nguyen (dung theo doi va Dispose FileSystemWatcher ben trong).
        /// </summary>
        public void Dispose()
        {
            // TODO: goi StopMonitoring() va giai phong tai nguyen khong quan ly (neu co).
            throw new NotImplementedException();
        }
    }
}
