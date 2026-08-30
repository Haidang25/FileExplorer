using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>Giai doan hien tai cua qua trinh tim file trung lap - xem DuplicateScanProgress.</summary>
    public enum DuplicateScanPhase
    {
        /// <summary>
        /// Giai doan 1: dang liet ke toan bo file trong rootPath va nhom theo
        /// Size. TONG SO file CHUA THE BIET TRUOC (phai duyet het moi biet co
        /// bao nhieu file) - xem DuplicateScanProgress.TotalCount = -1 trong
        /// giai doan nay.
        /// </summary>
        EnumeratingFiles,

        /// <summary>
        /// Giai doan 2: dang tinh hash MD5 cho cac file "ung vien" (cung Size
        /// voi it nhat 1 file khac). TONG SO file can hash DA BIET TRUOC (tinh
        /// xong Giai doan 1 moi biet duoc, nhung la mot con so co dinh trong
        /// suot Giai doan 2) - xem DuplicateScanProgress.TotalCount.
        /// </summary>
        HashingCandidates
    }

    /// <summary>
    /// Tien trinh bao cao qua IProgress&lt;DuplicateScanProgress&gt; trong luc
    /// DuplicateService.FindDuplicateFiles dang chay - xem tham so progress
    /// cua FindDuplicateFiles.
    /// </summary>
    public struct DuplicateScanProgress
    {
        /// <summary>Giai doan hien tai (EnumeratingFiles hoac HashingCandidates).</summary>
        public DuplicateScanPhase Phase { get; set; }

        /// <summary>So file da xu ly xong trong giai doan hien tai.</summary>
        public int ProcessedCount { get; set; }

        /// <summary>
        /// Tong so file can xu ly trong giai doan hien tai, hoac -1 neu chua
        /// biet truoc (chi xay ra trong Phase.EnumeratingFiles - xem ghi chu
        /// tai DuplicateScanPhase.EnumeratingFiles). Noi nhan (VD: DuplicateForm)
        /// nen hien ProgressBar dang Marquee (khong xac dinh) khi TotalCount
        /// &lt; 0, va dang Continuous (co Value/Maximum cu the) khi TotalCount &gt;= 0.
        /// </summary>
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// Tim cac file trung noi dung (khong chi trung ten) trong mot thu muc,
    /// dung cho MainForm.mnuToolsFindDuplicates_Click ("Tìm file trùng lặp").
    /// Tach rieng khoi SearchService (chi tap trung tim theo TEN/tu khoa) vi
    /// day la mot bai toan khac ve ban chat: so sanh NOI DUNG file voi nhau,
    /// khong lien quan den ten/keyword nguoi dung nhap.
    /// </summary>
    public class DuplicateService
    {
        /// <summary>
        /// Khoang cach toi thieu (thoi gian) giua 2 lan goi progress.Report -
        /// giong FolderService.FolderScanProgressInterval (150ms), tranh goi
        /// Report qua day dac (VD: hang nghin lan/giay voi thu muc nhieu file
        /// nho) lam ngop luong UI nhan progress (Progress&lt;T&gt; tu dong post moi
        /// lan Report len UI thread qua SynchronizationContext).
        /// </summary>
        private static readonly TimeSpan ProgressReportInterval = TimeSpan.FromMilliseconds(150);

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
        /// <param name="progress">
        /// Nhan bao cao tien trinh (tuy chon, mac dinh null = khong bao cao)
        /// qua ca 2 giai doan - xem DuplicateScanProgress/DuplicateScanPhase.
        /// Thuong la Progress&lt;DuplicateScanProgress&gt; duoc tao TREN LUONG UI
        /// (VD: DuplicateForm) TRUOC KHI goi FindDuplicateFiles tu mot luong
        /// nen (Task.Run) - IProgress&lt;T&gt;.Report tu dong marshal ve dung
        /// SynchronizationContext luc Progress&lt;T&gt; duoc tao, nen noi goi
        /// KHONG CAN tu Invoke/BeginInvoke ben trong callback cua progress.
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
        public List<List<FileItemModel>> FindDuplicateFiles(
            string rootPath, bool recursive = true, CancellationToken cancellationToken = default,
            IProgress<DuplicateScanProgress> progress = null)
        {
            var result = new List<List<FileItemModel>>();

            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
                return result;

            // Chi tao Stopwatch khi thuc su co progress de theo doi - giong
            // FolderService.GetFolderStatisticsCore, tranh chi phi Stopwatch
            // khong can thiet khi noi goi khong quan tam tien trinh (progress
            // == null, VD: goi FindDuplicateFiles tu code/test don gian).
            Stopwatch progressStopwatch = progress != null ? Stopwatch.StartNew() : null;

            // Giai doan 1: liet ke TOAN BO file (khong lay thu muc) va nhom
            // theo Size, dung LAI SearchService.Search voi keyword "*" (khop
            // moi ten qua co che wildcard cua no) thay vi tu viet vong lap
            // duyet thu muc rieng.
            var groupsBySize = new Dictionary<long, List<FileItemModel>>();
            int scannedCount = 0;

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
                scannedCount++;

                // TotalCount = -1 vi Giai doan 1 CHUA THE BIET TRUOC tong so
                // file trong rootPath (phai duyet het moi biet) - xem
                // DuplicateScanPhase.EnumeratingFiles.
                if (progressStopwatch != null && progressStopwatch.Elapsed >= ProgressReportInterval)
                {
                    progress.Report(new DuplicateScanProgress { Phase = DuplicateScanPhase.EnumeratingFiles, ProcessedCount = scannedCount, TotalCount = -1 });
                    progressStopwatch.Restart();
                }
            }

            // Bao cao MOC HOAN TAT Giai doan 1 (khong throttle - chi 1 lan duy
            // nhat khi chuyen giai doan) de noi nhan chac chan thay duoc con
            // so cuoi cung cua Giai doan 1, tranh truong hop lan Report gan
            // nhat (do throttle) dung lai o mot con so cu hon so thuc te.
            progress?.Report(new DuplicateScanProgress { Phase = DuplicateScanPhase.EnumeratingFiles, ProcessedCount = scannedCount, TotalCount = -1 });

            // Tinh TRUOC tong so file "ung vien" (thuoc nhom Size co >= 2 file)
            // se can hash trong Giai doan 2 - biet duoc con so nay ngay tu dau
            // Giai doan 2 (khac Giai doan 1) nen co the bao cao TotalCount cu
            // the ngay tu dau, cho phep noi nhan hien ProgressBar dang
            // Continuous (co % hoan thanh ro rang) thay vi Marquee.
            int totalCandidates = 0;
            foreach (List<FileItemModel> sameSizeGroup in groupsBySize.Values)
            {
                if (sameSizeGroup.Count >= 2)
                    totalCandidates += sameSizeGroup.Count;
            }

            int hashedCount = 0;
            progressStopwatch?.Restart();

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
                    finally
                    {
                        // Tang hashedCount VA bao cao KE CA khi file bi loi/bo
                        // qua o tren - file do van da duoc "xu ly xong" (du
                        // khong dong gop vao ket qua), nen van tinh vao tien
                        // do de thanh %/ProgressBar khong bi dung lai/khong
                        // bao gio cham 100% neu co file loi trong qua trinh.
                        hashedCount++;

                        if (progressStopwatch != null && progressStopwatch.Elapsed >= ProgressReportInterval)
                        {
                            progress.Report(new DuplicateScanProgress { Phase = DuplicateScanPhase.HashingCandidates, ProcessedCount = hashedCount, TotalCount = totalCandidates });
                            progressStopwatch.Restart();
                        }
                    }

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

            // Bao cao MOC HOAN TAT Giai doan 2 (khong throttle) - dam bao
            // ProgressBar/lblStatus luon ket thuc dung o 100% (hashedCount ==
            // totalCandidates), khong dung lai o mot con so throttle cu hon.
            progress?.Report(new DuplicateScanProgress { Phase = DuplicateScanPhase.HashingCandidates, ProcessedCount = hashedCount, TotalCount = totalCandidates });

            return result;
        }
    }
}
