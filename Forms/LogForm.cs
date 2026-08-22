using System.Windows.Forms;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Form rong, du kien dung de xem lich su thao tac (menu Cong cu > Xem nhat ky
    /// hoat dong), ket hop voi LogService.GetLogs(). Hien tai chua co noi dung/control
    /// gi - se bo sung khi co yeu cau cu the (VD: ListView liet ke LogEntryModel).
    /// </summary>
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }
    }
}
