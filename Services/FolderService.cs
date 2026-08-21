using System;
using System.Collections.Generic;
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
            // TODO:
            // 1. Kiem tra FileHelper.IsValidFileName(folderName)
            // 2. Kiem tra PermissionHelper.HasWritePermission(parentPath)
            // 3. Kiem tra ten trung lap (Skipped/Failed neu da ton tai)
            // 4. Directory.CreateDirectory(...)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Doi ten mot thu muc.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc hien tai.</param>
        /// <param name="newName">Ten moi (chi ten, khong bao gom duong dan).</param>
        public OperationResult RenameFolder(string folderPath, string newName)
        {
            // TODO:
            // 1. Kiem tra FileHelper.IsValidFileName(newName)
            // 2. Kiem tra thu muc dich (cung ten) chua ton tai
            // 3. Directory.Move(folderPath, duongDanMoi)
            throw new NotImplementedException();
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
            // TODO: kiem tra quyen ghi tai dich, kiem tra trung ten, Directory.Move(...)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sao chep thu muc (va toan bo noi dung ben trong, de quy) sang vi tri khac.
        /// </summary>
        /// <param name="sourcePath">Duong dan thu muc nguon.</param>
        /// <param name="destinationPath">Duong dan thu muc dich.</param>
        public OperationResult CopyFolder(string sourcePath, string destinationPath)
        {
            // TODO: de quy tao thu muc dich va copy tung file/thu muc con (File.Copy / Directory.CreateDirectory).
            throw new NotImplementedException();
        }
    }
}
