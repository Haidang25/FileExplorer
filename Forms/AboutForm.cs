using System.Windows.Forms;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Form rong, du kien thay the hop thoai About hien dang hien bang MessageBox trong
    /// MainForm.mnuHelpAbout_Click. Hien tai chua co noi dung/control gi - se bo sung
    /// (ten san pham, phien ban, ban quyen...) va noi voi mnuHelpAbout_Click khi co
    /// yeu cau cu the.
    /// </summary>
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }
    }
}
