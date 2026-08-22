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

            this.tlsMain = new System.Windows.Forms.ToolStrip();
            this.tsbBack = new System.Windows.Forms.ToolStripButton();
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
            this.btnUp = new System.Windows.Forms.Button();
            this.btnGo = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();

            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.trvFolders = new System.Windows.Forms.TreeView();
            this.lvwFiles = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.colModified = new System.Windows.Forms.ColumnHeader();

            this.stsMain = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslItemCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslTotalSize = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspProgress = new System.Windows.Forms.ToolStripProgressBar();

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

            this.imlIcons = new System.Windows.Forms.ImageList(this.components);

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

            this.mnsMain.SuspendLayout();
            this.tlsMain.SuspendLayout();
            this.stsMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.Panel2.SuspendLayout();
            this.spcMain.SuspendLayout();
            this.SuspendLayout();

            // 
            // mnsMain
            // 
            this.mnsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFile,
                this.mnuEdit,
                this.mnuView,
                this.mnuTools,
                this.mnuHelp});
            this.mnsMain.Location = new System.Drawing.Point(0, 0);
            this.mnsMain.Name = "mnsMain";
            this.mnsMain.Size = new System.Drawing.Size(800, 24);
            this.mnsMain.TabIndex = 0;
            this.mnsMain.Text = "mnsMain";

            // 
            // mnuFile ("Tệp")
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFileNewFolder,
                this.mnuFileNewFile,
                this.mnuFileSeparator1,
                this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Text = "&Tệp";

            this.mnuFileNewFolder.Name = "mnuFileNewFolder";
            this.mnuFileNewFolder.Text = "Tạo &thư mục mới";
            this.mnuFileNewFolder.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) | System.Windows.Forms.Keys.N));
            this.mnuFileNewFolder.Click += new System.EventHandler(this.mnuFileNewFolder_Click);

            this.mnuFileNewFile.Name = "mnuFileNewFile";
            this.mnuFileNewFile.Text = "Tạo &file mới";
            this.mnuFileNewFile.Click += new System.EventHandler(this.mnuFileNewFile_Click);

            this.mnuFileSeparator1.Name = "mnuFileSeparator1";

            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Text = "&Thoát";
            this.mnuFileExit.ShortcutKeys = System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4;
            this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);

            // 
            // mnuEdit ("Chỉnh sửa")
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
            this.mnuEdit.Text = "&Chỉnh sửa";

            this.mnuEditCut.Name = "mnuEditCut";
            this.mnuEditCut.Text = "&Cắt";
            this.mnuEditCut.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.X;
            this.mnuEditCut.Click += new System.EventHandler(this.mnuEditCut_Click);

            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.Text = "&Sao chép";
            this.mnuEditCopy.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C;
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);

            this.mnuEditPaste.Name = "mnuEditPaste";
            this.mnuEditPaste.Text = "&Dán";
            this.mnuEditPaste.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V;
            this.mnuEditPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);

            this.mnuEditSeparator1.Name = "mnuEditSeparator1";

            this.mnuEditDelete.Name = "mnuEditDelete";
            this.mnuEditDelete.Text = "&Xóa";
            this.mnuEditDelete.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.mnuEditDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);

            this.mnuEditRename.Name = "mnuEditRename";
            this.mnuEditRename.Text = "Đổi &tên";
            this.mnuEditRename.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.mnuEditRename.Click += new System.EventHandler(this.mnuEditRename_Click);

            this.mnuEditSeparator2.Name = "mnuEditSeparator2";

            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.Text = "Chọn tất &cả";
            this.mnuEditSelectAll.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A;
            this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);

            // 
            // mnuView ("Xem")
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuViewRefresh,
                this.mnuViewShowHidden,
                this.mnuViewSeparator1,
                this.mnuViewMode});
            this.mnuView.Name = "mnuView";
            this.mnuView.Text = "&Xem";

            this.mnuViewRefresh.Name = "mnuViewRefresh";
            this.mnuViewRefresh.Text = "&Làm mới";
            this.mnuViewRefresh.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuViewRefresh.Click += new System.EventHandler(this.mnuViewRefresh_Click);

            this.mnuViewShowHidden.Name = "mnuViewShowHidden";
            this.mnuViewShowHidden.Text = "Hiện file/thư mục ẩn";
            this.mnuViewShowHidden.CheckOnClick = true;
            this.mnuViewShowHidden.Click += new System.EventHandler(this.mnuViewShowHidden_Click);

            this.mnuViewSeparator1.Name = "mnuViewSeparator1";

            //
            // mnuViewMode ("Chế độ xem") - submenu chon 1 trong 4 kieu hien thi, dang radio.
            //
            this.mnuViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuViewModeLargeIcon,
                this.mnuViewModeSmallIcon,
                this.mnuViewModeList,
                this.mnuViewModeDetails});
            this.mnuViewMode.Name = "mnuViewMode";
            this.mnuViewMode.Text = "Chế độ xem";

            this.mnuViewModeLargeIcon.Name = "mnuViewModeLargeIcon";
            this.mnuViewModeLargeIcon.Text = "Biểu tượng lớn";
            this.mnuViewModeLargeIcon.Click += new System.EventHandler(this.mnuViewModeLargeIcon_Click);

            this.mnuViewModeSmallIcon.Name = "mnuViewModeSmallIcon";
            this.mnuViewModeSmallIcon.Text = "Biểu tượng nhỏ";
            this.mnuViewModeSmallIcon.Click += new System.EventHandler(this.mnuViewModeSmallIcon_Click);

            this.mnuViewModeList.Name = "mnuViewModeList";
            this.mnuViewModeList.Text = "Danh sách";
            this.mnuViewModeList.Click += new System.EventHandler(this.mnuViewModeList_Click);

            this.mnuViewModeDetails.Name = "mnuViewModeDetails";
            this.mnuViewModeDetails.Text = "Chi tiết";
            this.mnuViewModeDetails.Checked = true; // Mac dinh giong Windows Explorer.
            this.mnuViewModeDetails.Click += new System.EventHandler(this.mnuViewModeDetails_Click);

            // 
            // mnuTools ("Công cụ")
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
            this.mnuTools.Text = "&Công cụ";

            this.mnuToolsSearch.Name = "mnuToolsSearch";
            this.mnuToolsSearch.Text = "&Tìm kiếm...";
            this.mnuToolsSearch.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F;
            this.mnuToolsSearch.Click += new System.EventHandler(this.mnuToolsSearch_Click);

            this.mnuToolsFindDuplicates.Name = "mnuToolsFindDuplicates";
            this.mnuToolsFindDuplicates.Text = "Tìm file &trùng lặp...";
            this.mnuToolsFindDuplicates.Click += new System.EventHandler(this.mnuToolsFindDuplicates_Click);

            this.mnuToolsSeparator1.Name = "mnuToolsSeparator1";

            this.mnuToolsRecycleBin.Name = "mnuToolsRecycleBin";
            this.mnuToolsRecycleBin.Text = "Thùng &rác";
            this.mnuToolsRecycleBin.Click += new System.EventHandler(this.mnuToolsRecycleBin_Click);

            this.mnuToolsLogs.Name = "mnuToolsLogs";
            this.mnuToolsLogs.Text = "&Xem nhật ký hoạt động";
            this.mnuToolsLogs.Click += new System.EventHandler(this.mnuToolsLogs_Click);

            this.mnuToolsSeparator2.Name = "mnuToolsSeparator2";

            this.mnuToolsSettings.Name = "mnuToolsSettings";
            this.mnuToolsSettings.Text = "&Cài đặt...";
            this.mnuToolsSettings.Click += new System.EventHandler(this.mnuToolsSettings_Click);

            // 
            // mnuHelp ("Trợ giúp")
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Text = "Trợ &giúp";

            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Text = "&Giới thiệu...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);

            // 
            // tlsMain
            // 
            this.tlsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsbBack,
                this.tsbUp,
                this.tsbRefresh,
                this.tsbSeparator1,
                this.tsbNewFolder,
                this.tsbSeparator2,
                this.tsbCopy,
                this.tsbPaste,
                this.tsbSeparator3,
                this.tsbDelete});
            this.tlsMain.Location = new System.Drawing.Point(0, 24);
            this.tlsMain.Name = "tlsMain";
            this.tlsMain.Size = new System.Drawing.Size(800, 25);
            this.tlsMain.TabIndex = 1;
            this.tlsMain.Text = "tlsMain";

            this.tsbBack.Name = "tsbBack";
            this.tsbBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbBack.Text = "◄ Back";
            this.tsbBack.ToolTipText = "Quay lại thư mục trước";
            this.tsbBack.Click += new System.EventHandler(this.tsbBack_Click);

            this.tsbUp.Name = "tsbUp";
            this.tsbUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbUp.Text = "▲ Up";
            this.tsbUp.ToolTipText = "Lên thư mục cha";
            this.tsbUp.Click += new System.EventHandler(this.tsbUp_Click);

            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbRefresh.Text = "⟲ Refresh";
            this.tsbRefresh.ToolTipText = "Làm mới (F5)";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click);

            this.tsbSeparator1.Name = "tsbSeparator1";

            this.tsbNewFolder.Name = "tsbNewFolder";
            this.tsbNewFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbNewFolder.Text = "New Folder";
            this.tsbNewFolder.ToolTipText = "Tạo thư mục mới";
            this.tsbNewFolder.Click += new System.EventHandler(this.tsbNewFolder_Click);

            this.tsbSeparator2.Name = "tsbSeparator2";

            this.tsbCopy.Name = "tsbCopy";
            this.tsbCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbCopy.Text = "Copy";
            this.tsbCopy.ToolTipText = "Sao chép (Ctrl+C)";
            this.tsbCopy.Click += new System.EventHandler(this.tsbCopy_Click);

            this.tsbPaste.Name = "tsbPaste";
            this.tsbPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbPaste.Text = "Paste";
            this.tsbPaste.ToolTipText = "Dán (Ctrl+V)";
            this.tsbPaste.Click += new System.EventHandler(this.tsbPaste_Click);

            this.tsbSeparator3.Name = "tsbSeparator3";

            this.tsbDelete.Name = "tsbDelete";
            this.tsbDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbDelete.Text = "Delete";
            this.tsbDelete.ToolTipText = "Xóa (Del)";
            this.tsbDelete.Click += new System.EventHandler(this.tsbDelete_Click);

            // 
            // pnlAddressBar (thanh dia chi: Up + txtPath + Go)
            // 
            this.pnlAddressBar.Controls.Add(this.btnUp);
            this.pnlAddressBar.Controls.Add(this.btnGo);
            this.pnlAddressBar.Controls.Add(this.txtPath);
            this.pnlAddressBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlAddressBar.Location = new System.Drawing.Point(0, 49);
            this.pnlAddressBar.Name = "pnlAddressBar";
            this.pnlAddressBar.Padding = new System.Windows.Forms.Padding(2);
            this.pnlAddressBar.Size = new System.Drawing.Size(800, 28);
            this.pnlAddressBar.TabIndex = 2;

            this.btnUp.Name = "btnUp";
            this.btnUp.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnUp.Width = 32;
            this.btnUp.Text = "▲";
            this.btnUp.TabIndex = 0;
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);

            this.btnGo.Name = "btnGo";
            this.btnGo.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnGo.Width = 60;
            this.btnGo.Text = "Go";
            this.btnGo.TabIndex = 2;
            this.btnGo.UseVisualStyleBackColor = true;
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);

            this.txtPath.Name = "txtPath";
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPath.TabIndex = 1;
            this.txtPath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPath_KeyDown);

            // 
            // spcMain (chia 2 vung lam viec: Panel1 = cay thu muc, Panel2 = danh sach file)
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.Location = new System.Drawing.Point(0, 77);
            this.spcMain.Name = "spcMain";
            this.spcMain.Size = new System.Drawing.Size(800, 373);
            this.spcMain.SplitterDistance = 220;
            this.spcMain.SplitterWidth = 4;
            // FixedPanel = None (mac dinh): khi form resize, ca 2 panel co gian ti le
            // voi nhau thay vi chi Panel2 (phai) gianh het phan them/bot - giu duoc ty le
            // hop ly giua cay thu muc va danh sach file khi phong to/thu nho cua so.
            this.spcMain.Panel1MinSize = 120; // Du rong toi thieu de thay ten thu muc trong trvFolders.
            this.spcMain.Panel2MinSize = 200; // Du rong toi thieu de con thay it nhat vai cot cua lvwFiles.
            this.spcMain.TabIndex = 3;

            // Panel1 (trai) - chua trvFolders (cay thu muc).
            this.spcMain.Panel1.Controls.Add(this.trvFolders);
            this.spcMain.Panel1.Name = "spcMain.Panel1";

            // Panel2 (phai) - chua lvwFiles (danh sach file/thu muc).
            this.spcMain.Panel2.Controls.Add(this.lvwFiles);
            this.spcMain.Panel2.Name = "spcMain.Panel2";

            //
            // trvFolders (cay thu muc ben trai)
            //
            this.trvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvFolders.Name = "trvFolders";
            this.trvFolders.HideSelection = false;
            this.trvFolders.PathSeparator = System.IO.Path.DirectorySeparatorChar.ToString();
            this.trvFolders.TabIndex = 0;
            this.trvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvFolders_AfterSelect);
            this.trvFolders.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.trvFolders_BeforeExpand);
            this.trvFolders.ImageList = this.imlIcons;
            this.trvFolders.ImageKey = "folder";
            this.trvFolders.SelectedImageKey = "folder";

            //
            // lvwFiles (danh sach file/thu muc ben phai, che do Details)
            //
            this.lvwFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colName,
                this.colSize,
                this.colType,
                this.colModified});
            this.lvwFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwFiles.FullRowSelect = true;
            this.lvwFiles.GridLines = true;
            this.lvwFiles.HideSelection = false;
            this.lvwFiles.MultiSelect = true;
            this.lvwFiles.Name = "lvwFiles";
            this.lvwFiles.TabIndex = 0;
            this.lvwFiles.UseCompatibleStateImageBehavior = false;
            this.lvwFiles.View = System.Windows.Forms.View.Details;
            this.lvwFiles.ContextMenuStrip = this.cmsListView;
            this.lvwFiles.SmallImageList = this.imlIcons;
            this.lvwFiles.DoubleClick += new System.EventHandler(this.lvwFiles_DoubleClick);
            this.lvwFiles.SelectedIndexChanged += new System.EventHandler(this.lvwFiles_SelectedIndexChanged);

            this.colName.Text = "Tên";
            this.colName.Width = 240;

            this.colSize.Text = "Kích thước";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 100;

            this.colType.Text = "Loại";
            this.colType.Width = 130;

            this.colModified.Text = "Ngày sửa đổi";
            this.colModified.Width = 140;

            //
            // cmsListView (menu chuot phai tren lvwFiles)
            //
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
            this.cmsListView.Opening += new System.ComponentModel.CancelEventHandler(this.cmsListView_Opening);

            this.cmsOpen.Name = "cmsOpen";
            this.cmsOpen.Text = "&Mở";
            this.cmsOpen.Click += new System.EventHandler(this.cmsOpen_Click);

            this.cmsSeparator1.Name = "cmsSeparator1";

            this.cmsCut.Name = "cmsCut";
            this.cmsCut.Text = "&Cắt";
            this.cmsCut.Click += new System.EventHandler(this.mnuEditCut_Click);

            this.cmsCopy.Name = "cmsCopy";
            this.cmsCopy.Text = "Sao &chép";
            this.cmsCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);

            this.cmsPaste.Name = "cmsPaste";
            this.cmsPaste.Text = "&Dán";
            this.cmsPaste.Click += new System.EventHandler(this.mnuEditPaste_Click);

            this.cmsSeparator2.Name = "cmsSeparator2";

            this.cmsDelete.Name = "cmsDelete";
            this.cmsDelete.Text = "&Xóa";
            this.cmsDelete.Click += new System.EventHandler(this.mnuEditDelete_Click);

            this.cmsRename.Name = "cmsRename";
            this.cmsRename.Text = "Đổi &tên";
            this.cmsRename.Click += new System.EventHandler(this.mnuEditRename_Click);

            this.cmsSeparator3.Name = "cmsSeparator3";

            this.cmsNewFolder.Name = "cmsNewFolder";
            this.cmsNewFolder.Text = "Tạo thư mục &mới";
            this.cmsNewFolder.Click += new System.EventHandler(this.mnuFileNewFolder_Click);

            this.cmsRefresh.Name = "cmsRefresh";
            this.cmsRefresh.Text = "&Làm mới";
            this.cmsRefresh.Click += new System.EventHandler(this.mnuViewRefresh_Click);

            //
            // imlIcons (icon "folder"/"file" dung cho trvFolders va lvwFiles).
            // Anh thuc te duoc ve dong luc bang code (LoadIconImages trong MainForm.cs),
            // khong nhung san file .ico/.png de tranh phai quan ly them tai nguyen nhi phan.
            //
            this.imlIcons.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imlIcons.ImageSize = new System.Drawing.Size(16, 16);
            this.imlIcons.TransparentColor = System.Drawing.Color.Transparent;

            //
            // stsMain (so muc, dung luong, trang thai o duoi cung cua so)
            //
            this.stsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsslStatus,
                this.tspProgress,
                this.tsslItemCount,
                this.tsslTotalSize});
            this.stsMain.Name = "stsMain";
            this.stsMain.TabIndex = 4;

            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Text = "Sẵn sàng";
            this.tsslStatus.Spring = true; // Choan het khoang trong con lai, day 2 nhan sau ve ben phai.
            this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.tspProgress.Name = "tspProgress";
            this.tspProgress.Size = new System.Drawing.Size(120, 16);
            this.tspProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tspProgress.Visible = false; // Mac dinh an, chi hien khi co thao tac dang chay (VD: sao chep, tim kiem).

            this.tsslItemCount.Name = "tsslItemCount";
            this.tsslItemCount.Text = "0 mục";
            this.tsslItemCount.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsslItemCount.AutoSize = true;

            this.tsslTotalSize.Name = "tsslTotalSize";
            this.tsslTotalSize.Text = "0 byte";
            this.tsslTotalSize.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsslTotalSize.AutoSize = true;

            //
            // MainForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            // Gioi han kich thuoc toi thieu de mnsMain/tlsMain/pnlAddressBar/stsMain (cao co dinh,
            // Dock=Top/Bottom) va Panel1MinSize/Panel2MinSize cua spcMain luon co du cho de hien
            // thi dung, tranh cac control chong len nhau hoac spcMain bi am kich thuoc khi keo
            // form qua nho.
            this.MinimumSize = new System.Drawing.Size(500, 350);
            this.Controls.Add(this.mnsMain);
            this.Controls.Add(this.tlsMain);
            this.Controls.Add(this.pnlAddressBar);
            this.Controls.Add(this.spcMain);
            this.Controls.Add(this.stsMain);
            this.MainMenuStrip = this.mnsMain;
            this.Name = "MainForm";
            this.Text = "MainForm";

            this.mnsMain.ResumeLayout(false);
            this.tlsMain.ResumeLayout(false);
            this.tlsMain.PerformLayout();
            this.pnlAddressBar.ResumeLayout(false);
            this.spcMain.Panel1.ResumeLayout(false);
            this.spcMain.Panel2.ResumeLayout(false);
            this.spcMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.stsMain.ResumeLayout(false);
            this.stsMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
