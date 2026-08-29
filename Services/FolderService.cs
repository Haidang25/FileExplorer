using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop xu ly cac thao tac lien quan den thu muc (tao, doi ten, xoa,
    /// di chuyen, sao chep, lay thong tin/cay thu muc con...).
    /// Cac phuong thuc hien tai chi la khai bao (signature) + TODO, can trien khai
    /// logic thuc te ben trong. Su dung cung Models (FolderItemModel, OperationResult,
    /// FileOperationType) va Helpers (PermissionHelper, FileHelper) da co san.
    /// </summary>
    public class FolderService
    {
        // TODO: co the tiem (inject) them cac service khac neu can, VD:
        // - mot service ghi log (su dung LogEntryModel) de ghi lai moi thao tac
        // - PermissionHelper de kiem tra quyen ghi truoc khi thuc hien

        public FolderService()
        {
            // TODO: khoi tao cac phu thuoc (dependency) neu co, hien tai chua can.
        }

        /// <summary>
        /// Lay danh sach cac o dia (drive) hien co tren may, dung lam node goc cho
        /// TreeView duyet thu muc (VD: "This PC" > C:\, D:\...).
        ///
        /// Van tra ve ca nhung o dia chua san sang (IsReady = false — VD: o CD/DVD
        /// dang rong, o dia mang bi ngat ket noi) de danh sach o dia giong Windows
        /// Explorer (van thay o do ton tai, chi khong mo duoc), thay vi an han di.
        /// Voi o chua san sang, chi doc duoc DriveInfo.Name/DriveType (an toan ke
        /// ca khi IsReady = false) — moi thuoc tinh khac cua RootDirectory (VolumeLabel,
        /// CreationTime, Attributes...) deu nem IOException neu goi luc o chua san sang,
        /// nen duoc bo qua hoan toan cho truong hop nay.
        /// </summary>
        /// <returns>
        /// Danh sach FolderItemModel, moi item ung voi mot o dia (IsDrive = true).
        /// Kiem tra IsReady tren tung item truoc khi cho phep expand/dieu huong vao.
        /// </returns>
        public List<FolderItemModel> GetDrives()
        {
            var drives = new List<FolderItemModel>();

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (!drive.IsReady)
                {
                    // O chua san sang: khong duoc dong vao RootDirectory/VolumeLabel/
                    // CreationTime... (nem IOException) - chi dung Name/DriveType,
                    // ca hai deu doc duoc an toan bat ke IsReady.
                    drives.Add(new FolderItemModel
                    {
                        Name = $"{drive.Name.TrimEnd('\\')} ({DescribeDriveType(drive.DriveType)}, chưa sẵn sàng)",
                        FullPath = drive.Name,
                        ParentPath = null,
                        IsDrive = true,
                        IsReady = false,
                        HasSubFolders = false,
                        DriveType = drive.DriveType
                    });
                    continue;
                }

                try
                {
                    DirectoryInfo rootDirectory = drive.RootDirectory;

                    // Khong dung FolderItemModel.FromDirectoryInfo() vi ham do goi
                    // EnumerateDirectories() de kiem tra HasSubFolders — voi o dia
                    // dung lam node goc TreeView, ta luon muon hien dau (+) de nguoi
                    // dung tu expand, tranh cham/loi truy cap toan bo o dia ngay luc
                    // liet ke danh sach o dia (VD: o dia mang cham, o co qua nhieu file).
                    string volumeLabel = string.IsNullOrWhiteSpace(drive.VolumeLabel)
                        ? DescribeDriveType(drive.DriveType)
                        : drive.VolumeLabel;

                    // Nhan hien thi day du gom nhan o + duong dan + dung luong con
                    // trong, giong Windows Explorer (VD: "Local Disk (C:) — 120 GB trống").
                    string label = $"{volumeLabel} ({drive.Name.TrimEnd('\\')}) — " +
                        $"{FormatHelper.FormatSize(drive.AvailableFreeSpace)} trống";

                    drives.Add(new FolderItemModel
                    {
                        Name = label,
                        FullPath = rootDirectory.FullName,
                        ParentPath = null,
                        CreatedDate = rootDirectory.CreationTime,
                        ModifiedDate = rootDirectory.LastWriteTime,
                        Attributes = rootDirectory.Attributes,
                        IsDrive = true,
                        IsReady = true,
                        HasSubFolders = true,
                        DriveTotalSize = drive.TotalSize,
                        AvailableFreeSpace = drive.AvailableFreeSpace,
                        DriveType = drive.DriveType
                    });
                }
                catch (UnauthorizedAccessException) { /* Khong co quyen doc o dia nay - bo qua. */ }
                catch (IOException)
                {
                    // O bao IsReady = true nhung van loi khi doc (hiem, VD: vua thao
                    // ra dung luc doc) - them lai o dang "chua san sang" thay vi mat
                    // hoan toan khoi danh sach, giong hanh vi khi IsReady = false.
                    drives.Add(new FolderItemModel
                    {
                        Name = $"{drive.Name.TrimEnd('\\')} ({DescribeDriveType(drive.DriveType)}, chưa sẵn sàng)",
                        FullPath = drive.Name,
                        ParentPath = null,
                        IsDrive = true,
                        IsReady = false,
                        HasSubFolders = false,
                        DriveType = drive.DriveType
                    });
                }
            }

            return drives;
        }

        /// <summary>Ten hien thi tieng Viet ngan gon cho DriveType, dung trong nhan cua o dia chua san sang.</summary>
        private static string DescribeDriveType(DriveType driveType)
        {
            switch (driveType)
            {
                case DriveType.CDRom:
                    return "Ổ đĩa quang";
                case DriveType.Network:
                    return "Ổ mạng";
                case DriveType.Removable:
                    return "Ổ di động";
                default:
                    return "Ổ đĩa";
            }
        }

        /// <summary>
        /// Kiem tra thu muc co ton tai hay khong.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc can kiem tra.</param>
        public bool FolderExists(string folderPath)
        {
            // TODO: kiem tra folderPath hop le + Directory.Exists(folderPath)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay thong tin chi tiet cua mot thu muc (khong bao gom thu muc con).
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <returns>FolderItemModel chua thong tin thu muc.</returns>
        public FolderItemModel GetFolderInfo(string folderPath)
        {
            // TODO: dung FolderItemModel.FromPath(folderPath)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach thu muc con truc tiep (khong de quy) cua mot thu muc.
        /// Dung khi nguoi dung mo rong (expand) node tren TreeView.
        ///
        /// Sap xep theo ten (OrdinalIgnoreCase) de thu tu hien thi on dinh, khong
        /// phu thuoc thu tu tra ve cua he dieu hanh. Cac thu muc khong doc duoc
        /// (mat quyen ngay trong luc enumerate, o dia thao ra giua chung...) duoc
        /// bo qua thay vi lam hong ca danh sach.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc cha.</param>
        /// <param name="includeHidden">
        /// True: lay ca thu muc an/he thong (Hidden/System). False: bo qua cac thu
        /// muc do, dung khi nguoi dung tat tuy chon "Hien file/thu muc an".
        /// </param>
        /// <returns>
        /// Danh sach FolderItemModel cua cac thu muc con truc tiep. Danh sach rong
        /// (khong phai null) neu folderPath khong ton tai, khong co thu muc con,
        /// hoac khong co quyen doc.
        /// </returns>
        public List<FolderItemModel> GetSubFolders(string folderPath, bool includeHidden = true)
        {
            var subFolders = new List<FolderItemModel>();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return subFolders;

            try
            {
                var directoryInfos = new DirectoryInfo(folderPath)
                    .EnumerateDirectories()
                    .OrderBy(d => d.Name, StringComparer.OrdinalIgnoreCase);

                foreach (DirectoryInfo directoryInfo in directoryInfos)
                {
                    try
                    {
                        if (!includeHidden)
                        {
                            bool isHiddenOrSystem = directoryInfo.Attributes.HasFlag(FileAttributes.Hidden)
                                || directoryInfo.Attributes.HasFlag(FileAttributes.System);
                            if (isHiddenOrSystem)
                                continue;
                        }

                        subFolders.Add(FolderItemModel.FromDirectoryInfo(directoryInfo));
                    }
                    catch (UnauthorizedAccessException) { /* Khong doc duoc thu muc con nay - bo qua rieng no. */ }
                    catch (IOException) { /* VD: shortcut/junction hong, o dia mang vua ngat. */ }
                }
            }
            catch (UnauthorizedAccessException) { /* Khong co quyen liet ke thu muc cha - tra ve danh sach rong. */ }
            catch (IOException) { /* folderPath nam tren o dia vua thao ra, duong dan mang bi ngat... */ }

            return subFolders;
        }

        /// <summary>
        /// Tinh tong dung luong cua thu muc (de quy toan bo thu muc con).
        /// Co the mat thoi gian voi thu muc lon - nen chay bat dong bo (async)
        /// hoac chay tren luong rieng khi tich hop vao UI (xem PropertiesForm).
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <returns>Tong so byte cua tat ca file tim duoc (de quy). Tra ve 0 neu folderPath khong ton tai.</returns>
        public long GetFolderSize(string folderPath)
        {
            return GetFolderSize(folderPath, CancellationToken.None);
        }

        /// <summary>
        /// Nhu <see cref="GetFolderSize(string)"/> nhung nhan them CancellationToken de
        /// nguoi goi (VD: PropertiesForm chay tren Task.Run) co the huy giua chung neu
        /// nguoi dung dong hop thoai truoc khi tinh xong - tranh lang phi duyet tiep
        /// mot cay thu muc rat lon sau khi ket qua khong con can dung nua.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <param name="cancellationToken">Token de huy giua chung.</param>
        public long GetFolderSize(string folderPath, CancellationToken cancellationToken)
        {
            return GetFolderStatistics(folderPath, cancellationToken).TotalBytes;
        }

        /// <summary>
        /// Tinh dong thoi tong dung luong VA so luong tep/thu muc con (de quy toan
        /// bo cay thu muc) trong MOT lan duyet duy nhat - dung chung mot Stack voi
        /// GetFolderSize thay vi duyet rieng 2 lan (mot lan cho dung luong, mot lan
        /// cho so luong) de khong ton gap doi thoi gian I/O voi thu muc lon.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <returns>FolderStatistics gom TotalBytes/FileCount/FolderCount. Tra ve gia tri 0 neu folderPath khong ton tai.</returns>
        public FolderStatistics GetFolderStatistics(string folderPath)
        {
            return GetFolderStatistics(folderPath, CancellationToken.None);
        }

        /// <summary>Nhu <see cref="GetFolderStatistics(string)"/> nhung nhan them CancellationToken - xem ghi chu tai GetFolderSize.</summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <param name="cancellationToken">Token de huy giua chung.</param>
        public FolderStatistics GetFolderStatistics(string folderPath, CancellationToken cancellationToken)
        {
            return GetFolderStatisticsCore(folderPath, cancellationToken);
        }

        /// <summary>
        /// Phien ban bat dong bo (async) cua GetFolderStatistics - chay phep duyet
        /// (I/O + tinh toan CPU-bound, khong co await ben trong) tren mot luong
        /// threadpool rieng qua Task.Run, giup noi goi (VD: PropertiesForm) dung
        /// duoc "await" thay vi tu quan ly Task.Run/BeginInvoke thu cong, dong thoi
        /// khong lam dong bang luong UI khi thu muc lon/nhieu file. Day la cach lam
        /// bat dong bo nhat quan voi CopyFolderAsync trong cung service nay.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <param name="cancellationToken">Token de huy giua chung (VD: nguoi dung dong PropertiesForm truoc khi tinh xong).</param>
        public Task<FolderStatistics> GetFolderStatisticsAsync(string folderPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() => GetFolderStatisticsCore(folderPath, cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Nhu <see cref="GetFolderStatisticsAsync(string, CancellationToken)"/> nhung
        /// tra ve tong dung luong (byte) thay vi FolderStatistics day du - dung khi
        /// chi can dung luong, khong can dem so tep/thu muc con.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        /// <param name="cancellationToken">Token de huy giua chung.</param>
        public async Task<long> GetFolderSizeAsync(string folderPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            FolderStatistics stats = await GetFolderStatisticsAsync(folderPath, cancellationToken).ConfigureAwait(false);
            return stats.TotalBytes;
        }

        /// <summary>
        /// Phan logic duyet de quy thuc su, dung chung boi ca ban dong bo
        /// (GetFolderStatistics) lan bat dong bo (GetFolderStatisticsAsync) - tach
        /// rieng de tranh GetFolderStatisticsAsync goi nguoc lai GetFolderStatistics
        /// (se chay dung bo tren threadpool nhung code lai trong ham "dong bo",
        /// gay nham lan khi doc).
        /// </summary>
        private FolderStatistics GetFolderStatisticsCore(string folderPath, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return new FolderStatistics();

            long totalBytes = 0;
            long fileCount = 0;
            long folderCount = 0;

            // Dung Stack tu quan ly thay vi de quy ham (recursion) that su - tranh
            // StackOverflowException voi cay thu muc qua sau (hiem nhung co the xay
            // ra voi duong dan long hoac thu muc loop qua ReparsePoint bi loi).
            var pending = new Stack<string>();
            pending.Push(folderPath);

            while (pending.Count > 0)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string currentDir = pending.Pop();

                // Moi thu muc con duoc xu ly doc lap - neu MOT thu muc con bi tu
                // choi quyen truy cap (UnauthorizedAccessException) hoac bi xoa/di
                // chuyen giua luc dang duyet (IOException/DirectoryNotFoundException),
                // chi bo qua nhanh do va tiep tuc voi cac thu muc con khac thay vi
                // lam hong toan bo phep tinh tong.
                string[] files;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException || ex is System.Security.SecurityException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    try
                    {
                        totalBytes += new FileInfo(file).Length;
                        fileCount++;
                    }
                    catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException || ex is System.Security.SecurityException)
                    {
                        // Bo qua file khong doc duoc kich thuoc (VD: file he thong
                        // duoc bao ve, hoac bi xoa dung luc duyet toi) - giu nguyen
                        // ket qua da cong duoc, khong lam gian doan ca phep tinh.
                        // KHONG tang fileCount vi khong the xac nhan file van con
                        // ton tai/hop le tai thoi diem duyet.
                    }
                }

                string[] subDirs;
                try
                {
                    subDirs = Directory.GetDirectories(currentDir);
                }
                catch (Exception ex) when (ex is UnauthorizedAccessException || ex is IOException || ex is System.Security.SecurityException)
                {
                    continue;
                }

                foreach (string subDir in subDirs)
                {
                    folderCount++;
                    pending.Push(subDir);
                }
            }

            return new FolderStatistics
            {
                TotalBytes = totalBytes,
                FileCount = fileCount,
                FolderCount = folderCount
            };
        }

        /// <summary>
        /// Tao thu muc moi ben trong mot thu muc cha.
        /// </summary>
        /// <param name="parentPath">Duong dan thu muc cha.</param>
        /// <param name="folderName">Ten thu muc moi.</param>
        public OperationResult CreateFolder(string parentPath, string folderName)
        {
            if (string.IsNullOrWhiteSpace(parentPath) || !Directory.Exists(parentPath))
                return OperationResult.NotFound;

            if (!FileHelper.IsValidFileName(folderName))
                return OperationResult.Failed;

            string fullPath = Path.Combine(parentPath, folderName);

            if (Directory.Exists(fullPath) || File.Exists(fullPath))
                return OperationResult.Skipped; // Da ton tai muc trung ten.

            if (!PermissionHelper.HasWritePermission(parentPath))
                return OperationResult.AccessDenied;

            try
            {
                Directory.CreateDirectory(fullPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Doi ten mot thu muc.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc hien tai.</param>
        /// <param name="newName">Ten moi (chi ten, khong bao gom duong dan).</param>
        public OperationResult RenameFolder(string folderPath, string newName)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return OperationResult.NotFound;

            if (!FileHelper.IsValidFileName(newName))
                return OperationResult.Failed;

            string parentPath = Directory.GetParent(folderPath)?.FullName;
            string newPath = Path.Combine(parentPath ?? string.Empty, newName);

            if (Directory.Exists(newPath) || File.Exists(newPath))
                return OperationResult.Skipped; // Da co muc trung ten moi.

            if (!PermissionHelper.HasWritePermission(parentPath))
                return OperationResult.AccessDenied;

            try
            {
                Directory.Move(folderPath, newPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Xoa mot thu muc (va toan bo noi dung ben trong).
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc can xoa.</param>
        /// <param name="permanent">
        /// True: xoa vinh vien (Directory.Delete, khong the khoi phuc). False: chuyen
        /// vao Recycle Bin - nen goi RecycleBinService.DeleteToRecycleBin() truc tiep
        /// cho truong hop nay thay vi qua day, vi permanent = false hien chi la lop
        /// vo boc mong danh cho tinh nhat quan API, khong tu goi RecycleBinService
        /// (tranh FolderService phu thuoc nguoc vao RecycleBinService).
        /// </param>
        public OperationResult DeleteFolder(string folderPath, bool permanent = false)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return OperationResult.NotFound;

            if (!permanent)
            {
                // Chua ho tro chuyen vao Recycle Bin truc tiep tu FolderService - noi
                // goi nen dung RecycleBinService.DeleteToRecycleBin() cho truong hop nay.
                throw new NotSupportedException(
                    "FolderService.DeleteFolder(permanent: false) chua duoc ho tro - " +
                    "hay dung RecycleBinService.DeleteToRecycleBin() de chuyen vao Thung rac.");
            }

            string parentPath = Directory.GetParent(folderPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(parentPath))
                return OperationResult.AccessDenied;

            try
            {
                // Go co ReadOnly cho toan bo file con truoc (de quy) - Directory.Delete()
                // se nem UnauthorizedAccessException va dung ngay khi gap MOT file con
                // co thuoc tinh nay, du da co quyen ghi len thu muc cha, giong ly do da
                // xu ly trong FileService.DeleteFile.
                ClearReadOnlyAttributeRecursive(folderPath);

                // recursive: true de xoa dc thu muc khong rong (co file/thu muc con
                // ben trong) - giong hanh vi xoa vinh vien cua Windows Explorer.
                Directory.Delete(folderPath, recursive: true);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // Mot file nao do ben trong thu muc dang bi chuong trinh khac khoa -
                // Directory.Delete(recursive: true) that bai toan bo (khong xoa duoc
                // mot phan roi bo qua phan con lai nhu CopyDirectoryRecursiveAsync).
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Go co ReadOnly cho toan bo file (de quy qua tat ca thu muc con) ben trong
        /// mot thu muc, chuan bi truoc khi Directory.Delete(recursive: true) - ham do
        /// se that bai ngay khi gap file con dau tien con giu co nay. Bo qua rieng
        /// tung file/thu muc loi (VD: mat quyen doc mot nhanh con) thay vi lam hong
        /// ca qua trinh - Directory.Delete() sau do van se tu bao loi rieng cho phan
        /// con lai khong go duoc co, thay vi de ham nay nem exception som.
        /// </summary>
        private static void ClearReadOnlyAttributeRecursive(string folderPath)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    try { FileHelper.ClearReadOnlyAttribute(filePath); }
                    catch (UnauthorizedAccessException) { /* Bo qua rieng file nay - Directory.Delete se bao loi cu the hon sau. */ }
                    catch (IOException) { /* VD: file dang bi khoa boi ung dung khac. */ }
                }

                foreach (string subDir in Directory.GetDirectories(folderPath))
                {
                    ClearReadOnlyAttributeRecursive(subDir);
                }
            }
            catch (UnauthorizedAccessException) { /* Khong liet ke duoc thu muc nay - bo qua, de Directory.Delete tu bao loi. */ }
            catch (IOException) { /* Tuong tu. */ }
        }

        /// <summary>
        /// Di chuyen thu muc sang vi tri khac.
        /// </summary>
        /// <param name="sourcePath">Duong dan thu muc nguon.</param>
        /// <param name="destinationPath">Duong dan thu muc dich (thu muc cha se chua thu muc nguon).</param>
        public OperationResult MoveFolder(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
                return OperationResult.NotFound;

            // Chan truoc khi di chuyen thu muc vao chinh no hoac vao mot thu muc con
            // cua chinh no - Directory.Move() se nem IOException kho hieu ("Cannot
            // move a directory into itself") cho truong hop dau, va am tham gay loi
            // logic (thu muc bien mat/long nhau sai) cho truong hop sau neu khong chan.
            if (FileHelper.IsSameOrSubdirectory(sourcePath, destinationPath))
                return OperationResult.InvalidDestination;

            if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationParent = Directory.GetParent(destinationPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(destinationParent))
                return OperationResult.AccessDenied;

            if (FileHelper.IsOnDifferentDrive(sourcePath, destinationPath))
            {
                // Directory.Move() khong ho tro di chuyen truc tiep giua 2 o dia khac
                // nhau - tu dong chuyen sang CopyFolder (de quy, tu bat loi tung file/
                // thu muc con) roi DeleteFolder nguon, giong huong xu ly da lam voi
                // FileService.MoveFile. Neu Copy that bai hoan toan (ke ca thu muc
                // goc) thi tra thang ket qua do - chua dong den nguon. Neu Copy thanh
                // cong (co the co skippedPaths con so, van tinh la du de tiep tuc xoa
                // nguon) nhung Delete nguon that bai, tra PartialSuccess de nguoi dung
                // biet con ban goc chua xoa duoc.
                OperationResult copyResult = CopyFolder(sourcePath, destinationPath, out List<string> skippedPaths);
                if (copyResult != OperationResult.Success)
                    return copyResult;

                OperationResult deleteResult = DeleteFolder(sourcePath, permanent: true);
                return deleteResult == OperationResult.Success
                    ? OperationResult.Success
                    : OperationResult.PartialSuccess;
            }

            try
            {
                Directory.Move(sourcePath, destinationPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Sao chep thu muc (va toan bo noi dung ben trong, de quy) sang vi tri khac
        /// (dong bo) - giu lai de tuong thich voi cac noi goi cu/don gian chua can
        /// async (VD: MoveFolder khi fallback Copy+Delete giua 2 o dia khac nhau);
        /// ben trong chi goi CopyFolderAsync (khong progress) roi cho ket qua ngay,
        /// giong cach FileService.CopyFile() da lam voi CopyFileAsync(). Uu tien dung
        /// CopyFolderAsync() truc tiep o cac noi co the await va can bao cao tien do
        /// (VD: MainForm.mnuEditPaste_Click).
        /// </summary>
        /// <param name="sourcePath">Duong dan thu muc nguon.</param>
        /// <param name="destinationPath">Duong dan thu muc dich.</param>
        /// <param name="skippedPaths">
        /// Danh sach duong dan (thu muc/file) da bi bo qua do loi quyen/IO trong luc
        /// sao chep de quy. Danh sach rong neu sao chep het toan bo khong loi gi.
        /// </param>
        /// <returns>
        /// OperationResult.Success neu sao chep duoc it nhat thu muc goc (ke ca khi
        /// co bo sot mot so muc con - kiem tra skippedPaths de biet chi tiet).
        /// AccessDenied/Failed chi khi ngay ca thu muc goc cung khong tao duoc.
        /// </returns>
        public OperationResult CopyFolder(string sourcePath, string destinationPath, out List<string> skippedPaths)
        {
            var skipped = new List<string>();
            OperationResult result = CopyFolderAsync(sourcePath, destinationPath, skipped).GetAwaiter().GetResult();
            skippedPaths = skipped;
            return result;
        }

        /// <summary>
        /// Sao chep thu muc (va toan bo noi dung ben trong, de quy) sang vi tri khac
        /// theo kieu bat dong bo - tung file con duoc sao chep qua FileService.CopyFileAsync
        /// (doc/ghi bang FileStream + buffer) thay vi File.Copy dong bo, de: (1) khong
        /// chan UI thread ke ca khi thu muc chua file lon; (2) co the bao cao tien do
        /// tong the qua tham so progress.
        ///
        /// Neu mot thu muc con hoac file nao do ben trong khong doc/ghi duoc (VD:
        /// mat quyen, file dang bi khoa boi ung dung khac), chi bo qua rieng muc do
        /// va tiep tuc sao chep phan con lai cua cay thu muc, thay vi huy toan bo
        /// thao tac. Dung skippedPaths (tham so, khong phai out - async khong ho tro
        /// out) de biet co bo sot gi khong.
        /// </summary>
        /// <param name="sourcePath">Duong dan thu muc nguon.</param>
        /// <param name="destinationPath">Duong dan thu muc dich.</param>
        /// <param name="skippedPaths">
        /// Danh sach (da khoi tao san boi noi goi) de ham nay them vao duong dan
        /// cac muc bi bo qua do loi quyen/IO trong luc sao chep de quy.
        /// </param>
        /// <param name="progress">
        /// IProgress&lt;FileOperationProgress&gt; (thuong la Progress&lt;T&gt; tao tren UI
        /// thread) de bao cao tien do tong the (so file da xong / tong so file, cong
        /// them ti le byte cua file dang copy do dang). Bo qua (null) neu khong can
        /// theo doi tien do - luc do ham nay khong ton chi phi CountFiles()/Report().
        /// </param>
        /// <param name="cancellationToken">
        /// Token de huy giua chung (VD: nguoi dung bam nut Huy tren CopyProgressForm) -
        /// duoc truyen xuong tung FileService.CopyFileAsync va duoc kiem tra giua cac
        /// file/thu muc con trong CopyDirectoryRecursiveAsync, de dung lai gan nhu
        /// ngay thay vi phai cho sao chep xong het ca cay thu muc.
        /// </param>
        public async Task<OperationResult> CopyFolderAsync(
            string sourcePath, string destinationPath, List<string> skippedPaths,
            IProgress<FileOperationProgress> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
                return OperationResult.NotFound;

            // Chan truoc khi sao chep thu muc vao chinh no hoac vao mot thu muc con
            // cua chinh no - neu khong, CopyDirectoryRecursiveAsync se de quy vo han
            // (moi lan tao destinationDir ben trong sourceDir lai bi chinh vong lap
            // ben ngoai duyet tiep vao, gay tran ngan xep hoac day dia).
            if (FileHelper.IsSameOrSubdirectory(sourcePath, destinationPath))
                return OperationResult.InvalidDestination;

            if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationParent = Directory.GetParent(destinationPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(destinationParent))
                return OperationResult.AccessDenied;

            try
            {
                // Chi dem so file (CountFiles) khi thuc su co noi theo doi tien do -
                // ban than viec dem cung phai duyet toan bo cay thu muc mot lan, nen
                // tranh chi phi nay neu khong ai can den %.
                CopyProgressState progressState = null;
                if (progress != null)
                {
                    progressState = new CopyProgressState
                    {
                        Progress = progress,
                        TotalFiles = CountFiles(sourcePath)
                    };
                }

                // Chi loi o cap thu muc goc (khong tao duoc destinationDir dau tien -
                // VD: destinationParent bi mat quyen dung luc kiem tra o tren) moi lam
                // that bai toan bo thao tac - loi o cac thu muc/file con ben trong deu
                // duoc CopyDirectoryRecursiveAsync tu bat va ghi vao skippedPaths.
                await CopyDirectoryRecursiveAsync(sourcePath, destinationPath, skippedPaths, progressState, cancellationToken).ConfigureAwait(false);
                return OperationResult.Success;
            }
            catch (OperationCanceledException)
            {
                // Nguoi dung bam Huy giua chung - xoa thu muc dich dang do dang (chi
                // co MOT PHAN cay thu muc nguon, khong nguyen ven) de khong de lai
                // "rac" nua vien nua, giong cach CopyFileAsync da xoa file dich do
                // dang khi bi huy. AN TOAN xoa CA destinationPath (khong chi phan con
                // thieu) vi ngay tren da kiem tra destinationPath CHUA TON TAI truoc
                // khi ham nay bat dau tao no - moi thu ben trong deu do chinh lan
                // Copy nay tao ra, khong phai du lieu co san cua nguoi dung.
                try
                {
                    if (Directory.Exists(destinationPath))
                    {
                        ClearReadOnlyAttributeRecursive(destinationPath);
                        Directory.Delete(destinationPath, recursive: true);
                    }
                }
                catch (UnauthorizedAccessException) { /* Khong xoa duoc thu muc rac - bo qua, khong quan trong bang viec da huy theo yeu cau. */ }
                catch (IOException) { /* VD: mot file ben trong dang bi khoa boi ung dung khac. */ }

                return OperationResult.Cancelled;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Sao chep de quy toan bo noi dung mot thu muc (bao gom thu muc con) sang vi
        /// tri moi. Loi quyen/IO tren tung file hoac thu muc con RIENG LE duoc bat va
        /// bo qua ngay tai do (ghi duong dan loi vao skippedPaths), khong lam dung
        /// hoac huy phan con lai cua de quy - chi loi khi tao chinh destinationDir
        /// nay (Directory.CreateDirectory ngay dau ham) moi duoc nem ra ngoai, vi day
        /// la dieu kien tien quyet de co the sao chep bat cu thu gi vao ben trong no.
        /// </summary>
        private static async Task CopyDirectoryRecursiveAsync(
            string sourceDir, string destinationDir, List<string> skippedPaths, CopyProgressState progressState,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Directory.CreateDirectory(destinationDir);

            try
            {
                foreach (string filePath in Directory.GetFiles(sourceDir))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string fileName = Path.GetFileName(filePath);
                    try
                    {
                        string destFilePath = Path.Combine(destinationDir, fileName);

                        IProgress<long> fileProgress = progressState == null
                            ? null
                            : new FileBytesProgressAdapter(progressState, filePath);

                        var fileService = new FileService();
                        OperationResult copyResult = await fileService.CopyFileAsync(
                            filePath, destFilePath, overwrite: false, progress: fileProgress,
                            cancellationToken: cancellationToken).ConfigureAwait(false);

                        // CopyFileAsync tra ve Cancelled (khong nem exception) khi bi huy -
                        // ThrowIfCancellationRequested() o day se nem lai OperationCanceledException
                        // (chac chan nem duoc, vi token dung la da bi huy luc nay), de loi nay
                        // thoat het khoi 2 vong foreach (bo qua cac catch UnauthorizedAccessException/
                        // IOException ben duoi vi khong khop kieu) va duoc CopyFolderAsync bat lai.
                        if (copyResult == OperationResult.Cancelled)
                            cancellationToken.ThrowIfCancellationRequested();

                        if (copyResult != OperationResult.Success)
                            skippedPaths.Add(filePath);
                    }
                    catch (UnauthorizedAccessException) { skippedPaths.Add(filePath); }
                    catch (IOException) { skippedPaths.Add(filePath); } // VD: file dang bi ung dung khac khoa.
                    finally
                    {
                        // Luon tang FilesCompleted va bao cao lai du file vua roi
                        // thanh cong hay bi bo qua - neu khong, mot file loi ngay
                        // truoc khi CopyFileAsync kip goi Report() lan nao se lam
                        // thanh % tong the bi "ket" o gia tri cu cho den file tiep
                        // theo (hoac mai mai neu day la file cuoi cung).
                        if (progressState != null)
                        {
                            progressState.FilesCompleted++;
                            progressState.Report(fileName, 0, 0);
                        }
                    }
                }
            }
            catch (UnauthorizedAccessException) { skippedPaths.Add(sourceDir); }
            catch (IOException) { skippedPaths.Add(sourceDir); } // Khong liet ke duoc danh sach file cua sourceDir.

            try
            {
                foreach (string subDir in Directory.GetDirectories(sourceDir))
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                    await CopyDirectoryRecursiveAsync(subDir, destSubDir, skippedPaths, progressState, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (UnauthorizedAccessException) { skippedPaths.Add(sourceDir); }
            catch (IOException) { skippedPaths.Add(sourceDir); } // Khong liet ke duoc danh sach thu muc con cua sourceDir.
        }

        /// <summary>
        /// Dem so luong file (de quy qua toan bo thu muc con) cua mot thu muc, dung
        /// de uoc tinh CopyProgressState.TotalFiles TRUOC khi bat dau
        /// CopyDirectoryRecursiveAsync, phuc vu tinh % tien do tong the. Chi la UOC
        /// TINH (co the it hon thuc te neu co nhanh bi bo qua do loi quyen/IO ngay
        /// trong luc dem) - khong anh huong den qua trinh sao chep thuc te,
        /// CopyDirectoryRecursiveAsync tu bat loi rieng cho tung muc o buoc do nhu cu.
        /// </summary>
        private static int CountFiles(string folderPath)
        {
            int count = 0;

            try
            {
                count += Directory.GetFiles(folderPath).Length;

                foreach (string subDir in Directory.GetDirectories(folderPath))
                {
                    count += CountFiles(subDir);
                }
            }
            catch (UnauthorizedAccessException) { /* Bo qua rieng nhanh nay - chi anh huong do chinh xac cua uoc tinh. */ }
            catch (IOException) { /* Tuong tu. */ }

            return count;
        }

        /// <summary>
        /// Trang thai dung chung (mutable) cho MOT LAN goi CopyFolderAsync, cong don
        /// so file da xong qua nhieu file/thu muc con de tinh FileOperationProgress
        /// tren TOAN BO cay thu muc dang sao chep - xem CopyFolderAsync va
        /// CopyDirectoryRecursiveAsync. Chi duoc tao khi co noi theo doi tien do
        /// (progress != null trong CopyFolderAsync).
        /// </summary>
        private class CopyProgressState
        {
            public IProgress<FileOperationProgress> Progress;
            public int TotalFiles;
            public int FilesCompleted;

            /// <summary>Dong goi va gui mot FileOperationProgress voi trang thai FilesCompleted/TotalFiles hien tai.</summary>
            public void Report(string currentFileName, long currentFileBytesTransferred, long currentFileTotalBytes)
            {
                Progress.Report(new FileOperationProgress
                {
                    CurrentFileName = currentFileName,
                    FilesCompleted = FilesCompleted,
                    TotalFiles = TotalFiles,
                    CurrentFileBytesTransferred = currentFileBytesTransferred,
                    CurrentFileTotalBytes = currentFileTotalBytes
                });
            }
        }

        /// <summary>
        /// Chuyen tiep IProgress&lt;long&gt; (so byte luy ke CUA MOT file, do
        /// FileService.CopyFileAsync bao cao) thanh IProgress&lt;FileOperationProgress&gt;
        /// (tien do CUA CA THU MUC, do CopyProgressState.Progress - VD: MainForm -
        /// lang nghe), bang cach ghep them FilesCompleted/TotalFiles hien tai cua
        /// CopyProgressState va dung luong cua file nay (doc mot lan luc khoi tao,
        /// best-effort - loi thi coi nhu 0, khong lam gian doan qua trinh copy chi
        /// vi khong lay duoc dung luong de hien thi %).
        /// </summary>
        private class FileBytesProgressAdapter : IProgress<long>
        {
            private readonly CopyProgressState _state;
            private readonly string _fileName;
            private readonly long _fileTotalBytes;

            public FileBytesProgressAdapter(CopyProgressState state, string sourceFilePath)
            {
                _state = state;
                _fileName = Path.GetFileName(sourceFilePath);

                try { _fileTotalBytes = new FileInfo(sourceFilePath).Length; }
                catch (IOException) { _fileTotalBytes = 0; }
                catch (UnauthorizedAccessException) { _fileTotalBytes = 0; }
            }

            public void Report(long bytesTransferredForThisFile)
            {
                _state.Report(_fileName, bytesTransferredForThisFile, _fileTotalBytes);
            }
        }
    }

    /// <summary>
    /// Ket qua duyet de quy mot thu muc: tong dung luong (byte) va so luong tep/
    /// thu muc con tim duoc. Tra ve boi FolderService.GetFolderStatistics - tach
    /// rieng thanh struct (khong phai tuple) de ten cac truong ro rang tai noi
    /// goi (VD: PropertiesForm) thay vi Item1/Item2/Item3.
    /// </summary>
    public struct FolderStatistics
    {
        /// <summary>Tong dung luong (byte) cua tat ca tep tim duoc (de quy).</summary>
        public long TotalBytes { get; set; }

        /// <summary>So luong TEP (khong tinh thu muc) tim duoc, de quy toan bo cay thu muc con.</summary>
        public long FileCount { get; set; }

        /// <summary>So luong THU MUC CON tim duoc (khong tinh chinh thu muc goc), de quy toan bo cay thu muc con.</summary>
        public long FolderCount { get; set; }
    }
}
