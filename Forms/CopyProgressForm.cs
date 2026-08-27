using System.Windows.Forms;
using FileExplorerApp.Models;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Hop thoai hien ProgressBar trong luc dan (Copy) file/thu muc, giong hop thoai
    /// "Copying..." cua Windows Explorer. Hien MODELESS (Show(), khong phai
    /// ShowDialog()) de khong chan luong await trong MainForm.mnuEditPaste_Click -
    /// ca cua so chinh lan hop thoai nay deu tiep tuc phan hoi trong luc copy.
    ///
    /// Chua ho tro Cancel (ControlBox = false, khong co nut Huy) - se bo sung sau
    /// neu can, luc do FileService/FolderService can them CancellationToken.
    /// </summary>
    public partial class CopyProgressForm : Form
    {
        public CopyProgressForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cap nhat ProgressBar + cac nhan trang thai theo mot moc tien do moi.
        /// LUON duoc goi tren UI thread - noi goi (MainForm) tao Progress&lt;T&gt; ngay
        /// tren UI thread nen Report() tu dong Post() callback ve dung thread do (xem
        /// chu thich cua Models.FileOperationProgress) - nen co the gan truc tiep vao
        /// thuoc tinh cua control ma khong can Invoke/BeginInvoke.
        /// </summary>
        /// <param name="progress">Moc tien do moi nhan duoc.</param>
        public void UpdateProgress(FileOperationProgress progress)
        {
            if (IsDisposed)
                return; // Form co the da dong (VD: nguoi dung dong MainForm) truoc khi bao cao cuoi cung toi.

            progressBar.Value = progress.PercentComplete;

            lblCurrentItem.Text = string.IsNullOrEmpty(progress.CurrentFileName)
                ? "Đang chuẩn bị..."
                : $"Đang sao chép: \"{progress.CurrentFileName}\"";

            lblPercent.Text = progress.TotalFiles > 1
                ? $"{progress.FilesCompleted}/{progress.TotalFiles} mục — {progress.PercentComplete}%"
                : $"{progress.PercentComplete}%";
        }
    }
}
