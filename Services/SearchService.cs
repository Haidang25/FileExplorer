using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="keyword">
        /// Tu khoa can tim trong ten file/thu muc. Neu co chua ky tu dai dien (*
        /// hoac ?) thi tu dong so khop kieu wildcard tren toan bo ten (VD: "*.docx",
        /// "báo?cáo.*"); neu khong thi tim theo kieu "chua tu khoa" nhu binh thuong.
        /// </param>
        /// <param name="recursive">True: tim ca trong thu muc con (de quy). False: chi tim trong rootPath.</param>
        /// <returns>Danh sach FileItemModel (ca file lan thu muc) co Name chua keyword. Danh sach rong neu khong tim thay gi.</returns>
        public List<FileItemModel> SearchByName(string rootPath, string keyword, bool recursive = true)
        {
            // SearchByName la ban rut gon, dong bo cho noi goi don gian (xem doc phia
            // tren) nen van gom het vao List truoc khi tra ve; noi can "tra ket qua
            // som" (VD: SearchForm) nen goi truc tiep Search() va foreach tren ket qua
            // (IEnumerable, tra ve dan tung ket qua qua yield return) thay vi qua day.
            return Search(rootPath, keyword, recursive, includeHidden: true, cancellationToken: CancellationToken.None).ToList();
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
        /// <param name="keyword">
        /// Tu khoa can tim (so sanh voi Name, khong phan biet hoa/thuong). Neu co
        /// chua ky tu dai dien (* hoac ?) thi tu dong so khop kieu wildcard tren
        /// toan bo ten (VD: "*.docx", "báo?cáo.*"); neu khong thi tim theo kieu
        /// "chua tu khoa" nhu binh thuong. Xem them IsNameMatch().
        /// </param>
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
        /// IEnumerable FileItemModel (ca file lan thu muc) co Name chua keyword, TRA
        /// VE NGAY TUNG KET QUA MOT (yield return) trong luc quet, thay vi doi quet
        /// xong toan bo cay thu muc con moi tra ve danh sach day du - nho vay noi goi
        /// (VD: SearchForm chay tren luong nen) co the hien tung ket qua len UI ngay
        /// khi tim thay, khong phai doi ca thu muc goc (co the rat lon/nhieu tang con)
        /// quet xong. Enumerable rong (khong phai null) neu rootPath khong hop le
        /// hoac khong tim thay gi - khong nem exception ra ngoai (tru
        /// OperationCanceledException khi bi huy giua luc duyet).
        /// </returns>
        public IEnumerable<FileItemModel> Search(
            string rootPath, string keyword, bool recursive, bool includeHidden, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath) || string.IsNullOrWhiteSpace(keyword))
                yield break;

            foreach (FileItemModel item in SearchRecursive(rootPath, keyword, recursive, includeHidden, cancellationToken))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Ban bat dong bo (async) cua Search() - dung async iterator (async
        /// IAsyncEnumerable + yield return) de noi goi co the "await foreach" va
        /// nhan tung ket qua NGAY khi tim thay, TRONG luc dang await, thay vi phai
        /// tu boc ca ham dong bo bang Task.Run roi doi den khi xong het (cach
        /// SearchForm dang lam voi Search()). Moi lan await foreach lap qua mot phan
        /// tu, control tu dong nhuong lai (yield) cho UI thread giua cac lan doc thu
        /// muc - phu hop de vua stream ket qua len ListView vua giu UI phan hoi,
        /// khong can Task.Run boc ngoai nua.
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="keyword">
        /// Tu khoa can tim (so sanh voi Name, khong phan biet hoa/thuong). Neu co
        /// chua ky tu dai dien (* hoac ?) thi tu dong so khop kieu wildcard tren
        /// toan bo ten; neu khong thi tim theo kieu "chua tu khoa" nhu binh thuong.
        /// Xem them IsNameMatch().
        /// </param>
        /// <param name="recursive">True: tim ca trong thu muc con (de quy). False: chi tim truc tiep trong rootPath.</param>
        /// <param name="includeHidden">True: tim ca trong cac muc an/he thong (Hidden/System).</param>
        /// <param name="cancellationToken">Token cho phep huy qua trinh tim kiem giua chung.</param>
        /// <returns>
        /// IAsyncEnumerable FileItemModel, dung voi "await foreach (var item in
        /// SearchAsync(...))" - tra ve tung ket qua ngay khi tim thay. Enumerable
        /// rong (khong phai null) neu rootPath khong hop le hoac khong tim thay gi.
        /// </returns>
        public async IAsyncEnumerable<FileItemModel> SearchAsync(
            string rootPath, string keyword, bool recursive, bool includeHidden,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath) || string.IsNullOrWhiteSpace(keyword))
                yield break;

            await foreach (FileItemModel item in SearchRecursiveAsync(rootPath, keyword, recursive, includeHidden, cancellationToken))
            {
                yield return item;
            }
        }

        /// <summary>
        /// Duyet de quy (neu recursive) mot thu muc, TRA VE NGAY (yield return) tung
        /// muc (file hoac thu muc con) co Name chua keyword - khong gom vao List roi
        /// tra ve mot lan, de nguoi goi (Search()) nhan duoc ket qua som nhat co the,
        /// thay vi phai doi quet xong toan bo cay thu muc con (co the rat sau/nhieu
        /// muc) moi thay ket qua dau tien. Loi quyen/IO tren tung nhanh RIENG LE (VD:
        /// mat quyen doc mot thu muc con) duoc bo qua ngay tai do, khong lam dung ca
        /// qua trinh - giong cach FolderService.GetSubFolders/FileService.GetFiles da lam.
        /// </summary>
        private static IEnumerable<FileItemModel> SearchRecursive(
            string folderPath, string keyword, bool recursive, bool includeHidden,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<string> entryPaths;
            try
            {
                // Directory.EnumerateFileSystemEntries (thay vi Directory.GetFileSystemEntries)
                // tu no da lazy - khong doi liet ke xong toan bo thu muc con moi bat
                // dau tra ve muc dau tien; ket hop voi yield return ben duoi thi ca
                // chuoi tu day den SearchByName/nguoi goi cuoi cung deu "tra ket qua
                // som" nhat co the.
                entryPaths = Directory.EnumerateFileSystemEntries(folderPath);
            }
            catch (UnauthorizedAccessException) { yield break; } // Khong co quyen liet ke thu muc nay - bo qua rieng nhanh nay.
            catch (IOException) { yield break; } // VD: thu muc nam tren o dia vua thao ra.

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
                if (IsNameMatch(name, keyword))
                {
                    FileItemModel matchedItem = null;
                    try
                    {
                        matchedItem = FileItemModel.FromPath(entryPath);
                    }
                    catch (UnauthorizedAccessException) { /* Khop ten nhung khong doc them duoc thong tin chi tiet - bo qua rieng muc nay. */ }
                    catch (FileNotFoundException) { /* Muc vua bi xoa giua luc quet - bo qua rieng, phai dat TRUOC IOException vi la lop con cua no. */ }
                    catch (IOException) { /* Tuong tu FileNotFoundException. */ }

                    // yield return khong duoc phep dat truc tiep trong than try/catch
                    // co catch (chi hop le trong try co finally) - nen tach FileItemModel.FromPath()
                    // ra try/catch rieng phia tren, roi yield return ket qua o day.
                    if (matchedItem != null)
                        yield return matchedItem;
                }

                bool isDirectory = attributes.HasFlag(FileAttributes.Directory);
                if (isDirectory && recursive)
                {
                    foreach (FileItemModel childItem in SearchRecursive(entryPath, keyword, recursive, includeHidden, cancellationToken))
                    {
                        yield return childItem;
                    }
                }
            }
        }

        /// <summary>
        /// Ban bat dong bo (async) cua SearchRecursive() - dung cho SearchAsync().
        /// Logic giong het SearchRecursive (xem doc phia tren), chi khac: (1) khai
        /// bao "async IAsyncEnumerable" + "await foreach" khi de quy vao thu muc con
        /// (thay vi "IEnumerable" + "foreach" thuong); (2) chen await Task.Yield()
        /// truoc khi xu ly moi entry - buoc nay KHONG chuyen sang luong khac (khac
        /// Task.Run), ma chi tra dieu khien lai cho SynchronizationContext hien tai
        /// (VD: UI thread cua SearchForm) giua cac buoc, giup UI van ve/xu ly duoc
        /// cac thong diep khac (VD: nut Huy, keo cua so) ngay trong luc dang duyet,
        /// khong can tach luong nen rieng (Task.Run) nhu SearchForm dang lam voi
        /// Search() dong bo.
        /// </summary>
        private static async IAsyncEnumerable<FileItemModel> SearchRecursiveAsync(
            string folderPath, string keyword, bool recursive, bool includeHidden,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<string> entryPaths;
            try
            {
                entryPaths = Directory.EnumerateFileSystemEntries(folderPath);
            }
            catch (UnauthorizedAccessException) { yield break; } // Khong co quyen liet ke thu muc nay - bo qua rieng nhanh nay.
            catch (IOException) { yield break; } // VD: thu muc nam tren o dia vua thao ra.

            foreach (string entryPath in entryPaths)
            {
                cancellationToken.ThrowIfCancellationRequested();

                // Nhuong lai dieu khien cho cac thong diep khac (VD: nut Huy tren
                // SearchForm) truoc khi xu ly moi muc - de UI khong bi "treo" cam
                // giac trong luc duyet mot thu muc rat nhieu muc, du van chay tren
                // cung mot luong (khong Task.Run).
                await Task.Yield();

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
                if (IsNameMatch(name, keyword))
                {
                    FileItemModel matchedItem = null;
                    try
                    {
                        matchedItem = FileItemModel.FromPath(entryPath);
                    }
                    catch (UnauthorizedAccessException) { /* Khop ten nhung khong doc them duoc thong tin chi tiet - bo qua rieng muc nay. */ }
                    catch (FileNotFoundException) { /* Muc vua bi xoa giua luc quet - bo qua rieng, phai dat TRUOC IOException vi la lop con cua no. */ }
                    catch (IOException) { /* Tuong tu FileNotFoundException. */ }

                    if (matchedItem != null)
                        yield return matchedItem;
                }

                bool isDirectory = attributes.HasFlag(FileAttributes.Directory);
                if (isDirectory && recursive)
                {
                    await foreach (FileItemModel childItem in SearchRecursiveAsync(entryPath, keyword, recursive, includeHidden, cancellationToken))
                    {
                        yield return childItem;
                    }
                }
            }
        }

        /// <summary>
        /// Kiem tra name co khop voi keyword hay khong, tu dong nhan biet 2 kieu:
        /// - Neu keyword co chua ky tu dai dien (* hoac ?): so khop kieu wildcard
        ///   tren TOAN BO ten (giong hop thoai tim kiem cua Windows Explorer) - VD:
        ///   "*.docx" khop moi file .docx, "báo?cáo.*" khop "báo cáo.txt" (? = 1 ky
        ///   tu bat ky, * = 0 hoac nhieu ky tu bat ky).
        /// - Neu khong co ky tu dai dien: giu nguyen hanh vi cu - "chua" keyword o
        ///   bat ky vi tri nao trong ten (IndexOf), khong phan biet hoa/thuong.
        /// Ca 2 truong hop deu khong phan biet hoa/thuong.
        /// </summary>
        private static bool IsNameMatch(string name, string keyword)
        {
            bool hasWildcard = keyword.IndexOf('*') >= 0 || keyword.IndexOf('?') >= 0;

            if (!hasWildcard)
                return name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;

            string pattern = "^" + Regex.Escape(keyword)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            return Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase);
        }
    }
}
