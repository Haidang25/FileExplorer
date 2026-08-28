using System;
using System.Collections.Generic;
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
    /// de nhan va hien tung ket qua NGAY khi tim thay, khong lam treo UI ma cung
    /// khong can tach luong nen (Task.Run) rieng - ho tro Huy giua chung qua
    /// CancellationTokenSource, cung mau voi CopyProgressForm/mnuEditPaste_Click da
    /// lam voi thao tac Copy.
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
                try
                {
                    // SearchService.SearchAsync() la async iterator (IAsyncEnumerable) -
                    // "await foreach" nhan tung ket qua NGAY khi tim thay, ngay trong
                    // luc dang await (khac voi Search() dong bo cu, phai boc ca ham
                    // bang Task.Run va doi den khi xong toan bo moi co ket qua dau
                    // tien). Nho vay co the them tung dong vao lvwResults va cap nhat
                    // lblStatus theo thoi gian thuc trong luc quet, khong can Task.Run.
                    // Khong dung .WithCancellation(token) o day (extension do thuoc
                    // package System.Linq.Async, chua duoc cai) - CancellationToken
                    // da duoc truyen thang vao SearchAsync() lam tham so cuoi (co
                    // [EnumeratorCancellation]) nen viec huy van hoat dong dung, chi
                    // khac ve cu phap goi.
                    await foreach (FileItemModel item in _searchService.SearchAsync(
                        rootFolder, keyword, recursive, includeHidden, token))
                    {
                        AddResultItem(item);
                        foundCount++;
                        lblStatus.Text = $"Đang tìm... đã thấy {foundCount} mục.";
                    }

                    lblStatus.Text = $"Tìm thấy {foundCount} mục.";
                }
                catch (OperationCanceledException)
                {
                    lblStatus.Text = $"Đã hủy tìm kiếm (đã thấy {foundCount} mục).";
                }
                finally
                {
                    btnSearch.Enabled = true;
                    btnCancelSearch.Enabled = false;
                    _searchCts = null;
                }
            }
        }

        private void btnCancelSearch_Click(object sender, EventArgs e)
        {
            // Vo hieu hoa ngay de tranh bam nhieu lan trong luc cho vong lap trong
            // SearchService.SearchRecursive kip kiem tra ThrowIfCancellationRequested().
            btnCancelSearch.Enabled = false;
            _searchCts?.Cancel();
        }

        /// <summary>
        /// Them MOT ket qua vao lvwResults ngay khi tim thay (goi tu await foreach
        /// trong btnSearch_Click) - thay cho PopulateResults(List) cu von phai doi
        /// gom du toan bo ket qua roi moi do mot lan. Khong dung BeginUpdate/EndUpdate
        /// o day nua vi moi lan goi chi them 1 dong (khac voi do ca loat cung luc).
        /// </summary>
        private void AddResultItem(FileItemModel item)
        {
            var listItem = new ListViewItem(item.Name) { Tag = item.FullPath };
            listItem.SubItems.Add(item.ParentPath);
            listItem.SubItems.Add(item.IsDirectory ? string.Empty : item.SizeFormatted);
            listItem.SubItems.Add(FormatHelper.FormatDate(item.ModifiedDate));
            lvwResults.Items.Add(listItem);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
