using System;
using System.Collections.Generic;
using System.Threading;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop xu ly tim kiem file/thu muc theo nhieu tieu chi (ten, phan mo rong,
    /// kich thuoc, thoi gian sua doi...). Cac phuong thuc hien tai chi la khai bao
    /// (signature) + TODO, can trien khai logic thuc te ben trong.
    /// Ket qua tra ve la FileItemModel (dai dien ca file va thu muc) de tan dung
    /// lai model da co san.
    /// </summary>
    public class SearchService
    {
        // TODO: co the tiem (inject) them cac service khac neu can, VD:
        // - FolderService/FileService de tai su dung logic doc thong tin muc

        public SearchService()
        {
            // TODO: khoi tao cac phu thuoc (dependency) neu co, hien tai chua can.
        }

        /// <summary>
        /// Tim kiem file/thu muc co ten chua tu khoa (khong phan biet hoa/thuong).
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="keyword">Tu khoa can tim trong ten file/thu muc.</param>
        /// <param name="recursive">True: tim ca trong thu muc con (de quy). False: chi tim trong rootPath.</param>
        public List<FileItemModel> SearchByName(string rootPath, string keyword, bool recursive = true)
        {
            // TODO: EnumerateFileSystemEntries de quy (neu recursive), so sanh
            // Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tim kiem file theo phan mo rong (VD: ".docx", ".jpg").
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="extension">Phan mo rong can tim (co hoac khong co dau cham).</param>
        /// <param name="recursive">True: tim ca trong thu muc con.</param>
        public List<FileItemModel> SearchByExtension(string rootPath, string extension, bool recursive = true)
        {
            // TODO: chuan hoa extension (them "." neu thieu), so sanh
            // Path.GetExtension(...) khong phan biet hoa/thuong.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tim kiem file co kich thuoc trong mot khoang (byte).
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="minBytes">Kich thuoc toi thieu (byte).</param>
        /// <param name="maxBytes">Kich thuoc toi da (byte).</param>
        /// <param name="recursive">True: tim ca trong thu muc con.</param>
        public List<FileItemModel> SearchBySizeRange(string rootPath, long minBytes, long maxBytes, bool recursive = true)
        {
            // TODO: chi ap dung cho file (IsDirectory == false), loc theo Size.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tim kiem file/thu muc co thoi gian sua doi trong mot khoang.
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="fromDate">Tu ngay.</param>
        /// <param name="toDate">Den ngay.</param>
        /// <param name="recursive">True: tim ca trong thu muc con.</param>
        public List<FileItemModel> SearchByModifiedDateRange(string rootPath, DateTime fromDate, DateTime toDate, bool recursive = true)
        {
            // TODO: loc theo ModifiedDate cua FileItemModel trong khoang [fromDate, toDate].
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tim kiem tong quat theo tu khoa, co ho tro huy (cancel) giua chung -
        /// nen dung cho tim kiem tren thu muc lon de khong lam treo UI.
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="keyword">Tu khoa can tim.</param>
        /// <param name="recursive">True: tim ca trong thu muc con.</param>
        /// <param name="cancellationToken">Token cho phep huy qua trinh tim kiem.</param>
        public List<FileItemModel> Search(string rootPath, string keyword, bool recursive, CancellationToken cancellationToken)
        {
            // TODO: giong SearchByName nhung kiem tra cancellationToken.IsCancellationRequested
            // (hoac ThrowIfCancellationRequested()) sau moi buoc de dung som khi bi huy.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tim cac file trung lap (noi dung giong nhau) trong mot thu muc.
        /// Moi phan tu ket qua la mot nhom (>= 2 file) duoc xac dinh la trung lap.
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="recursive">True: tim ca trong thu muc con.</param>
        /// <remarks>
        /// Goi y trien khai (de hieu qua voi thu muc lon, tranh hash tat ca file):
        /// 1. Liet ke toan bo file, nhom theo Size truoc (2 file kich thuoc khac nhau
        ///    chac chan khong trung noi dung -> loai ngay, khong can hash).
        /// 2. Voi moi nhom co >= 2 file cung Size, tinh hash noi dung (VD: SHA256 qua
        ///    System.Security.Cryptography) va nhom tiep theo hash.
        /// 3. Chi giu lai cac nhom (theo hash) co >= 2 file - do la cac nhom trung lap thuc su.
        /// </remarks>
        public List<List<FileItemModel>> FindDuplicateFiles(string rootPath, bool recursive = true)
        {
            throw new NotImplementedException();
        }
    }
}
