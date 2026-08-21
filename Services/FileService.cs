using System;
using System.Collections.Generic;
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
        public List<FileItemModel> GetFiles(string folderPath)
        {
            // TODO: enumerate DirectoryInfo(folderPath).GetFiles(),
            // map sang FileItemModel.FromFileInfo cho tung item.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tao file moi (rong) ben trong mot thu muc.
        /// </summary>
        /// <param name="parentPath">Duong dan thu muc chua file moi.</param>
        /// <param name="fileName">Ten file moi (bao gom phan mo rong).</param>
        public OperationResult CreateFile(string parentPath, string fileName)
        {
            // TODO:
            // 1. Kiem tra FileHelper.IsValidFileName(fileName)
            // 2. Kiem tra PermissionHelper.HasWritePermission(parentPath)
            // 3. Kiem tra file trung ten (Skipped/Failed neu da ton tai)
            // 4. File.Create(...)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Doi ten mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file hien tai.</param>
        /// <param name="newName">Ten moi (bao gom phan mo rong, khong bao gom duong dan).</param>
        public OperationResult RenameFile(string filePath, string newName)
        {
            // TODO:
            // 1. Kiem tra FileHelper.IsValidFileName(newName)
            // 2. Kiem tra file dich (cung ten) chua ton tai
            // 3. File.Move(filePath, duongDanMoi)
            throw new NotImplementedException();
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
            // TODO: kiem tra quyen ghi tai dich, kiem tra trung ten, File.Move(...)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sao chep file sang vi tri khac.
        /// </summary>
        /// <param name="sourcePath">Duong dan file nguon.</param>
        /// <param name="destinationPath">Duong dan file dich (bao gom ten file).</param>
        /// <param name="overwrite">True neu cho phep ghi de file dich da ton tai.</param>
        public OperationResult CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            // TODO: kiem tra quyen ghi tai dich, xu ly truong hop trung ten
            // (ghi de/doi ten tu dong/bo qua tuy overwrite), File.Copy(...)
            throw new NotImplementedException();
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
