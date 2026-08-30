using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private readonly RecycleBinService _recycleBinService = new RecycleBinService();
        private readonly LogService _logService = new LogService();
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

        /// <summary>
        /// Xoa (chuyen vao Thung rac) cac tep DA CHON (danh dau checkbox tren
        /// lvwDuplicates) - dung checkbox thay vi SelectedItems (chon thuong)
        /// vi day la thao tac XOA HANG LOAT co the anh huong nhieu file, nen
        /// can mot cach chon RO RANG/CHU DONG (tick checkbox) hon la vo tinh
        /// bam/keo chuot lam thay doi vung chon thong thuong.
        /// </summary>
        /// <remarks>
        /// AN TOAN QUAN TRONG NHAT: KHONG cho phep xoa HET tat ca file trong
        /// MOT NHOM trung lap (VD: nhom co 3 file, nguoi dung tick ca 3) - lam
        /// vay se xoa sach ca noi dung do khoi may (dung la trung lap, nhung
        /// van la du lieu cua nguoi dung, khong nen mat toan bo mot cach vo y
        /// chi vi thao tac "don trung lap"). Validate TRUOC khi xoa bat ky
        /// file nao (khong xoa mot phan roi moi bao loi giua chung) - neu co
        /// nhom vi pham, bao loi CU THE ten nhom do va KHONG XOA GI CA, de
        /// nguoi dung tu bo tick lai it nhat 1 file/nhom truoc khi thu lai.
        /// </remarks>
        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            List<ListViewItem> checkedItems = lvwDuplicates.CheckedItems.Cast<ListViewItem>().ToList();

            if (checkedItems.Count == 0)
            {
                MessageBox.Show(this, "Chưa chọn tệp nào để xóa (tick vào ô checkbox trước tên tệp).",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Kiem tra AN TOAN: voi moi nhom (ListViewGroup) co it nhat 1 tep
            // duoc tick, neu SO TEP DUOC TICK trong nhom do BANG DUNG so
            // luong tep hien co cua nhom (tat ca deu bi tick) - vi pham quy
            // tac "phai giu lai it nhat 1 ban".
            var groupsWithAllChecked = checkedItems
                .GroupBy(item => item.Group)
                .Where(g => g.Count() == g.Key.Items.Count)
                .Select(g => g.Key.Header)
                .ToList();

            if (groupsWithAllChecked.Count > 0)
            {
                MessageBox.Show(
                    this,
                    "Không thể xóa TẤT CẢ tệp trong một nhóm trùng lặp - phải giữ lại ít nhất 1 bản.\n\n" +
                    "Nhóm vi phạm:\n" + string.Join("\n", groupsWithAllChecked) +
                    "\n\nHãy bỏ tick lại ít nhất 1 tệp trong (các) nhóm trên rồi thử lại.",
                    "Không thể xóa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                this,
                $"Bạn có chắc muốn chuyển {checkedItems.Count} tệp đã chọn vào Thùng rác?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            int successCount = 0;
            var failedNames = new List<string>();

            foreach (ListViewItem item in checkedItems)
            {
                string fullPath = item.Tag as string;
                if (string.IsNullOrWhiteSpace(fullPath))
                    continue;

                OperationResult result = _recycleBinService.DeleteToRecycleBin(fullPath);

                // Ghi log TUNG tep rieng le (khong gop 1 dong) vi moi tep co
                // the co OperationResult khac nhau - giong nguyen tac
                // MainForm.mnuEditDelete_Click da ap dung voi xoa hang loat.
                _logService.LogOperation(FileOperationType.Delete, fullPath, null, result, "Xóa tệp trùng lặp (DuplicateForm)");

                if (result == OperationResult.Success)
                {
                    successCount++;
                }
                else
                {
                    failedNames.Add(Path.GetFileName(fullPath));
                }
            }

            // Mot MessageBox TONG KET DUY NHAT (khong phai 1 MessageBox/tep) -
            // voi so luong tep co the len den vai chuc trong 1 lan xoa hang
            // loat, hien rieng tung hop thoai se rat kho chiu; nguoi dung chi
            // can biet TONG QUAN ket qua (bao nhieu thanh cong/loi ten gi).
            string summary = failedNames.Count == 0
                ? $"Đã chuyển {successCount} tệp vào Thùng rác."
                : $"Đã chuyển {successCount} tệp vào Thùng rác.\n\nKhông xóa được {failedNames.Count} tệp (có thể đang bị khóa bởi chương trình khác):\n" +
                  string.Join("\n", failedNames);

            MessageBox.Show(this, summary, "Kết quả xóa", MessageBoxButtons.OK,
                failedNames.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            // Quet lai TOAN BO tu dau (thay vi tu xoa tung dong khoi ListView)
            // de danh sach hien thi luon KHOP VOI THUC TE tren dia, tranh sai
            // lech neu co loi xay ra giua chung hoac mot nhom nay chi con lai
            // 1 tep (khong con la "trung lap" nua, nen khong nen tiep tuc
            // hien thi nhu mot nhom).
            _ = RunScanAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
