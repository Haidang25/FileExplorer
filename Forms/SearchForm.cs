using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh tim kiem file/thu muc (menu Cong cu > Tim kiem..., hoac Enter tren o
    /// tim kiem cua thanh cong cu MainForm). Nhap tu khoa + thu muc goc + tuy chon,
    /// goi SearchService.Search() tren luong nen (Task.Run) de khong lam treo UI, ho
    /// tro Huy giua chung qua CancellationTokenSource - cung mau voi CopyProgressForm/
    /// mnuEditPaste_Click da lam voi thao tac Copy.
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
                try
                {
                    // SearchService.Search() la ham dong bo (co the mat thoi gian voi
                    // thu muc lon) - chay qua Task.Run de khong chan UI thread, giong
                    // ly do CopyFileAsync/CopyFolderAsync duoc thiet ke bat dong bo.
                    List<FileItemModel> results = await Task.Run(
                        () => _searchService.Search(rootFolder, keyword, recursive, includeHidden, token), token);

                    PopulateResults(results);
                    lblStatus.Text = $"Tìm thấy {results.Count} mục.";
                }
                catch (OperationCanceledException)
                {
                    lblStatus.Text = "Đã hủy tìm kiếm.";
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

        /// <summary>Do danh sach ket qua vao lvwResults, dung BeginUpdate/EndUpdate de tranh nhap nhay khi co nhieu ket qua.</summary>
        private void PopulateResults(List<FileItemModel> results)
        {
            lvwResults.BeginUpdate();
            try
            {
                foreach (FileItemModel item in results)
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
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
