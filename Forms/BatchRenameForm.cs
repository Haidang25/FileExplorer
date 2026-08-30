using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FileExplorerApp.Helpers;

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

        /// <summary>
        /// Nhan dien token dang "{ten_token}" hoac "{ten_token:dinh_dang}" trong
        /// pattern nguoi dung go, VD: "{name}", "{n:000}", "{date:yyyyMMdd}".
        /// </summary>
        private static readonly Regex TokenRegex = new Regex(@"\{(name|ext|n|date)(?::([^}]+))?\}", RegexOptions.IgnoreCase);

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
        /// txtPattern va ve lai toan bo lvwPreview. Cac ten moi TRUNG LAP voi
        /// nhau (khong phan biet hoa/thuong - giong quy tac he thong file
        /// Windows) duoc to do (AppTheme.Error) de canh bao truoc, vi day la
        /// truong hop se gay loi/de mat file neu that su ap dung doi ten.
        /// </summary>
        private void UpdatePreview()
        {
            string pattern = txtPattern.Text;
            var newNames = new string[_paths.Count];
            for (int i = 0; i < _paths.Count; i++)
            {
                newNames[i] = GenerateNewName(_paths[i], pattern, i);
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
        /// Sinh ten file/thu muc moi cho MOT duong dan theo pattern, thay the
        /// cac token ho tro:
        /// - {name}: ten goc, khong gom phan mo rong (Path.GetFileNameWithoutExtension).
        /// - {ext}: phan mo rong goc, KEM dau cham (VD ".jpg"); thu muc thuong
        ///   khong co phan mo rong nen se la chuoi rong.
        /// - {n} hoac {n:000}: so thu tu (bat dau tu 1 theo vi tri trong danh
        ///   sach da chon) - phan sau dau ":" quyet dinh do rong dem so 0 dau
        ///   (VD {n:000} -> "001", "002"...).
        /// - {date} hoac {date:yyyyMMdd}: ngay gio hien tai, phan sau dau ":"
        ///   la chuoi dinh dang DateTime tuy chinh.
        ///
        /// Neu pattern KHONG chua token {ext}, phan mo rong goc se duoc TU
        /// DONG noi vao cuoi ten moi - tranh nguoi dung vo tinh lam mat phan
        /// mo rong (VD go "{name}_backup" van ra "abc_backup.jpg" chu khong
        /// mat ".jpg"). Cac ky tu khong hop le trong ten file (Path.GetInvalidFileNameChars)
        /// duoc thay bang "_" de ten moi luon la mot ten file hop le.
        /// </summary>
        private static string GenerateNewName(string originalPath, string pattern, int index)
        {
            string originalName = Path.GetFileName(originalPath);
            if (string.IsNullOrWhiteSpace(pattern))
                return originalName;

            string extension = Path.GetExtension(originalPath) ?? string.Empty;

            string result = TokenRegex.Replace(pattern, match =>
            {
                string token = match.Groups[1].Value.ToLowerInvariant();
                string format = match.Groups[2].Success ? match.Groups[2].Value : null;

                switch (token)
                {
                    case "name":
                        return Path.GetFileNameWithoutExtension(originalPath);
                    case "ext":
                        return extension;
                    case "n":
                        {
                            int width = string.IsNullOrEmpty(format) ? 1 : format.Length;
                            return (index + 1).ToString().PadLeft(width, '0');
                        }
                    case "date":
                        return DateTime.Now.ToString(string.IsNullOrEmpty(format) ? "yyyyMMdd" : format);
                    default:
                        return match.Value;
                }
            });

            bool patternHasExtensionToken = pattern.IndexOf("{ext}", StringComparison.OrdinalIgnoreCase) >= 0
                || Regex.IsMatch(pattern, @"\{ext:", RegexOptions.IgnoreCase);
            if (!patternHasExtensionToken && !string.IsNullOrEmpty(extension))
                result += extension;

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                result = result.Replace(invalidChar, '_');

            return string.IsNullOrEmpty(result) ? originalName : result;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
