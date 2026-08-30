using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Tim cac file trung noi dung (khong chi trung ten) trong mot thu muc,
    /// dung cho MainForm.mnuToolsFindDuplicates_Click ("Tìm file trùng lặp").
    /// Tach rieng khoi SearchService (chi tap trung tim theo TEN/tu khoa) vi
    /// day la mot bai toan khac ve ban chat: so sanh NOI DUNG file voi nhau,
    /// khong lien quan den ten/keyword nguoi dung nhap.
    /// </summary>
    public class DuplicateService
    {
        // Composition (khong ke thua) voi SearchService - tai su dung phuong
        // thuc Search() CONG KHAI da co san (duyet thu muc, bo qua loi
        // quyen/IO tren tung nhanh rieng le, wildcard keyword) de liet ke toan
        // bo file trong rootPath, thay vi tu viet lai mot vong lap
        // Directory.EnumerateFileSystemEntries khac o day - tranh trung lap
        // logic duyet thu muc giua 2 Service.
        private readonly SearchService _searchService;

        public DuplicateService() : this(new SearchService())
        {
        }

        /// <summary>
        /// Constructor nhan SearchService tu ben ngoai (dependency injection) -
        /// huu ich cho unit test (co the truyen mot SearchService gia/mock) va
        /// cho phep noi goi (VD: MainForm) dung LAI CUNG mot instance
        /// SearchService da co san thay vi DuplicateService tu tao instance
        /// rieng khong can thiet.
        /// </summary>
        public DuplicateService(SearchService searchService)
        {
            _searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
        }

        /// <summary>
        /// Tim cac file trung lap (noi dung giong nhau) trong mot thu muc.
        /// Moi phan tu ket qua la mot nhom (>= 2 file) duoc xac dinh la trung
        /// lap (cung Size VA cung hash MD5 noi dung).
        /// </summary>
        /// <param name="rootPath">Thu muc goc bat dau tim kiem.</param>
        /// <param name="recursive">True: tim ca trong thu muc con.</param>
        /// <param name="cancellationToken">
        /// Cho phep huy giua chung (VD: nguoi dung dong man hinh tim trung lap
        /// dang chay) - duoc kiem tra o CA hai giai doan (liet ke file VA tinh
        /// hash), vi giai doan hash co the ton nhieu thoi gian hon giai doan
        /// liet ke voi thu muc chua nhieu file lon.
        /// </param>
        /// <remarks>
        /// TOI UU 2 GIAI DOAN de tranh phai hash TOAN BO file trong rootPath
        /// (rat cham voi thu muc lon/nhieu file dung luong cao - hash la thao
        /// tac ton I/O + CPU dang ke, xem HashHelper):
        ///
        /// Giai doan 1 - Nhom theo Size: liet ke toan bo file (khong quan tam
        /// thu muc, chi lay muc KHONG phai IsDirectory) qua
        /// SearchService.Search(rootPath, "*", ...) (wildcard "*" khop MOI
        /// ten), nhom theo Size vao Dictionary&lt;long, List&lt;FileItemModel&gt;&gt;.
        /// Day la buoc LOC RE TIEN (chi so sanh mot so da co san tu
        /// FileItemModel.FromPath, khong can doc noi dung file) dua tren
        /// nguyen ly: HAI FILE KICH THUOC KHAC NHAU CHAC CHAN KHONG THE co noi
        /// dung giong nhau - loai bo ngay cac nhom chi co 1 file (kich thuoc
        /// "doc nhat", khong co ung vien trung lap nao khac) MA KHONG CAN hash
        /// chung, thuong loai duoc phan lon file trong mot thu muc thuc te (VD:
        /// hang tram file co kich thuoc khac nhau tung byte).
        ///
        /// Giai doan 2 - Hash cac nhom con lai: CHI voi cac nhom (theo Size) co
        /// >= 2 file (ung vien trung lap thuc su), tinh HashHelper.ComputeMd5
        /// cho tung file trong nhom, roi nhom TIEP THEO theo hash. Chi giu lai
        /// cac nhom (theo hash) co >= 2 file - day la cac nhom trung lap THUC
        /// SU (khac voi Giai doan 1, noi 2 file cung Size CO THE khac noi dung).
        ///
        /// Loi hash MOT file rieng le (VD: file bi khoa boi chuong trinh khac
        /// dung luc dang quet, mat quyen truy cap, bi xoa giua luc quet) chi
        /// LOAI BO rieng file do khoi ket qua (khong lam hong ca qua trinh tim
        /// trung lap) - giong nguyen tac "loi tren tung nhanh khong lam dung
        /// ca qua trinh" da ap dung xuyen suot cac Service khac trong du an.
        /// </remarks>
        public List<List<FileItemModel>> FindDuplicateFiles(string rootPath, bool recursive = true, CancellationToken cancellationToken = default)
        {
            var result = new List<List<FileItemModel>>();

            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
                return result;

            // Giai doan 1: liet ke TOAN BO file (khong lay thu muc) va nhom
            // theo Size, dung LAI SearchService.Search voi keyword "*" (khop
            // moi ten qua co che wildcard cua no) thay vi tu viet vong lap
            // duyet thu muc rieng.
            var groupsBySize = new Dictionary<long, List<FileItemModel>>();

            foreach (FileItemModel item in _searchService.Search(rootPath, "*", recursive, includeHidden: true, cancellationToken: cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (item.IsDirectory)
                    continue; // Chi so sanh NOI DUNG FILE - thu muc khong co "noi dung" de hash.

                if (!groupsBySize.TryGetValue(item.Size, out List<FileItemModel> sameSize))
                {
                    sameSize = new List<FileItemModel>();
                    groupsBySize[item.Size] = sameSize;
                }
                sameSize.Add(item);
            }

            // Giai doan 2: voi moi nhom Size co >= 2 file (ung vien trung lap),
            // hash tung file roi nhom tiep theo theo hash - chi nhom con lai
            // MOI CAN hash, cac nhom Size chi co 1 file bi loai NGAY tu Giai
            // doan 1 (xem remarks), tranh hash nhung file chac chan khong
            // trung lap voi ai khac trong rootPath.
            foreach (List<FileItemModel> sameSizeGroup in groupsBySize.Values)
            {
                if (sameSizeGroup.Count < 2)
                    continue;

                cancellationToken.ThrowIfCancellationRequested();

                var groupsByHash = new Dictionary<string, List<FileItemModel>>();

                foreach (FileItemModel item in sameSizeGroup)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    string hash;
                    try
                    {
                        hash = HashHelper.ComputeMd5(item.FullPath, cancellationToken);
                    }
                    catch (UnauthorizedAccessException) { continue; } // Mat quyen doc file nay - bo qua rieng, khong lam hong ca nhom.
                    catch (FileNotFoundException) { continue; } // File vua bi xoa giua luc quet - phai dat TRUOC IOException vi la lop con cua no.
                    catch (IOException) { continue; } // VD: file dang bi khoa boi chuong trinh khac.

                    if (!groupsByHash.TryGetValue(hash, out List<FileItemModel> sameHash))
                    {
                        sameHash = new List<FileItemModel>();
                        groupsByHash[hash] = sameHash;
                    }
                    sameHash.Add(item);
                }

                foreach (List<FileItemModel> sameHashGroup in groupsByHash.Values)
                {
                    if (sameHashGroup.Count >= 2)
                    {
                        result.Add(sameHashGroup);
                    }
                }
            }

            return result;
        }
    }
}
