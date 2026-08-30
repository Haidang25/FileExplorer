using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh tim va hien thi file trung noi dung (menu Cong cu > Tim file
    /// trung lap), mo tu MainForm.mnuToolsFindDuplicates_Click. Goi
    /// DuplicateService.FindDuplicateFiles roi hien KET QUA THEO NHOM tren
    /// lvwDuplicates: moi nhom trung lap (>= 2 file cung noi dung) la MOT
    /// ListViewGroup rieng, giup nguoi dung de nhan biet "day la N file
    /// giong nhau" thay vi mot danh sach dai khong phan biet nhom nao voi
    /// nhom nao.
    /// </summary>
    public partial class DuplicateForm : Form
    {
        private readonly DuplicateService _duplicateService = new DuplicateService();
        private readonly string _rootFolder;

        // Chi khac null trong luc dang co mot lan quet dang chay - xem
        // SearchForm._searchCts (cung mau: nut Huy goi Cancel(), null lai ngay
        // khi quet ket thuc de tranh goi Cancel() tren CancellationTokenSource
        // da Dispose()).
        private CancellationTokenSource _scanCts;

        public DuplicateForm(string rootFolder)
        {
            InitializeComponent();

            _rootFolder = rootFolder;
            lblRootFolderValue.Text = rootFolder;

            // Tu dong quet ngay khi mo form (nguoi dung da chon "Tim file
            // trung lap" tu menu Cong cu voi y dinh ro rang la muon xem ket
            // qua ngay, khong can bam them nut Quet lan dau) - btnScan van
            // giu nguyen de quet LAI sau do (VD: sau khi da xoa vai file
            // trung lap, muon xem lai danh sach con lai).
            this.Load += async (sender, e) => await RunScanAsync();
        }

        /// <summary>
        /// Chay DuplicateService.FindDuplicateFiles tren mot luong nen
        /// (Task.Run) - ham nay la DONG BO/CHAN (khong phai async iterator
        /// nhu SearchService.SearchAsync), va co the ton kha nhieu thoi gian
        /// voi thu muc lon (hash noi dung tung file) - Task.Run tranh treo
        /// UI thread trong luc quet, "await" ben duoi nhuong lai dieu khien
        /// cho UI trong luc cho ket qua.
        /// </summary>
        private async Task RunScanAsync()
        {
            lvwDuplicates.Groups.Clear();
            lvwDuplicates.Items.Clear();
            lblStatus.Text = "Đang quét...";
            btnScan.Enabled = false;
            btnCancelScan.Enabled = true;

            bool recursive = chkRecursive.Checked;

            using (_scanCts = new CancellationTokenSource())
            {
                CancellationToken token = _scanCts.Token;

                try
                {
                    List<List<FileItemModel>> duplicateGroups = await Task.Run(
                        () => _duplicateService.FindDuplicateFiles(_rootFolder, recursive, token),
                        token);

                    PopulateResults(duplicateGroups);

                    lblStatus.Text = duplicateGroups.Count == 0
                        ? "Không tìm thấy tệp trùng lặp nào."
                        : $"Tìm thấy {duplicateGroups.Count} nhóm trùng lặp ({CountTotalFiles(duplicateGroups)} tệp).";
                }
                catch (OperationCanceledException)
                {
                    lblStatus.Text = "Đã hủy quét.";
                }
                finally
                {
                    btnScan.Enabled = true;
                    btnCancelScan.Enabled = false;
                    _scanCts = null;
                }
            }
        }

        private static int CountTotalFiles(List<List<FileItemModel>> groups)
        {
            int total = 0;
            foreach (List<FileItemModel> group in groups)
            {
                total += group.Count;
            }
            return total;
        }

        /// <summary>
        /// Do ket qua vao lvwDuplicates THEO NHOM: moi nhom trong
        /// duplicateGroups tro thanh MOT ListViewGroup rieng (Header hien so
        /// thu tu + so luong tep + kich thuoc moi tep, VD "Nhóm 1 — 3 tệp,
        /// mỗi tệp 2.5 MB"), cac ListViewItem cua nhom do duoc gan Group
        /// tuong ung qua ListViewItem.Group - day la co che nhom NGUYEN SINH
        /// (built-in) cua WinForms ListView (khong phai tu ve header thu
        /// cong bang ListViewItem gia), nen ListView tu ve duong phan cach +
        /// tieu de nhom, tu dong hoat dong trong ca View.Details/LargeIcon...
        /// </summary>
        private void PopulateResults(List<List<FileItemModel>> duplicateGroups)
        {
            lvwDuplicates.BeginUpdate();
            try
            {
                for (int groupIndex = 0; groupIndex < duplicateGroups.Count; groupIndex++)
                {
                    List<FileItemModel> duplicateGroup = duplicateGroups[groupIndex];

                    // Sap xep MOI NHOM theo Vi tri (ParentPath) de cac ban
                    // trung nam trong cung thu muc de nhan ra hon la thu tu
                    // ngau nhien tu Dictionary.Values (khong dam bao thu tu).
                    duplicateGroup.Sort((a, b) => string.Compare(a.ParentPath, b.ParentPath, StringComparison.OrdinalIgnoreCase));

                    string sizeText = duplicateGroup[0].SizeFormatted;
                    string headerText = string.Format(
                        CultureInfo.InvariantCulture,
                        "Nhóm {0} — {1} tệp, mỗi tệp {2}",
                        groupIndex + 1,
                        duplicateGroup.Count,
                        sizeText);

                    var listViewGroup = new ListViewGroup(headerText);
                    lvwDuplicates.Groups.Add(listViewGroup);

                    foreach (FileItemModel item in duplicateGroup)
                    {
                        var listItem = new ListViewItem(item.Name) { Tag = item.FullPath, Group = listViewGroup };
                        listItem.SubItems.Add(item.ParentPath);
                        listItem.SubItems.Add(item.SizeFormatted);
                        listItem.SubItems.Add(FormatHelper.FormatDate(item.ModifiedDate));
                        lvwDuplicates.Items.Add(listItem);
                    }
                }
            }
            finally
            {
                lvwDuplicates.EndUpdate();
            }
        }

        private async void btnScan_Click(object sender, EventArgs e)
        {
            await RunScanAsync();
        }

        private void btnCancelScan_Click(object sender, EventArgs e)
        {
            // Vo hieu hoa ngay de tranh bam nhieu lan trong luc cho vong lap
            // trong DuplicateService kip kiem tra ThrowIfCancellationRequested().
            btnCancelScan.Enabled = false;
            _scanCts?.Cancel();
        }

        /// <summary>
        /// Double-click mot ket qua: mo thu muc chua file do tren MainForm
        /// (Owner), giong hanh vi tuong tu SearchForm.lvwResults_DoubleClick -
        /// giup nguoi dung xem nhanh MOT ban cu the trong nhom truoc khi
        /// quyet dinh giu/xoa ban nao.
        /// </summary>
        private void lvwDuplicates_DoubleClick(object sender, EventArgs e)
        {
            if (lvwDuplicates.SelectedItems.Count == 0)
                return;

            string fullPath = lvwDuplicates.SelectedItems[0].Tag as string;
            if (string.IsNullOrWhiteSpace(fullPath))
                return;

            if (Owner is MainForm mainForm)
            {
                mainForm.NavigateToAndSelect(fullPath);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
