using System;
using System.Collections.Generic;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh xem noi dung Recycle Bin (menu Cong cu > Thung rac), doc du
    /// lieu tu <see cref="RecycleBinService.GetRecycleBinItems"/>. Cho phep
    /// khoi phuc mot/nhieu muc da chon ve vi tri goc, hoac don trong toan bo
    /// Thung rac.
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE: mirror cau truc cua LogForm (doc TOAN BO danh
    /// sach mot lan vao _allItems, ve len ListView, "Lam moi" doc lai tu dia)
    /// de nhat quan voi Form xem-danh-sach-chi-doc khac da co trong ung dung,
    /// thay vi tao mot khuon mau moi.
    /// </remarks>
    public partial class RecycleBinForm : Form
    {
        private readonly RecycleBinService _recycleBinService = new RecycleBinService();
        private List<RecycleBinItemModel> _allItems = new List<RecycleBinItemModel>();

        public RecycleBinForm()
        {
            InitializeComponent();
            ApplyTheme();
            LoadItems();
        }

        /// <summary>
        /// Ap dung AppTheme cho lvwItems va 2 nut hanh dong chinh (btnRestore/
        /// btnEmptyRecycleBin), giong quy uoc da dung tai LogForm.ApplyTheme.
        /// </summary>
        private void ApplyTheme()
        {
            lvwItems.BackColor = AppTheme.Surface;
            lvwItems.ForeColor = AppTheme.TextPrimary;
            lvwItems.BorderStyle = BorderStyle.FixedSingle;

            btnRestore.FlatStyle = FlatStyle.Flat;
            btnRestore.FlatAppearance.BorderColor = AppTheme.Accent;
            btnRestore.BackColor = AppTheme.Accent;
            btnRestore.ForeColor = System.Drawing.Color.White;

            btnEmptyRecycleBin.FlatStyle = FlatStyle.Flat;
            btnEmptyRecycleBin.FlatAppearance.BorderColor = AppTheme.Error;
            btnEmptyRecycleBin.BackColor = AppTheme.Error;
            btnEmptyRecycleBin.ForeColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// Doc lai TOAN BO danh sach tu RecycleBinService.GetRecycleBinItems()
        /// roi ve lai lvwItems - goi luc mo Form va sau moi thao tac thay doi
        /// noi dung Thung rac (Khoi phuc/Don trong) de danh sach hien thi luon
        /// khop voi thuc te.
        /// </summary>
        private void LoadItems()
        {
            _allItems = _recycleBinService.GetRecycleBinItems();
            PopulateListView(_allItems);
        }

        /// <summary>
        /// Ve lai lvwItems tu danh sach RecycleBinItemModel, va cap nhat
        /// lblStatus voi tong so muc + tong dung luong dang hien thi.
        /// </summary>
        private void PopulateListView(List<RecycleBinItemModel> items)
        {
            lvwItems.BeginUpdate();
            lvwItems.Items.Clear();

            long totalSize = 0;
            foreach (RecycleBinItemModel model in items)
            {
                var listItem = new ListViewItem(model.Name);
                listItem.SubItems.Add(model.OriginalPath ?? string.Empty);
                listItem.SubItems.Add(FormatHelper.FormatDate(model.DeletedDate));
                listItem.SubItems.Add(model.IsDirectory ? string.Empty : FormatHelper.FormatSize(model.Size));
                listItem.SubItems.Add(model.IsDirectory ? "Thư mục" : "Tệp");
                // Luu ca model goc vao Tag - btnRestore_Click can OriginalPath
                // CHINH XAC (khong phai chuoi da dinh dang lai) de goi
                // RestoreFromRecycleBin.
                listItem.Tag = model;

                lvwItems.Items.Add(listItem);
                totalSize += model.Size;
            }

            lvwItems.EndUpdate();
            lblStatus.Text = $"{items.Count} mục - Tổng dung lượng: {FormatHelper.FormatSize(totalSize)}";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadItems();
        }

        /// <summary>
        /// Khoi phuc TOAN BO cac muc dang duoc chon (lvwItems.SelectedItems) ve
        /// vi tri goc - cho phep chon nhieu muc cung luc (lvwItems.MultiSelect =
        /// true, xem Designer.cs) de khoi phuc theo lo, khong bat nguoi dung
        /// phai lam tung muc mot khi can khoi phuc nhieu file/thu muc.
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (lvwItems.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Vui lòng chọn ít nhất một mục để khôi phục.",
                    "Khôi phục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            int successCount = 0;
            var failedNames = new List<string>();

            foreach (ListViewItem selected in lvwItems.SelectedItems)
            {
                var model = (RecycleBinItemModel)selected.Tag;
                OperationResult result = _recycleBinService.RestoreFromRecycleBin(model.OriginalPath);

                if (result == OperationResult.Success)
                    successCount++;
                else
                    failedNames.Add(model.Name);
            }

            // Doc lai danh sach NGAY sau khi khoi phuc (du thanh cong mot phan
            // hay toan bo) - cac muc da khoi phuc thanh cong khong con trong
            // Thung rac nua, phai bien mat khoi lvwItems ngay, khong doi nguoi
            // dung tu bam "Lam mới".
            LoadItems();

            if (failedNames.Count == 0)
            {
                MessageBox.Show(
                    this,
                    $"Đã khôi phục {successCount} mục về vị trí ban đầu.",
                    "Khôi phục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                // Ap dung ErrorHandler tap trung (xem Helpers/ErrorHandler.cs)
                // thay MessageBox.Show rai rac - kem huong dan khoi phuc thu
                // cong vi RecycleBinService.RestoreFromRecycleBin co the khong
                // tim thay verb "Khôi phục" tren mot so phien ban/ngon ngu
                // Windows (xem remarks tai RecycleBinService.RestoreFromRecycleBin).
                ErrorHandler.Show(
                    this,
                    $"Khôi phục thành công {successCount} mục. Không thể khôi phục {failedNames.Count} mục:\n" +
                    string.Join(", ", failedNames) +
                    "\n\nCó thể vị trí gốc không còn tồn tại, hoặc phiên bản Windows hiện tại không hỗ trợ khôi phục tự động - " +
                    "bạn có thể mở Thùng rác (Recycle Bin) của Windows Explorer để khôi phục các mục này thủ công.",
                    "Khôi phục");
            }
        }

        /// <summary>
        /// Don sach toan bo Recycle Bin (xoa vinh vien) sau khi nguoi dung xac
        /// nhan - day la thao tac KHONG THE HOAN TAC (khac voi DeleteToRecycleBin
        /// thong thuong, cac muc nay se KHONG con cach nao khoi phuc lai duoc
        /// nua), nen bat buoc hoi lai truoc, cung tinh than voi
        /// LogForm.btnClearLogs_Click.
        /// </summary>
        private void btnEmptyRecycleBin_Click(object sender, EventArgs e)
        {
            if (_allItems.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Thùng rác đang trống.",
                    "Dọn trống thùng rác",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                this,
                $"Bạn có chắc chắn muốn xóa VĨNH VIỄN toàn bộ {_allItems.Count} mục trong Thùng rác?\nHành động này KHÔNG THỂ hoàn tác.",
                "Dọn trống thùng rác",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            OperationResult result = _recycleBinService.EmptyRecycleBin();
            LoadItems();

            if (result == OperationResult.Success)
            {
                MessageBox.Show(
                    this,
                    "Đã dọn trống Thùng rác.",
                    "Dọn trống thùng rác",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                ErrorHandler.Show(
                    this,
                    "Không thể dọn trống Thùng rác. Vui lòng thử lại sau.",
                    "Dọn trống thùng rác");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
