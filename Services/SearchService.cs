using System;
using System.Collections.Generic;
using System.IO;
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
        /// Ban rut gon, dong bo, khong huy duoc cua Search() - chi la lop vo mong goi
        /// lai Search() voi includeHidden: true (luon tim ca muc an/he thong) va
        /// CancellationToken.None (khong ho tro huy giua chung), danh cho cac noi goi
        /// don gian chua can toi tuy chon do (VD: goi truc tiep tu code, khong qua
        /// SearchForm). SearchForm nen dung thang Search() de co ca hai tinh nang tren.
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="keyword">Tu khoa can tim trong ten file/thu muc.</param>
        /// <param name="recursive">True: tim ca trong thu muc con (de quy). False: chi tim trong rootPath.</param>
        /// <returns>Danh sach FileItemModel (ca file lan thu muc) co Name chua keyword. Danh sach rong neu khong tim thay gi.</returns>
        public List<FileItemModel> SearchByName(string rootPath, string keyword, bool recursive = true)
        {
            return Search(rootPath, keyword, recursive, includeHidden: true, cancellationToken: CancellationToken.None);
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
        /// nen dung cho tim kiem tren thu muc lon de khong lam treo UI. Dung cho
        /// SearchForm - chay tren luong nen (Task.Run) va truyen CancellationToken
        /// tu nut Huy tren form vao day.
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="keyword">Tu khoa can tim (so sanh voi Name, khong phan biet hoa/thuong).</param>
        /// <param name="recursive">True: tim ca trong thu muc con (de quy). False: chi tim truc tiep trong rootPath.</param>
        /// <param name="includeHidden">
        /// True: tim ca trong cac muc an/he thong (Hidden/System). False: bo qua hoan
        /// toan cac muc do - ca khong dua vao ket qua LAN khong de quy vao ben trong
        /// neu do la thu muc an, giong tuy chon "Hien file/thu muc an" cua MainForm.
        /// </param>
        /// <param name="cancellationToken">
        /// Token cho phep huy qua trinh tim kiem giua chung - duoc kiem tra truoc khi
        /// xu ly moi muc, nen dung lai gan nhu ngay thay vi phai quet xong het cay
        /// thu muc con dang do dang.
        /// </param>
        /// <returns>
        /// Danh sach FileItemModel (ca file lan thu muc) co Name chua keyword. Danh
        /// sach rong (khong phai null) neu rootPath khong hop le hoac khong tim thay
        /// gi - khong nem exception ra ngoai (tru OperationCanceledException khi bi huy).
        /// </returns>
        public List<FileItemModel> Search(
            string rootPath, string keyword, bool recursive, bool includeHidden, CancellationToken cancellationToken)
        {
            var results = new List<FileItemModel>();

            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath) || string.IsNullOrWhiteSpace(keyword))
                return results;

            SearchRecursive(rootPath, keyword, recursive, includeHidden, cancellationToken, results);
            return results;
        }

        /// <summary>
        /// Duyet de quy (neu recursive) mot thu muc, gom vao results moi muc (file
        /// hoac thu muc con) co Name chua keyword. Loi quyen/IO tren tung nhanh RIENG
        /// LE (VD: mat quyen doc mot thu muc con) duoc bo qua ngay tai do, khong lam
        /// dung ca qua trinh - giong cach FolderService.GetSubFolders/FileService.GetFiles
        /// da lam.
        /// </summary>
        private static void SearchRecursive(
            string folderPath, string keyword, bool recursive, bool includeHidden,
            CancellationToken cancellationToken, List<FileItemModel> results)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<string> entryPaths;
            try
            {
                entryPaths = Directory.EnumerateFileSystemEntries(folderPath);
            }
            catch (UnauthorizedAccessException) { return; } // Khong co quyen liet ke thu muc nay - bo qua rieng nhanh nay.
            catch (IOException) { return; } // VD: thu muc nam tren o dia vua thao ra.

            foreach (string entryPath in entryPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                FileAttributes attributes;
                try
                {
                    attributes = File.GetAttributes(entryPath);
                }
                catch (UnauthorizedAccessException) { continue; } // Khong doc duoc thuoc tinh muc nay - bo qua rieng no.
                catch (IOException) { continue; } // VD: shortcut/junction hong.

                if (!includeHidden)
                {
                    bool isHiddenOrSystem = attributes.HasFlag(FileAttributes.Hidden) || attributes.HasFlag(FileAttributes.System);
                    if (isHiddenOrSystem)
                        continue; // Bo qua hoan toan - ca khoi ket qua lan khong de quy vao ben trong (neu la thu muc).
                }

                string name = Path.GetFileName(entryPath);
                if (name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    try
                    {
                        results.Add(FileItemModel.FromPath(entryPath));
                    }
                    catch (UnauthorizedAccessException) { /* Khop ten nhung khong doc them duoc thong tin chi tiet - bo qua rieng muc nay. */ }
                    catch (IOException) { /* Tuong tu. */ }
                    catch (FileNotFoundException) { /* Muc vua bi xoa giua luc quet - bo qua. */ }
                }

                bool isDirectory = attributes.HasFlag(FileAttributes.Directory);
                if (isDirectory && recursive)
                    SearchRecursive(entryPath, keyword, recursive, includeHidden, cancellationToken, results);
            }
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
