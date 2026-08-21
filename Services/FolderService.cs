using System;
using System.Collections.Generic;
using System.IO;
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
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc cha.</param>
        public List<FolderItemModel> GetSubFolders(string folderPath)
        {
            // TODO: enumerate DirectoryInfo(folderPath).GetDirectories(),
            // map sang FolderItemModel.FromDirectoryInfo cho tung item.
            throw new NotImplementedException();
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
        /// True: xoa vinh vien (Directory.Delete). False: chuyen vao Recycle Bin
        /// (can dung thu vien ho tro, VD Microsoft.VisualBasic.FileIO.FileSystem.DeleteDirectory).
        /// </param>
        public OperationResult DeleteFolder(string folderPath, bool permanent = false)
        {
            // TODO: kiem tra quyen, kiem tra ton tai, thuc hien xoa theo tham so permanent.
            throw new NotImplementedException();
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
        /// </summary>
        /// <param name="sourcePath">Duong dan thu muc nguon.</param>
        /// <param name="destinationPath">Duong dan thu muc dich.</param>
        public OperationResult CopyFolder(string sourcePath, string destinationPath)
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
                CopyDirectoryRecursive(sourcePath, destinationPath);
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
        /// Sao chep de quy toan bo noi dung mot thu muc (bao gom thu muc con) sang vi tri moi.
        /// </summary>
        private static void CopyDirectoryRecursive(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string destFilePath = Path.Combine(destinationDir, Path.GetFileName(filePath));
                File.Copy(filePath, destFilePath, overwrite: false);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectoryRecursive(subDir, destSubDir);
            }
        }
    }
}
