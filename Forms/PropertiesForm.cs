using System.Windows.Forms;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Hien thi thuoc tinh (Properties) cua file/thu muc dang duoc chon trong
    /// MainForm, bo cuc giong tab "General" cua hop thoai Properties trong Windows
    /// Explorer: icon + ten muc tren cung, cac cap nhan-gia tri (Loai, Vi tri, Kich
    /// thuoc, Ngay tao/sua/truy cap), nhom checkbox thuoc tinh (Chi doc, An), va 3
    /// nut OK/Huy/Ap dung. Hien tai MOI CHI CO BO CUC (Designer.cs) - constructor
    /// nhan duong dan thuc te, doc thong tin (FileInfo/DirectoryInfo, FileHelper) va
    /// gan vao cac control se duoc bo sung sau, cung voi noi tren MainForm mo form nay.
    /// </summary>
    public partial class PropertiesForm : Form
    {
        public PropertiesForm()
        {
            InitializeComponent();
        }
    }
}
