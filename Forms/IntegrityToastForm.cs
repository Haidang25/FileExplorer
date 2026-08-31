using System;
using System.Drawing;
using System.Windows.Forms;
using FileExplorerApp.Helpers;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Popup "toast" nho, KHONG modal, TU DONG DONG sau vai giay - dung de
    /// canh bao REAL-TIME khi IntegrityService phat hien mot vi pham toan
    /// ven (hien tai chi dung cho ContentModified - "tep bi sua", xem
    /// MainForm.IntegrityService_IntegrityViolationDetected).
    /// </summary>
    /// <remarks>
    /// VI SAO TU VIET (khong dung NotifyIcon.ShowBalloonTip cua Windows): ung
    /// dung nay CHUA CO NotifyIcon/khay he thong nao san (kiem tra
    /// MainForm.Designer.cs khong thay khai bao) - them mot NotifyIcon chi de
    /// hien 1 balloon tip la thua can thiet (con keo theo phai tu quan ly
    /// icon/tooltip/an hien khi thu nho ung dung). Mot Form nho khong vien,
    /// TopMost, tu dinh vi o goc man hinh la du va don gian hon, khong phu
    /// thuoc gi vao khay he thong.
    ///
    /// KHONG dung MessageBox.Show: MessageBox la MODAL (chan ca luong UI cho
    /// den khi nguoi dung bam OK) - neu giam sat phat hien NHIEU vi pham lien
    /// tiep (VD mot chuong trinh doc hai dang sua hang loat file), nguoi dung
    /// se bi "khoa" boi mot chuoi hop thoai OK lien tuc, rat kho chiu va lam
    /// gian doan cong viec dang lam. Toast KHONG modal, tu dong bien mat, phu
    /// hop hon nhieu cho canh bao NEN (background) kieu nay.
    ///
    /// Moi lan phat hien mot vi pham MOI se tao mot instance IntegrityToastForm
    /// RIENG (xem ShowToast) - nhieu toast co the CHONG LEN NHAU o cung mot vi
    /// tri neu xay ra qua nhanh lien tiep (gioi han da biet, chap nhan duoc
    /// cho pham vi tinh nang hien tai - xep chong (stack) nhieu toast theo
    /// chieu doc se lam o mot yeu cau khac neu can).
    /// </remarks>
    public partial class IntegrityToastForm : Form
    {
        public IntegrityToastForm()
        {
            InitializeComponent();

            // Bam VAO BAT KY DAU tren toast (khong chi vien ngoai Form) deu
            // dong ngay - cac Label con NAM DE len tren Form nen phai tu
            // dang ky Click RIENG cho tung Label, click tren vung co Label
            // se KHONG "roi xuong" toi Click cua Form neu khong lam vay.
            Click += (s, e) => Close();
            lblIcon.Click += (s, e) => Close();
            lblTitle.Click += (s, e) => Close();
            lblFilePath.Click += (s, e) => Close();

            tmrAutoClose.Start();
        }

        /// <summary>
        /// Ve VIEN MONG thu cong (1px) - can thiet vi FormBorderStyle.None
        /// khong tu co vien nao, neu khong toast se trong nhu mot khoi mau
        /// "troi noi" khong ro ranh gioi tren nen desktop/cua so khac.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (var borderPen = new Pen(AppTheme.Border))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }
        }

        private void tmrAutoClose_Tick(object sender, EventArgs e)
        {
            tmrAutoClose.Stop();
            Close();
        }

        /// <summary>
        /// Tao va hien mot toast canh bao MOI o goc duoi-phai vung lam viec
        /// (working area) cua man hinh dang chua referenceControl - KHONG
        /// modal (goi Show(), khong phai ShowDialog()), tu dong dong sau
        /// tmrAutoClose.Interval (5 giay, xem Designer) hoac khi nguoi dung
        /// bam vao.
        /// </summary>
        /// <param name="referenceControl">
        /// Mot control/Form dang hien de xac dinh MAN HINH nao can hien toast
        /// (may nhieu man hinh) - KHONG duoc dung lam Owner cua toast (toast
        /// can hien doc lap, KE CA khi referenceControl dang bi thu nho luc
        /// giam sat nen phat hien vi pham - xem <remarks> dau lop). Truyen
        /// null se dung Screen.PrimaryScreen thay the.
        /// </param>
        /// <param name="filePath">Duong dan tep bi phat hien sua doi, hien o dong thu 2 cua toast.</param>
        public static void ShowToast(Control referenceControl, string filePath)
        {
            var toast = new IntegrityToastForm();
            toast.lblFilePath.Text = filePath;

            Rectangle workingArea = referenceControl != null
                ? Screen.FromControl(referenceControl).WorkingArea
                : Screen.PrimaryScreen.WorkingArea;

            const int margin = 16;
            toast.Location = new Point(
                workingArea.Right - toast.Width - margin,
                workingArea.Bottom - toast.Height - margin);

            toast.Show();
        }
    }
}
