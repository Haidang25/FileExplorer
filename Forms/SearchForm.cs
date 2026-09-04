using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh tim kiem file/thu muc (menu Cong cu > Tim kiem..., hoac Enter tren o
    /// tim kiem cua thanh cong cu MainForm). Nhap tu khoa + thu muc goc + tuy chon,
    /// goi SearchService.SearchAsync() (async IAsyncEnumerable) qua "await foreach"
    /// de nhan ket qua NGAY khi tim thay, khong lam treo UI ma cung khong can tach
    /// luong nen (Task.Run) rieng - ho tro Huy giua chung qua CancellationTokenSource,
    /// cung mau voi CopyProgressForm/mnuEditPaste_Click da lam voi thao tac Copy. Ket
    /// qua duoc GOM THEO LO (xem ResultBatchSize/FlushResultBatch) truoc khi do vao
    /// lvwResults, thay vi them tung dong mot, de giam so lan ve lai ListView.
    /// </summary>
    public partial class SearchForm : Form
    {
        private readonly SearchService _searchService = new SearchService();

        // Chi khac null trong luc dang co mot lan tim kiem dang chay - dung de nut
        // Huy (btnCancelSearch_Click) co the goi Cancel(); null lai ngay khi tim
        // kiem ket thuc (thanh cong, loi, hay bi huy) de tranh goi Cancel() tren mot
        // CancellationTokenSource da Dispose().
        private CancellationTokenSource _searchCts;

        public SearchForm() : this(rootFolder: null, keyword: null)
        {
        }

        /// <summary>
        /// Khoi tao SearchForm voi thu muc goc va tu khoa co san (VD: MainForm truyen
        /// san _currentPath va noi dung dang go trong o tim kiem cua thanh cong cu).
        /// </summary>
        /// <param name="rootFolder">Duong dan thu muc goc dien san vao txtRootFolder. Bo qua neu null/rong.</param>
        /// <param name="keyword">Tu khoa dien san vao txtKeyword. Bo qua neu null/rong.</param>
        public SearchForm(string rootFolder, string keyword = null)
        {
            InitializeComponent();

            if (!string.IsNullOrWhiteSpace(rootFolder))
                txtRootFolder.Text = rootFolder;

            if (!string.IsNullOrWhiteSpace(keyword))
                txtKeyword.Text = keyword;
        }

        private void btnBrowseRootFolder_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (!string.IsNullOrWhiteSpace(txtRootFolder.Text) && Directory.Exists(txtRootFolder.Text))
                    dialog.SelectedPath = txtRootFolder.Text;

                if (dialog.ShowDialog(this) == DialogResult.OK)
                    txtRootFolder.Text = dialog.SelectedPath;
            }
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string keyword = txtKeyword.Text.Trim();
            string rootFolder = txtRootFolder.Text.Trim();

            if (string.IsNullOrEmpty(keyword))
            {
                MessageBox.Show(this, "Nhập từ khóa cần tìm.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtKeyword.Focus();
                return;
            }

            if (string.IsNullOrEmpty(rootFolder) || !Directory.Exists(rootFolder))
            {
                MessageBox.Show(this, "Thư mục gốc không hợp lệ hoặc không tồn tại.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRootFolder.Focus();
                return;
            }

            bool recursive = chkRecursive.Checked;
            bool includeHidden = chkIncludeHidden.Checked;

            lvwResults.Items.Clear();
            lblStatus.Text = "Đang tìm kiếm...";
            btnSearch.Enabled = false;
            btnCancelSearch.Enabled = true;

            using (_searchCts = new CancellationTokenSource())
            {
                CancellationToken token = _searchCts.Token;
                int foundCount = 0;

                // Do thoi gian tim kiem thuc te (tu luc bat dau den luc xong/bi huy) -
                // hien kem so ket qua trong lblStatus, xem FormatElapsed() ben duoi.
                var stopwatch = Stopwatch.StartNew();

                // Buffer tich luy ket qua giua cac lan do vao lvwResults - xem
                // ResultBatchSize va FlushResultBatch() ben duoi.
                var batch = new List<FileItemModel>(ResultBatchSize);

                try
                {
                    // SearchService.SearchAsync() la async iterator (IAsyncEnumerable) -
                    // "await foreach" nhan tung ket qua NGAY khi tim thay, ngay trong
                    // luc dang await (khac voi Search() dong bo cu, phai boc ca ham
                    // bang Task.Run va doi den khi xong toan bo moi co ket qua dau
                    // tien). Van giu duoc loi ich "thay ket qua dan" do, nhung thay vi
                    // do tung dong mot vao lvwResults (nhieu lan cap nhat UI lien tiep,
                    // co the giat/nhap nhay khi ket qua ra rat nhanh), ta gom lai theo
                    // LO (ResultBatchSize muc/lan) va chi do vao ListView khi du 1 lo -
                    // xem FlushResultBatch(). Khong dung .WithCancellation(token) o day
                    // (extension do thuoc package System.Linq.Async, chua duoc cai) -
                    // CancellationToken da duoc truyen thang vao SearchAsync() lam
                    // tham so cuoi (co [EnumeratorCancellation]) nen viec huy van hoat
                    // dong dung, chi khac ve cu phap goi.
                    // onItemsScanned: goi dinh ky ngay ca khi CHUA tim thay ket qua khop
                    // nao (foundCount van = 0) - truoc day lblStatus chi doi den khi du
                    // 1 lo (ResultBatchSize = 50) ket qua moi cap nhat, nen khi quet mot
                    // cay lon (VD: ca o C:) ma khong khop gi ca, man hinh dung im hoan
                    // toan tu luc bam Tim kiem - de nguoi dung nham la ung dung bi treo,
                    // du thuc te van dang quet binh thuong. Bien scannedCount duoi day
                    // duoc doc lai trong lblStatus ca o cac cho khac (sau khi hoan tat/
                    // bi huy) de con so cuoi hien thi luon khop voi lan bao cao gan nhat.
                    int scannedCount = 0;

                    await foreach (FileItemModel item in _searchService.SearchAsync(
                        rootFolder, keyword, recursive, includeHidden, token,
                        onItemsScanned: count =>
                        {
                            scannedCount = count;
                            if (foundCount == 0)
                                lblStatus.Text = $"Đang quét... đã kiểm tra {scannedCount:N0} mục, chưa thấy kết quả ({FormatElapsed(stopwatch.Elapsed)}).";
                        }))
                    {
                        batch.Add(item);
                        foundCount++;

                        if (batch.Count >= ResultBatchSize)
                        {
                            FlushResultBatch(batch);
                            lblStatus.Text = $"Đang tìm... đã thấy {foundCount} mục ({FormatElapsed(stopwatch.Elapsed)}).";
                        }
                    }

                    // Do het phan con lai trong buffer (chua du 1 lo) sau khi quet xong.
                    FlushResultBatch(batch);
                    stopwatch.Stop();

                    // Truong hop khong tim thay gi (foundCount == 0): thong bao rieng,
                    // ro rang hon la de lblStatus hien "Tìm thấy 0 mục..." de nguoi
                    // dung de nham la loi/dang tim, va goi y kiem tra lai tu khoa/tuy
                    // chon (VD: quen bat "Tim trong thu muc con") - giong cach Windows
                    // Explorer hien "Không tìm thấy mục nào khớp với tìm kiếm của bạn."
                    lblStatus.Text = foundCount == 0
                        ? $"Không tìm thấy mục nào khớp với \"{keyword}\" ({FormatElapsed(stopwatch.Elapsed)})."
                        : $"Tìm thấy {foundCount} mục trong {FormatElapsed(stopwatch.Elapsed)}.";

                    if (foundCount == 0)
                    {
                        MessageBox.Show(this,
                            $"Không tìm thấy tệp/thư mục nào khớp với \"{keyword}\" trong " +
                            $"\"{rootFolder}\"{(recursive ? " (kể cả thư mục con)" : "")}.\n\n" +
                            "Hãy kiểm tra lại từ khóa hoặc thử bật tùy chọn tìm trong thư mục con.",
                            "Không tìm thấy kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (OperationCanceledException)
                {
                    // Van do het nhung ket qua da tich luy duoc truoc khi bi huy,
                    // khong de mat phan con lai trong buffer.
                    FlushResultBatch(batch);
                    stopwatch.Stop();
                    lblStatus.Text = $"Đã hủy tìm kiếm (đã thấy {foundCount} mục sau {FormatElapsed(stopwatch.Elapsed)}).";
                }
                finally
                {
                    btnSearch.Enabled = true;
                    btnCancelSearch.Enabled = false;
                    _searchCts = null;
                }
            }
        }

        /// <summary>
        /// So ket qua toi da tich luy truoc khi do mot lo vao lvwResults - can bang
        /// giua "thay ket qua dan" (so nho hon thi do thuong xuyen hon, gan giong
        /// tung dong mot) va giam so lan cap nhat UI (so lon hon thi it giat/nhap
        /// nhay hon, nhung phai doi lau hon moi thay lo dau tien).
        /// </summary>
        private const int ResultBatchSize = 50;

        /// <summary>
        /// Dinh dang mot khoang thoi gian (TimeSpan tu Stopwatch) thanh chuoi ngan
        /// hien thi trong lblStatus - duoi 1 giay thi hien theo milli-giay (VD:
        /// "370 ms", giong cach da bao cao trong buoc test 1.000 file truoc day cua
        /// project nay), tu 1 giay tro len thi hien theo giay voi 1 so le (VD: "2.3 s").
        /// </summary>
        private static string FormatElapsed(TimeSpan elapsed)
        {
            return elapsed.TotalSeconds < 1
                ? $"{elapsed.TotalMilliseconds:0} ms"
                : $"{elapsed.TotalSeconds:0.0} s";
        }

        private void btnCancelSearch_Click(object sender, EventArgs e)
        {
            // Vo hieu hoa ngay de tranh bam nhieu lan trong luc cho vong lap trong
            // SearchService.SearchRecursive kip kiem tra ThrowIfCancellationRequested().
            btnCancelSearch.Enabled = false;
            _searchCts?.Cancel();
        }

        /// <summary>
        /// Do toan bo ket qua dang co trong batch vao lvwResults theo MOT LO (dung
        /// BeginUpdate/EndUpdate de ListView chi ve lai 1 lan cho ca lo, thay vi ve
        /// lai sau moi dong nhu khi them tung muc mot), roi xoa buffer de tiep tuc
        /// tich luy lo tiep theo. Khong lam gi ca (khong goi BeginUpdate/EndUpdate)
        /// neu batch dang rong - tranh ve lai ListView vo ich khi khong co gi moi.
        /// </summary>
        private void FlushResultBatch(List<FileItemModel> batch)
        {
            if (batch.Count == 0)
                return;

            lvwResults.BeginUpdate();
            try
            {
                foreach (FileItemModel item in batch)
                {
                    var listItem = new ListViewItem(item.Name) { Tag = item.FullPath };
                    listItem.SubItems.Add(item.ParentPath);
                    listItem.SubItems.Add(item.IsDirectory ? string.Empty : item.SizeFormatted);
                    listItem.SubItems.Add(FormatHelper.FormatDate(item.ModifiedDate));
                    lvwResults.Items.Add(listItem);
                }
            }
            finally
            {
                lvwResults.EndUpdate();
            }

            batch.Clear();
        }

        /// <summary>
        /// Double-click mot ket qua trong lvwResults: mo thu muc chua file/thu muc
        /// do tren MainForm (Owner cua SearchForm - xem cach mnuToolsSearch_Click
        /// tao SearchForm(_currentPath, ...) roi ShowDialog(this)), giong hanh vi
        /// "Open file location" cua Windows Explorer. Khong lam gi neu khong co muc
        /// nao dang duoc double-click (VD: click vao vung trong cua ListView) hoac
        /// Owner khong phai MainForm (phong truong hop SearchForm duoc mo boi mot
        /// noi khac trong tuong lai).
        /// </summary>
        private void lvwResults_DoubleClick(object sender, EventArgs e)
        {
            if (lvwResults.SelectedItems.Count == 0)
                return;

            string fullPath = lvwResults.SelectedItems[0].Tag as string;
            if (string.IsNullOrWhiteSpace(fullPath))
                return;

            if (Owner is MainForm mainForm)
            {
                mainForm.NavigateToAndSelect(fullPath);
                // Dong SearchForm ngay sau khi dieu huong xong, giong hanh vi
                // Windows Explorer (chon "Open file location" tu ket qua tim kiem
                // se dua nguoi dung ve luon cua so chinh) - tranh de 2 cua so (ket
                // qua tim kiem + MainForm) mo cung luc gay roi.
                Close();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
