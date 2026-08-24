using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop xu ly cac thao tac lien quan den file (tao, doi ten, xoa,
    /// di chuyen, sao chep, mo file, lay thong tin/danh sach file...).
    /// Cac phuong thuc hien tai chi la khai bao (signature) + TODO, can trien khai
    /// logic thuc te ben trong. Su dung cung Models (FileItemModel, OperationResult,
    /// FileOperationType) va Helpers (FileHelper, PermissionHelper) da co san.
    /// </summary>
    public class FileService
    {
        // TODO: co the tiem (inject) them cac service khac neu can, VD:
        // - mot service ghi log (su dung LogEntryModel) de ghi lai moi thao tac

        public FileService()
        {
            // TODO: khoi tao cac phu thuoc (dependency) neu co, hien tai chua can.
        }

        /// <summary>
        /// Kiem tra file co ton tai hay khong.
        /// </summary>
        /// <param name="filePath">Duong dan file can kiem tra.</param>
        public bool FileExists(string filePath)
        {
            // TODO: kiem tra filePath hop le + File.Exists(filePath)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay thong tin chi tiet cua mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file.</param>
        /// <returns>FileItemModel chua thong tin file.</returns>
        public FileItemModel GetFileInfo(string filePath)
        {
            // TODO: dung FileItemModel.FromPath(filePath), co the ket hop
            // FileHelper.GetFileType(filePath) neu can hien thi loai file.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach cac file (khong bao gom thu muc con) truc tiep trong mot thu muc.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc chua cac file.</param>
        /// <param name="includeHidden">
        /// True: lay ca file an/he thong (Hidden/System). False: bo qua cac file do.
        /// </param>
        /// <returns>
        /// Danh sach FileItemModel (IsDirectory = false), sap xep theo ten. Danh sach
        /// rong (khong phai null) neu folderPath khong ton tai hoac khong co quyen doc.
        /// </returns>
        public List<FileItemModel> GetFiles(string folderPath, bool includeHidden = true)
        {
            var files = new List<FileItemModel>();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return files;

            try
            {
                var fileInfos = new DirectoryInfo(folderPath)
                    .EnumerateFiles()
                    .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    try
                    {
                        if (!includeHidden)
                        {
                            bool isHiddenOrSystem = fileInfo.Attributes.HasFlag(FileAttributes.Hidden)
                                || fileInfo.Attributes.HasFlag(FileAttributes.System);
                            if (isHiddenOrSystem)
                                continue;
                        }

                        files.Add(FileItemModel.FromFileInfo(fileInfo));
                    }
                    catch (UnauthorizedAccessException) { /* Khong doc duoc file nay - bo qua rieng no. */ }
                    catch (IOException) { /* VD: file dang bi khoa boi ung dung khac. */ }
                }
            }
            catch (UnauthorizedAccessException) { /* Khong co quyen liet ke thu muc - tra ve danh sach rong. */ }
            catch (IOException) { /* Thu muc nam tren o dia vua thao ra, duong dan mang bi ngat... */ }

            return files;
        }

        /// <summary>
        /// Lay danh sach TOAN BO cac muc (ca thu muc con VA file) truc tiep trong mot
        /// thu muc, thu muc liet ke truoc roi den file - giong thu tu hien thi cua
        /// Windows Explorer va lvwFiles trong MainForm. Ket hop ca FolderService (cho
        /// phan thu muc con) va FileService (cho phan file) vao mot loi goi duy nhat,
        /// tranh MainForm phai tu viet lai logic duyet + loc an/he thong o 2 noi.
        /// </summary>
        /// <param name="path">Duong dan thu muc can liet ke noi dung.</param>
        /// <param name="includeHidden">
        /// True: lay ca muc an/he thong (Hidden/System). False: bo qua cac muc do,
        /// dung khi nguoi dung tat tuy chon "Hien file/thu muc an".
        /// </param>
        /// <returns>
        /// Danh sach FileItemModel: cac thu muc con (IsDirectory = true) truoc, sap
        /// xep theo ten, roi den cac file (IsDirectory = false), cung sap xep theo
        /// ten. Danh sach rong (khong phai null) neu path khong ton tai hoac khong
        /// co quyen doc - khong nem exception ra ngoai.
        /// </returns>
        public List<FileItemModel> GetItems(string path, bool includeHidden = true)
        {
            var items = new List<FileItemModel>();

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return items;

            var folderService = new FolderService();

            foreach (FolderItemModel folder in folderService.GetSubFolders(path, includeHidden))
            {
                items.Add(new FileItemModel
                {
                    Name = folder.Name,
                    FullPath = folder.FullPath,
                    ParentPath = folder.ParentPath,
                    Extension = string.Empty,
                    IsDirectory = true,
                    Size = 0,
                    CreatedDate = folder.CreatedDate,
                    ModifiedDate = folder.ModifiedDate,
                    LastAccessedDate = folder.ModifiedDate, // FolderItemModel khong luu rieng LastAccessedDate.
                    Attributes = folder.Attributes
                });
            }

            items.AddRange(GetFiles(path, includeHidden));

            return items;
        }

        /// <summary>
        /// Tao file moi (rong) ben trong mot thu muc.
        /// </summary>
        /// <param name="parentPath">Duong dan thu muc chua file moi.</param>
        /// <param name="fileName">Ten file moi (bao gom phan mo rong).</param>
        public OperationResult CreateFile(string parentPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(parentPath) || !Directory.Exists(parentPath))
                return OperationResult.NotFound;

            if (!FileHelper.IsValidFileName(fileName))
                return OperationResult.Failed;

            string fullPath = Path.Combine(parentPath, fileName);

            if (File.Exists(fullPath) || Directory.Exists(fullPath))
                return OperationResult.Skipped; // Da ton tai muc trung ten.

            if (!PermissionHelper.HasWritePermission(parentPath))
                return OperationResult.AccessDenied;

            try
            {
                using (File.Create(fullPath))
                {
                    // Chi can tao file rong - dong ngay sau khi tao.
                }
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
        /// Doi ten mot muc bat ky - tu dong nhan biet la file hay thu muc de goi
        /// ham xu ly tuong ung (RenameFile o day, hoac FolderService.RenameFolder),
        /// giup noi goi (VD: MainForm) khong can tu kiem tra Directory.Exists roi
        /// re nhanh giua _fileService/_folderService nhu truoc.
        /// </summary>
        /// <param name="path">Duong dan hien tai cua file hoac thu muc.</param>
        /// <param name="newName">Ten moi (chi ten, khong bao gom duong dan; voi file thi bao gom phan mo rong).</param>
        /// <returns>
        /// OperationResult.NotFound neu path khong ton tai (ca file lan thu muc);
        /// cac ket qua khac giong RenameFile/FolderService.RenameFolder.
        /// </returns>
        public OperationResult Rename(string path, string newName)
        {
            if (string.IsNullOrWhiteSpace(path))
                return OperationResult.NotFound;

            if (Directory.Exists(path))
                return new FolderService().RenameFolder(path, newName);

            if (File.Exists(path))
                return RenameFile(path, newName);

            return OperationResult.NotFound;
        }

        /// <summary>
        /// Doi ten mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file hien tai.</param>
        /// <param name="newName">Ten moi (bao gom phan mo rong, khong bao gom duong dan).</param>
        public OperationResult RenameFile(string filePath, string newName)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return OperationResult.NotFound;

            if (!FileHelper.IsValidFileName(newName))
                return OperationResult.Failed;

            string directory = Path.GetDirectoryName(filePath);
            string newPath = Path.Combine(directory ?? string.Empty, newName);

            if (File.Exists(newPath) || Directory.Exists(newPath))
                return OperationResult.Skipped; // Da co muc trung ten moi.

            if (!PermissionHelper.HasWritePermission(directory))
                return OperationResult.AccessDenied;

            try
            {
                File.Move(filePath, newPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // File dang duoc mo/khoa boi chuong trinh khac (VD: dang mo trong
                // Word, Notepad++...) - tach rieng voi Failed de bao thong bao cu
                // the hon, huong dan nguoi dung dong chuong trinh do roi thu lai.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Xoa mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file can xoa.</param>
        /// <param name="permanent">
        /// True: xoa vinh vien (File.Delete). False: chuyen vao Recycle Bin
        /// (can dung thu vien ho tro, VD Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile).
        /// </param>
        public OperationResult DeleteFile(string filePath, bool permanent = false)
        {
            // TODO: kiem tra quyen, kiem tra ton tai, thuc hien xoa theo tham so permanent.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Di chuyen file sang vi tri khac.
        /// </summary>
        /// <param name="sourcePath">Duong dan file nguon.</param>
        /// <param name="destinationPath">Duong dan file dich (bao gom ten file).</param>
        public OperationResult MoveFile(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return OperationResult.NotFound;

            if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationDir = Path.GetDirectoryName(destinationPath);
            if (!PermissionHelper.HasWritePermission(destinationDir))
                return OperationResult.AccessDenied;

            try
            {
                File.Move(sourcePath, destinationPath);
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
        /// Sao chep file sang vi tri khac.
        /// </summary>
        /// <param name="sourcePath">Duong dan file nguon.</param>
        /// <param name="destinationPath">Duong dan file dich (bao gom ten file).</param>
        /// <param name="overwrite">True neu cho phep ghi de file dich da ton tai.</param>
        public OperationResult CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return OperationResult.NotFound;

            if (!overwrite && (File.Exists(destinationPath) || Directory.Exists(destinationPath)))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationDir = Path.GetDirectoryName(destinationPath);
            if (!PermissionHelper.HasWritePermission(destinationDir))
                return OperationResult.AccessDenied;

            try
            {
                File.Copy(sourcePath, destinationPath, overwrite);
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
        /// Mo file bang ung dung mac dinh cua he thong.
        /// </summary>
        /// <param name="filePath">Duong dan file can mo.</param>
        public OperationResult OpenFile(string filePath)
        {
            // TODO: kiem tra File.Exists(filePath), dung Process.Start(new ProcessStartInfo(filePath)
            // { UseShellExecute = true }) va bat try/catch Win32Exception (VD: khong co ung dung mo).
            throw new NotImplementedException();
        }
    }
}
