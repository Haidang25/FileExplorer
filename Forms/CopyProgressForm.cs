using System;
using System.Drawing;
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
    /// Khong tu huy gi ca - chi phat CancelRequested khi nguoi dung bam nut Huy, noi
    /// goi (MainForm) moi la noi thuc su Cancel() mot CancellationTokenSource va
    /// truyen CancellationToken do xuong FileService/FolderService. ControlBox = false
    /// (khong co nut X) de bat buoc di qua nut Huy nay - tranh nham lan la dong hop
    /// thoai (X) se dung duoc thao tac dang chay ngam.
    /// </summary>
    public partial class CopyProgressForm : Form
    {
        /// <summary>Phat khi nguoi dung bam nut Huy - noi goi tu Cancel() CancellationTokenSource cua minh.</summary>
        public event EventHandler CancelRequested;

        public CopyProgressForm()
        {
            InitializeComponent();
            FileExplorerApp.Helpers.UiScale.Apply(this); // Phong to giao dien de trinh chieu (xem Helpers/UiScale.cs).
            ArrangeControlsAfterScaling();
        }

        /// <summary>
        /// Sap xep lai noi dung sau khi UiScale va Windows DPI scaling da chay.
        ///
        /// Designer luu ClientSize theo kich thuoc 9pt. Tren Windows 10 co DPI cao,
        /// cac control co the duoc phong lon hon chieu cao client cua Form, lam nut
        /// Huy bi cat o mep duoi. Khong dung cac he so DPI co dinh o day; thay vao do
        /// lay chinh kich thuoc/PreferredSize da scale cua tung control de tinh lai
        /// kich thuoc cua so.
        /// </summary>
        private void ArrangeControlsAfterScaling()
        {
            int margin = Math.Max(16, Font.Height);
            int gap = Math.Max(8, Font.Height / 2);

            int labelHeight = Math.Max(lblCurrentItem.Height, lblCurrentItem.PreferredHeight);
            int percentHeight = Math.Max(lblPercent.Height, lblPercent.PreferredHeight);
            int progressHeight = Math.Max(progressBar.Height, Font.Height + gap);

            Size preferredButtonSize = btnCancel.GetPreferredSize(Size.Empty);
            int buttonWidth = Math.Max(btnCancel.Width, preferredButtonSize.Width + gap * 2);
            int buttonHeight = Math.Max(btnCancel.Height, preferredButtonSize.Height + gap);

            int contentWidth = Math.Max(progressBar.Width,
                Math.Max(lblCurrentItem.Width, lblPercent.Width));
            int clientWidth = Math.Max(ClientSize.Width, contentWidth + margin * 2);
            int fullWidth = clientWidth - margin * 2;

            int y = margin;
            lblCurrentItem.SetBounds(margin, y, fullWidth, labelHeight);

            y = lblCurrentItem.Bottom + gap;
            progressBar.SetBounds(margin, y, fullWidth, progressHeight);

            y = progressBar.Bottom + gap;
            lblPercent.SetBounds(margin, y, fullWidth, percentHeight);

            y = lblPercent.Bottom + gap;
            btnCancel.SetBounds(clientWidth - margin - buttonWidth, y, buttonWidth, buttonHeight);

            ClientSize = new Size(clientWidth, btnCancel.Bottom + margin);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Vo hieu hoa ngay de tranh nguoi dung bam nhieu lan trong luc cho thao
            // tac thuc su dung lai (co the mat mot chut thoi gian vi phai cho buffer
            // dang doc/ghi do dang hoan tat truoc khi ThrowIfCancellationRequested()
            // duoc kiem tra o vong lap tiep theo).
            btnCancel.Enabled = false;
            btnCancel.Text = "Đang hủy...";
            CancelRequested?.Invoke(this, EventArgs.Empty);
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
