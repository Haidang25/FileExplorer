using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Form doi ten hang loat: nguoi dung go "mau ten" (pattern) vao txtPattern,
    /// lvwPreview cap nhat NGAY (khong can bam nut nao) de hien ten moi du kien
    /// cho tung file/thu muc da chon truoc do tren MainForm.
    ///
    /// Ghi chu: form nay hien chi lam PHAN THIET KE + XEM TRUOC - nut "Đổi tên"
    /// (btnApply) da co tren giao dien nhung dang Enabled = false va CHUA duoc
    /// noi logic thuc su doi ten tren dia (se lam o mot yeu cau rieng), tranh
    /// vo tinh doi ten file cua nguoi dung truoc khi tinh nang duoc yeu cau ro.
    /// </summary>
    public partial class BatchRenameForm : Form
    {
        /// <summary>
        /// Danh sach duong dan day du (file hoac thu muc) can doi ten, giu
        /// nguyen thu tu nguoi dung da chon tren MainForm - thu tu nay quyet
        /// dinh gia tri cua token {n} (so thu tu) trong pattern.
        /// </summary>
        private readonly List<string> _paths;

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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
