using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Form doi ten hang loat: nguoi dung go "mau ten" (pattern) vao txtPattern,
    /// lvwPreview cap nhat NGAY (khong can bam nut nao) de hien ten moi du kien
    /// cho tung file/thu muc da chon truoc do tren MainForm - nguoi dung XEM
    /// LAI toan bo danh sach nay, CHI KHI bam "Đổi tên" moi hien MOT hop thoai
    /// xac nhan cuoi cung roi moi thuc su doi ten tren dia (qua
    /// FileService.BatchRename) - khong co buoc trung gian nao vo tinh doi
    /// ten ma khong duoc nguoi dung xac nhan.
    /// </summary>
    public partial class BatchRenameForm : Form
    {
        /// <summary>
        /// Danh sach duong dan day du (file hoac thu muc) can doi ten, giu
        /// nguyen thu tu nguoi dung da chon tren MainForm - thu tu nay quyet
        /// dinh gia tri cua token {n} (so thu tu) trong pattern. Duoc CAP
        /// NHAT LAI (thay duong dan cu bang duong dan moi) sau moi lan doi ten
        /// thanh cong, de neu nguoi dung tiep tuc sua pattern va doi tiep,
        /// danh sach luon khop voi thuc te hien co tren dia.
        /// </summary>
        private readonly List<string> _paths;

        private readonly FileService _fileService = new FileService();
        private readonly LogService _logService = new LogService();

        public BatchRenameForm(List<string> paths)
        {
            InitializeComponent();

            _paths = paths ?? new List<string>();

            // Mau tên mac dinh: giu nguyen ten goc - de nguoi dung thay ngay
            // danh sach ben duoi truoc khi bat dau sua pattern.
            txtPattern.Text = "{name}";

            this.Load += (sender, e) => UpdatePreview();
        }

        private void txtPattern_TextChanged(object sender, EventArgs e)
        {
            UpdatePreview();
        }

        /// <summary>
        /// Tinh lai ten moi cho tung duong dan theo pattern hien tai trong
        /// txtPattern (qua FileService.GenerateBatchRenameName - CUNG mot ham
        /// se duoc dung khi thuc su ap dung doi ten, dam bao preview khong bao
        /// gio "noi doi") va ve lai toan bo lvwPreview. Cac ten moi TRUNG LAP
        /// voi nhau (khong phan biet hoa/thuong - giong quy tac he thong file
        /// Windows) duoc to do (AppTheme.Error) de canh bao truoc, vi day la
        /// truong hop se gay loi/de mat file neu that su ap dung doi ten.
        /// </summary>
        private void UpdatePreview()
        {
            string pattern = txtPattern.Text;
            var newNames = new string[_paths.Count];
            for (int i = 0; i < _paths.Count; i++)
            {
                newNames[i] = FileService.GenerateBatchRenameName(_paths[i], pattern, i);
            }

            var duplicateNewNames = new HashSet<string>(
                newNames
                    .GroupBy(n => n, StringComparer.OrdinalIgnoreCase)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key),
                StringComparer.OrdinalIgnoreCase);

            lvwPreview.BeginUpdate();
            lvwPreview.Items.Clear();
            for (int i = 0; i < _paths.Count; i++)
            {
                string oldName = Path.GetFileName(_paths[i]);
                var item = new ListViewItem(oldName);
                item.SubItems.Add(newNames[i]);
                item.Tag = _paths[i];
                if (duplicateNewNames.Contains(newNames[i]))
                    item.ForeColor = AppTheme.Error;
                lvwPreview.Items.Add(item);
            }
            lvwPreview.EndUpdate();
        }

        /// <summary>
        /// Nguoi dung da XEM QUA toan bo lvwPreview (cap nhat song song luc go
        /// pattern) va bam "Đổi tên" - hien THEM mot hop thoai xac nhan cuoi
        /// cung (mac dinh chon "Không" de tranh bam nham) truoc khi thuc su
        /// goi FileService.BatchRename doi ten tren dia, roi ghi log TUNG muc
        /// rieng le (giong nguyen tac DuplicateForm.btnDeleteSelected_Click da
        /// ap dung) va hien MOT hop thoai tong ket duy nhat.
        /// </summary>
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_paths.Count == 0)
                return;

            DialogResult confirm = MessageBox.Show(
                this,
                $"Bạn có chắc muốn đổi tên {_paths.Count} mục theo mẫu \"{txtPattern.Text}\" như danh sách xem trước bên trên?\n\n" +
                "Hành động này KHÔNG thể hoàn tác (undo).",
                "Xác nhận đổi tên",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            List<BatchRenameItemResult> results = _fileService.BatchRename(_paths, txtPattern.Text);

            int successCount = 0;
            var problemLines = new List<string>();

            for (int i = 0; i < results.Count; i++)
            {
                BatchRenameItemResult item = results[i];

                // Ghi log TUNG muc rieng le (khong gop 1 dong) vi moi muc co
                // the co OperationResult khac nhau.
                _logService.LogOperation(FileOperationType.Rename, item.OriginalPath, item.NewPath,
                    item.Result, "Đổi tên hàng loạt (BatchRenameForm)");

                if (item.Result == OperationResult.Success)
                {
                    successCount++;
                    // Cap nhat lai duong dan trong _paths thanh duong dan MOI
                    // de UpdatePreview() ben duoi (va lan doi tiep theo, neu
                    // nguoi dung sua pattern va bam "Đổi tên" lan nua) phan
                    // anh dung thuc te hien co tren dia.
                    _paths[i] = item.NewPath;
                }
                else
                {
                    problemLines.Add($"{Path.GetFileName(item.OriginalPath)}: {DescribeResult(item.Result)}");
                }
            }

            string summary = problemLines.Count == 0
                ? $"Đã đổi tên thành công {successCount} mục."
                : $"Đã đổi tên thành công {successCount} mục.\n\nKhông đổi được {problemLines.Count} mục:\n" +
                  string.Join("\n", problemLines);

            MessageBox.Show(this, summary, "Kết quả đổi tên", MessageBoxButtons.OK,
                problemLines.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            // Ve lai lvwPreview theo _paths (da duoc cap nhat o tren) va pattern
            // hien tai - giup nguoi dung thay ngay ket qua thuc te, ke ca cac
            // muc bi bo qua/loi van con giu duong dan CU trong danh sach.
            UpdatePreview();
        }

        /// <summary>
        /// Dien giai ngan gon (tieng Viet) cho MOT OperationResult khong thanh
        /// cong, dung trong danh sach loi cua hop thoai tong ket - xem cac
        /// truong hop tuong tu da giai thich chi tiet hon o
        /// MainForm.BuildOperationResultMessage.
        /// </summary>
        private static string DescribeResult(OperationResult result)
        {
            switch (result)
            {
                case OperationResult.Skipped:
                    return "đã có mục trùng tên tại vị trí đó";
                case OperationResult.AccessDenied:
                    return "không đủ quyền truy cập";
                case OperationResult.FileInUse:
                    return "đang bị chương trình khác sử dụng";
                case OperationResult.NotFound:
                    return "không tìm thấy (có thể đã bị xóa/di chuyển)";
                default:
                    return "tên không hợp lệ hoặc có lỗi xảy ra";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
