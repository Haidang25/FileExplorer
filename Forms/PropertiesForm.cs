using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Hien thi thuoc tinh (Properties) cua file/thu muc dang duoc chon trong
    /// MainForm, bo cuc giong tab "General" cua hop thoai Properties trong Windows
    /// Explorer: icon + ten muc tren cung, cac cap nhan-gia tri (Loai, Vi tri, Kich
    /// thuoc, Ngay tao/sua/truy cap), nhom checkbox thuoc tinh (Chi doc, An), va 3
    /// nut OK/Huy/Ap dung.
    /// </summary>
    /// <remarks>
    /// Pham vi hien tai: constructor doc va hien THUC TE 4 truong duoc yeu cau
    /// (Ten, Duong dan, Loai, Kich thuoc) tu FileItemModel.FromPath(path). Cac
    /// truong con lai tren form (Ngay tao/sua/truy cap, checkbox Chi doc/An) van
    /// duoc gan gia tri thuc luon tien (FileItemModel co san du du lieu, khong ly
    /// do de trong/hien placeholder) nhung hanh vi luu lai thay doi thuoc tinh
    /// (nut Ap dung/OK ghi de FileAttributes that su xuong dia) CHUA duoc noi day -
    /// se bo sung khi co yeu cau rieng, cung voi noi tren MainForm mo form nay.
    /// </remarks>
    public partial class PropertiesForm : Form
    {
        public PropertiesForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Khoi tao PropertiesForm va nap ngay thong tin cua mot file/thu muc cu the.
        /// </summary>
        /// <param name="path">Duong dan day du toi file hoac thu muc can xem thuoc tinh.</param>
        /// <exception cref="FileNotFoundException">path khong ton tai (ca file lan thu muc) - nem thang tu FileItemModel.FromPath.</exception>
        public PropertiesForm(string path) : this()
        {
            FileItemModel item = FileItemModel.FromPath(path);
            LoadItem(item);
        }

        /// <summary>
        /// Gan thong tin cua mot FileItemModel vao cac control tren form - tach
        /// rieng khoi constructor de sau nay co the goi lai (VD: nut Ap dung muon
        /// nap lai sau khi ghi thuoc tinh xuong dia) ma khong can tao lai ca form.
        /// </summary>
        private void LoadItem(FileItemModel item)
        {
            this.Text = item.Name;

            lblName.Text = item.Name;

            // "Loai": voi thu muc dung chuoi co dinh giong cot "Loai" cua lvwFiles
            // (xem MainForm.LoadListViewFiles) de nhat quan trong toan ung dung;
            // voi file goi FileHelper.GetFileType() - cung ham dang dung cho cot
            // "Loai" tren lvwFiles, tranh 2 noi hien 2 kieu mo ta khac nhau cho
            // cung mot loai file.
            lblTypeValue.Text = item.IsDirectory ? "Thư mục tệp" : FileHelper.GetFileType(item.FullPath);

            // "Vi tri": thu muc CHUA muc nay (ParentPath), khong phai chinh
            // FullPath cua muc - giong hop thoai Properties cua Windows, dong thoi
            // giup phan biet ro voi lblName da hien Ten o tren. Voi o dia goc
            // (ParentPath null, VD "C:\"), hien lai chinh FullPath.
            lblLocationValue.Text = item.ParentPath ?? item.FullPath;

            // "Kich thuoc": thu muc hien tai KHONG tinh tong dung luong de quy (se
            // can duyet toan bo cay thu muc con, co the rat cham voi thu muc lon,
            // giong ly do FolderService/FileService khong lam san) - hien "--" thay
            // vi mot con so gay hieu lam la da tinh chinh xac. Voi file, dung dung
            // FileItemModel.SizeFormatted (giong cot "Kich thuoc" tren lvwFiles) va
            // hien them so byte chinh xac trong ngoac, giong Windows Explorer.
            lblSizeValue.Text = item.IsDirectory
                ? "--"
                : $"{item.SizeFormatted} ({item.Size:N0} byte)";

            lblCreatedValue.Text = FormatHelper.FormatDate(item.CreatedDate);
            lblModifiedValue.Text = FormatHelper.FormatDate(item.ModifiedDate);
            lblAccessedValue.Text = FormatHelper.FormatDate(item.LastAccessedDate);

            chkReadOnly.Checked = item.IsReadOnly;
            chkHidden.Checked = item.IsHidden;

            // picIcon: dung icon that cua FILE tu he thong (giong bieu tuong
            // Windows Explorer hien trong hop thoai Properties that), khong dung
            // lai ImageList (imlIcons) cua MainForm vi danh sach do chi co vai icon
            // nhom chung (anh/van ban/nen...), khong phai icon rieng tung loai file
            // nhu Windows tu ve. Icon.ExtractAssociatedIcon chi nhan duong dan FILE
            // (nem ArgumentException voi thu muc) - voi thu muc de trong (null),
            // chua co icon thu muc chuan de tai su dung ngoai imlIcons cua MainForm.
            if (!item.IsDirectory)
            {
                try
                {
                    using (Icon icon = Icon.ExtractAssociatedIcon(item.FullPath))
                    {
                        picIcon.Image = icon?.ToBitmap();
                    }
                }
                catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is System.Security.SecurityException)
                {
                    picIcon.Image = null;
                }
            }
        }
    }
}
