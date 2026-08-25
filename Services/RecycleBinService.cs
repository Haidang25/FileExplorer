using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop xu ly cac thao tac lien quan den Recycle Bin (Thung rac):
    /// chuyen file/thu muc vao thung rac (thay vi xoa vinh vien), khoi phuc,
    /// don thung rac, xem thong tin/dung luong thung rac.
    /// Cac phuong thuc hien tai chi la khai bao (signature) + TODO, can trien khai
    /// logic thuc te ben trong.
    /// </summary>
    /// <remarks>
    /// Goi y trien khai:
    /// - Chuyen vao thung rac: dung Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile /
    ///   DeleteDirectory voi tham so RecycleOption.SendToRecycleBin (can them reference
    ///   "Microsoft.VisualBasic" vao csproj).
    /// - Doc danh sach / khoi phuc / lay dung luong thung rac: .NET Framework khong co
    ///   API dung san day du, thuong phai dung Shell32 COM interop
    ///   (Shell32.Shell.NameSpace("shell:RecycleBinFolder")) hoac P/Invoke SHQueryRecycleBin.
    /// </remarks>
    public class RecycleBinService
    {
        public RecycleBinService()
        {
            // TODO: khoi tao cac phu thuoc (dependency) neu co, hien tai chua can.
        }

        /// <summary>
        /// Xoa mot file hoac thu muc bang cach chuyen vao Recycle Bin (khong xoa
        /// vinh vien - nguoi dung van co the khoi phuc lai qua RestoreFromRecycleBin
        /// hoac tu mo Recycle Bin cua Windows).
        /// </summary>
        /// <param name="path">Duong dan file/thu muc can xoa (chuyen vao thung rac).</param>
        public OperationResult DeleteToRecycleBin(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return OperationResult.Failed;

            try
            {
                if (Directory.Exists(path))
                {
                    FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    return OperationResult.Success;
                }

                if (File.Exists(path))
                {
                    FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    return OperationResult.Success;
                }

                return OperationResult.NotFound;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (OperationCanceledException)
            {
                // Nguoi dung bam Cancel tren hop thoai loi (neu UIOption khac OnlyErrorDialogs).
                return OperationResult.Cancelled;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // File (hoac mot file nao do ben trong thu muc) dang bi chuong trinh
                // khac khoa - tach rieng voi Failed de bao thong bao cu the hon, giong
                // da lam voi FileService.RenameFile/DeleteFile.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Khoi phuc mot muc da bi xoa tu Recycle Bin ve vi tri ban dau.
        /// </summary>
        /// <param name="originalPath">Duong dan goc cua muc truoc khi bi xoa.</param>
        public OperationResult RestoreFromRecycleBin(string originalPath)
        {
            // TODO: can duyet Recycle Bin qua Shell32 COM interop de tim muc co
            // duong dan goc (original location) trung khop, sau do goi thao tac
            // "Restore" tren item do (IFileOperation / Shell verb "undelete").
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach cac muc hien co trong Recycle Bin.
        /// </summary>
        public List<RecycleBinItemModel> GetRecycleBinItems()
        {
            // TODO: duyet qua Shell32.Shell.NameSpace("shell:RecycleBinFolder"),
            // moi ExtendedProperty (ten, duong dan goc, ngay xoa, kich thuoc)
            // map sang RecycleBinItemModel.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tinh tong dung luong hien tai cua Recycle Bin (byte).
        /// </summary>
        public long GetRecycleBinSize()
        {
            // TODO: co the dung P/Invoke SHQueryRecycleBin (shell32.dll) de lay
            // i64Size, hoac cong tong Size cua GetRecycleBinItems().
            throw new NotImplementedException();
        }

        /// <summary>
        /// Kiem tra Recycle Bin co dang rong hay khong.
        /// </summary>
        public bool IsRecycleBinEmpty()
        {
            // TODO: GetRecycleBinItems().Count == 0, hoac kiem tra nhanh qua SHQueryRecycleBin.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Don sach toan bo Recycle Bin (xoa vinh vien tat ca cac muc dang co).
        /// </summary>
        public OperationResult EmptyRecycleBin()
        {
            // TODO: P/Invoke SHEmptyRecycleBin (shell32.dll) voi cac co
            // SHERB_NOCONFIRMATION / SHERB_NOPROGRESSUI / SHERB_NOSOUND tuy nhu cau.
            throw new NotImplementedException();
        }
    }
}
