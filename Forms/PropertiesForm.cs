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
    /// Constructor doc va hien day du cac truong: Ten, Duong dan, Loai, Kich
    /// thuoc, Ngay tao/sua/truy cap, va 4 checkbox thuoc tinh (Chi doc, An, He
    /// thong, Luu tru). Trong so do, chi Chi doc (ReadOnly) va An (Hidden) cho
    /// phep NGUOI DUNG bat/tat va ghi that xuong dia qua nut Ap dung/OK (goi
    /// File.GetAttributes/SetAttributes, chi doi dung 2 bit nay, giu nguyen cac
    /// bit con lai gom ca He thong/Luu tru) - He thong va Luu tru hien tai chi de
    /// XEM (chua co yeu cau cho sua). Con noi tren MainForm de mo form nay van
    /// chua duoc lam - se bo sung khi co yeu cau rieng.
    /// </remarks>
    public partial class PropertiesForm : Form
    {
        // Duong dan cua muc dang xem/sua thuoc tinh - luu lai de nut Ap dung/OK
        // biet ghi FileAttributes xuong dia nao. Null neu form duoc mo bang
        // constructor khong tham so (khong co muc cu the - khong cho phep Ap dung).
        private string _targetPath;

        // Gia tri IsDirectory cua muc dang xem - can de goi dung
        // File.SetAttributes hay chi doc/ghi qua FileInfo/DirectoryInfo tuong ung
        // (ca hai deu dung chung API File.GetAttributes/SetAttributes voi duong
        // dan nen khong that su can phan biet, nhung luu lai de ro rang y dinh).
        private bool _isDirectory;

        public PropertiesForm()
        {
            InitializeComponent();
            btnApply.Enabled = false;
            btnOK.Click += btnOK_Click;
            btnCancel.Click += btnCancel_Click;
            btnApply.Click += btnApply_Click;
            chkReadOnly.CheckedChanged += AttributeCheckBox_CheckedChanged;
            chkHidden.CheckedChanged += AttributeCheckBox_CheckedChanged;
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
            _targetPath = item.FullPath;
            _isDirectory = item.IsDirectory;

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
            chkSystem.Checked = item.IsSystem;
            chkArchive.Checked = item.IsArchiveFlag;

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

            // LoadItem gan Checked bang code (khong phai nguoi dung tick) nen se
            // tu kich hoat AttributeCheckBox_CheckedChanged va bat nham btnApply -
            // tat lai o day de Ap dung chi bat khi NGUOI DUNG thuc su doi thuoc tinh.
            btnApply.Enabled = false;
        }

        /// <summary>
        /// Nguoi dung tick/bo tick chkReadOnly hoac chkHidden - bat nut Ap dung de
        /// cho phep ghi thay doi xuong dia (chi bat, chua ghi ngay - giong hanh vi
        /// hop thoai Properties that cua Windows).
        /// </summary>
        private void AttributeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            btnApply.Enabled = true;
        }

        /// <summary>
        /// Ghi trang thai chkReadOnly/chkHidden hien tai xuong FileAttributes that
        /// su cua _targetPath tren dia. Chi doi 2 co ReadOnly/Hidden theo yeu cau -
        /// giu nguyen moi co khac (System, Archive, ReparsePoint...) dang co san
        /// bang cach doc lai Attributes hien tai roi chi Set/Clear đung 2 bit can.
        /// </summary>
        private void ApplyAttributeChanges()
        {
            if (string.IsNullOrEmpty(_targetPath))
                return;

            try
            {
                FileAttributes current = File.GetAttributes(_targetPath);

                current = SetFlag(current, FileAttributes.ReadOnly, chkReadOnly.Checked);
                current = SetFlag(current, FileAttributes.Hidden, chkHidden.Checked);

                File.SetAttributes(_targetPath, current);
                btnApply.Enabled = false;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                MessageBox.Show(
                    this,
                    $"Không thể áp dụng thay đổi thuộc tính:\n{ex.Message}",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>Bat hoac tat mot bit FileAttributes cu the, giu nguyen cac bit con lai.</summary>
        private static FileAttributes SetFlag(FileAttributes attributes, FileAttributes flag, bool enable)
        {
            return enable ? (attributes | flag) : (attributes & ~flag);
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyAttributeChanges();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (btnApply.Enabled)
            {
                ApplyAttributeChanges();
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
