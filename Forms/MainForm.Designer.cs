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

        private System.Windows.Forms.MenuStrip menuStrip1;

        private System.Windows.Forms.ToolStrip toolStrip1;
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

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeViewFolders;

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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();

            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
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

            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeViewFolders = new System.Windows.Forms.TreeView();

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

            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();

            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuFile,
                this.mnuEdit,
                this.mnuView,
                this.mnuTools,
                this.mnuHelp});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";

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
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(800, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";

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
            // splitContainer1 (chia 2 vung lam viec: Panel1 = cay thu muc, Panel2 = danh sach file)
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 77);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Size = new System.Drawing.Size(800, 373);
            this.splitContainer1.SplitterDistance = 220;
            this.splitContainer1.SplitterWidth = 4;
            this.splitContainer1.TabIndex = 3;

            // Panel1 (trai) - chua treeViewFolders (cay thu muc).
            this.splitContainer1.Panel1.Controls.Add(this.treeViewFolders);
            this.splitContainer1.Panel1.Name = "splitContainer1.Panel1";

            // Panel2 (phai) - danh cho danh sach file (ListView) sau nay.
            this.splitContainer1.Panel2.Name = "splitContainer1.Panel2";

            //
            // treeViewFolders (cay thu muc ben trai)
            //
            this.treeViewFolders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFolders.Name = "treeViewFolders";
            this.treeViewFolders.HideSelection = false;
            this.treeViewFolders.PathSeparator = System.IO.Path.DirectorySeparatorChar.ToString();
            this.treeViewFolders.TabIndex = 0;
            this.treeViewFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewFolders_AfterSelect);
            this.treeViewFolders.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewFolders_BeforeExpand);

            // 
            // MainForm
            // 
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.pnlAddressBar);
            this.Controls.Add(this.splitContainer1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";

            this.menuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlAddressBar.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
