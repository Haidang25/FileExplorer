using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Properties;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Cua so chinh cua ung dung. Chua MenuStrip (Tep/Chinh sua/Xem/Cong cu/Tro giup)
    /// va se la noi chua TreeView/ListView duyet thu muc trong cac buoc tiep theo.
    /// Cac handler menu hien tai chi la khung (TODO), can noi voi cac Services da co
    /// (FileService, FolderService, SearchService, RecycleBinService, LogService).
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly FolderService _folderService = new FolderService();
        private readonly FileService _fileService = new FileService();
        private readonly RecycleBinService _recycleBinService = new RecycleBinService();

        // TODO: thay bang duong dan dang duoc chon tren TreeView/ListView khi da co
        // giao dien dieu huong thuc te. Tam thoi mac dinh la Desktop de New Folder/New File
        // co noi de tao.
        private string _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // "Clipboard" noi bo cua ung dung cho Cut/Copy/Paste: danh sach duong dan
        // day du da chon, va co dang la Cut (true, se xoa nguon sau khi dan) hay
        // Copy (false, giu nguyen nguon) hay khong.
        private List<string> _clipboardPaths = new List<string>();
        private bool _clipboardIsCut;

        // Che do hien thi hien tai (tuong ung System.Windows.Forms.View de gan truc tiep
        // cho ListView.View khi da co ListView). Mac dinh la Details, giong Windows Explorer.
        private View _currentViewMode = View.Details;

        // True neu dang hien thi ca file/thu muc an (IsHidden). Mac dinh la false.
        private bool _showHiddenItems;

        // Lich su cac thu muc da tham (cho nut Back tren ToolStrip). Moi lan NavigateTo
        // duoc goi, duong dan hien tai (truoc khi doi) duoc day vao day.
        private readonly Stack<string> _backHistory = new Stack<string>();

        // Lich su "tien" tuong ung nut Forward: chi duoc day vao khi bam Back, va bi
        // xoa het moi khi nguoi dung dieu huong toi mot thu muc moi (khong phai qua
        // Back/Forward), giong hanh vi trinh duyet/Explorer thong thuong.
        private readonly Stack<string> _forwardHistory = new Stack<string>();

        // True trong luc dang chon node tren trvFolders bang code (VD: khi NavigateTo
        // duoc goi tu noi khac, khong phai tu chinh nguoi dung bam vao cay thu muc), de
        // tranh trvFolders_AfterSelect goi lai NavigateTo va gay vong lap/History sai.
        private bool _isSyncingTreeView;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "SFileManager";
            // Dung icon da gan cho file .exe (ApplicationIcon) lam icon cua form,
            // khong phu thuoc duong dan tuong doi luc chay.
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            ApplyTheme();
            LoadIconImages();
            LoadTreeViewFolders();
            LoadDisplaySettings();
            // mnuViewRefresh_Click dong bo txtPath VA nap noi dung lvwFiles cho
            // _currentPath mac dinh (Desktop), nen khong can gan txtPath.Text rieng nua.
            mnuViewRefresh_Click(this, EventArgs.Empty);
        }

        /// <summary>
        /// Nap _showHiddenItems va _currentViewMode tu Properties.Settings.Default
        /// (nguoi dung chon o SettingsForm) va dong bo lai trang thai Checked cua
        /// mnuViewShowHidden/menu che do xem tuong ung. Goi truoc lan
        /// mnuViewRefresh_Click dau tien de ListView hien dung tu dau, khong qua
        /// cac Click handler (tranh luu lai Settings ngay trong luc khoi tao).
        /// </summary>
        private void LoadDisplaySettings()
        {
            _showHiddenItems = Settings.Default.ShowHiddenFiles;
            mnuViewShowHidden.Checked = _showHiddenItems;

            View savedMode = (View)Settings.Default.DefaultViewMode;
            ToolStripMenuItem selectedItem;
            switch (savedMode)
            {
                case View.LargeIcon:
                    selectedItem = mnuViewModeLargeIcon;
                    break;
                case View.SmallIcon:
                    selectedItem = mnuViewModeSmallIcon;
                    break;
                case View.List:
                    selectedItem = mnuViewModeList;
                    break;
                default:
                    savedMode = View.Details;
                    selectedItem = mnuViewModeDetails;
                    break;
            }
            SetViewMode(savedMode, selectedItem);
        }

        /// <summary>
        /// Ap dung bang mau dung chung (AppTheme, xem Helpers/AppTheme.cs va
        /// Helpers/AppThemeRenderer.cs) cho toan bo control tinh cua MainForm.
        /// Chi dung BackColor/ForeColor/Renderer — cac thuoc tinh WinForms co san,
        /// dung theo dung nguyen tac trong "00_He_Thong_Mau_Sac.md" muc 3 (khong bo
        /// goc lon, khong do bong, khong ve lai control bang GraphicsPath).
        /// </summary>
        private void ApplyTheme()
        {
            this.BackColor = AppTheme.Background;
            this.ForeColor = AppTheme.TextPrimary;

            // MenuStrip/ToolStrip/StatusStrip/ContextMenuStrip: dung chung mot
            // renderer (AppThemeRenderer) de dam bao dong bo giua cac thanh nay.
            AppThemeRenderer renderer = new AppThemeRenderer();
            mnsMain.Renderer = renderer;
            mnsMain.BackColor = AppTheme.Surface;
            tlsMain.Renderer = renderer;
            tlsMain.BackColor = AppTheme.Surface;
            stsMain.Renderer = renderer;
            stsMain.BackColor = AppTheme.Surface;
            cmsListView.Renderer = renderer;
            cmsListView.BackColor = AppTheme.Surface;
            cmsListView.ForeColor = AppTheme.TextPrimary;

            // Thanh dia chi.
            pnlAddressBar.BackColor = AppTheme.Surface;
            txtPath.BackColor = AppTheme.Surface;
            txtPath.ForeColor = AppTheme.TextPrimary;
            txtPath.BorderStyle = BorderStyle.FixedSingle;
            btnUp.FlatStyle = FlatStyle.Flat;
            btnUp.FlatAppearance.BorderColor = AppTheme.Border;
            btnUp.BackColor = AppTheme.Surface;
            btnUp.ForeColor = AppTheme.TextPrimary;
            btnGo.FlatStyle = FlatStyle.Flat;
            btnGo.FlatAppearance.BorderColor = AppTheme.Accent;
            btnGo.BackColor = AppTheme.Accent;
            btnGo.ForeColor = System.Drawing.Color.White;

            txtSearch.BackColor = AppTheme.Surface;
            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            // Neu dang hien placeholder ("Tìm kiếm...") thi giu mau chu nhat (TextSecondary),
            // con lai dung mau chu chinh — tranh ApplyTheme() ghi de mau placeholder khi
            // duoc goi lai (VD: sau khi dong SettingsForm).
            txtSearch.ForeColor = txtSearch.Text == SearchPlaceholderText
                ? AppTheme.TextSecondary
                : AppTheme.TextPrimary;

            // Vung lam viec: SplitContainer (mau nen chinh la mau duong phan
            // cach giua 2 panel), TreeView, ListView.
            spcMain.BackColor = AppTheme.Border;
            trvFolders.BackColor = AppTheme.Surface;
            trvFolders.ForeColor = AppTheme.TextPrimary;
            trvFolders.BorderStyle = BorderStyle.FixedSingle;
            trvFolders.LineColor = AppTheme.Border;

            lvwFiles.BackColor = AppTheme.Surface;
            lvwFiles.ForeColor = AppTheme.TextPrimary;
            lvwFiles.BorderStyle = BorderStyle.FixedSingle;

            // Ghi chu: mau dong duoc chon (SelectedRow trong AppTheme) do he
            // dieu hanh tu ve theo mau he thong khi TreeView/ListView khong o
            // che do OwnerDraw. WinForms khong co thuoc tinh de doi rieng mau
            // nay ma khong phai tu ve lai toan bo dong — neu can dung dung mau
            // AppTheme.SelectedRow, buoc tiep theo se can bat OwnerDraw cho
            // lvwFiles (DrawItem/DrawSubItem), hien tai chua trien khai.
        }

        /// <summary>
        /// Ve va nap 2 icon placeholder ("folder"/"file") vao imlIcons, dung
        /// chung cho trvFolders va lvwFiles. Ve bang code thay vi nhung
        /// san file .ico/.png de khong phai quan ly them tai nguyen nhi phan.
        /// </summary>
        private void LoadIconImages()
        {
            imlIcons.Images.Add("folder", CreateFolderIcon());
            imlIcons.Images.Add("file", CreateFileIcon());

            // Icon rieng cho tung loai o dia (xem GetDriveImageKey), de TreeView phan
            // biet truc quan o cung (Fixed) voi o roi/USB/dia quang/o mang, giong
            // Windows Explorer.
            imlIcons.Images.Add("driveFixed", CreateDriveIcon(DriveIconStyle.Fixed));
            imlIcons.Images.Add("driveRemovable", CreateDriveIcon(DriveIconStyle.Removable));
            imlIcons.Images.Add("driveCDRom", CreateDriveIcon(DriveIconStyle.CDRom));
            imlIcons.Images.Add("driveNetwork", CreateDriveIcon(DriveIconStyle.Network));
            imlIcons.Images.Add("driveNotReady", CreateDriveIcon(DriveIconStyle.NotReady));
        }

        /// <summary>Kieu ve icon o dia, dung noi bo cho CreateDriveIcon.</summary>
        private enum DriveIconStyle
        {
            Fixed,
            Removable,
            CDRom,
            Network,
            NotReady
        }

        /// <summary>
        /// Chon ImageKey trong imlIcons phu hop voi mot o dia (FolderItemModel co
        /// IsDrive = true), dua tren IsReady va DriveType. O chua san sang luon
        /// dung chung mot icon xam ("driveNotReady") bat ke loai o thuc su la gi,
        /// de nhan biet ngay "khong dung duoc" ma khong can doc chu thich.
        /// </summary>
        private static string GetDriveImageKey(FolderItemModel drive)
        {
            if (!drive.IsReady)
                return "driveNotReady";

            switch (drive.DriveType)
            {
                case DriveType.Removable:
                    return "driveRemovable";
                case DriveType.CDRom:
                    return "driveCDRom";
                case DriveType.Network:
                    return "driveNetwork";
                case DriveType.Fixed:
                default:
                    return "driveFixed";
            }
        }

        /// <summary>
        /// Icon thu muc placeholder: hinh cai cap mau cam, cung tong mau voi
        /// Resources/app.ico da tao truoc do.
        /// </summary>
        private static Bitmap CreateFolderIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                using (var tabBrush = new SolidBrush(Color.FromArgb(255, 214, 148, 46)))
                using (var bodyBrush = new SolidBrush(Color.FromArgb(255, 230, 168, 62)))
                using (var borderPen = new Pen(Color.FromArgb(255, 160, 104, 28)))
                {
                    g.FillRectangle(tabBrush, 1, 3, 6, 2);
                    g.FillRectangle(bodyBrush, 1, 5, 14, 9);
                    g.DrawRectangle(borderPen, 1, 5, 13, 8);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Icon file placeholder: hinh to giay trang, goc tren-phai gap lai, vien xam.
        /// </summary>
        private static Bitmap CreateFileIcon()
        {
            var bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                Point[] outline =
                {
                    new Point(3, 1), new Point(10, 1), new Point(13, 4),
                    new Point(13, 15), new Point(3, 15)
                };

                using (var bodyBrush = new SolidBrush(Color.White))
                using (var borderPen = new Pen(Color.FromArgb(255, 120, 120, 120)))
                {
                    g.FillPolygon(bodyBrush, outline);
                    g.DrawPolygon(borderPen, outline);
                    g.DrawLine(borderPen, 10, 1, 10, 4);
                    g.DrawLine(borderPen, 10, 4, 13, 4);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Ve icon o dia 16x16 theo tung kieu (xem DriveIconStyle), dung placeholder
        /// hinh don gian (khong dung file .ico rieng) giong CreateFolderIcon/CreateFileIcon.
        /// Mau sac lay theo AppTheme.Accent (o cung/mac dinh) hoac mau trung tinh cho
        /// cac loai o dac biet, va mau xam nhat (AppTheme.TextSecondary) rieng cho o
        /// chua san sang de nguoi dung nhan ra ngay tren TreeView.
        /// </summary>
        private static Bitmap CreateDriveIcon(DriveIconStyle style)
        {
            var bitmap = new Bitmap(16, 16);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                switch (style)
                {
                    case DriveIconStyle.NotReady:
                        // O chua san sang: hinh o cung nhung to mau xam nhat, khong
                        // ve chi tiet ben trong, de "nhat" hon han cac icon con lai.
                        using (var bodyBrush = new SolidBrush(AppTheme.TextSecondary))
                        using (var borderPen = new Pen(Color.FromArgb(255, 120, 120, 120)))
                        {
                            g.FillRectangle(bodyBrush, 1, 5, 14, 8);
                            g.DrawRectangle(borderPen, 1, 5, 13, 7);
                        }
                        break;

                    case DriveIconStyle.Removable:
                        // O roi/USB: hinh chu nhat dung, phan dau nho nhu dau cam USB.
                        using (var bodyBrush = new SolidBrush(AppTheme.Accent))
                        using (var borderPen = new Pen(Color.FromArgb(255, 90, 70, 180)))
                        {
                            g.FillRectangle(bodyBrush, 4, 3, 8, 11);
                            g.DrawRectangle(borderPen, 4, 3, 7, 10);
                            g.FillRectangle(Brushes.White, 6, 6, 4, 3);
                        }
                        break;

                    case DriveIconStyle.CDRom:
                        // O dia quang: hinh tron co lo tron nho o giua (dia CD/DVD).
                        using (var bodyBrush = new SolidBrush(Color.FromArgb(255, 200, 200, 210)))
                        using (var borderPen = new Pen(Color.FromArgb(255, 120, 120, 130)))
                        {
                            g.FillEllipse(bodyBrush, 1, 1, 14, 14);
                            g.DrawEllipse(borderPen, 1, 1, 13, 13);
                            g.FillEllipse(new SolidBrush(AppTheme.Accent), 6, 6, 4, 4);
                        }
                        break;

                    case DriveIconStyle.Network:
                        // O mang: hinh o cung ben duoi + 2 "song" cong ben tren the
                        // hien ket noi mang, giong bieu tuong o mang trong Explorer.
                        using (var bodyBrush = new SolidBrush(Color.FromArgb(255, 214, 148, 46)))
                        using (var borderPen = new Pen(Color.FromArgb(255, 160, 104, 28)))
                        using (var wavePen = new Pen(AppTheme.Accent, 2))
                        {
                            g.FillRectangle(bodyBrush, 1, 8, 14, 6);
                            g.DrawRectangle(borderPen, 1, 8, 13, 5);
                            g.DrawArc(wavePen, 3, 1, 6, 6, 200, 140);
                            g.DrawArc(wavePen, 5, 3, 4, 4, 200, 140);
                        }
                        break;

                    case DriveIconStyle.Fixed:
                    default:
                        // O cung mac dinh: hinh o cung don gian, khe sang o giua.
                        using (var bodyBrush = new SolidBrush(Color.FromArgb(255, 108, 92, 231)))
                        using (var borderPen = new Pen(Color.FromArgb(255, 70, 58, 160)))
                        {
                            g.FillRectangle(bodyBrush, 1, 4, 14, 9);
                            g.DrawRectangle(borderPen, 1, 4, 13, 8);
                            g.DrawLine(Pens.White, 3, 8, 13, 8);
                        }
                        break;
                }
            }

            return bitmap;
        }

        #region Menu Tep (File)

        private void mnuFileNewFolder_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox(
                "Nhap ten thu muc moi:", "Tao thu muc moi", "New Folder");

            if (string.IsNullOrWhiteSpace(name))
                return; // Nguoi dung bam Cancel hoac de trong.

            OperationResult result = _folderService.CreateFolder(_currentPath, name);
            ShowOperationResultMessage(result, $"tao thu muc \"{name}\"");

            if (result == OperationResult.Success)
            {
                // TODO: goi lai ham lam moi ListView/TreeView khi da co (VD: LoadCurrentFolder()).
                mnuViewRefresh_Click(sender, e);
            }
        }

        private void mnuFileNewFile_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox(
                "Nhap ten file moi (bao gom phan mo rong, VD: moi.txt):", "Tao file moi", "New File.txt");

            if (string.IsNullOrWhiteSpace(name))
                return; // Nguoi dung bam Cancel hoac de trong.

            OperationResult result = _fileService.CreateFile(_currentPath, name);
            ShowOperationResultMessage(result, $"tao file \"{name}\"");

            if (result == OperationResult.Success)
            {
                // TODO: goi lai ham lam moi ListView/TreeView khi da co (VD: LoadCurrentFolder()).
                mnuViewRefresh_Click(sender, e);
            }
        }

        /// <summary>
        /// Hien thong bao phu hop voi ket qua tra ve tu Services, dung chung cho
        /// cac thao tac tao/doi ten/xoa/di chuyen/sao chep file va thu muc.
        /// </summary>
        /// <param name="result">Ket qua thao tac.</param>
        /// <param name="actionDescription">Mo ta ngan gon thao tac da thuc hien (VD: "tao thu muc \"abc\"").</param>
        private void ShowOperationResultMessage(OperationResult result, string actionDescription)
        {
            switch (result)
            {
                case OperationResult.Success:
                    MessageBox.Show($"Da {actionDescription} thanh cong.", "Thong bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case OperationResult.Skipped:
                    MessageBox.Show($"Khong the {actionDescription}: da co muc trung ten trong thu muc nay.",
                        "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case OperationResult.AccessDenied:
                    MessageBox.Show($"Khong the {actionDescription}: khong du quyen truy cap thu muc nay.",
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case OperationResult.NotFound:
                    MessageBox.Show($"Khong the {actionDescription}: khong tim thay thu muc dich.",
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    MessageBox.Show($"Khong the {actionDescription}: ten khong hop le hoac co loi xay ra.",
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Menu Chinh sua (Edit)

        /// <summary>
        /// Lay danh sach duong dan day du dang duoc chon tren giao dien.
        /// </summary>
        /// <remarks>
        /// Doc truc tiep tu lvwFiles.SelectedItems - moi ListViewItem duoc tao trong
        /// LoadListViewFiles() da gan Tag la duong dan day du (FullPath) tuong ung.
        /// </remarks>
        private List<string> GetSelectedPaths()
        {
            return lvwFiles.SelectedItems
                .Cast<ListViewItem>()
                .Select(item => item.Tag as string)
                .Where(path => !string.IsNullOrEmpty(path))
                .ToList();
        }

        private void mnuEditCut_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chua chon muc nao de cat.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _clipboardPaths = selected;
            _clipboardIsCut = true;
        }

        private void mnuEditCopy_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chua chon muc nao de sao chep.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _clipboardPaths = selected;
            _clipboardIsCut = false;
        }

        private void mnuEditPaste_Click(object sender, EventArgs e)
        {
            if (_clipboardPaths.Count == 0)
            {
                MessageBox.Show("Chua co gi trong clipboard de dan (hay Cut/Copy truoc).",
                    "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (string sourcePath in _clipboardPaths)
            {
                string name = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(_currentPath, name);
                bool isDirectory = Directory.Exists(sourcePath);

                OperationResult result;
                if (isDirectory)
                {
                    result = _clipboardIsCut
                        ? _folderService.MoveFolder(sourcePath, destinationPath)
                        : _folderService.CopyFolder(sourcePath, destinationPath);
                }
                else
                {
                    result = _clipboardIsCut
                        ? _fileService.MoveFile(sourcePath, destinationPath)
                        : _fileService.CopyFile(sourcePath, destinationPath);
                }

                ShowOperationResultMessage(result, $"dan \"{name}\"");
            }

            if (_clipboardIsCut)
            {
                // Sau khi Cut + Paste xong thi clipboard het gia tri (giong Windows Explorer).
                _clipboardPaths = new List<string>();
            }

            mnuViewRefresh_Click(sender, e);
        }

        private void mnuEditDelete_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chua chon muc nao de xoa.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Ban co chac muon chuyen {selected.Count} muc da chon vao Thung rac?",
                "Xac nhan xoa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            foreach (string path in selected)
            {
                OperationResult result = _recycleBinService.MoveToRecycleBin(path);
                ShowOperationResultMessage(result, $"xoa \"{Path.GetFileName(path)}\"");
            }

            // TODO: neu nguoi dung giu Shift khi bam Delete (hoac chon muc "Xoa vinh vien"),
            // goi FileService.DeleteFile/FolderService.DeleteFolder voi permanent = true thay
            // vi RecycleBinService.MoveToRecycleBin.

            mnuViewRefresh_Click(sender, e);
        }

        private void mnuEditRename_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count != 1)
            {
                MessageBox.Show("Vui long chon dung mot muc de doi ten.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = selected[0];
            string oldName = Path.GetFileName(path);
            string newName = Interaction.InputBox("Nhap ten moi:", "Doi ten", oldName);

            if (string.IsNullOrWhiteSpace(newName) || newName == oldName)
                return; // Nguoi dung bam Cancel, de trong, hoac khong doi gi.

            bool isDirectory = Directory.Exists(path);
            OperationResult result = isDirectory
                ? _folderService.RenameFolder(path, newName)
                : _fileService.RenameFile(path, newName);

            ShowOperationResultMessage(result, $"doi ten \"{oldName}\" thanh \"{newName}\"");

            if (result == OperationResult.Success)
            {
                mnuViewRefresh_Click(sender, e);
            }
        }

        private void mnuEditSelectAll_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvwFiles.Items)
            {
                item.Selected = true;
            }

            lvwFiles.Focus();
        }

        #endregion

        #region Menu Xem (View)

        private void mnuViewRefresh_Click(object sender, EventArgs e)
        {
            // Dong bo thanh dia chi voi duong dan hien tai - lam truoc tien de txtPath
            // luon dung ke ca khi phan duyet noi dung ben duoi gap loi.
            txtPath.Text = _currentPath;

            LoadListViewFiles();
        }

        /// <summary>
        /// Nap lai toan bo noi dung (thu muc con + file) cua _currentPath vao
        /// lvwFiles: thu muc liet ke truoc, sau do den file, giong Windows Explorer.
        /// </summary>
        private void LoadListViewFiles()
        {
            lvwFiles.BeginUpdate();
            lvwFiles.Items.Clear();

            int itemCount = 0;
            long totalSize = 0;

            try
            {
                foreach (string folderPath in Directory.GetDirectories(_currentPath))
                {
                    FolderItemModel folder = FolderItemModel.FromPath(folderPath);
                    if (folder == null || (folder.IsHidden && !_showHiddenItems))
                        continue;

                    var item = new ListViewItem(folder.Name, "folder") { Tag = folder.FullPath };
                    item.SubItems.Add(string.Empty); // Thu muc khong hien kich thuoc truc tiep.
                    item.SubItems.Add("Thư mục tệp");
                    item.SubItems.Add(folder.ModifiedDate.ToString("dd/MM/yyyy HH:mm"));
                    lvwFiles.Items.Add(item);
                    itemCount++;
                }

                foreach (string filePath in Directory.GetFiles(_currentPath))
                {
                    FileItemModel file = FileItemModel.FromPath(filePath);
                    if (file == null || (file.IsHidden && !_showHiddenItems))
                        continue;

                    var item = new ListViewItem(file.Name, "file") { Tag = file.FullPath };
                    item.SubItems.Add(file.SizeFormatted);
                    item.SubItems.Add(FileHelper.GetFileType(file.FullPath));
                    item.SubItems.Add(file.ModifiedDate.ToString("dd/MM/yyyy HH:mm"));
                    lvwFiles.Items.Add(item);
                    itemCount++;
                    totalSize += file.Size;
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Không đủ quyền truy cập thư mục này.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("Không thể đọc nội dung thư mục này.", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                lvwFiles.EndUpdate();
            }

            tsslItemCount.Text = $"{itemCount} mục";
            tsslTotalSize.Text = FormatHelper.FormatSize(totalSize);
            tsslStatus.Text = "Sẵn sàng";
        }

        /// <summary>
        /// Cap nhat nhan trang thai (tsslStatus) theo so muc/kich thuoc dang duoc chon
        /// tren lvwFiles, giong thanh trang thai cua Windows Explorer.
        /// </summary>
        private void lvwFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedCount = lvwFiles.SelectedItems.Count;
            if (selectedCount == 0)
            {
                tsslStatus.Text = "Sẵn sàng";
                return;
            }

            long selectedSize = 0;
            foreach (ListViewItem item in lvwFiles.SelectedItems)
            {
                string path = item.Tag as string;
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                {
                    try
                    {
                        selectedSize += new FileInfo(path).Length;
                    }
                    catch (IOException)
                    {
                        // Bo qua neu file vua bi xoa/khoa giua luc dang tinh tong.
                    }
                }
            }

            tsslStatus.Text = selectedSize > 0
                ? $"{selectedCount} mục được chọn ({FormatHelper.FormatSize(selectedSize)})"
                : $"{selectedCount} mục được chọn";
        }

        /// <summary>
        /// Nhap doi vao mot muc trong lvwFiles: mo thu muc (dieu huong den) neu
        /// la thu muc, hoac mo file bang ung dung mac dinh cua he thong neu la file.
        /// </summary>
        private void lvwFiles_DoubleClick(object sender, EventArgs e)
        {
            OpenSelectedItem();
        }

        /// <summary>
        /// Muc "Mở" tren cmsListView (menu chuot phai) - dung chung logic voi nhap doi.
        /// </summary>
        private void cmsOpen_Click(object sender, EventArgs e)
        {
            OpenSelectedItem();
        }

        /// <summary>
        /// Mo muc dang duoc chon (dung duy nhat 1 muc): dieu huong vao neu la thu muc,
        /// hoac mo bang ung dung mac dinh cua he thong neu la file.
        /// </summary>
        private void OpenSelectedItem()
        {
            if (lvwFiles.SelectedItems.Count != 1)
                return;

            string path = lvwFiles.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(path))
                return;

            if (Directory.Exists(path))
            {
                NavigateTo(path);
            }
            else if (File.Exists(path))
            {
                try
                {
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở file:\n{ex.Message}", "Lỗi",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Bat/tat cac muc tren cmsListView (menu chuot phai) truoc khi hien ra, tuy
        /// theo dang co muc nao duoc chon tren lvwFiles va clipboard noi bo co
        /// gi de dan hay khong - tranh nguoi dung bam vao muc khong the thuc hien duoc.
        /// </summary>
        private void cmsListView_Opening(object sender, CancelEventArgs e)
        {
            bool hasSelection = lvwFiles.SelectedItems.Count > 0;
            bool hasSingleSelection = lvwFiles.SelectedItems.Count == 1;

            cmsOpen.Enabled = hasSingleSelection;
            cmsCut.Enabled = hasSelection;
            cmsCopy.Enabled = hasSelection;
            cmsPaste.Enabled = _clipboardPaths.Count > 0;
            cmsDelete.Enabled = hasSelection;
            cmsRename.Enabled = hasSingleSelection;
        }

        private void mnuViewShowHidden_Click(object sender, EventArgs e)
        {
            _showHiddenItems = mnuViewShowHidden.Checked;

            Settings.Default.ShowHiddenFiles = _showHiddenItems;
            Settings.Default.Save();

            mnuViewRefresh_Click(sender, e);
        }

        /// <summary>
        /// Chon mot che do hien thi (Large Icon/Small Icon/List/Details), bo chon
        /// 3 che do con lai (hanh xu nhu radio button) va luu vao _currentViewMode.
        /// </summary>
        /// <param name="mode">Che do hien thi vua duoc chon.</param>
        /// <param name="selectedItem">Muc menu tuong ung voi mode (se duoc danh dau Checked).</param>
        private void SetViewMode(View mode, ToolStripMenuItem selectedItem)
        {
            foreach (ToolStripMenuItem item in mnuViewMode.DropDownItems.OfType<ToolStripMenuItem>())
            {
                item.Checked = item == selectedItem;
            }

            _currentViewMode = mode;
            lvwFiles.View = _currentViewMode;

            Settings.Default.DefaultViewMode = (int)_currentViewMode;
            Settings.Default.Save();
        }

        private void mnuViewModeLargeIcon_Click(object sender, EventArgs e)
        {
            SetViewMode(View.LargeIcon, mnuViewModeLargeIcon);
        }

        private void mnuViewModeSmallIcon_Click(object sender, EventArgs e)
        {
            SetViewMode(View.SmallIcon, mnuViewModeSmallIcon);
        }

        private void mnuViewModeList_Click(object sender, EventArgs e)
        {
            SetViewMode(View.List, mnuViewModeList);
        }

        private void mnuViewModeDetails_Click(object sender, EventArgs e)
        {
            SetViewMode(View.Details, mnuViewModeDetails);
        }

        #endregion

        #region Menu Cong cu (Tools)

        private void mnuToolsSearch_Click(object sender, EventArgs e)
        {
            // TODO: mo form/hop thoai tim kiem, dung SearchService de tim va hien ket qua.
        }

        private void mnuToolsFindDuplicates_Click(object sender, EventArgs e)
        {
            // TODO: mo form hien thi tien trinh + ket qua, goi
            // SearchService.FindDuplicateFiles(_currentPath) (nen chay tren luong rieng
            // hoac async vi co the mat thoi gian voi thu muc lon), sau do hien tung
            // nhom file trung lap de nguoi dung chon xoa bot ban trung.
        }

        private void mnuToolsRecycleBin_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem noi dung Thung rac, dung RecycleBinService.GetRecycleBinItems.
        }

        private void mnuToolsLogs_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem lich su thao tac, dung LogService.GetLogs.
        }

        private void mnuToolsSettings_Click(object sender, EventArgs e)
        {
            using (SettingsForm settingsForm = new SettingsForm())
            {
                if (settingsForm.ShowDialog(this) == DialogResult.OK)
                {
                    // SettingsForm da luu Settings.Default va cap nhat AppTheme.IsDarkMode
                    // trong bo nho — ap dung lai theme + trang thai hien thi cho MainForm
                    // ngay, khong can khoi dong lai ung dung.
                    ApplyTheme();
                    LoadDisplaySettings();
                    mnuViewRefresh_Click(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Menu Tro giup (Help)

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string productName = GetAssemblyAttribute<AssemblyProductAttribute>(assembly)?.Product
                ?? assembly.GetName().Name;
            string version = assembly.GetName().Version?.ToString() ?? "1.0.0.0";
            string copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(assembly)?.Copyright;
            string company = GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly)?.Company;
            string description = GetAssemblyAttribute<AssemblyDescriptionAttribute>(assembly)?.Description;

            var lines = new List<string>
            {
                productName,
                $"Phiên bản {version}"
            };

            if (!string.IsNullOrWhiteSpace(description))
                lines.Add(description);

            if (!string.IsNullOrWhiteSpace(company))
                lines.Add(company);

            if (!string.IsNullOrWhiteSpace(copyright))
                lines.Add(copyright);

            MessageBox.Show(
                string.Join(Environment.NewLine, lines),
                "Giới thiệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Doc mot custom attribute cap assembly (VD: AssemblyProductAttribute,
        /// AssemblyCopyrightAttribute...) de hien thi trong hop thoai About, tranh
        /// phai ghi cung (hardcode) thong tin phien ban/ten san pham trong code.
        /// </summary>
        private static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {
            return Attribute.GetCustomAttribute(assembly, typeof(T)) as T;
        }

        #endregion

        #region ToolStrip (Back/Up/Refresh/New Folder/Copy/Paste/Delete)

        /// <summary>
        /// Di chuyen den mot thu muc moi: day thu muc hien tai vao lich su Back,
        /// cap nhat _currentPath, roi lam moi noi dung hien thi.
        /// </summary>
        /// <param name="path">Duong dan thu muc can di chuyen den.</param>
        private void NavigateTo(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;

            _backHistory.Push(_currentPath);
            // Dieu huong toi mot thu muc moi (khong phai qua tsbForward_Click) se lam
            // "gay" nhanh history tien, giong hanh vi trinh duyet/Explorer thong thuong.
            _forwardHistory.Clear();
            _currentPath = path;

            // TODO: khi da co ListView duyet thu muc thuc te, ham nay se la
            // noi trung tam de cap nhat dia chi hien thi (VD: mot TextBox address bar).
            mnuViewRefresh_Click(this, EventArgs.Empty);

            // Dong bo lai lua chon tren trvFolders (VD: khi NavigateTo duoc goi tu
            // Back/Up/txtPath thay vi tu chinh nguoi dung bam vao cay thu muc).
            SelectTreeViewNodeForPath(path);
        }

        private void tsbBack_Click(object sender, EventArgs e)
        {
            if (_backHistory.Count == 0)
            {
                MessageBox.Show("Không có gì để quay lại.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _forwardHistory.Push(_currentPath);
            _currentPath = _backHistory.Pop();
            mnuViewRefresh_Click(sender, e);
            SelectTreeViewNodeForPath(_currentPath);
        }

        private void tsbForward_Click(object sender, EventArgs e)
        {
            if (_forwardHistory.Count == 0)
            {
                MessageBox.Show("Không có gì để đi tới.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _backHistory.Push(_currentPath);
            _currentPath = _forwardHistory.Pop();
            mnuViewRefresh_Click(sender, e);
            SelectTreeViewNodeForPath(_currentPath);
        }

        private void tsbUp_Click(object sender, EventArgs e)
        {
            DirectoryInfo parent = Directory.GetParent(_currentPath);
            if (parent == null)
            {
                MessageBox.Show("Đã ở thư mục gốc (ổ đĩa), không thể lên cao hơn.",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            NavigateTo(parent.FullName);
        }

        // Cac nut con lai chi la loi tat cua menu tuong ung, dung lai dung 1 noi
        // xu ly logic (menu MenuStrip) de tranh trung lap code.

        private void tsbRefresh_Click(object sender, EventArgs e) => mnuViewRefresh_Click(sender, e);

        private void tsbNewFolder_Click(object sender, EventArgs e) => mnuFileNewFolder_Click(sender, e);

        private void tsbCopy_Click(object sender, EventArgs e) => mnuEditCopy_Click(sender, e);

        private void tsbPaste_Click(object sender, EventArgs e) => mnuEditPaste_Click(sender, e);

        private void tsbDelete_Click(object sender, EventArgs e) => mnuEditDelete_Click(sender, e);

        #endregion

        #region Thanh dia chi (txtPath, Go, Up)

        private void btnUp_Click(object sender, EventArgs e) => tsbUp_Click(sender, e);

        private void btnGo_Click(object sender, EventArgs e)
        {
            string path = txtPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(path))
                return;

            if (!Directory.Exists(path))
            {
                MessageBox.Show($"Không tìm thấy thư mục:\n{path}", "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPath.Text = _currentPath; // Khoi phuc lai duong dan cu tren thanh dia chi.
                return;
            }

            NavigateTo(path);
        }

        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Tranh tieng "beep" va xuong dong trong TextBox.
                btnGo_Click(sender, e);
            }
        }

        #endregion

        #region O tim kiem nhanh (txtSearch)

        private const string SearchPlaceholderText = "Tìm kiếm...";

        /// <summary>Xoa chu placeholder khi nguoi dung bam vao o tim kiem.</summary>
        private void txtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == SearchPlaceholderText)
            {
                txtSearch.Text = string.Empty;
                txtSearch.ForeColor = AppTheme.TextPrimary;
            }
        }

        /// <summary>Khoi phuc chu placeholder neu nguoi dung roi o tim kiem ma khong nhap gi.</summary>
        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = SearchPlaceholderText;
                txtSearch.ForeColor = AppTheme.TextSecondary;
            }
        }

        /// <summary>Nhan Enter trong o tim kiem se mo man hinh Tim kiem (SearchForm, TODO).</summary>
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                mnuToolsSearch_Click(sender, e);
            }
        }

        #endregion

        #region TreeView thu muc (trvFolders, panel trai cua spcMain)

        // Nhan gia tri "..." dung lam node "gia" (dummy) de bao TreeView node do co the
        // mo rong duoc, ma chua can doc thuc te noi dung thu muc con ngay tu dau (lazy
        // loading) - tranh doc toan bo cay thu muc cung mot luc, rat nang voi o dia lon.
        private const string LazyLoadPlaceholder = "...";

        /// <summary>
        /// Nap danh sach o dia lam node goc cua trvFolders, dung
        /// FolderService.GetDrives() (bao gom ca o dia chua san sang - VD: o CD
        /// rong, o mang mat ket noi - de giong Windows Explorer, thay vi an di).
        ///
        /// Voi o dia da san sang, Tag duoc gan la duong dan goc (FullPath) va them
        /// mot node "gia" ben trong de mui ten mo rong xuat hien, noi dung thuc su
        /// chi duoc doc khi nguoi dung bam mo rong (xem trvFolders_BeforeExpand).
        /// Voi o dia chua san sang, Tag duoc gan truc tiep la FolderItemModel (thay
        /// vi string duong dan) de trvFolders_BeforeExpand/AfterSelect nhan biet va
        /// tu choi mo rong/dieu huong mot cach than thien, khong nem IOException.
        /// </summary>
        private void LoadTreeViewFolders()
        {
            trvFolders.Nodes.Clear();

            foreach (FolderItemModel drive in _folderService.GetDrives())
            {
                var driveNode = new TreeNode(drive.Name);
                string driveImageKey = GetDriveImageKey(drive);
                driveNode.ImageKey = driveImageKey;
                driveNode.SelectedImageKey = driveImageKey;

                if (drive.IsReady)
                {
                    driveNode.Tag = drive.FullPath;
                    driveNode.Nodes.Add(new TreeNode(LazyLoadPlaceholder));
                }
                else
                {
                    // Khong gan duong dan string vao Tag de tsbBack/AfterSelect/
                    // BeforeExpand khong the vo tinh coi day la mot thu muc dieu
                    // huong duoc - gan ca FolderItemModel lam dau hieu "o chua san sang".
                    driveNode.Tag = drive;
                    driveNode.ForeColor = AppTheme.TextSecondary;
                }

                trvFolders.Nodes.Add(driveNode);
            }
        }

        /// <summary>
        /// Doc danh sach thu muc con thuc su cua mot node (thay the node "gia"), duoc
        /// goi khi nguoi dung sap mo rong node do lan dau tien.
        /// </summary>
        private void trvFolders_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;

            if (node.Tag is FolderItemModel notReadyDrive && !notReadyDrive.IsReady)
            {
                // O dia chua san sang (xem LoadTreeViewFolders) - khong co gi de mo
                // rong, huy luon thao tac thay vi de TreeView co gang doc thu muc con
                // va nem IOException.
                e.Cancel = true;
                return;
            }

            bool isLazyPlaceholder = node.Nodes.Count == 1
                && node.Nodes[0].Text == LazyLoadPlaceholder
                && node.Nodes[0].Tag == null;

            if (!isLazyPlaceholder)
                return; // Da nap that roi (hoac khong co thu muc con), khong can lam lai.

            node.Nodes.Clear();

            string path = node.Tag as string;
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                foreach (string subFolderPath in Directory.GetDirectories(path))
                {
                    var subFolderInfo = new DirectoryInfo(subFolderPath);

                    bool isHidden = (subFolderInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;
                    if (isHidden && !_showHiddenItems)
                        continue;

                    var childNode = new TreeNode(subFolderInfo.Name) { Tag = subFolderInfo.FullName };
                    childNode.ImageKey = "folder";
                    childNode.SelectedImageKey = "folder";
                    childNode.Nodes.Add(new TreeNode(LazyLoadPlaceholder));
                    node.Nodes.Add(childNode);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Khong du quyen doc thu muc nay (VD: System Volume Information) - bo qua,
                // de node hien thi khong co thu muc con thay vi bao loi lam gian doan nguoi dung.
            }
            catch (IOException)
            {
                // O dia thao ra, duong dan mang bi ngat... - bo qua tuong tu.
            }
        }

        /// <summary>
        /// Khi nguoi dung chon mot node tren cay thu muc, dieu huong ung dung den
        /// thu muc tuong ung (dung chung NavigateTo voi txtPath/Back/Up).
        /// </summary>
        private void trvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (_isSyncingTreeView)
                return; // Dang tu dong chon lai node do NavigateTo goi, khong can dieu huong lai.

            if (e.Node.Tag is FolderItemModel notReadyDrive && !notReadyDrive.IsReady)
            {
                // O dia chua san sang (xem LoadTreeViewFolders) - bao cho nguoi dung
                // biet thay vi im lang khong lam gi, hoac nem loi khi co Directory.Exists.
                MessageBox.Show($"{notReadyDrive.Name}\nỔ đĩa hiện chưa sẵn sàng (chưa có đĩa, hoặc mất kết nối).",
                    "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = e.Node.Tag as string;
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                NavigateTo(path);
            }
        }

        /// <summary>
        /// Tim va chon node tren trvFolders tuong ung voi mot duong dan cho truoc
        /// (neu node do da duoc nap), de cay thu muc luon dong bo voi _currentPath khi
        /// dieu huong tu noi khac (txtPath, Back, Up) thay vi tu chinh cay thu muc.
        /// </summary>
        /// <param name="path">Duong dan can dong bo len trvFolders.</param>
        private void SelectTreeViewNodeForPath(string path)
        {
            // TODO: hien tai chi la khung don gian, chua tu mo rong (expand) cac node
            // cha con thieu de lam lo node dich khi duong dan nam sau trong cay. Se
            // hoan thien khi can dong bo chinh xac hai chieu giua txtPath/ListView/TreeView.
        }

        #endregion
    }
}
