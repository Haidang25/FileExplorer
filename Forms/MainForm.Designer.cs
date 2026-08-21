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

        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsSearch;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsRecycleBin;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsLogs;

        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();

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

            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsRecycleBin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsLogs = new System.Windows.Forms.ToolStripMenuItem();

            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();

            this.menuStrip1.SuspendLayout();
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
                this.mnuViewShowHidden});
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

            // 
            // mnuTools ("Công cụ")
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuToolsSearch,
                this.mnuToolsRecycleBin,
                this.mnuToolsLogs});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Text = "&Công cụ";

            this.mnuToolsSearch.Name = "mnuToolsSearch";
            this.mnuToolsSearch.Text = "&Tìm kiếm...";
            this.mnuToolsSearch.ShortcutKeys = System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F;
            this.mnuToolsSearch.Click += new System.EventHandler(this.mnuToolsSearch_Click);

            this.mnuToolsRecycleBin.Name = "mnuToolsRecycleBin";
            this.mnuToolsRecycleBin.Text = "Thùng &rác";
            this.mnuToolsRecycleBin.Click += new System.EventHandler(this.mnuToolsRecycleBin_Click);

            this.mnuToolsLogs.Name = "mnuToolsLogs";
            this.mnuToolsLogs.Text = "Nhật &ký hoạt động";
            this.mnuToolsLogs.Click += new System.EventHandler(this.mnuToolsLogs_Click);

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
            // MainForm
            // 
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "MainForm";

            this.menuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
