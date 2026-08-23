namespace FileExplorerApp.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private System.Windows.Forms.MenuStrip mnsMain;

        private System.Windows.Forms.ToolStrip tlsMain;
        private System.Windows.Forms.ToolStripButton tsbBack;
        private System.Windows.Forms.ToolStripButton tsbForward;
        private System.Windows.Forms.ToolStripButton tsbUp;
        private System.Windows.Forms.ToolStripButton tsbRefresh;
        private System.Windows.Forms.ToolStripSeparator tsbSeparator1;
        private System.Windows.Forms.ToolStripButton tsbNewFolder;
        private System.Windows.Forms.ToolStripSeparator tsbSeparator2;
        private System.Windows.Forms.ToolStripButton tsbCopy;
        private System.Windows.Forms.ToolStripButton tsbPaste;
        private System.Windows.Forms.ToolStripSeparator tsbSeparator3;
        private System.Windows.Forms.ToolStripButton tsbDelete;

        private System.Windows.Forms.Panel pnlAddressBar;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.TreeView trvFolders;
        private System.Windows.Forms.ListView lvwFiles;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.ColumnHeader colModified;

        private System.Windows.Forms.StatusStrip stsMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslItemCount;
        private System.Windows.Forms.ToolStripStatusLabel tsslTotalSize;
        private System.Windows.Forms.ToolStripProgressBar tspProgress;

        private System.Windows.Forms.ContextMenuStrip cmsListView;
        private System.Windows.Forms.ToolStripMenuItem cmsOpen;
        private System.Windows.Forms.ToolStripSeparator cmsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem cmsCut;
        private System.Windows.Forms.ToolStripMenuItem cmsCopy;
        private System.Windows.Forms.ToolStripMenuItem cmsPaste;
        private System.Windows.Forms.ToolStripSeparator cmsSeparator2;
        private System.Windows.Forms.ToolStripMenuItem cmsDelete;
        private System.Windows.Forms.ToolStripMenuItem cmsRename;
        private System.Windows.Forms.ToolStripSeparator cmsSeparator3;
        private System.Windows.Forms.ToolStripMenuItem cmsNewFolder;
        private System.Windows.Forms.ToolStripMenuItem cmsRefresh;

        private System.Windows.Forms.ImageList imlIcons;

        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNewFolder;
        private System.Windows.Forms.ToolStripMenuItem mnuFileNewFile;
        private System.Windows.Forms.ToolStripSeparator mnuFileSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;

        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCut;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        private System.Windows.Forms.ToolStripMenuItem mnuEditPaste;
        private System.Windows.Forms.ToolStripSeparator mnuEditSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuEditDelete;
        private System.Windows.Forms.ToolStripMenuItem mnuEditRename;
        private System.Windows.Forms.ToolStripSeparator mnuEditSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuEditSelectAll;

        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewRefresh;
        private System.Windows.Forms.ToolStripMenuItem mnuViewShowHidden;
        private System.Windows.Forms.ToolStripSeparator mnuViewSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuViewMode;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeLargeIcon;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeSmallIcon;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeList;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeDetails;

        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsSearch;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsFindDuplicates;
        private System.Windows.Forms.ToolStripSeparator mnuToolsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsRecycleBin;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsLogs;
        private System.Windows.Forms.ToolStripSeparator mnuToolsSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsSettings;

        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mnsMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNewFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNewFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditRename = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewShowHidden = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewMode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeLargeIcon = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeSmallIcon = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeList = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsFindDuplicates = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsRecycleBin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsLogs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tlsMain = new System.Windows.Forms.ToolStrip();
            this.tsbBack = new System.Windows.Forms.ToolStripButton();
            this.tsbForward = new System.Windows.Forms.ToolStripButton();
            this.tsbUp = new System.Windows.Forms.ToolStripButton();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.tsbSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbNewFolder = new System.Windows.Forms.ToolStripButton();
            this.tsbSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbCopy = new System.Windows.Forms.ToolStripButton();
            this.tsbPaste = new System.Windows.Forms.ToolStripButton();
            this.tsbSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbDelete = new System.Windows.Forms.ToolStripButton();
            this.pnlAddressBar = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnUp = new System.Windows.Forms.Button();
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.trvFolders = new System.Windows.Forms.TreeView();
            this.imlIcons = new System.Windows.Forms.ImageList(this.components);
            this.lvwFiles = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSize = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colModified = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsListView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cmsOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsCut = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsRename = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.cmsNewFolder = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslItemCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslTotalSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.mnsMain.SuspendLayout();
            this.tlsMain.SuspendLayout();
            this.pnlAddressBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.cmsListView.SuspendLayout();
            this.stsMain.SuspendLayout();
            this.SuspendLayout();
            //
            // mnsMain
            //
            this.mnsMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.mnsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuTools,
            this.mnuHelp});
            this.mnsMain.Location = new System.Drawing.Point(0, 0);
            this.mnsMain.Name = "mnsMain";
            this.mnsMain.Size = new System.Drawing.Size(1200, 28);
            this.mnsMain.TabIndex = 0;
            this.mnsMain.Text = "mnsMain";
            //
            // mnuFile
            //
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileNewFolder,
            this.mnuFileNewFile,
            this.mnuFileSeparator1,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(48, 24);
            this.mnuFile.Text = "&Tệp";
            //
            // mnuFileNewFolder
            //
            this.mnuFileNewFolder.Name = "mnuFileNewFolder";
            this.mnuFileNewFolder.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
            | System.Windows.Forms.Keys.N)));
            this.mnuFileNewFolder.Size = new System.Drawing.Size(298, 26);
            this.mnuFileNewFolder.Text = "Tạo &thư mục mới";
            this.mnuFileNewFolder.Click += new System.EventHandler(this.mnuFileNewFolder_Click);
            //
            // mnuFileNewFile
            //
            this.mnuFileNewFile.Name = "mnuFileNewFile";
            this.mnuFileNewFile.Size = new System.Drawing.Size(298, 26);
            this.mnuFileNewFile.Text = "Tạo &file mới";
            this.mnuFileNewFile.Click += new System.EventHandler(this.mnuFileNewFile_Click);
            //
            // mnuFileSeparator1
            //
            this.mnuFileSeparator1.Name = "mnuFileSeparator1";
            this.mnuFileSeparator1.Size = new System.Drawing.Size(295, 6);
            //
            // mnuFileExit
            //
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.mnuFileExit.Size = new System.Drawing.Size(298, 26);
            this.mnuFileExit.Text = "&Thoát";
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
            //
            // mnuEdit
            //
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditCut,
            this.mnuEditCopy,
            this.mnuEditPaste,
            this.mnuEditSeparator1,
            this.mnuEditDelete,
            this.mnuEditRename,
            this.mnuEditSeparator2,
            this.mnuEditSelectAll});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(87, 24);
            this.mnuEdit.Text = "&Chỉnh sửa";
            //
            // mnuEditCut
            //
            this.mnuEditCut.Name = "mnuEditCut";
            this.mnuEditCut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X)));
            this.mnuEditCut.Size = new System.Drawing.Size(219, 26);
            this.mnuEditCut.Text = "&Cắt";
            this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);
            //
            // mnuEditCopy
            //
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuEditCopy.Size = new System.Drawing.Size(219, 26);
            this.mnuEditCopy.Text = "&Sao chép";
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            //
            // mnuEditPaste
            //
            this.mnuEditPaste.Name = "mnuEditPaste";
            this.mnuEditPaste.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.mnuEditPaste.Size = new System.Drawing.Size(219, 26);
            this.mnuEditPaste.Text = "&Dán";
            this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
            //
            // mnuEditSeparator1
            //
            this.mnuEditSeparator1.Name = "mnuEditSeparator1";
            this.mnuEditSeparator1.Size = new System.Drawing.Size(216, 6);
            //
            // mnuEditDelete
            //
            this.mnuEditDelete.Name = "mnuEditDelete";
            this.mnuEditDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mnuEditDelete.Size = new System.Drawing.Size(219, 26);
            this.mnuEditDelete.Text = "&Xóa";
            this.mnuEditDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);
            //
            // mnuEditRename
            //
            this.mnuEditRename.Name = "mnuEditRename";
            this.mnuEditRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnuEditRename.Size = new System.Drawing.Size(219, 26);
            this.mnuEditRename.Text = "Đổi &tên";
            this.mnuEditRename.Click += new System.EventHandler(this.mnuEditRename_Click);
            //
            // mnuEditSeparator2
            //
            this.mnuEditSeparator2.Name = "mnuEditSeparator2";
            this.mnuEditSeparator2.Size = new System.Drawing.Size(216, 6);
            //
            // mnuEditSelectAll
            //
            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuEditSelectAll.Size = new System.Drawing.Size(219, 26);
            this.mnuEditSelectAll.Text = "Chọn tất &cả";
            this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
            //
            // mnuView
            //
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewRefresh,
            this.mnuViewShowHidden,
            this.mnuViewSeparator1,
            this.mnuViewMode});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(53, 24);
            this.mnuView.Text = "&Xem";
            //
            // mnuViewRefresh
            //
            this.mnuViewRefresh.Name = "mnuViewRefresh";
            this.mnuViewRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuViewRefresh.Size = new System.Drawing.Size(228, 26);
            this.mnuViewRefresh.Text = "&Làm mới";
            this.mnuViewRefresh.Click += new System.EventHandler(this.mnuViewRefresh_Click);
            //
            // mnuViewShowHidden
            //
            this.mnuViewShowHidden.CheckOnClick = true;
            this.mnuViewShowHidden.Name = "mnuViewShowHidden";
            this.mnuViewShowHidden.Size = new System.Drawing.Size(228, 26);
            this.mnuViewShowHidden.Text = "Hiện file/thư mục ẩn";
            this.mnuViewShowHidden.Click += new System.EventHandler(this.mnuViewShowHidden_Click);
            //
            // mnuViewSeparator1
            //
            this.mnuViewSeparator1.Name = "mnuViewSeparator1";
            this.mnuViewSeparator1.Size = new System.Drawing.Size(225, 6);
            //
            // mnuViewMode
            //
            this.mnuViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewModeLargeIcon,
            this.mnuViewModeSmallIcon,
            this.mnuViewModeList,
            this.mnuViewModeDetails});
            this.mnuViewMode.Name = "mnuViewMode";
            this.mnuViewMode.Size = new System.Drawing.Size(228, 26);
            this.mnuViewMode.Text = "Chế độ xem";
            //
            // mnuViewModeLargeIcon
            //
            this.mnuViewModeLargeIcon.Name = "mnuViewModeLargeIcon";
            this.mnuViewModeLargeIcon.Size = new System.Drawing.Size(194, 26);
            this.mnuViewModeLargeIcon.Text = "Biểu tượng lớn";
            this.mnuViewModeLargeIcon.Click += new System.EventHandler(this.mnuViewModeLargeIcon_Click);
            //
            // mnuViewModeSmallIcon
            //
            this.mnuViewModeSmallIcon.Name = "mnuViewModeSmallIcon";
            this.mnuViewModeSmallIcon.Size = new System.Drawing.Size(194, 26);
            this.mnuViewModeSmallIcon.Text = "Biểu tượng nhỏ";
            this.mnuViewModeSmallIcon.Click += new System.EventHandler(this.mnuViewModeSmallIcon_Click);
            //
            // mnuViewModeList
            //
            this.mnuViewModeList.Name = "mnuViewModeList";
            this.mnuViewModeList.Size = new System.Drawing.Size(194, 26);
            this.mnuViewModeList.Text = "Danh sách";
            this.mnuViewModeList.Click += new System.EventHandler(this.mnuViewModeList_Click);
            //
            // mnuViewModeDetails
            //
            this.mnuViewModeDetails.Checked = true;
            this.mnuViewModeDetails.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mnuViewModeDetails.Name = "mnuViewModeDetails";
            this.mnuViewModeDetails.Size = new System.Drawing.Size(194, 26);
            this.mnuViewModeDetails.Text = "Chi tiết";
            this.mnuViewModeDetails.Click += new System.EventHandler(this.mnuViewModeDetails_Click);
            //
            // mnuTools
            //
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsSearch,
            this.mnuToolsFindDuplicates,
            this.mnuToolsSeparator1,
            this.mnuToolsRecycleBin,
            this.mnuToolsLogs,
            this.mnuToolsSeparator2,
            this.mnuToolsSettings});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(77, 24);
            this.mnuTools.Text = "&Công cụ";
            //
            // mnuToolsSearch
            //
            this.mnuToolsSearch.Name = "mnuToolsSearch";
            this.mnuToolsSearch.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mnuToolsSearch.Size = new System.Drawing.Size(246, 26);
            this.mnuToolsSearch.Text = "&Tìm kiếm...";
            this.mnuToolsSearch.Click += new System.EventHandler(this.mnuToolsSearch_Click);
            //
            // mnuToolsFindDuplicates
            //
            this.mnuToolsFindDuplicates.Name = "mnuToolsFindDuplicates";
            this.mnuToolsFindDuplicates.Size = new System.Drawing.Size(246, 26);
            this.mnuToolsFindDuplicates.Text = "Tìm file &trùng lặp...";
            this.mnuToolsFindDuplicates.Click += new System.EventHandler(this.mnuToolsFindDuplicates_Click);
            //
            // mnuToolsSeparator1
            //
            this.mnuToolsSeparator1.Name = "mnuToolsSeparator1";
            this.mnuToolsSeparator1.Size = new System.Drawing.Size(243, 6);
            //
            // mnuToolsRecycleBin
            //
            this.mnuToolsRecycleBin.Name = "mnuToolsRecycleBin";
            this.mnuToolsRecycleBin.Size = new System.Drawing.Size(246, 26);
            this.mnuToolsRecycleBin.Text = "Thùng &rác";
            this.mnuToolsRecycleBin.Click += new System.EventHandler(this.mnuToolsRecycleBin_Click);
            //
            // mnuToolsLogs
            //
            this.mnuToolsLogs.Name = "mnuToolsLogs";
            this.mnuToolsLogs.Size = new System.Drawing.Size(246, 26);
            this.mnuToolsLogs.Text = "&Xem nhật ký hoạt động";
            this.mnuToolsLogs.Click += new System.EventHandler(this.mnuToolsLogs_Click);
            //
            // mnuToolsSeparator2
            //
            this.mnuToolsSeparator2.Name = "mnuToolsSeparator2";
            this.mnuToolsSeparator2.Size = new System.Drawing.Size(243, 6);
            //
            // mnuToolsSettings
            //
            this.mnuToolsSettings.Name = "mnuToolsSettings";
            this.mnuToolsSettings.Size = new System.Drawing.Size(246, 26);
            this.mnuToolsSettings.Text = "&Cài đặt...";
            this.mnuToolsSettings.Click += new System.EventHandler(this.mnuToolsSettings_Click);
            //
            // mnuHelp
            //
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(78, 24);
            this.mnuHelp.Text = "Trợ &giúp";
            //
            // mnuHelpAbout
            //
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(165, 26);
            this.mnuHelpAbout.Text = "&Giới thiệu...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            //
            // tlsMain
            //
            this.tlsMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.tlsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tlsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbBack,
            this.tsbForward,
            this.tsbUp,
            this.tsbRefresh,
            this.tsbSeparator1,
            this.tsbNewFolder,
            this.tsbSeparator2,
            this.tsbCopy,
            this.tsbPaste,
            this.tsbSeparator3,
            this.tsbDelete});
            this.tlsMain.Location = new System.Drawing.Point(0, 28);
            this.tlsMain.Name = "tlsMain";
            this.tlsMain.Size = new System.Drawing.Size(1200, 58);
            this.tlsMain.TabIndex = 1;
            this.tlsMain.Text = "tlsMain";
            //
            // tsbBack
            //
            this.tsbBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbBack.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbBack.Name = "tsbBack";
            this.tsbBack.Size = new System.Drawing.Size(54, 55);
            this.tsbBack.Text = "←\r\nBack";
            this.tsbBack.ToolTipText = "Quay lại thư mục trước";
            this.tsbBack.Click += new System.EventHandler(this.tsbBack_Click);
            //
            // tsbForward
            //
            this.tsbForward.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbForward.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbForward.Name = "tsbForward";
            this.tsbForward.Size = new System.Drawing.Size(64, 55);
            this.tsbForward.Text = "→\r\nForward";
            this.tsbForward.ToolTipText = "Đi tới thư mục vừa quay lại";
            this.tsbForward.Click += new System.EventHandler(this.tsbForward_Click);
            //
            // tsbUp
            //
            this.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbUp.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbUp.Name = "tsbUp";
            this.tsbUp.Size = new System.Drawing.Size(38, 55);
            this.tsbUp.Text = "↑\r\nUp";
            this.tsbUp.ToolTipText = "Lên thư mục cha";
            this.tsbUp.Click += new System.EventHandler(this.tsbUp_Click);
            //
            // tsbRefresh
            //
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(63, 55);
            this.tsbRefresh.Text = "⟳\r\nRefresh";
            this.tsbRefresh.ToolTipText = "Làm mới (F5)";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);
            //
            // tsbSeparator1
            //
            this.tsbSeparator1.Name = "tsbSeparator1";
            this.tsbSeparator1.Size = new System.Drawing.Size(6, 58);
            //
            // tsbNewFolder
            //
            this.tsbNewFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbNewFolder.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbNewFolder.Name = "tsbNewFolder";
            this.tsbNewFolder.Size = new System.Drawing.Size(76, 55);
            this.tsbNewFolder.Text = "+\r\nNew Folder";
            this.tsbNewFolder.ToolTipText = "Tạo thư mục mới";
            this.tsbNewFolder.Click += new System.EventHandler(this.tsbNewFolder_Click);
            //
            // tsbSeparator2
            //
            this.tsbSeparator2.Name = "tsbSeparator2";
            this.tsbSeparator2.Size = new System.Drawing.Size(6, 58);
            //
            // tsbCopy
            //
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbCopy.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.Size = new System.Drawing.Size(49, 55);
            this.tsbCopy.Text = "⧉\r\nCopy";
            this.tsbCopy.ToolTipText = "Sao chép (Ctrl+C)";
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);
            //
            // tsbPaste
            //
            this.tsbPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbPaste.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbPaste.Name = "tsbPaste";
            this.tsbPaste.Size = new System.Drawing.Size(48, 55);
            this.tsbPaste.Text = "▤\r\nPaste";
            this.tsbPaste.ToolTipText = "Dán (Ctrl+V)";
            this.tsbPaste.Click += new System.EventHandler(this.tsbPaste_Click);
            //
            // tsbSeparator3
            //
            this.tsbSeparator3.Name = "tsbSeparator3";
            this.tsbSeparator3.Size = new System.Drawing.Size(6, 58);
            //
            // tsbDelete
            //
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.tsbDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.Size = new System.Drawing.Size(52, 55);
            this.tsbDelete.Text = "✕\r\nDelete";
            this.tsbDelete.ToolTipText = "Xóa (Del)";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);
            //
            // pnlAddressBar
            //
            this.pnlAddressBar.Controls.Add(this.txtSearch);
            this.pnlAddressBar.Controls.Add(this.btnGo);
            this.pnlAddressBar.Controls.Add(this.txtPath);
            this.pnlAddressBar.Controls.Add(this.btnUp);
            this.pnlAddressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddressBar.Location = new System.Drawing.Point(0, 86);
            this.pnlAddressBar.Name = "pnlAddressBar";
            this.pnlAddressBar.Padding = new System.Windows.Forms.Padding(8, 6, 8, 6);
            this.pnlAddressBar.Size = new System.Drawing.Size(1200, 40);
            this.pnlAddressBar.TabIndex = 2;
            //
            // txtSearch
            //
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtSearch.Location = new System.Drawing.Point(892, 6);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(300, 26);
            this.txtSearch.TabIndex = 3;
            this.txtSearch.Text = "Tìm kiếm...";
            this.txtSearch.ForeColor = System.Drawing.SystemColors.GrayText;
            this.txtSearch.Enter += new System.EventHandler(this.txtSearch_Enter);
            this.txtSearch.Leave += new System.EventHandler(this.txtSearch_Leave);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            //
            // btnGo
            //
            this.btnGo.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnGo.Location = new System.Drawing.Point(832, 6);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(60, 26);
            this.btnGo.TabIndex = 2;
            this.btnGo.Text = "▶";
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            //
            // txtPath
            //
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.Location = new System.Drawing.Point(38, 6);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(794, 26);
            this.txtPath.TabIndex = 1;
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyDown);
            //
            // btnUp
            //
            this.btnUp.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnUp.Location = new System.Drawing.Point(8, 6);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(30, 26);
            this.btnUp.TabIndex = 0;
            this.btnUp.Text = "▲";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            //
            // spcMain
            //
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.Location = new System.Drawing.Point(0, 126);
            this.spcMain.Name = "spcMain";
            //
            // spcMain.Panel1
            //
            this.spcMain.Panel1.Controls.Add(this.trvFolders);
            this.spcMain.Panel1MinSize = 120;
            //
            // spcMain.Panel2
            //
            this.spcMain.Panel2.Controls.Add(this.lvwFiles);
            this.spcMain.Panel2MinSize = 200;
            this.spcMain.Size = new System.Drawing.Size(1200, 544);
            this.spcMain.SplitterDistance = 300;
            this.spcMain.TabIndex = 3;
            //
            // trvFolders
            //
            this.trvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvFolders.HideSelection = false;
            this.trvFolders.ImageKey = "folder";
            this.trvFolders.ImageList = this.imlIcons;
            this.trvFolders.Location = new System.Drawing.Point(0, 0);
            this.trvFolders.Name = "trvFolders";
            this.trvFolders.SelectedImageKey = "folder";
            this.trvFolders.Size = new System.Drawing.Size(300, 544);
            this.trvFolders.TabIndex = 0;
            this.trvFolders.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvFolders_BeforeExpand);
            this.trvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvFolders_AfterSelect);
            //
            // imlIcons
            //
            this.imlIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imlIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.imlIcons.TransparentColor = System.Drawing.Color.Transparent;
            //
            // lvwFiles
            //
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSize,
            this.colType,
            this.colModified});
            this.lvwFiles.ContextMenuStrip = this.cmsListView;
            this.lvwFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwFiles.FullRowSelect = true;
            this.lvwFiles.GridLines = false;
            this.lvwFiles.HideSelection = false;
            this.lvwFiles.Location = new System.Drawing.Point(0, 0);
            this.lvwFiles.Name = "lvwFiles";
            this.lvwFiles.Size = new System.Drawing.Size(896, 544);
            this.lvwFiles.SmallImageList = this.imlIcons;
            this.lvwFiles.TabIndex = 0;
            this.lvwFiles.UseCompatibleStateImageBehavior = false;
            this.lvwFiles.View = System.Windows.Forms.View.Details;
            this.lvwFiles.SelectedIndexChanged += new System.EventHandler(this.lvwFiles_SelectedIndexChanged);
            this.lvwFiles.DoubleClick += new System.EventHandler(this.lvwFiles_DoubleClick);
            //
            // colName
            //
            this.colName.Text = "Tên";
            this.colName.Width = 380;
            //
            // colSize
            //
            this.colSize.Text = "Kích thước";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 120;
            //
            // colType
            //
            this.colType.Text = "Loại";
            this.colType.Width = 160;
            //
            // colModified
            //
            this.colModified.Text = "Ngày sửa";
            this.colModified.Width = 200;
            //
            // cmsListView
            //
            this.cmsListView.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsListView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsOpen,
            this.cmsSeparator1,
            this.cmsCut,
            this.cmsCopy,
            this.cmsPaste,
            this.cmsSeparator2,
            this.cmsDelete,
            this.cmsRename,
            this.cmsSeparator3,
            this.cmsNewFolder,
            this.cmsRefresh});
            this.cmsListView.Name = "cmsListView";
            this.cmsListView.Size = new System.Drawing.Size(192, 214);
            this.cmsListView.Opening += new System.ComponentModel.CancelEventHandler(this.cmsListView_Opening);
            //
            // cmsOpen
            //
            this.cmsOpen.Name = "cmsOpen";
            this.cmsOpen.Size = new System.Drawing.Size(191, 24);
            this.cmsOpen.Text = "&Mở";
            this.cmsOpen.Click += new System.EventHandler(this.cmsOpen_Click);
            //
            // cmsSeparator1
            //
            this.cmsSeparator1.Name = "cmsSeparator1";
            this.cmsSeparator1.Size = new System.Drawing.Size(188, 6);
            //
            // cmsCut
            //
            this.cmsCut.Name = "cmsCut";
            this.cmsCut.Size = new System.Drawing.Size(191, 24);
            this.cmsCut.Text = "&Cắt";
            this.cmsCut.Click += new System.EventHandler(this.mnuEditCut_Click);
            //
            // cmsCopy
            //
            this.cmsCopy.Name = "cmsCopy";
            this.cmsCopy.Size = new System.Drawing.Size(191, 24);
            this.cmsCopy.Text = "Sao &chép";
            this.cmsCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            //
            // cmsPaste
            //
            this.cmsPaste.Name = "cmsPaste";
            this.cmsPaste.Size = new System.Drawing.Size(191, 24);
            this.cmsPaste.Text = "&Dán";
            this.cmsPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);
            //
            // cmsSeparator2
            //
            this.cmsSeparator2.Name = "cmsSeparator2";
            this.cmsSeparator2.Size = new System.Drawing.Size(188, 6);
            //
            // cmsDelete
            //
            this.cmsDelete.Name = "cmsDelete";
            this.cmsDelete.Size = new System.Drawing.Size(191, 24);
            this.cmsDelete.Text = "&Xóa";
            this.cmsDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);
            //
            // cmsRename
            //
            this.cmsRename.Name = "cmsRename";
            this.cmsRename.Size = new System.Drawing.Size(191, 24);
            this.cmsRename.Text = "Đổi &tên";
            this.cmsRename.Click += new System.EventHandler(this.mnuEditRename_Click);
            //
            // cmsSeparator3
            //
            this.cmsSeparator3.Name = "cmsSeparator3";
            this.cmsSeparator3.Size = new System.Drawing.Size(188, 6);
            //
            // cmsNewFolder
            //
            this.cmsNewFolder.Name = "cmsNewFolder";
            this.cmsNewFolder.Size = new System.Drawing.Size(191, 24);
            this.cmsNewFolder.Text = "Tạo thư mục &mới";
            this.cmsNewFolder.Click += new System.EventHandler(this.mnuFileNewFolder_Click);
            //
            // cmsRefresh
            //
            this.cmsRefresh.Name = "cmsRefresh";
            this.cmsRefresh.Size = new System.Drawing.Size(191, 24);
            this.cmsRefresh.Text = "&Làm mới";
            this.cmsRefresh.Click += new System.EventHandler(this.mnuViewRefresh_Click);
            //
            // stsMain
            //
            this.stsMain.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.stsMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tspProgress,
            this.tsslItemCount,
            this.tsslTotalSize});
            this.stsMain.Location = new System.Drawing.Point(0, 670);
            this.stsMain.Name = "stsMain";
            this.stsMain.Size = new System.Drawing.Size(1200, 30);
            this.stsMain.TabIndex = 4;
            //
            // tsslStatus
            //
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(956, 24);
            this.tsslStatus.Spring = true;
            this.tsslStatus.Text = "Sẵn sàng";
            this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // tspProgress
            //
            this.tspProgress.Name = "tspProgress";
            this.tspProgress.Size = new System.Drawing.Size(120, 22);
            this.tspProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tspProgress.Visible = false;
            //
            // tsslItemCount
            //
            this.tsslItemCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsslItemCount.Name = "tsslItemCount";
            this.tsslItemCount.Size = new System.Drawing.Size(53, 24);
            this.tsslItemCount.Text = "0 mục";
            //
            // tsslTotalSize
            //
            this.tsslTotalSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsslTotalSize.Name = "tsslTotalSize";
            this.tsslTotalSize.Size = new System.Drawing.Size(54, 24);
            this.tsslTotalSize.Text = "0 byte";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.spcMain);
            this.Controls.Add(this.pnlAddressBar);
            this.Controls.Add(this.tlsMain);
            this.Controls.Add(this.mnsMain);
            this.Controls.Add(this.stsMain);
            this.MainMenuStrip = this.mnsMain;
            this.MinimumSize = new System.Drawing.Size(700, 450);
            this.Name = "MainForm";
            this.Text = "MainForm";
            this.mnsMain.ResumeLayout(false);
            this.mnsMain.PerformLayout();
            this.tlsMain.ResumeLayout(false);
            this.tlsMain.PerformLayout();
            this.pnlAddressBar.ResumeLayout(false);
            this.pnlAddressBar.PerformLayout();
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
            this.cmsListView.ResumeLayout(false);
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
