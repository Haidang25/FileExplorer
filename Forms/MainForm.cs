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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Properties;
using FileExplorerApp.Services;
// Alias de dung ngan gon FileIconCategory thay vi FileHelper.FileIconCategory
// moi lan tham chieu (enum nam long ben trong static class FileHelper).
using FileIconCategory = FileExplorerApp.Helpers.FileHelper.FileIconCategory;

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
        private readonly LogService _logService = new LogService();

        /// <summary>
        /// Theo doi thay doi (tao/xoa/doi ten/sua) trong _currentPath tu BEN NGOAI
        /// ung dung (Explorer, chuong trinh khac...) de tu dong lam moi lvwFiles -
        /// xem RestartFolderMonitoring/Watcher_*Changed. Chi thuc su bat theo doi
        /// khi Settings.Default.AutoRefreshEnabled = true (xem RestartFolderMonitoring).
        /// </summary>
        private readonly FileMonitorService _fileMonitorService = new FileMonitorService();

        /// <summary>
        /// Gop nhieu su kien FileSystemWatcher lien tiep trong thoi gian ngan (VD:
        /// sao chep hang tram file lien tuc vao thu muc dang mo) thanh MOT lan
        /// LoadListViewFiles() duy nhat, thay vi nap lai ListView moi khi co 1 su
        /// kien - tranh giat/lag ListView va giam tai I/O khong can thiet. Khoang
        /// cho (Interval) lay tu Settings.Default.WatcherDelayMs (nguoi dung tuy
        /// chinh duoc o SettingsForm). Timer nay chay tren luong UI (System.Windows.
        /// Forms.Timer), nen Tick co the goi thang LoadListViewFiles() ma khong can
        /// Invoke them.
        /// </summary>
        private readonly System.Windows.Forms.Timer _watcherDebounceTimer = new System.Windows.Forms.Timer();

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

        /// <summary>
        /// Tap phan mo rong (khong phan biet hoa/thuong) cho tung nhom loc cua
        /// cboFileTypeFilter - dung rieng bang nay thay vi tai su dung
        /// FileHelper.FileIconCategory, vi FileIconCategory.Media gom CA am thanh
        /// (.mp3, .wav, .flac) VA video chung 1 nhom, trong khi cboFileTypeFilter
        /// can rieng nhom "Video" (khong gom am thanh). Nhom "Tất cả" khong can co
        /// trong bang nay (xu ly rieng trong MatchesFileTypeFilter).
        /// </summary>
        private static readonly Dictionary<string, HashSet<string>> FileTypeFilterExtensions =
            new Dictionary<string, HashSet<string>>
            {
                ["Ảnh"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".ico", ".webp" },
                ["Văn bản"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".txt", ".doc", ".docx", ".pdf", ".rtf", ".odt" },
                ["Video"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v" },
                ["Nén"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    { ".zip", ".rar", ".7z", ".tar", ".gz" },
            };

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

        // Bo sap xep dung chung cho lvwFiles - gan cho lvwFiles.ListViewItemSorter
        // ngay trong constructor (xem ben duoi), va duoc dieu khien qua
        // lvwFiles_ColumnClick (click vao header cot). Xem chi tiet cach so sanh
        // trong Helpers/ListViewItemComparer.cs.
        private readonly ListViewItemComparer _listViewSorter = new ListViewItemComparer();

        public MainForm()
        {
            InitializeComponent();
            lvwFiles.ListViewItemSorter = _listViewSorter;
            // Chon san "Tất cả" (khong loc gi) - Items.AddRange trong Designer khong
            // tu chon muc nao, ComboBoxStyle.DropDownList (khong cho go tay) se hien
            // rong neu khong dat SelectedIndex ngay tu dau.
            cboFileTypeFilter.SelectedIndex = 0;
            this.Text = "SFileManager";
            // Dung icon da gan cho file .exe (ApplicationIcon) lam icon cua form,
            // khong phu thuoc duong dan tuong doi luc chay.
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            ApplyTheme();
            LoadIconImages();
            LoadTreeViewFolders();
            LoadDisplaySettings();
            InitializeFolderMonitoring();
            // mnuViewRefresh_Click dong bo txtPath VA nap noi dung lvwFiles cho
            // _currentPath mac dinh (Desktop), nen khong can gan txtPath.Text rieng nua.
            // Cung la noi RestartFolderMonitoring lan dau duoc goi (ben trong
            // mnuViewRefresh_Click) de bat theo doi _currentPath mac dinh ngay tu dau.
            mnuViewRefresh_Click(this, EventArgs.Empty);

            // Dam bao FileMonitorService/_watcherDebounceTimer duoc giai phong
            // dung luc dong cua so - khong sua Dispose(bool) trong
            // MainForm.Designer.cs (file do Designer tu sinh lai, sua tay o do
            // de bi mat khi mo lai bang Visual Studio Designer).
            this.FormClosed += MainForm_FormClosed;
        }

        /// <summary>
        /// Cau hinh timer debounce (Interval tu Settings.Default.WatcherDelayMs)
        /// va dang ky 4 su kien du lieu cua FileMonitorService (Created/Deleted/
        /// Changed/Renamed) - tat ca deu chi lam MOT viec: (re)start
        /// _watcherDebounceTimer, KHONG goi thang LoadListViewFiles() ngay lap
        /// tuc (xem _watcherDebounceTimer). MonitorError duoc bo qua co y (khong
        /// hien MessageBox) vi day la loi nen tang (VD: o dia rut giua chung) ma
        /// nguoi dung khong the xu ly gi tu hop thoai - qua nhieu MessageBox bat
        /// ngo khi dang thao tac se gay kho chiu hon la huu ich.
        /// </summary>
        private void InitializeFolderMonitoring()
        {
            _watcherDebounceTimer.Interval = Math.Max(Settings.Default.WatcherDelayMs, 50);
            _watcherDebounceTimer.Tick += WatcherDebounceTimer_Tick;

            _fileMonitorService.FileCreated += (sender, e) => OnExternalChangeDetected();
            _fileMonitorService.FileDeleted += (sender, e) => OnExternalChangeDetected();
            _fileMonitorService.FileChanged += (sender, e) => OnExternalChangeDetected();
            _fileMonitorService.FileRenamed += (sender, e) => OnExternalChangeDetected();
            _fileMonitorService.MonitorError += FileMonitorService_MonitorError;
        }

        /// <summary>
        /// Xu ly su kien MonitorError cua FileMonitorService - CHI phan ung dac
        /// biet voi InternalBufferOverflowException (tran bo dem noi bo cua
        /// FileSystemWatcher), cac loai loi khac (VD: o dia bi rut, mat quyen
        /// truy cap thu muc dang theo doi) van duoc BO QUA co y giong truoc day
        /// (khong hien MessageBox gay kho chiu, va ung dung khong the tu sua
        /// nhung loi nen tang do tu hop thoai).
        /// </summary>
        /// <remarks>
        /// TRAN BO DEM la truong hop RIENG BIET, quan trong hon han cac loi
        /// khac: no xay ra khi qua NHIEU thay doi xay ra don don trong thoi
        /// gian ngan (VD: giai nen/sao chep hang chuc nghin file cung luc vao
        /// thu muc dang theo doi) vuot qua kich thuoc bo dem noi bo cua
        /// FileSystemWatcher - luc nay FileSystemWatcher SE MAT (khong raise)
        /// mot so su kien Created/Deleted/Changed/Renamed xay ra trong luc
        /// tran, ma KHONG CO CACH NAO biet chinh xac da mat su kien nao. Vi
        /// vay, KHONG THE tin tuong debounce+LoadListViewFiles thong thuong
        /// (dua vao cac su kien RIENG LE) nua trong tinh huong nay - phai NAP
        /// LAI TOAN BO danh sach ngay (khong qua _watcherDebounceTimer, vi ban
        /// chat day KHONG PHAI "nhieu su kien lien tiep can gop lai" ma la
        /// "khong con biet chinh xac trang thai thuc te, phai doc lai tu dau
        /// de dam bao dung") de dam bao lvwFiles khop lai voi thuc te tren dia,
        /// du co the mat mot vai giay hien thi sai truoc do.
        /// </remarks>
        private void FileMonitorService_MonitorError(object sender, ErrorEventArgs e)
        {
            if (!(e.GetException() is InternalBufferOverflowException))
                return;

            if (!IsHandleCreated || IsDisposed)
                return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    // Dung timer debounce dang cho (neu co) - khong can gop chung
                    // voi bat ky su kien le te nao khac, nap lai toan bo NGAY.
                    _watcherDebounceTimer.Stop();
                    LoadListViewFiles();
                }));
            }
            catch (InvalidOperationException)
            {
                // Form dang trong qua trinh dong - xem ly do tuong tu tai
                // OnExternalChangeDetected.
            }
        }

        /// <summary>
        /// Handler CHUNG cho ca 4 su kien Created/Deleted/Changed/Renamed - noi
        /// dung xu ly giong het nhau (lam moi toan bo ListView), nen khong can
        /// phan biet TUNG loai thay doi cu the o day (khac voi LogService, noi
        /// tung loai thao tac can ghi log rieng - day chi la "co gi do thay doi,
        /// nap lai cho chac"). Duoc goi tu luong THREADPOOL cua FileSystemWatcher
        /// (xem remarks tren cac su kien trong FileMonitorService), nen BAT BUOC
        /// phai qua BeginInvoke truoc khi dung _watcherDebounceTimer (mot Control/
        /// Component gan voi luong UI) - goi thang se nem
        /// InvalidOperationException ("Cross-thread operation not valid").
        /// </summary>
        private void OnExternalChangeDetected()
        {
            if (!IsHandleCreated || IsDisposed)
                return;

            try
            {
                BeginInvoke(new Action(() =>
                {
                    // Khoi dong lai (restart) timer moi lan co su kien moi trong
                    // luc timer dang dem - Stop() roi Start() lai tu dau bao dam
                    // LoadListViewFiles() chi chay SAU KHI da yen (khong co su
                    // kien moi nao) trong dung khoang WatcherDelayMs, thay vi cu
                    // moi WatcherDelayMs lai chay mot lan du van con thay doi
                    // dang dien ra (VD: dang giua qua trinh sao chep hang loat).
                    _watcherDebounceTimer.Stop();
                    _watcherDebounceTimer.Start();
                }));
            }
            catch (InvalidOperationException)
            {
                // Form dang trong qua trinh dong (handle vua bi huy giua luc
                // BeginInvoke duoc goi) - bo qua, khong con y nghia lam moi
                // ListView cua mot Form sap dong.
            }
        }

        private void WatcherDebounceTimer_Tick(object sender, EventArgs e)
        {
            _watcherDebounceTimer.Stop();

            // Chi nap lai NOI DUNG (khong goi lai toan bo mnuViewRefresh_Click)
            // vi khong can dong bo lai txtPath (duong dan khong doi, chi noi
            // dung ben trong thay doi) va - quan trong hon - KHONG duoc goi lai
            // RestartFolderMonitoring() tu day (se lam trong "Dispose roi tao
            // lai watcher" moi lan co thay doi, lang phi khong can thiet vi
            // _currentPath khong doi trong tinh huong nay).
            LoadListViewFiles();
        }

        /// <summary>
        /// (Bat/tat va) tro FileMonitorService sang theo doi _currentPath hien
        /// tai - goi moi khi _currentPath thay doi (dau mnuViewRefresh_Click, noi
        /// DUY NHAT dieu huong nao cung di qua) VA sau khi nguoi dung doi
        /// Settings.Default.AutoRefreshEnabled trong SettingsForm.
        /// </summary>
        private void RestartFolderMonitoring()
        {
            _watcherDebounceTimer.Stop();
            _fileMonitorService.StopMonitoring();

            if (!Settings.Default.AutoRefreshEnabled)
                return;

            try
            {
                _fileMonitorService.StartMonitoring(_currentPath, includeSubdirectories: false);
            }
            catch (ArgumentException)
            {
                // _currentPath khong ton tai/khong hop le (hiem - VD thu muc vua
                // bi xoa boi chuong trinh khac dung luc dieu huong toi) - bo qua,
                // LoadListViewFiles() (goi ngay sau do trong mnuViewRefresh_Click)
                // se tu bao loi phu hop cho nguoi dung, khong can bao trung o day.
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _watcherDebounceTimer.Stop();
            _watcherDebounceTimer.Dispose();
            _fileMonitorService.Dispose();
            pbxPreview.Image?.Dispose();
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

            txtQuickFilter.BackColor = AppTheme.Surface;
            txtQuickFilter.BorderStyle = BorderStyle.FixedSingle;
            // Tuong tu txtSearch ngay tren - giu mau chu nhat khi dang hien placeholder.
            txtQuickFilter.ForeColor = txtQuickFilter.Text == QuickFilterPlaceholderText
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

            // Nhan bao thu muc trong (xem UpdateEmptyFolderMessage) - nen trong suot
            // len tren lvwFiles nen chi can chinh mau chu, dung mau nhat (TextSecondary)
            // giong placeholder cua txtSearch de khong lam nguoi dung tuong day la du
            // lieu that.
            lblEmptyFolder.ForeColor = AppTheme.TextSecondary;
            lblEmptyFolder.BackColor = AppTheme.Surface;

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

            // Icon rieng cho tung nhom file tren lvwFiles, dua tren
            // FileHelper.GetFileIconCategory() (VD: anh, tai lieu, bang tinh...) -
            // nhom nao khong khop se dung lai "file" (icon to giay trang trung tinh
            // co san) thay vi ve them mot ImageCategory.Generic rieng khong can thiet.
            imlIcons.Images.Add("fileImage", CreateFileTypeIcon(FileIconCategory.Image));
            imlIcons.Images.Add("fileDocument", CreateFileTypeIcon(FileIconCategory.Document));
            imlIcons.Images.Add("fileSpreadsheet", CreateFileTypeIcon(FileIconCategory.Spreadsheet));
            imlIcons.Images.Add("fileArchive", CreateFileTypeIcon(FileIconCategory.Archive));
            imlIcons.Images.Add("fileMedia", CreateFileTypeIcon(FileIconCategory.Media));
            imlIcons.Images.Add("fileCode", CreateFileTypeIcon(FileIconCategory.Code));
        }

        /// <summary>
        /// Chon ImageKey trong imlIcons phu hop voi mot file, dua tren
        /// FileHelper.GetFileIconCategory() (xac dinh theo phan mo rong). Nhom
        /// Generic (khong khop nhom rieng nao) dung lai icon "file" trung tinh mac
        /// dinh da co san, tranh ve them mot icon giong het no.
        /// </summary>
        private static string GetFileImageKey(string path)
        {
            switch (FileHelper.GetFileIconCategory(path))
            {
                case FileIconCategory.Image:
                    return "fileImage";
                case FileIconCategory.Document:
                    return "fileDocument";
                case FileIconCategory.Spreadsheet:
                    return "fileSpreadsheet";
                case FileIconCategory.Archive:
                    return "fileArchive";
                case FileIconCategory.Media:
                    return "fileMedia";
                case FileIconCategory.Code:
                    return "fileCode";
                case FileIconCategory.Generic:
                default:
                    return "file";
            }
        }

        /// <summary>
        /// Kiem tra mot file co khop voi nhom dang duoc chon tren cboFileTypeFilter
        /// hay khong, dua theo phan mo rong (xem FileTypeFilterExtensions). Nhom
        /// "Tất cả" (hoac khi cboFileTypeFilter chua chon gi/dang null - VD: truoc
        /// khi InitializeComponent set SelectedIndex) luon khop moi file. CHI ap
        /// dung cho file - LoadListViewFiles() da tu bo qua ham nay cho thu muc.
        /// </summary>
        /// <param name="filePath">Duong dan file can kiem tra.</param>
        private bool MatchesFileTypeFilter(string filePath)
        {
            string selected = cboFileTypeFilter.SelectedItem as string;

            if (string.IsNullOrEmpty(selected) || selected == "Tất cả")
                return true;

            if (!FileTypeFilterExtensions.TryGetValue(selected, out HashSet<string> extensions))
                return true; // Nhom la, chua khai bao trong bang - an toan la khong loc gi ca.

            return extensions.Contains(Path.GetExtension(filePath));
        }

        /// <summary>
        /// Doi nhom loc tren cboFileTypeFilter: nap lai lvwFiles ngay theo nhom moi.
        /// </summary>
        private void cboFileTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadListViewFiles();
        }

        /// <summary>
        /// Kiem tra ten mot file/thu muc co khop voi noi dung dang go trong
        /// txtQuickFilter hay khong - kieu "chua" (IndexOf), khong phan biet
        /// hoa/thuong, giong cach o tim kiem cua Windows Explorer loc ngay trong
        /// thu muc hien tai. Rong hoac dang hien chu placeholder (xem
        /// QuickFilterPlaceholderText) thi khop moi ten (khong loc gi).
        /// </summary>
        /// <param name="name">Ten (khong bao gom duong dan) can kiem tra.</param>
        private bool MatchesQuickFilter(string name)
        {
            string filterText = txtQuickFilter.Text;

            if (string.IsNullOrEmpty(filterText) || filterText == QuickFilterPlaceholderText)
                return true;

            return name.IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0;
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
        /// Ve icon file 16x16 rieng cho mot nhom (xem FileHelper.FileIconCategory):
        /// dung lai hinh to giay trang gap goc giong CreateFileIcon() lam nen, roi
        /// them mot dau hieu nho, mau sac dac trung ben trong de phan biet nhanh
        /// giua cac nhom ma khong can doc cot "Loai" (VD: dai mau cho Anh, luoi cho
        /// Bang tinh, khoa keo cho Nen...). Khong ve cho Generic vi nhom do dung lai
        /// icon "file" trung tinh co san (xem GetFileImageKey).
        /// </summary>
        private static Bitmap CreateFileTypeIcon(FileIconCategory category)
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

                switch (category)
                {
                    case FileIconCategory.Image:
                        // Dai nui + mat troi nho, giong bieu tuong anh don gian.
                        using (var mountainBrush = new SolidBrush(Color.FromArgb(255, 76, 175, 80)))
                        using (var sunBrush = new SolidBrush(Color.FromArgb(255, 255, 193, 7)))
                        {
                            g.FillEllipse(sunBrush, 5, 6, 3, 3);
                            Point[] mountain = { new Point(4, 13), new Point(7, 9), new Point(9, 11), new Point(11, 8), new Point(12, 13) };
                            g.FillPolygon(mountainBrush, mountain);
                        }
                        break;

                    case FileIconCategory.Document:
                        // Cac dong ke ngang tuong trung cho van ban.
                        using (var linePen = new Pen(Color.FromArgb(255, 100, 130, 200), 1.2f))
                        {
                            g.DrawLine(linePen, 5, 6, 11, 6);
                            g.DrawLine(linePen, 5, 9, 11, 9);
                            g.DrawLine(linePen, 5, 12, 9, 12);
                        }
                        break;

                    case FileIconCategory.Spreadsheet:
                        // Luoi 2x2 tuong trung cho bang tinh.
                        using (var gridPen = new Pen(Color.FromArgb(255, 33, 150, 83), 1f))
                        {
                            g.DrawRectangle(gridPen, 4, 5, 8, 8);
                            g.DrawLine(gridPen, 8, 5, 8, 13);
                            g.DrawLine(gridPen, 4, 9, 12, 9);
                        }
                        break;

                    case FileIconCategory.Archive:
                        // Khoa keo doc giua than file, tuong trung file nen.
                        using (var zipPen = new Pen(Color.FromArgb(255, 158, 118, 40), 1.4f))
                        {
                            g.DrawLine(zipPen, 8, 5, 8, 13);
                            g.DrawRectangle(new Pen(Color.FromArgb(255, 158, 118, 40)), 7, 7, 2, 2);
                        }
                        break;

                    case FileIconCategory.Media:
                        // Not nhac don gian, tuong trung am thanh/video.
                        using (var noteBrush = new SolidBrush(Color.FromArgb(255, 156, 39, 176)))
                        using (var notePen = new Pen(Color.FromArgb(255, 156, 39, 176), 1.4f))
                        {
                            g.DrawLine(notePen, 9, 5, 9, 11);
                            g.FillEllipse(noteBrush, 6, 10, 3, 3);
                        }
                        break;

                    case FileIconCategory.Code:
                        // Dau ngoac nhon "< >" tuong trung ma nguon.
                        using (var codeFont = new Font("Consolas", 6.5f, System.Drawing.FontStyle.Bold))
                        using (var codeBrush = new SolidBrush(Color.FromArgb(255, 66, 133, 244)))
                        {
                            g.DrawString("<>", codeFont, codeBrush, 4.5f, 6.5f);
                        }
                        break;

                    case FileIconCategory.Generic:
                    default:
                        // Khong ve them gi - giong het CreateFileIcon() (khong nen goi
                        // ham nay voi Generic, xem GetFileImageKey).
                        break;
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

            string newFolderPath = Path.Combine(_currentPath, name);
            OperationResult result = _folderService.CreateFolder(_currentPath, name);
            ShowOperationResultMessage(result, $"tao thu muc \"{name}\"");

            // Ghi log CA KHI thanh cong lan that bai (VD: trung ten, khong du
            // quyen), kem theo THONG DIEP LOI cu the (giong het MessageBox se
            // hien - xem LogOperationResult/BuildOperationResultMessage) khi that
            // bai, giup tra soat sau nay (VD: "tai sao thu muc X khong duoc tao
            // luc do") biet ngay NGUYEN NHAN thay vi chi thay ten enum tro tui.
            LogOperationResult(FileOperationType.CreateFolder, newFolderPath, null, result, $"tạo thư mục \"{name}\"");

            if (result == OperationResult.Success)
            {
                mnuViewRefresh_Click(sender, e);

                // Chon san thu muc vua tao tren lvwFiles, giong hanh vi Windows Explorer
                // (tao xong la thay ngay va co the doi ten/mo luon khong can tu tim).
                SelectAndFocusListViewItem(newFolderPath);
            }
        }

        /// <summary>
        /// Chon (Selected = true) va cuon toi mot muc tren lvwFiles theo duong dan day
        /// du, dung sau khi tao/doi ten mot muc de nguoi dung thay ngay ket qua thay vi
        /// phai tu tim lai trong danh sach. Khong lam gi neu khong tim thay muc do (VD:
        /// LoadListViewFiles dang loc theo _showHiddenItems va muc do bi an).
        /// </summary>
        /// <summary>
        /// Dieu huong MainForm den thu muc chua mot muc, roi chon/focus ngay muc do
        /// tren lvwFiles - dung cho SearchForm (goi qua Owner, xem SearchForm.
        /// lvwResults_DoubleClick) khi nguoi dung double-click mot ket qua tim kiem
        /// va muon "mo thu muc chua tep" giong Windows Explorer, thay vi phai tu di
        /// chuyen thu cong den do. Cong khai (public) vi SearchForm la mot Form
        /// khac, khong the goi truc tiep NavigateTo()/SelectAndFocusListViewItem()
        /// (ca hai deu private).
        /// </summary>
        /// <param name="fullPath">Duong dan day du cua file/thu muc can chon (tu FileItemModel.FullPath).</param>
        public void NavigateToAndSelect(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
                return;

            // Thu muc CHUA muc do - neu fullPath la file thi la thu muc cha; neu la
            // chinh mot thu muc thi cung la thu muc cha cua NO (mo thu muc cha ra,
            // roi chon chinh thu muc con nay trong danh sach - giong hanh vi "Open
            // file location" cua Windows Explorer, ap dung nhat quan cho ca file lan thu muc).
            string containingFolder = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrWhiteSpace(containingFolder) || !Directory.Exists(containingFolder))
                return;

            NavigateTo(containingFolder);
            SelectAndFocusListViewItem(fullPath);
        }

        /// <param name="fullPath">Duong dan day du cua muc can chon.</param>
        private void SelectAndFocusListViewItem(string fullPath)
        {
            foreach (ListViewItem item in lvwFiles.Items)
            {
                if (string.Equals(item.Tag as string, fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    lvwFiles.SelectedItems.Clear();
                    item.Selected = true;
                    item.Focused = true;
                    item.EnsureVisible();
                    lvwFiles.Focus();
                    break;
                }
            }
        }

        private void mnuFileNewFile_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox(
                "Nhap ten file moi (bao gom phan mo rong, VD: moi.txt):", "Tao file moi", "New File.txt");

            if (string.IsNullOrWhiteSpace(name))
                return; // Nguoi dung bam Cancel hoac de trong.

            string newFilePath = Path.Combine(_currentPath, name);
            OperationResult result = _fileService.CreateFile(_currentPath, name);
            ShowOperationResultMessage(result, $"tao file \"{name}\"");

            // Ghi log ca khi thanh cong lan that bai, kem thong diep loi cu the -
            // xem ghi chu tuong tu tai mnuFileNewFolder_Click.
            LogOperationResult(FileOperationType.CreateFile, newFilePath, null, result, $"tạo file \"{name}\"");

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
            string message = BuildOperationResultMessage(result, actionDescription);

            string caption;
            MessageBoxIcon icon;
            switch (result)
            {
                case OperationResult.Success:
                    caption = "Thông báo";
                    icon = MessageBoxIcon.Information;
                    break;

                case OperationResult.Skipped:
                    caption = "Cảnh báo";
                    icon = MessageBoxIcon.Warning;
                    break;

                case OperationResult.AccessDenied:
                case OperationResult.NotFound:
                    caption = "Lỗi";
                    icon = MessageBoxIcon.Error;
                    break;

                case OperationResult.FileInUse:
                    caption = "Tệp đang được sử dụng";
                    icon = MessageBoxIcon.Warning;
                    break;

                case OperationResult.InvalidDestination:
                    caption = "Vị trí đích không hợp lệ";
                    icon = MessageBoxIcon.Warning;
                    break;

                case OperationResult.PartialSuccess:
                    caption = "Hoàn tất một phần";
                    icon = MessageBoxIcon.Warning;
                    break;

                default:
                    caption = "Lỗi";
                    icon = MessageBoxIcon.Error;
                    break;
            }

            MessageBox.Show(message, caption, MessageBoxButtons.OK, icon);
        }

        /// <summary>
        /// Tra ve THONG DIEP CHI TIET (khong phai caption/icon) ung voi mot
        /// OperationResult cu the - tach rieng khoi ShowOperationResultMessage de
        /// CUNG MOT NOI DUNG duoc dung o CA 2 noi: hien MessageBox cho nguoi dung
        /// (nhu truoc gio) VA ghi vao LogEntryModel.Message khi ghi log that bai
        /// (xem cac loi goi LogOperationResult trong cac handler Copy/Move/Delete/
        /// Rename/CreateFile/CreateFolder) - tranh 2 noi
        /// dinh nghia 2 chuoi mo ta khac nhau cho cung mot OperationResult, de
        /// lech nhau ve sau khi sua chi 1 trong 2 cho.
        /// </summary>
        /// <param name="result">Ket qua thao tac.</param>
        /// <param name="actionDescription">Mo ta ngan gon thao tac (VD: "tao thu muc \"abc\"").</param>
        private static string BuildOperationResultMessage(OperationResult result, string actionDescription)
        {
            switch (result)
            {
                case OperationResult.Success:
                    return $"Đã {actionDescription} thành công.";

                case OperationResult.Skipped:
                    return $"Không thể {actionDescription}: đã có mục trùng tên trong thư mục này.";

                case OperationResult.AccessDenied:
                    return $"Không thể {actionDescription}: không đủ quyền truy cập thư mục này.";

                case OperationResult.NotFound:
                    return $"Không thể {actionDescription}: không tìm thấy thư mục đích.";

                case OperationResult.FileInUse:
                    return $"Không thể {actionDescription}: tệp đang được chương trình khác sử dụng. " +
                        "Vui lòng đóng chương trình đang mở tệp này rồi thử lại.";

                case OperationResult.InvalidDestination:
                    return $"Không thể {actionDescription}: không thể di chuyển/sao chép một thư mục " +
                        "vào chính nó hoặc vào một thư mục con của chính nó.";

                case OperationResult.PartialSuccess:
                    return $"Đã {actionDescription} sang vị trí mới, nhưng không xóa được bản gốc " +
                        "(có thể do đang bị chương trình khác sử dụng hoặc thiếu quyền). " +
                        "Vui lòng tự xóa bản gốc thủ công nếu cần.";

                case OperationResult.Cancelled:
                    return $"Đã hủy {actionDescription} theo yêu cầu người dùng.";

                default:
                    return $"Không thể {actionDescription}: tên không hợp lệ hoặc có lỗi xảy ra.";
            }
        }

        /// <summary>
        /// Ghi log MOT thao tac, tu dong kem theo THONG DIEP LOI (giong het chuoi
        /// MessageBox se hien, xem BuildOperationResultMessage) khi result KHONG
        /// PHAI Success - de LogEntryModel.Message luon giai thich ro NGUYEN NHAN
        /// that bai (VD: "khong du quyen truy cap", "da co muc trung ten") thay vi
        /// chi ghi ten OperationResult enum tro tui (VD chi "AccessDenied") ma
        /// nguoi xem log sau nay phai tu suy doan y nghia. Voi Success, KHONG kem
        /// message (rong) vi khong can giai thich gi them cho mot thao tac da
        /// thanh cong - tranh lam dong log dai them mot cach khong can thiet.
        /// </summary>
        /// <param name="operation">Loai thao tac.</param>
        /// <param name="source">Duong dan nguon.</param>
        /// <param name="destination">Duong dan dich (co the null).</param>
        /// <param name="result">Ket qua thao tac.</param>
        /// <param name="actionDescription">Mo ta ngan gon thao tac, dung de dung chung mot cau chu voi ShowOperationResultMessage (VD: "tao thu muc \"abc\"").</param>
        /// <param name="extraNote">
        /// Ghi chu CO DINH them vao Message BAT KE thanh cong hay that bai (VD:
        /// "Xóa vào Thùng rác" de phan biet voi "Xóa vĩnh viễn" - ca hai cung
        /// dung FileOperationType.Delete nen can ghi chu them de phan biet khi
        /// xem lai log). Null/rong neu khong can. Khi that bai, noi voi thong
        /// diep loi bang dau gach ngang " - " thanh MOT dong Message duy nhat.
        /// </param>
        /// <param name="itemCount">So luong muc lien quan (mac dinh 1).</param>
        private void LogOperationResult(FileOperationType operation, string source, string destination, OperationResult result, string actionDescription, string extraNote = null, int itemCount = 1)
        {
            string message;
            if (result == OperationResult.Success)
            {
                message = extraNote;
            }
            else
            {
                string failureReason = BuildOperationResultMessage(result, actionDescription);
                message = string.IsNullOrEmpty(extraNote) ? failureReason : $"{extraNote} - {failureReason}";
            }

            _logService.LogOperation(operation, source, destination, result, message, itemCount);
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

            // To mo ngay cac item vua Cut tren lvwFiles (khong can refresh lai tu
            // dia) - vong lap qua toan bo Items de khoi phuc mau binh thuong cho cac
            // item cua lan Cut TRUOC (neu co, VD: Cut A roi lai Cut B ma chua Paste)
            // va to mo cac item cua lan Cut nay.
            foreach (ListViewItem item in lvwFiles.Items)
            {
                ApplyCutVisualState(item);
            }
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

            // Copy khong to mo item nao ca, nhung neu truoc do da co mot lan Cut
            // (_clipboardIsCut vua doi thanh false) thi can khoi phuc lai mau binh
            // thuong cho cac item da bi to mo boi lan Cut do.
            foreach (ListViewItem item in lvwFiles.Items)
            {
                ApplyCutVisualState(item);
            }
        }

        private async void mnuEditPaste_Click(object sender, EventArgs e)
        {
            if (_clipboardPaths.Count == 0)
            {
                MessageBox.Show("Chua co gi trong clipboard de dan (hay Cut/Copy truoc).",
                    "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Gom het duong dan bi bo qua (do loi quyen/IO tren tung thu muc/file con
            // rieng le trong luc CopyFolder de quy) cua CA LOAT muc dang dan, de chi
            // bao MOT LAN sau khi dan xong toan bo, thay vi lam gian doan nguoi dung
            // bang nhieu hop thoai giua chung khi dan nhieu muc cung luc.
            var allSkippedPaths = new List<string>();

            // Neu nguoi dung da tick "Ap dung cho tat ca" tren ConflictResolutionForm,
            // ghi nho lai hanh dong (Overwrite/Skip) de tu dong dung lai cho cac muc
            // trung ten con lai, khong hoi lai tung muc mot nua. Rename khong duoc
            // ghi nho vi ten moi chi hop le rieng cho muc da hoi.
            ConflictAction? rememberedAction = null;

            // Hop thoai ProgressBar (giong Windows Explorer) + tspProgress/tsslStatus
            // tren status bar - ca hai deu duoc cap nhat tu CUNG mot pasteProgress ben
            // duoi. copyProgressForm.Show(this) la MODELESS (khong phai ShowDialog) nen
            // khong chan cac lenh await ben trong vong lap Paste.
            //
            // cts la nguon phat CancellationToken cho toan bo lan Paste nay - khi
            // nguoi dung bam nut Huy tren copyProgressForm (su kien CancelRequested),
            // chi can goi cts.Cancel(); FileService/FolderService tu kiem tra token
            // nay giua vong lap doc/ghi buffer va giua tung file/thu muc con.
            using (var cts = new CancellationTokenSource())
            using (var copyProgressForm = new CopyProgressForm())
            {
                copyProgressForm.CancelRequested += (s, args) => cts.Cancel();
                copyProgressForm.Show(this);

                // Hien thanh tien do (tspProgress) + cap nhat trang thai (tsslStatus) tren
                // status bar trong suot qua trinh dan. pasteProgress khai bao KIEU
                // IProgress<FileOperationProgress> (khong phai Progress<FileOperationProgress>)
                // vi Progress<T> cai dat IProgress<T>.Report() theo kieu EXPLICIT - goi
                // truc tiep .Report() tren bien kieu Progress<T> se khong bien dich duoc.
                // Duoc tao NGAY TAI DAY tren UI thread nen tu dong Post() callback ve dung
                // UI thread moi lan Report(), du Report() thuc su duoc goi tu dau (VD: tu
                // trong vong lap doc/ghi cua CopyFileAsync sau khi da ConfigureAwait(false))
                // - xem them chu thich cua FileOperationProgress.
                int totalItemsInBatch = _clipboardPaths.Count;
                int currentItemIndex = 0;

                IProgress<FileOperationProgress> pasteProgress = new Progress<FileOperationProgress>(p =>
                {
                    tspProgress.Value = p.PercentComplete;
                    tsslStatus.Text = totalItemsInBatch > 1
                        ? $"Đang dán mục {currentItemIndex}/{totalItemsInBatch}: \"{p.CurrentFileName}\" ({p.PercentComplete}%)"
                        : $"Đang dán \"{p.CurrentFileName}\"... ({p.PercentComplete}%)";
                    copyProgressForm.UpdateProgress(p);
                });

                tspProgress.Value = 0;
                tspProgress.Visible = true;

                try
                {
                    foreach (string sourcePath in _clipboardPaths)
                    {
                        currentItemIndex++;

                        string name = Path.GetFileName(sourcePath);
                        string destinationPath = Path.Combine(_currentPath, name);
                        bool isDirectory = Directory.Exists(sourcePath);
                        bool hasConflict = File.Exists(destinationPath) || Directory.Exists(destinationPath);
                        bool overwriteFile = false; // Chi dat true khi nguoi dung xac nhan Overwrite cho truong hop file+Copy.

                        if (hasConflict)
                        {
                            ConflictAction action;
                            string newName = null;

                            if (rememberedAction.HasValue)
                            {
                                action = rememberedAction.Value;
                            }
                            else
                            {
                                using (var dialog = new ConflictResolutionForm(sourcePath, _currentPath))
                                {
                                    DialogResult dialogResult = dialog.ShowDialog(this);
                                    action = dialogResult == DialogResult.OK ? dialog.SelectedAction : ConflictAction.Cancel;
                                    newName = dialog.NewName;

                                    if (dialog.ApplyToAll && action != ConflictAction.Rename)
                                        rememberedAction = action;
                                }
                            }

                            if (action == ConflictAction.Cancel)
                                break; // Nguoi dung dong dialog - dung het cac muc con lai trong clipboard.

                            if (action == ConflictAction.Skip)
                                continue; // Bo qua rieng muc nay, tiep tuc muc tiep theo.

                            if (action == ConflictAction.Rename)
                            {
                                name = newName;
                                destinationPath = Path.Combine(_currentPath, newName);
                            }
                            else if (action == ConflictAction.Overwrite)
                            {
                                if (!isDirectory && !_clipboardIsCut)
                                {
                                    // File + Copy: File.Copy co tham so overwrite rieng, hieu
                                    // qua hon xoa-roi-tao-lai - danh dau de dung ben duoi.
                                    overwriteFile = true;
                                }
                                else
                                {
                                    // Thu muc (Directory.Move/CopyFolder khong co tham so
                                    // overwrite), hoac file+Cut (File.Move cung khong co) -
                                    // xoa truoc muc dich cu (vao Thung rac, an toan hon xoa
                                    // vinh vien) roi moi dan vao.
                                    OperationResult deleteResult = _recycleBinService.DeleteToRecycleBin(destinationPath);
                                    if (deleteResult != OperationResult.Success)
                                    {
                                        ShowOperationResultMessage(deleteResult, $"ghi đè \"{name}\"");
                                        continue;
                                    }
                                }
                            }
                        }

                        OperationResult result;
                        if (isDirectory)
                        {
                            if (_clipboardIsCut)
                            {
                                result = _folderService.MoveFolder(sourcePath, destinationPath);
                            }
                            else
                            {
                                var skippedPaths = new List<string>();
                                result = await _folderService.CopyFolderAsync(sourcePath, destinationPath, skippedPaths, pasteProgress, cts.Token);
                                allSkippedPaths.AddRange(skippedPaths);
                            }
                        }
                        else
                        {
                            if (_clipboardIsCut)
                            {
                                result = _fileService.MoveFile(sourcePath, destinationPath);
                            }
                            else
                            {
                                // Quy tu IProgress<long> (byte luy ke CUA RIENG file nay, do
                                // CopyFileAsync bao cao) sang IProgress<FileOperationProgress>
                                // (pasteProgress) - tuong tu FolderService.FileBytesProgressAdapter,
                                // nhung o day chi co 1 file (TotalFiles = 1, FilesCompleted = 0)
                                // nen khong can CopyProgressState rieng.
                                long sourceFileSize = 0;
                                try { sourceFileSize = new FileInfo(sourcePath).Length; }
                                catch (IOException) { /* Khong lay duoc dung luong - van tiep tuc copy, chi khong hien % chinh xac. */ }
                                catch (UnauthorizedAccessException) { /* Tuong tu. */ }

                                string currentFileName = name;
                                var fileProgress = new Progress<long>(bytesTransferred =>
                                {
                                    pasteProgress.Report(new FileOperationProgress
                                    {
                                        CurrentFileName = currentFileName,
                                        FilesCompleted = 0,
                                        TotalFiles = 1,
                                        CurrentFileBytesTransferred = bytesTransferred,
                                        CurrentFileTotalBytes = sourceFileSize
                                    });
                                });

                                // CopyFileAsync: doc/ghi bang FileStream + buffer, khong chan UI
                                // thread khi dan file lon - await ngay tai day, UI van phan hoi
                                // duoc trong luc copy (VD: nguoi dung van co the di chuyen/resize
                                // cua so) vi mnuEditPaste_Click da chuyen thanh async void.
                                result = await _fileService.CopyFileAsync(sourcePath, destinationPath, overwriteFile, fileProgress, cts.Token);
                            }
                        }

                        // Ghi log tung muc rieng le (giong cach lam voi Delete o
                        // mnuEditDelete_Click/lvwFiles_KeyDown) - moi muc trong batch
                        // co the co OperationResult KHAC NHAU (VD: muc dau Success,
                        // muc sau AccessDenied), gop chung se mat thong tin muc nao
                        // that bai, kem thong diep loi cu the. _clipboardIsCut phan
                        // biet Move (Cut roi Paste) voi Copy (Copy roi Paste) - ca
                        // thu muc lan file deu dung chung logic nay vi
                        // FileOperationType.Move/Copy khong phan biet file/thu muc
                        // (giong FileOperationType.Rename). actionDescription
                        // ("dán ...") khop voi cau ShowOperationResultMessage se
                        // hien ngay sau day, de MessageBox va log noi cung mot cau.
                        FileOperationType pasteOperationType = _clipboardIsCut ? FileOperationType.Move : FileOperationType.Copy;
                        LogOperationResult(pasteOperationType, sourcePath, destinationPath, result, $"dán \"{name}\"");

                        if (result == OperationResult.Cancelled)
                            break; // Nguoi dung bam Huy tren CopyProgressForm - dung ngay, khong hien thong bao ket qua cho muc dang do dang.

                        ShowOperationResultMessage(result, $"dan \"{name}\"");
                    }
                }
                finally
                {
                    // Luon dong hop thoai ProgressBar + an lai thanh tien do tren status
                    // bar va tra tsslStatus ve trang thai binh thuong sau khi dan xong -
                    // ke ca khi nguoi dung Cancel giua chung (break) hoac phat sinh loi
                    // khong luong truoc, tranh de lai hop thoai/thanh tien do dung yen
                    // gay hieu lam la ung dung dang treo.
                    copyProgressForm.Close();
                    tspProgress.Visible = false;
                    tspProgress.Value = 0;
                    tsslStatus.Text = "Sẵn sàng";
                }
            }

            if (allSkippedPaths.Count > 0)
            {
                // Bao rieng cho nguoi dung biet co bo sot noi dung trong luc sao chep
                // thu muc (VD: file dang bi khoa boi ung dung khac, thu muc con mat
                // quyen doc) - khac voi ShowOperationResultMessage() o tren vi ket qua
                // tong the van la Success (thu muc goc da duoc tao/sao chep xong).
                string preview = string.Join("\n", allSkippedPaths.Take(10));
                if (allSkippedPaths.Count > 10)
                    preview += $"\n... và {allSkippedPaths.Count - 10} mục khác.";

                MessageBox.Show(
                    $"Đã dán xong nhưng bỏ qua {allSkippedPaths.Count} mục không thể sao chép " +
                    $"(thiếu quyền truy cập hoặc đang bị khóa bởi ứng dụng khác):\n\n{preview}",
                    "Hoàn tất với một số mục bị bỏ qua", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                OperationResult result = _recycleBinService.DeleteToRecycleBin(path);
                ShowOperationResultMessage(result, $"xoa \"{Path.GetFileName(path)}\"");

                // Ghi log tung muc rieng le (khong gop 1 dong ItemCount = selected.Count)
                // vi moi muc co the co OperationResult KHAC NHAU (VD: muc A xoa
                // thanh cong, muc B bi AccessDenied) - gop chung se lam mat thong
                // tin muc nao that bai. "Xóa vào Thùng rác" trong extraNote de
                // phan biet voi nhanh xoa vinh vien (Shift+Delete) ben duoi, cung
                // ghi FileOperationType.Delete nhung khac muc dich; khi that bai,
                // LogOperationResult tu noi them thong diep loi cu the vao sau.
                LogOperationResult(FileOperationType.Delete, path, null, result, $"xóa \"{Path.GetFileName(path)}\"", "Xóa vào Thùng rác");
            }

            mnuViewRefresh_Click(sender, e);
        }

        /// <summary>
        /// Bat rieng Shift+Delete tren lvwFiles: xoa vinh vien (bo qua Thung rac),
        /// giong hanh vi chuan cua Windows Explorer. Delete thuong (khong Shift) van
        /// di qua mnuEditDelete_Click (ShortcutKeys = Keys.Delete, khong kem Shift)
        /// nhu cu - hai duong khong trung nhau vi WinForms chi kich hoat ShortcutKeys
        /// khi to hop phim khop hoan toan (Delete thuong se khong khop Shift+Delete).
        /// </summary>
        private void lvwFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Delete || !e.Shift)
                return;

            e.Handled = true; // Tranh su kien Delete thuong (mnuEditDelete) xu ly lai lan nua.

            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chưa chọn mục nào để xóa.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Liet ke ro ten tung muc (toi da 10, sau do rut gon "... va N muc khac")
            // de nguoi dung biet chinh xac dang xoa gi truoc khi xac nhan, thay vi
            // chi thay so luong chung chung - quan trong hon voi xoa vinh vien vi
            // khong the "undo" bang cach mo lai Thung rac nhu Delete thuong.
            const int maxNamesToShow = 10;
            var names = selected.Take(maxNamesToShow).Select(p => "  • " + Path.GetFileName(p)).ToList();
            if (selected.Count > maxNamesToShow)
                names.Add($"  ... và {selected.Count - maxNamesToShow} mục khác.");

            string message = $"Bạn có chắc muốn XÓA VĨNH VIỄN {selected.Count} mục sau đây?\n\n" +
                string.Join(Environment.NewLine, names) +
                "\n\nHành động này KHÔNG THỂ khôi phục (không đi qua Thùng rác) - " +
                "nếu là thư mục, toàn bộ tệp/thư mục con bên trong cũng bị xóa vĩnh viễn.";

            DialogResult confirm = MessageBox.Show(message, "Xác nhận xóa vĩnh viễn",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            foreach (string path in selected)
            {
                OperationResult result = _fileService.DeletePermanently(path);
                ShowOperationResultMessage(result, $"xóa vĩnh viễn \"{Path.GetFileName(path)}\"");

                // Ghi log tung muc rieng le - xem ghi chu tuong tu tai
                // mnuEditDelete_Click. "Xóa vĩnh viễn" trong extraNote la thong
                // tin QUAN TRONG can giu lai (khac voi xoa vao Thung rac van con
                // co the khoi phuc duoc) vi FileOperationType khong co gia tri
                // rieng cho xoa vinh vien.
                LogOperationResult(FileOperationType.Delete, path, null, result, $"xóa vĩnh viễn \"{Path.GetFileName(path)}\"", "Xóa vĩnh viễn (Shift+Delete)");
            }

            mnuViewRefresh_Click(sender, e);
        }

        /// <summary>
        /// Muc "Doi ten" tren menu Chinh sua / F2: thay vi hien InputBox rieng, chuyen
        /// sang sua ten truc tiep tren chinh o ten cua lvwFiles (LabelEdit) - giong
        /// hanh vi chuan cua Windows Explorer. Logic doi ten thuc su nam o
        /// lvwFiles_AfterLabelEdit, duoc goi tu dong khi nguoi dung go xong va nhan Enter.
        /// </summary>
        private void mnuEditRename_Click(object sender, EventArgs e)
        {
            if (lvwFiles.SelectedItems.Count != 1)
            {
                MessageBox.Show("Vui long chon dung mot muc de doi ten.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lvwFiles.SelectedItems[0].BeginEdit();
        }

        /// <summary>
        /// Chan LabelEdit ngay tu dau neu muc dang chon khong con hop le (VD: da bi
        /// xoa/di chuyen boi tien trinh khac giua luc dang hien danh sach) - tranh
        /// AfterLabelEdit phai xu ly mot Tag khong con dung.
        /// </summary>
        private void lvwFiles_BeforeLabelEdit(object sender, LabelEditEventArgs e)
        {
            string path = lvwFiles.Items[e.Item].Tag as string;
            if (string.IsNullOrEmpty(path) || (!File.Exists(path) && !Directory.Exists(path)))
                e.CancelEdit = true;
        }

        /// <summary>
        /// Nguoi dung go xong ten moi truc tiep tren o (LabelEdit) va nhan Enter (hoac
        /// bam ra ngoai) - e.Label la ten moi (null neu nguoi dung nhan Esc de huy,
        /// hoac khong doi gi ca). Luon e.CancelEdit = true de tu quan ly lai Text cua
        /// item (thanh ten that su sau khi doi, hoac giu nguyen ten cu neu that bai/huy)
        /// thay vi de WinForms tu dong ap e.Label vao Text - vi item.Text can khop voi
        /// FileService.Rename tra ve thanh cong hay khong.
        /// </summary>
        private void lvwFiles_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            e.CancelEdit = true; // Luon tu cap nhat lai Text ben duoi, khong de WinForms tu ap e.Label.

            if (e.Label == null)
                return; // Nguoi dung nhan Esc hoac khong doi gi - giu nguyen ten cu.

            ListViewItem item = lvwFiles.Items[e.Item];
            string path = item.Tag as string;
            string oldName = Path.GetFileName(path);
            string newName = e.Label;

            if (string.IsNullOrWhiteSpace(newName) || newName == oldName)
                return;

            // Kiem tra hop le ngay tai day (truoc khi goi Rename) de bao loi cu the
            // hon "khong the doi ten: co loi xay ra" chung chung cua nhanh default
            // trong ShowOperationResultMessage - nguoi dung biet ngay ly do (VD: chua
            // ky tu cam \ / : * ? " < > |) thay vi phai tu doan.
            if (!FileHelper.IsValidFileName(newName))
            {
                const string invalidNameMessage =
                    "Không được để trống, chứa ký tự \\ / : * ? \" < > |, kết thúc bằng khoảng trắng/dấu chấm, " +
                    "trùng tên thiết bị hệ thống (CON, PRN...), hoặc dài quá 255 ký tự.";

                MessageBox.Show(
                    $"Tên \"{newName}\" không hợp lệ: {invalidNameMessage}",
                    "Tên không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Ghi log CA TRUONG HOP nay - tuy chua goi den _fileService.Rename
                // (bi chan tu som boi FileHelper.IsValidFileName), day van la MOT
                // LAN NGUOI DUNG THU doi ten va that bai, nen van dang duoc ghi
                // vao lich su giong cac loai that bai khac (trung ten, file dang
                // bi khoa...) thay vi bo sot rieng truong hop nay.
                LogOperationResult(FileOperationType.Rename, path, null, OperationResult.Failed,
                    $"đổi tên \"{oldName}\" thành \"{newName}\"", $"Tên mới không hợp lệ: {invalidNameMessage}");

                return; // CancelEdit = true da khoi phuc lai ten cu tren o hien thi.
            }

            OperationResult result = _fileService.Rename(path, newName);
            string renamedPath = Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, newName);

            // Ghi log ca khi thanh cong lan that bai (VD: trung ten, file dang bi
            // khoa boi chuong trinh khac), kem thong diep loi cu the. Dung lai
            // _fileService.Rename cho CA file lan thu muc (khong co RenameFolder
            // rieng trong FileOperationType) nen ghi FileOperationType.Rename
            // chung, dung voi thuc te loi goi Service ben tren. Source = duong
            // dan CU (truoc doi ten), Destination = duong dan MOI (sau doi ten) -
            // phan anh dung ban chat "truoc/sau" cua rename, giup GetLogs sau nay
            // hien duoc ca ten cu lan ten moi tren cung 1 dong.
            LogOperationResult(FileOperationType.Rename, path, renamedPath, result, $"đổi tên \"{oldName}\" thành \"{newName}\"");

            if (result == OperationResult.Success)
            {
                item.Text = newName;
                item.Tag = renamedPath;

                // Doi thu tu (thu muc/file van con dung nhom truoc/sau) co the thay doi
                // vi ten moi co the sap xep khac ten cu - lam moi lai toan bo cho chac
                // chan dung thu tu, dong thoi chon lai chinh muc vua doi ten.
                mnuViewRefresh_Click(sender, e);
                SelectAndFocusListViewItem(renamedPath);
            }
            else
            {
                ShowOperationResultMessage(result, $"đổi tên \"{oldName}\" thành \"{newName}\"");
                // Giu nguyen item.Text (khong gan gi) vi CancelEdit = true da tu dong
                // khoi phuc lai ten cu tren o hien thi.
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

        private void mnuEditProperties_Click(object sender, EventArgs e)
        {
            ShowPropertiesForSelectedItem();
        }

        /// <summary>
        /// Mo PropertiesForm cho DUY NHAT mot muc dang duoc chon tren lvwFiles -
        /// giong Windows Explorer, hop thoai Properties chi ho tro xem/sua mot muc
        /// tai mot thoi diem (chon nhieu muc thi menu Thuoc tinh se bi vo hieu hoa,
        /// xem cmsListView_Opening). Duoc goi tu ca menu Chinh sua > Thuoc tinh,
        /// menu chuot phai > Thuoc tinh, va phim tat Alt+Enter (lvwFiles_KeyDown).
        /// </summary>
        private void ShowPropertiesForSelectedItem()
        {
            if (lvwFiles.SelectedItems.Count != 1)
                return;

            string path = lvwFiles.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(path))
                return;

            using (var propertiesForm = new PropertiesForm(path))
            {
                propertiesForm.ShowDialog(this);
            }

            // Thuoc tinh ReadOnly/Hidden co the da bi doi trong hop thoai (nut Ap
            // dung/OK) - lam moi lai danh sach de icon/mau chu (item bi an thuong
            // hien mau khac, xem LoadListViewFiles) phan anh dung trang thai moi.
            mnuViewRefresh_Click(sender: this, e: EventArgs.Empty);
            SelectAndFocusListViewItem(path);
        }

        #endregion

        #region Menu Xem (View)

        private void mnuViewRefresh_Click(object sender, EventArgs e)
        {
            // Dong bo thanh dia chi voi duong dan hien tai - lam truoc tien de txtPath
            // luon dung ke ca khi phan duyet noi dung ben duoi gap loi.
            txtPath.Text = _currentPath;

            // Moi lan noi dung duoc nap lai deu co the vi _currentPath VUA DOI
            // (NavigateTo/Back/Forward/Up deu goi mnuViewRefresh_Click ngay sau
            // khi gan _currentPath moi) - tro FileMonitorService sang thu muc
            // moi TRUOC khi nap ListView, de khong bo lo thay doi nao xay ra
            // ngay sau khi nap xong nhung truoc khi watcher kip bat.
            RestartFolderMonitoring();

            LoadListViewFiles();
        }

        /// <summary>
        /// Nap lai toan bo noi dung (thu muc con + file) cua _currentPath vao
        /// lvwFiles: thu muc liet ke truoc, sau do den file, giong Windows Explorer.
        /// </summary>
        /// <summary>
        /// Neu item nay dang la mot trong cac muc da Cut (_clipboardPaths voi
        /// _clipboardIsCut == true), to mo ForeColor (AppTheme.TextSecondary) de bao
        /// hieu truc quan "da danh dau de di chuyen, chua thuc su bien mat" - giong
        /// hanh vi Windows Explorer. Nguoc lai giu/khoi phuc mau chu binh thuong
        /// (AppTheme.TextPrimary) - can co nhanh else vi ham nay cung duoc goi lai
        /// khi refresh sau khi Paste xong (luc do _clipboardPaths da rong).
        /// </summary>
        private void ApplyCutVisualState(ListViewItem item)
        {
            bool isCut = _clipboardIsCut
                && item.Tag is string path
                && _clipboardPaths.Contains(path, StringComparer.OrdinalIgnoreCase);

            item.ForeColor = isCut ? AppTheme.TextSecondary : AppTheme.TextPrimary;
        }

        private void LoadListViewFiles()
        {
            lvwFiles.BeginUpdate();
            lvwFiles.Items.Clear();

            int itemCount = 0;
            long totalSize = 0;

            try
            {
                // FileService.GetItems() da tra ve san thu muc con truoc, file sau
                // (giong thu tu Windows Explorer) va tu bat/bo qua rieng tung muc loi
                // quyen/IO ben trong no - khong con can 2 try/catch rieng cho thu muc
                // va file nhu truoc; mot muc loi rieng le se khong lam mat ca danh
                // sach nua (chi khong xuat hien trong ket qua tra ve).
                foreach (FileItemModel entry in _fileService.GetItems(_currentPath, _showHiddenItems))
                {
                    // Bo loc theo nhom loai tep (cboFileTypeFilter) CHI ap dung cho
                    // FILE - thu muc luon hien du chon nhom nao, giong Windows
                    // Explorer khi loc theo "Kind"/"Type" van luon hien thu muc de
                    // nguoi dung con di chuyen vao trong duoc. Xem MatchesFileTypeFilter().
                    if (!entry.IsDirectory && !MatchesFileTypeFilter(entry.FullPath))
                        continue;

                    // O loc nhanh theo ten (txtQuickFilter) ap dung cho CA file va
                    // thu muc - khac voi bo loc nhom loai tep, o day nguoi dung dang
                    // muon tim mot ten cu the trong thu muc hien tai (khong de quy,
                    // khong mo SearchForm), nen thu muc khong duoc "mien" nhu tren.
                    if (!MatchesQuickFilter(entry.Name))
                        continue;

                    ListViewItem item;

                    if (entry.IsDirectory)
                    {
                        item = new ListViewItem(entry.Name, "folder") { Tag = entry.FullPath };
                        // SubItems[1] (Kich thuoc) khong co gia tri hien thi cho thu
                        // muc, nhung van gan Tag = 0L de ListViewItemComparer co gia
                        // tri so de so sanh khi sap xep theo cot Kich thuoc (xem
                        // ListViewItemComparer.CompareByColumn) - khong dung -1 vi se
                        // bi hieu la "chua co gia tri" thay vi "kich thuoc = 0".
                        var sizeSubItem = item.SubItems.Add(string.Empty); // Thu muc khong hien kich thuoc truc tiep.
                        sizeSubItem.Tag = 0L;
                        item.SubItems.Add("Thư mục tệp");
                        var modifiedSubItem = item.SubItems.Add(FormatHelper.FormatDate(entry.ModifiedDate));
                        modifiedSubItem.Tag = entry.ModifiedDate;
                        lvwFiles.Items.Add(item);
                    }
                    else
                    {
                        item = new ListViewItem(entry.Name, GetFileImageKey(entry.FullPath)) { Tag = entry.FullPath };
                        // Tag cua tung SubItem giu lai GIA TRI GOC (long/DateTime) -
                        // ListViewItemComparer dung gia tri nay de so sanh dung kieu
                        // so/ngay thay vi so sanh chuoi da dinh dang (VD: "1 KB" >
                        // "20 KB" theo chuoi nhung sai ve gia tri thuc, hoac ngay dang
                        // "dd/MM/yyyy" sap xep chuoi se sai thu tu thang/nam).
                        var sizeSubItem = item.SubItems.Add(entry.SizeFormatted);
                        sizeSubItem.Tag = entry.Size;
                        item.SubItems.Add(FileHelper.GetFileType(entry.FullPath));
                        var modifiedSubItem = item.SubItems.Add(FormatHelper.FormatDate(entry.ModifiedDate));
                        modifiedSubItem.Tag = entry.ModifiedDate;
                        lvwFiles.Items.Add(item);
                        totalSize += entry.Size;
                    }

                    ApplyCutVisualState(item);

                    itemCount++;
                }
            }
            finally
            {
                lvwFiles.EndUpdate();
            }

            UpdateEmptyFolderMessage(itemCount);

            tsslItemCount.Text = $"{itemCount} mục";
            tsslTotalSize.Text = FormatHelper.FormatSize(totalSize);
            tsslStatus.Text = "Sẵn sàng";
        }

        /// <summary>
        /// Hien/an lblEmptyFolder de lvwFiles khong bi bo trong khi thu muc hien
        /// tai khong co gi de hien thi - giong Windows Explorer bao "Thư mục này
        /// trống" thay vi chi de mot bang trang khong ro ly do.
        ///
        /// Phan biet 2 truong hop rieng: thu muc thuc su khong co gi ben trong, va
        /// thu muc co noi dung nhung tat ca deu bi an (do tuy chon "Hien file/thu
        /// muc an" dang tat) - truong hop sau can noi ro cho nguoi dung de tranh
        /// nham tuong thu muc trong trong khi thuc ra chi dang bi loc.
        /// </summary>
        /// <param name="visibleItemCount">So muc dang hien thi tren lvwFiles sau khi loc.</param>
        private void UpdateEmptyFolderMessage(int visibleItemCount)
        {
            if (visibleItemCount > 0)
            {
                lblEmptyFolder.Visible = false;
                return;
            }

            // Chi kiem tra lai voi includeHidden: true khi thuc su can (danh sach dang
            // hien la rong) - tranh goi GetItems() 2 lan cho truong hop thong thuong
            // (co noi dung) vi ham nay duyet dia moi lan goi.
            bool hasHiddenItemsOnly = !_showHiddenItems
                && _fileService.GetItems(_currentPath, includeHidden: true).Count > 0;

            lblEmptyFolder.Text = hasHiddenItemsOnly
                ? "Thư mục này chỉ chứa các mục đang ẩn.\nBật \"Hiện file/thư mục ẩn\" trong menu Xem để xem."
                : "Thư mục này trống";
            lblEmptyFolder.Visible = true;
        }

        /// <summary>
        /// Kiem tra du lieu dang duoc keo vao lvwFiles (tu Windows Explorer
        /// hoac ung dung khac ben ngoai - KHONG phai keo-tha noi bo giua
        /// lvwFiles/trvFolders, se xu ly rieng o mot yeu cau khac) co dung la
        /// mot hoac nhieu file/thu muc (DataFormats.FileDrop) hay khong.
        /// e.Effect = Copy neu hop le (con tro chuot doi thanh dau "+", cho
        /// phep tha) - None neu khong (VD: keo van ban/anh truc tiep tu trinh
        /// duyet, khong phai duong dan file thuc te tren dia), WinForms se tu
        /// hien con tro "cam" phu hop, khong can tu ve.
        /// </summary>
        private void lvwFiles_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
        }

        /// <summary>
        /// Xu ly khi nguoi dung THA (drop) file duoc keo TU BEN NGOAI (Windows
        /// Explorer hoac ung dung khac - KHONG phai keo-tha noi bo giua
        /// lvwFiles/trvFolders, se lam o mot yeu cau khac) vao lvwFiles - sao
        /// chep TUNG file vao _currentPath (thu muc dang mo) qua
        /// FileService.CopyFile (ban dong bo don gian, danh cho thao tac tuc
        /// thi nhu the nay; nang cap len CopyFileAsync + CopyProgressForm
        /// giong mnuEditPaste_Click se lam o mot yeu cau rieng neu can theo
        /// doi tien do/huy giua chung khi keo nhieu file lon).
        ///
        /// PHAM VI HIEN TAI: chi xu ly FILE. Thu muc duoc keo vao se bi BO QUA
        /// (co bao rieng trong tong ket, khong am tham lam ngo) - sao chep ca
        /// thu muc (de quy) can FolderService.CopyFolderAsync, se lam o mot
        /// yeu cau khac. File trung ten voi mot muc DA CO trong thu muc dich
        /// se bi Skipped (CHUA hoi Ghi de/Doi ten/Bo qua nhu
        /// ConflictResolutionForm da lam cho Paste - co the bo sung sau).
        /// </summary>
        private void lvwFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] droppedPaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (droppedPaths == null || droppedPaths.Length == 0)
                return;

            int successCount = 0;
            var skippedFolderNames = new List<string>();
            var problemLines = new List<string>();

            foreach (string sourcePath in droppedPaths)
            {
                if (string.IsNullOrWhiteSpace(sourcePath))
                    continue;

                if (Directory.Exists(sourcePath))
                {
                    // Chua ho tro keo-tha thu muc trong pham vi nay - xem <summary>.
                    skippedFolderNames.Add(Path.GetFileName(sourcePath));
                    continue;
                }

                if (!File.Exists(sourcePath))
                    continue; // Duong dan khong con hop le giua luc keo-tha (VD: vua bi xoa) - bo qua.

                string name = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(_currentPath, name);

                OperationResult result = _fileService.CopyFile(sourcePath, destinationPath);
                LogOperationResult(FileOperationType.Copy, sourcePath, destinationPath, result, $"kéo-thả \"{name}\" vào thư mục này");

                if (result == OperationResult.Success)
                    successCount++;
                else
                    problemLines.Add(BuildOperationResultMessage(result, $"sao chép \"{name}\""));
            }

            // MOT hop thoai tong ket duy nhat (khong phai 1 hop thoai/file) - giong
            // nguyen tac da dung o DuplicateForm/BatchRenameForm, tranh lam gian
            // doan nguoi dung bang nhieu hop thoai lien tiep khi keo nhieu file
            // cung luc. Khong hien gi ca neu MOI file deu thanh cong va khong co
            // thu muc nao bi bo qua (tuong tu Windows Explorer, sao chep thanh
            // cong khong can bao rieng).
            var summaryParts = new List<string>();
            if (skippedFolderNames.Count > 0)
                summaryParts.Add($"Đã bỏ qua {skippedFolderNames.Count} thư mục (chưa hỗ trợ kéo-thả thư mục vào đây): " +
                    string.Join(", ", skippedFolderNames));
            if (problemLines.Count > 0)
                summaryParts.Add(string.Join("\n", problemLines));

            if (summaryParts.Count > 0)
            {
                MessageBox.Show(this,
                    $"Đã sao chép thành công {successCount} tệp.\n\n" + string.Join("\n\n", summaryParts),
                    "Kéo-thả tệp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            mnuViewRefresh_Click(sender, e);
        }

        /// <summary>
        /// Giong lvwFiles_DragEnter - kiem tra du lieu keo vao trvFolders co
        /// phai DataFormats.FileDrop hay khong, dung cho tinh nang (se lam o
        /// yeu cau khac) tha file/thu muc truc tiep vao MOT NHANH cu the tren
        /// cay thu muc de sao chep/di chuyen toi do.
        /// </summary>
        private void trvFolders_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                ? DragDropEffects.Copy
                : DragDropEffects.None;
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
                UpdatePreview();
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

            UpdatePreview();
        }

        /// <summary>
        /// Gioi han dung luong toi da (byte) cho mot anh duoc phep preview -
        /// tranh doc nguyen ca anh RAW/PSD/anh do phan giai sieu cao vao RAM
        /// chi de hien thumbnail, vua ton bo nho vua co the treo UI khi
        /// Image.FromStream giai ma anh qua lon.
        /// </summary>
        private const long MaxPreviewImageBytes = 20 * 1024 * 1024; // 20 MB

        /// <summary>So dong toi da doc de preview van ban - tranh nap ca file
        /// log/csv hang trieu dong vao txtPreview lam treo UI.</summary>
        private const int MaxPreviewTextLines = 200;

        /// <summary>Gioi han tong so ky tu doc duoc khi preview van ban, dung
        /// song song voi MaxPreviewTextLines de phong khi file khong xuong
        /// dong (VD file minify .js/.json tren mot dong rat dai).</summary>
        private const int MaxPreviewTextChars = 100_000;

        /// <summary>
        /// Cac phan mo rong duoc coi la van ban thuan (plain text) de doc N
        /// dong dau lam preview trong txtPreview. KHONG dung
        /// FileHelper.FileIconCategory.Code vi nhom do gom ca file nhi phan
        /// (.exe/.dll/.msi) khong doc duoc nhu van ban.
        /// </summary>
        private static readonly HashSet<string> TextPreviewExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".txt", ".md", ".log", ".ini", ".config", ".csv",
            ".json", ".xml", ".html", ".htm", ".css", ".js", ".ts",
            ".cs", ".vb", ".sql", ".bat", ".ps1", ".py", ".java",
            ".c", ".cpp", ".h", ".yaml", ".yml"
        };

        /// <summary>
        /// Dispatcher chinh cho khu vuc preview ben phai lvwFiles: chi hien
        /// preview khi CHINH XAC mot muc duoc chon (khong phai thu muc) -
        /// dua vao phan mo rong de chon giua preview anh (pbxPreview) hay
        /// preview van ban (txtPreview), moi truong hop khac deu goi
        /// ClearPreview voi thong bao phu hop.
        /// </summary>
        private void UpdatePreview()
        {
            if (lvwFiles.SelectedItems.Count != 1)
            {
                ClearPreview(lvwFiles.SelectedItems.Count == 0
                    ? "Không có tệp để xem trước"
                    : "Chọn 1 tệp để xem trước");
                return;
            }

            string path = lvwFiles.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(path) || Directory.Exists(path) || !File.Exists(path))
            {
                ClearPreview("Không có tệp để xem trước");
                return;
            }

            if (FileHelper.GetFileIconCategory(path) == FileIconCategory.Image)
            {
                UpdateImagePreview(path);
            }
            else if (TextPreviewExtensions.Contains(Path.GetExtension(path)))
            {
                UpdateTextPreview(path);
            }
            else
            {
                ClearPreview("Không có bản xem trước cho loại tệp này");
            }
        }

        /// <summary>
        /// Xoa preview hien tai (Dispose Image cu de tranh ro handle GDI+, an
        /// ca pbxPreview lan txtPreview) va AN LUON ca panel preview
        /// (spcFilesPreview.Panel2Collapsed = true) de tra lai toan bo chieu
        /// rong cho lvwFiles khi khong co gi de xem truoc - VD khong chon gi,
        /// chon nhieu muc, chon thu muc, hoac chon mot file khong ho tro
        /// preview (khong phai anh/van ban). lblPreviewCaption.Text van duoc
        /// gan (du dang bi an) de neu sau nay panel duoc mo lai bang tay thi
        /// van co noi dung hop ly thay vi de trong.
        /// </summary>
        private void ClearPreview(string message)
        {
            Image oldImage = pbxPreview.Image;
            pbxPreview.Image = null;
            oldImage?.Dispose();
            pbxPreview.Visible = false;

            txtPreview.Visible = false;
            txtPreview.Text = string.Empty;

            lblPreviewCaption.Text = message;
            spcFilesPreview.Panel2Collapsed = true;
        }

        /// <summary>
        /// Cap nhat pbxPreview cho file anh (theo
        /// FileHelper.FileIconCategory.Image, xac dinh qua phan mo rong).
        ///
        /// Anh duoc doc qua MemoryStream (khong dung Image.FromFile truc tiep)
        /// vi Image.FromFile giu file khoa (locked) cho den khi Image bi
        /// Dispose - se xung dot voi cac thao tac doi ten/xoa/di chuyen file
        /// dang duoc preview. Image cu (neu co) luon duoc Dispose truoc khi
        /// gan Image moi de tranh ro handle GDI+.
        /// </summary>
        private void UpdateImagePreview(string path)
        {
            Image oldImage = pbxPreview.Image;
            pbxPreview.Image = null;
            oldImage?.Dispose();
            txtPreview.Visible = false;
            txtPreview.Text = string.Empty;

            long fileSize;
            try
            {
                fileSize = new FileInfo(path).Length;
            }
            catch (IOException)
            {
                pbxPreview.Visible = false;
                lblPreviewCaption.Text = "Không thể xem trước ảnh này";
                spcFilesPreview.Panel2Collapsed = true;
                return;
            }

            if (fileSize > MaxPreviewImageBytes)
            {
                pbxPreview.Visible = false;
                lblPreviewCaption.Text =
                    $"Ảnh quá lớn để xem trước ({FormatHelper.FormatSize(fileSize)} > {FormatHelper.FormatSize(MaxPreviewImageBytes)})";
                spcFilesPreview.Panel2Collapsed = true;
                return;
            }

            try
            {
                byte[] bytes = File.ReadAllBytes(path);
                using (var stream = new MemoryStream(bytes))
                {
                    pbxPreview.Image = Image.FromStream(stream);
                }
                pbxPreview.Visible = true;
                lblPreviewCaption.Text = Path.GetFileName(path);
                spcFilesPreview.Panel2Collapsed = false;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException
                || ex is ArgumentException || ex is OutOfMemoryException)
            {
                // Anh bi hong, dang bi khoa boi tien trinh khac, hoac khong du
                // bo nho de giai ma - hien thong bao thay vi de crash preview.
                pbxPreview.Image = null;
                pbxPreview.Visible = false;
                lblPreviewCaption.Text = "Không thể xem trước ảnh này";
                spcFilesPreview.Panel2Collapsed = true;
            }
        }

        /// <summary>
        /// Do encoding cua mot mau byte dau file de chon Encoding phu hop cho
        /// StreamReader, uu tien theo thu tu:
        /// - Co BOM (UTF-8 / UTF-16 LE-BE / UTF-32 LE-BE): dung dung encoding
        ///   tuong ung, StreamReader se tu bo qua cac byte BOM nay khi doc.
        /// - Khong co BOM nhung toan bo mau la UTF-8 hop le (giai ma nghiem
        ///   ngat, throwOnInvalidBytes): coi la UTF-8 khong BOM (kieu file
        ///   van ban pho bien nhat hien nay, VD luu tu VS Code/Notepad mac
        ///   dinh "UTF-8").
        /// - Con lai: coi la "ANSI", dung Encoding.Default, tuc trang ma
        ///   (code page) mac dinh cua he dieu hanh Windows dang chay (VD
        ///   CP1258 tren May tinh cau hinh Tieng Viet, CP1252 tren May tinh
        ///   Tieng Anh) - dung y nghia "ANSI" ma Notepad tren Windows dung.
        ///
        /// Vi mot ky tu UTF-8 co the dai toi 4 byte, mau doc co the bi cat cut
        /// dung giua mot ky tu neu file lon hon kich thuoc mau - ham se thu lai
        /// toi da 3 lan, moi lan bot 1 byte cuoi mau, truoc khi ket luan la
        /// khong phai UTF-8 hop le.
        /// </summary>
        private static Encoding DetectTextEncoding(byte[] sample, int length)
        {
            if (length >= 3 && sample[0] == 0xEF && sample[1] == 0xBB && sample[2] == 0xBF)
                return Encoding.UTF8; // UTF-8 co BOM
            if (length >= 4 && sample[0] == 0x00 && sample[1] == 0x00 && sample[2] == 0xFE && sample[3] == 0xFF)
                return new UTF32Encoding(bigEndian: true, byteOrderMark: true); // UTF-32 BE
            if (length >= 4 && sample[0] == 0xFF && sample[1] == 0xFE && sample[2] == 0x00 && sample[3] == 0x00)
                return new UTF32Encoding(bigEndian: false, byteOrderMark: true); // UTF-32 LE
            if (length >= 2 && sample[0] == 0xFF && sample[1] == 0xFE)
                return Encoding.Unicode; // UTF-16 LE
            if (length >= 2 && sample[0] == 0xFE && sample[1] == 0xFF)
                return Encoding.BigEndianUnicode; // UTF-16 BE

            var strictUtf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);
            for (int trim = 0; trim <= 3 && trim <= length; trim++)
            {
                try
                {
                    strictUtf8.GetString(sample, 0, length - trim);
                    return new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                }
                catch (DecoderFallbackException)
                {
                    // Co the do ky tu UTF-8 nhieu byte bi cat cut o cuoi mau -
                    // thu bot byte cuoi roi kiem tra lai truoc khi bo cuoc.
                }
            }

            // Khong phai UTF-8 hop le -> coi la van ban ANSI theo trang ma mac
            // dinh cua he thong.
            return Encoding.Default;
        }

        /// <summary>
        /// Cap nhat txtPreview bang cach doc toi da MaxPreviewTextLines dong
        /// dau cua file (them gioi han MaxPreviewTextChars phong khi file
        /// khong xuong dong) - dung StreamReader.ReadLine theo tung dong thay
        /// vi File.ReadAllText/ReadAllLines de KHONG BAO GIO nap ca file vao
        /// RAM, du file van ban do lon toi dau. Mo file voi FileShare.ReadWrite
        /// de van xem duoc preview ngay ca khi file dang duoc ung dung khac
        /// (VD trinh soan thao) mo va ghi. Encoding (UTF-8 co/khong BOM,
        /// UTF-16, hoac ANSI) duoc tu dong do qua DetectTextEncoding truoc khi
        /// doc noi dung, tranh hien thi sai dau tieng Viet khi file la ANSI.
        /// </summary>
        private void UpdateTextPreview(string path)
        {
            pbxPreview.Image?.Dispose();
            pbxPreview.Image = null;
            pbxPreview.Visible = false;

            var sb = new StringBuilder();
            int lineCount = 0;
            bool truncated = false;

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] sample = new byte[Math.Min(65536L, fileStream.Length)];
                    int sampleRead = 0;
                    while (sampleRead < sample.Length)
                    {
                        int read = fileStream.Read(sample, sampleRead, sample.Length - sampleRead);
                        if (read == 0) break;
                        sampleRead += read;
                    }
                    fileStream.Position = 0;

                    Encoding encoding = DetectTextEncoding(sample, sampleRead);
                    using (var reader = new StreamReader(fileStream, encoding, detectEncodingFromByteOrderMarks: false))
                    {
                        string line;
                        while (lineCount < MaxPreviewTextLines && (line = reader.ReadLine()) != null)
                        {
                            if (sb.Length + line.Length > MaxPreviewTextChars)
                            {
                                sb.Append(line, 0, Math.Max(0, MaxPreviewTextChars - sb.Length));
                                truncated = true;
                                break;
                            }

                            sb.AppendLine(line);
                            lineCount++;
                        }

                        if (!truncated && reader.Peek() != -1)
                            truncated = true;
                    }
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException
                || ex is ArgumentException)
            {
                // File dang bi khoa hoan toan boi tien trinh khac, hoac duong
                // dan khong hop le giua luc dang doc - hien thong bao thay vi
                // de loi lam vo preview.
                txtPreview.Visible = false;
                lblPreviewCaption.Text = "Không thể xem trước tệp này";
                spcFilesPreview.Panel2Collapsed = true;
                return;
            }

            if (truncated)
                sb.AppendLine("… (đã rút gọn)");

            txtPreview.Text = sb.ToString();
            txtPreview.Visible = true;
            lblPreviewCaption.Text = Path.GetFileName(path);
            spcFilesPreview.Panel2Collapsed = false;
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
        /// Click vao header cot cua lvwFiles: sap xep lai danh sach theo cot do -
        /// click lai CUNG mot cot se doi chieu tang/giam, giong hanh vi Windows
        /// Explorer. Logic so sanh thuc te nam trong ListViewItemComparer
        /// (_listViewSorter, da gan cho lvwFiles.ListViewItemSorter tu constructor) -
        /// o day chi bao no doi cot/chieu roi goi Sort().
        /// </summary>
        private void lvwFiles_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _listViewSorter.SetSortColumn(e.Column);
            lvwFiles.Sort();
            UpdateColumnSortIndicators();
        }

        /// <summary>
        /// Ten goc (khong co mui ten) cua tung cot tren lvwFiles, dung de dung lai
        /// khi ve/xoa mui ten sap xep (xem UpdateColumnSortIndicators) - tranh phai
        /// tu cat chuoi (VD: Substring bo 2 ky tu cuoi) moi lan doi, de nham lam mat
        /// dan ten cot qua nhieu lan bam.
        /// </summary>
        private static readonly string[] ColumnBaseNames = { "Tên", "Kích thước", "Loại", "Ngày sửa" };

        /// <summary>
        /// Cap nhat lai Text cua ca 4 ColumnHeader tren lvwFiles de hien mui ten chi
        /// chieu sap xep (▲ tang dan, ▼ giam dan) o CUOI ten cot dang duoc dung de
        /// sap xep - cac cot khac chi hien ten goc, khong co mui ten. WinForms
        /// ListView khong co API rieng de ve mui ten header (khac DataGridView) nen
        /// dung cach don gian la doi thang Text cua ColumnHeader, thay vi phai
        /// P/Invoke Win32 (SendMessage + HDF_SORTUP/HDF_SORTDOWN) de he thong tu ve.
        /// Goi tu lvwFiles_ColumnClick moi lan doi cot/chieu sap xep.
        /// </summary>
        private void UpdateColumnSortIndicators()
        {
            var columns = new[] { colName, colSize, colType, colModified };

            for (int i = 0; i < columns.Length; i++)
            {
                string arrow = i == _listViewSorter.SortColumn
                    ? (_listViewSorter.Descending ? " ▼" : " ▲")
                    : string.Empty;

                columns[i].Text = ColumnBaseNames[i] + arrow;
            }
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
            cmsProperties.Enabled = hasSingleSelection;
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
            // Dien san thu muc hien tai lam thu muc goc, va tu khoa dang go tren
            // thanh cong cu (neu khac chu placeholder "Tim kiem...") lam tu khoa -
            // nguoi dung van doi duoc ca hai truoc khi bam Tim kiem trong SearchForm.
            string initialKeyword = txtSearch.Text == SearchPlaceholderText ? null : txtSearch.Text;

            using (var searchForm = new SearchForm(_currentPath, initialKeyword))
            {
                searchForm.ShowDialog(this);
            }
        }

        private void mnuToolsFindDuplicates_Click(object sender, EventArgs e)
        {
            using (var duplicateForm = new DuplicateForm(_currentPath))
            {
                duplicateForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Thu thap duong dan cua cac muc dang duoc chon trong lvwFiles (ca
        /// file va thu muc), GIU NGUYEN thu tu hien thi trong danh sach (khong
        /// theo thu tu click chon) vi thu tu nay se quyet dinh gia tri token
        /// {n} trong BatchRenameForm, roi mo BatchRenameForm voi danh sach do.
        /// </summary>
        private void mnuToolsBatchRename_Click(object sender, EventArgs e)
        {
            var selectedPaths = new List<string>();
            foreach (ListViewItem item in lvwFiles.Items)
            {
                if (!item.Selected)
                    continue;

                string path = item.Tag as string;
                if (!string.IsNullOrEmpty(path) && (File.Exists(path) || Directory.Exists(path)))
                    selectedPaths.Add(path);
            }

            if (selectedPaths.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một mục để đổi tên hàng loạt.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var batchRenameForm = new BatchRenameForm(selectedPaths))
            {
                batchRenameForm.ShowDialog(this);
            }
        }

        private void mnuToolsRecycleBin_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem noi dung Thung rac, dung RecycleBinService.GetRecycleBinItems.
        }

        private void mnuToolsLogs_Click(object sender, EventArgs e)
        {
            using (var logForm = new LogForm())
            {
                logForm.ShowDialog(this);
            }
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

                    // Nguoi dung co the vua doi WatcherDelayMs - cap nhat lai
                    // Interval cua timer debounce NGAY, tranh phai dong/mo lai
                    // ung dung moi thay doi co hieu luc.
                    _watcherDebounceTimer.Interval = Math.Max(Settings.Default.WatcherDelayMs, 50);

                    // mnuViewRefresh_Click goi RestartFolderMonitoring() ben
                    // trong, tu dong bat/tat theo doi theo AutoRefreshEnabled
                    // moi nhat - khong can tu goi rieng o day.
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

        #region O loc nhanh theo ten trong thu muc hien tai (txtQuickFilter)

        private const string QuickFilterPlaceholderText = "Lọc theo tên...";

        /// <summary>Xoa chu placeholder khi nguoi dung bam vao o loc nhanh.</summary>
        private void txtQuickFilter_Enter(object sender, EventArgs e)
        {
            if (txtQuickFilter.Text == QuickFilterPlaceholderText)
            {
                txtQuickFilter.Text = string.Empty;
                txtQuickFilter.ForeColor = AppTheme.TextPrimary;
            }
        }

        /// <summary>Khoi phuc chu placeholder neu nguoi dung roi o loc nhanh ma khong nhap gi.</summary>
        private void txtQuickFilter_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtQuickFilter.Text))
            {
                txtQuickFilter.Text = QuickFilterPlaceholderText;
                txtQuickFilter.ForeColor = AppTheme.TextSecondary;
            }
        }

        /// <summary>
        /// Loc lai lvwFiles ngay khi noi dung o loc nhanh thay doi - khac voi
        /// txtSearch (phai nhan Enter moi mo SearchForm), o nay loc theo thoi gian
        /// thuc TRONG thu muc hien tai, khong de quy/khong mo form nao ca. Khong
        /// loc gi them khi dang la chinh chu placeholder (VD: TextChanged tu
        /// txtQuickFilter_Leave gan lai placeholder) - MatchesQuickFilter() da tu
        /// xu ly truong hop nay, nhung kiem tra o day de tranh goi LoadListViewFiles()
        /// mot lan thua khi Leave/Enter doi Text (khong thuc su la nguoi dung dang loc).
        /// </summary>
        private void txtQuickFilter_TextChanged(object sender, EventArgs e)
        {
            LoadListViewFiles();
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
            // BeginUpdate/EndUpdate: tam ngung ve lai TreeView trong luc nap toan bo
            // node o dia, tranh nhap nhay va tang toc do khi co nhieu o dia (VD: may
            // gan nhieu o ngoai/o mang).
            trvFolders.BeginUpdate();
            try
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
            finally
            {
                trvFolders.EndUpdate();
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

            string path = node.Tag as string;
            if (string.IsNullOrEmpty(path))
                return;

            // BeginUpdate/EndUpdate: tam ngung ve lai TreeView trong luc thay node
            // "gia" bang danh sach thu muc con thuc su - tranh nhap nhay khi thu muc
            // co nhieu thu muc con (VD: expand C:\Windows\System32). An toan khi goi
            // trong chinh su kien BeforeExpand cua TreeView dang duoc mo rong.
            trvFolders.BeginUpdate();
            try
            {
                node.Nodes.Clear();

                // Dung FolderService.GetSubFolders() thay vi tu Directory.GetDirectories()
                // de tan dung viec loc file/thu muc an co san (includeHidden) va HasSubFolders
                // da duoc tinh san (FolderItemModel.FromDirectoryInfo) - tranh phai goi rieng
                // EnumerateDirectories().Any() cho tung thu muc con o day.
                foreach (FolderItemModel subFolder in _folderService.GetSubFolders(path, _showHiddenItems))
                {
                    var childNode = new TreeNode(subFolder.Name) { Tag = subFolder.FullPath };
                    childNode.ImageKey = "folder";
                    childNode.SelectedImageKey = "folder";

                    // Chi them node "gia" (placeholder) neu thu muc nay thuc su co thu
                    // muc con - tranh hien dau (+) tren thu muc rong (leaf), bam vao se
                    // chi thay mot node trong khong co y nghia.
                    if (subFolder.HasSubFolders)
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
            finally
            {
                trvFolders.EndUpdate();
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
