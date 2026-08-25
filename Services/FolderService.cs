using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// hoac chay tren luong rieng khi tich hop vao UI.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc.</param>
        public long GetFolderSize(string folderPath)
        {
            // TODO: de quy qua toan bo file/thu muc con va cong tong Length.
            throw new NotImplementedException();
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
                // mot phan roi bo qua phan con lai nhu CopyDirectoryRecursive).
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

            if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationParent = Directory.GetParent(destinationPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(destinationParent))
                return OperationResult.AccessDenied;

            try
            {
                Directory.Move(sourcePath, destinationPath);
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
        /// Sao chep thu muc (va toan bo noi dung ben trong, de quy) sang vi tri khac.
        ///
        /// Neu mot thu muc con hoac file nao do ben trong khong doc/ghi duoc (VD:
        /// mat quyen, file dang bi khoa boi ung dung khac), chi bo qua rieng muc do
        /// va tiep tuc sao chep phan con lai cua cay thu muc, thay vi huy toan bo
        /// thao tac. Dung SkippedPaths (xem sau ham nay) de biet co bo sot gi khong.
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
            skippedPaths = new List<string>();

            if (string.IsNullOrWhiteSpace(sourcePath) || !Directory.Exists(sourcePath))
                return OperationResult.NotFound;

            if (Directory.Exists(destinationPath) || File.Exists(destinationPath))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationParent = Directory.GetParent(destinationPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(destinationParent))
                return OperationResult.AccessDenied;

            try
            {
                // Chi loi o cap thu muc goc (khong tao duoc destinationDir dau tien -
                // VD: destinationParent bi mat quyen dung luc kiem tra o tren) moi lam
                // that bai toan bo thao tac - loi o cac thu muc/file con ben trong deu
                // duoc CopyDirectoryRecursive tu bat va ghi vao skippedPaths.
                CopyDirectoryRecursive(sourcePath, destinationPath, skippedPaths);
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
        /// Sao chep de quy toan bo noi dung mot thu muc (bao gom thu muc con) sang vi
        /// tri moi. Loi quyen/IO tren tung file hoac thu muc con RIENG LE duoc bat va
        /// bo qua ngay tai do (ghi duong dan loi vao skippedPaths), khong lam dung
        /// hoac huy phan con lai cua de quy - chi loi khi tao chinh destinationDir
        /// nay (Directory.CreateDirectory ngay dau ham) moi duoc nem ra ngoai, vi day
        /// la dieu kien tien quyet de co the sao chep bat cu thu gi vao ben trong no.
        /// </summary>
        private static void CopyDirectoryRecursive(string sourceDir, string destinationDir, List<string> skippedPaths)
        {
            Directory.CreateDirectory(destinationDir);

            try
            {
                foreach (string filePath in Directory.GetFiles(sourceDir))
                {
                    try
                    {
                        string destFilePath = Path.Combine(destinationDir, Path.GetFileName(filePath));
                        File.Copy(filePath, destFilePath, overwrite: false);
                    }
                    catch (UnauthorizedAccessException) { skippedPaths.Add(filePath); }
                    catch (IOException) { skippedPaths.Add(filePath); } // VD: file dang bi ung dung khac khoa.
                }
            }
            catch (UnauthorizedAccessException) { skippedPaths.Add(sourceDir); }
            catch (IOException) { skippedPaths.Add(sourceDir); } // Khong liet ke duoc danh sach file cua sourceDir.

            try
            {
                foreach (string subDir in Directory.GetDirectories(sourceDir))
                {
                    string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                    CopyDirectoryRecursive(subDir, destSubDir, skippedPaths);
                }
            }
            catch (UnauthorizedAccessException) { skippedPaths.Add(sourceDir); }
            catch (IOException) { skippedPaths.Add(sourceDir); } // Khong liet ke duoc danh sach thu muc con cua sourceDir.
        }
    }
}
