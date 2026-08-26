using System;
using System.IO;
using System.Windows.Forms;
using FileExplorerApp.Models;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Hop thoai hoi nguoi dung xu ly the nao khi Paste (Copy/Cut) gap mot muc da
    /// trung ten voi mot muc co san trong thu muc dich: Ghi de / Doi ten / Bo qua.
    /// Dung chung cho ca file va thu muc trung ten.
    ///
    /// Ho tro "Ap dung cho tat ca" (checkbox chkApplyToAll) de MainForm.mnuEditPaste_Click
    /// co the ghi nho lua chon nay va tu dong dung lai cho cac muc trung ten con lai
    /// trong cung mot lan Paste, khong can hoi lai tung muc mot khi nguoi dung da tick.
    /// </summary>
    public partial class ConflictResolutionForm : Form
    {
        /// <summary>Hanh dong nguoi dung da chon (Overwrite/Rename/Skip/Cancel).</summary>
        public ConflictAction SelectedAction { get; private set; } = ConflictAction.Cancel;

        /// <summary>Ten moi nguoi dung nhap khi chon Rename - chi co gia tri khi SelectedAction == Rename.</summary>
        public string NewName { get; private set; }

        /// <summary>True neu nguoi dung tick "Ap dung cho tat ca cac muc trung ten con lai".</summary>
        public bool ApplyToAll { get; private set; }

        private readonly string _sourcePath;
        private readonly string _destinationDirectory;

        /// <param name="sourcePath">Duong dan day du cua muc dang duoc dan (nguon), dung de goi y ten moi.</param>
        /// <param name="destinationDirectory">Thu muc dich dang dan vao, dung de kiem tra ten moi co con trung khong.</param>
        public ConflictResolutionForm(string sourcePath, string destinationDirectory)
        {
            InitializeComponent();

            _sourcePath = sourcePath;
            _destinationDirectory = destinationDirectory;

            string name = Path.GetFileName(sourcePath);
            lblMessage.Text = $"Đã có mục trùng tên \"{name}\" trong thư mục đích.\n" +
                "Bạn muốn ghi đè, đổi tên, hay bỏ qua mục này?";
            txtNewName.Text = SuggestNewName(sourcePath, destinationDirectory);
        }

        /// <summary>
        /// Goi y mot ten moi khong con trung trong destinationDirectory, theo dang
        /// "ten (2).ext", "ten (3).ext"... giong hanh vi Windows Explorer.
        /// </summary>
        private static string SuggestNewName(string sourcePath, string destinationDirectory)
        {
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(sourcePath);
            string extension = Path.GetExtension(sourcePath); // Rong voi thu muc - Path.GetExtension tu xu ly dung.

            for (int counter = 2; counter < 1000; counter++)
            {
                string candidate = $"{nameWithoutExtension} ({counter}){extension}";
                string candidatePath = Path.Combine(destinationDirectory, candidate);

                if (!File.Exists(candidatePath) && !Directory.Exists(candidatePath))
                    return candidate;
            }

            // Truong hop cuc hiem (999 ban trung) - tra ve ten goc, nguoi dung tu sua tiep.
            return Path.GetFileName(sourcePath);
        }

        private void btnOverwrite_Click(object sender, EventArgs e)
        {
            SelectedAction = ConflictAction.Overwrite;
            ApplyToAll = chkApplyToAll.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnRename_Click(object sender, EventArgs e)
        {
            string newName = txtNewName.Text.Trim();

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Vui lòng nhập tên mới.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!FileExplorerApp.Helpers.FileHelper.IsValidFileName(newName))
            {
                MessageBox.Show("Tên mới không hợp lệ.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string newPath = Path.Combine(_destinationDirectory, newName);
            if (File.Exists(newPath) || Directory.Exists(newPath))
            {
                MessageBox.Show("Tên mới vẫn trùng với một mục khác trong thư mục đích.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SelectedAction = ConflictAction.Rename;
            NewName = newName;
            // ApplyToAll KHONG duoc dung cho Rename (ten moi chi hop le cho muc nay) -
            // MainForm se khong ap dung ApplyToAll khi SelectedAction == Rename, du
            // checkbox co the van dang tick tren giao dien.
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSkip_Click(object sender, EventArgs e)
        {
            SelectedAction = ConflictAction.Skip;
            ApplyToAll = chkApplyToAll.Checked;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
