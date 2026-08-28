namespace FileExplorerApp.Forms
{
    partial class SearchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblKeyword;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.Label lblRootFolder;
        private System.Windows.Forms.TextBox txtRootFolder;
        private System.Windows.Forms.Button btnBrowseRootFolder;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.CheckBox chkIncludeHidden;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnCancelSearch;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListView lvwResults;
        private System.Windows.Forms.ColumnHeader colResultName;
        private System.Windows.Forms.ColumnHeader colResultLocation;
        private System.Windows.Forms.ColumnHeader colResultSize;
        private System.Windows.Forms.ColumnHeader colResultModified;
        private System.Windows.Forms.Button btnClose;

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblKeyword = new System.Windows.Forms.Label();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.lblRootFolder = new System.Windows.Forms.Label();
            this.txtRootFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseRootFolder = new System.Windows.Forms.Button();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.chkIncludeHidden = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnCancelSearch = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lvwResults = new System.Windows.Forms.ListView();
            this.colResultName = new System.Windows.Forms.ColumnHeader();
            this.colResultLocation = new System.Windows.Forms.ColumnHeader();
            this.colResultSize = new System.Windows.Forms.ColumnHeader();
            this.colResultModified = new System.Windows.Forms.ColumnHeader();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpOptions.SuspendLayout();
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            //
            // lblKeyword
            //
            this.lblKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblKeyword.Location = new System.Drawing.Point(16, 16);
            this.lblKeyword.Name = "lblKeyword";
            this.lblKeyword.Size = new System.Drawing.Size(200, 20);
            this.lblKeyword.TabIndex = 0;
            this.lblKeyword.Text = "Từ khóa:";
            //
            // txtKeyword
            //
            this.txtKeyword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyword.Location = new System.Drawing.Point(16, 38);
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(668, 23);
            this.txtKeyword.TabIndex = 1;
            //
            // lblRootFolder
            //
            this.lblRootFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.lblRootFolder.Location = new System.Drawing.Point(16, 70);
            this.lblRootFolder.Name = "lblRootFolder";
            this.lblRootFolder.Size = new System.Drawing.Size(200, 20);
            this.lblRootFolder.TabIndex = 2;
            this.lblRootFolder.Text = "Thư mục gốc:";
            //
            // txtRootFolder
            //
            this.txtRootFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRootFolder.Location = new System.Drawing.Point(16, 92);
            this.txtRootFolder.Name = "txtRootFolder";
            this.txtRootFolder.Size = new System.Drawing.Size(580, 23);
            this.txtRootFolder.TabIndex = 3;
            //
            // btnBrowseRootFolder
            //
            this.btnBrowseRootFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseRootFolder.Location = new System.Drawing.Point(604, 91);
            this.btnBrowseRootFolder.Name = "btnBrowseRootFolder";
            this.btnBrowseRootFolder.Size = new System.Drawing.Size(64, 25);
            this.btnBrowseRootFolder.TabIndex = 4;
            this.btnBrowseRootFolder.Text = "...";
            this.btnBrowseRootFolder.Click += new System.EventHandler(this.btnBrowseRootFolder_Click);
            //
            // grpOptions
            //
            this.grpOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOptions.Controls.Add(this.chkRecursive);
            this.grpOptions.Controls.Add(this.chkIncludeHidden);
            this.grpOptions.Location = new System.Drawing.Point(16, 128);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(668, 72);
            this.grpOptions.TabIndex = 5;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Tùy chọn";
            //
            // chkRecursive
            //
            this.chkRecursive.Checked = true;
            this.chkRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRecursive.Location = new System.Drawing.Point(16, 24);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(320, 24);
            this.chkRecursive.TabIndex = 0;
            this.chkRecursive.Text = "Tìm cả trong thư mục con";
            //
            // chkIncludeHidden
            //
            this.chkIncludeHidden.Location = new System.Drawing.Point(16, 44);
            this.chkIncludeHidden.Name = "chkIncludeHidden";
            this.chkIncludeHidden.Size = new System.Drawing.Size(320, 24);
            this.chkIncludeHidden.TabIndex = 1;
            this.chkIncludeHidden.Text = "Bao gồm mục ẩn/hệ thống";
            //
            // btnSearch
            //
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSearch.Location = new System.Drawing.Point(16, 212);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(120, 32);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Text = "Tìm kiếm";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            //
            // btnCancelSearch
            //
            this.btnCancelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancelSearch.Enabled = false;
            this.btnCancelSearch.Location = new System.Drawing.Point(144, 212);
            this.btnCancelSearch.Name = "btnCancelSearch";
            this.btnCancelSearch.Size = new System.Drawing.Size(120, 32);
            this.btnCancelSearch.TabIndex = 7;
            this.btnCancelSearch.Text = "Hủy";
            this.btnCancelSearch.Click += new System.EventHandler(this.btnCancelSearch_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(280, 218);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(404, 20);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Sẵn sàng";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //
            // lvwResults
            //
            this.lvwResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwResults.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colResultName,
            this.colResultLocation,
            this.colResultSize,
            this.colResultModified});
            this.lvwResults.FullRowSelect = true;
            this.lvwResults.Location = new System.Drawing.Point(16, 256);
            this.lvwResults.Name = "lvwResults";
            this.lvwResults.Size = new System.Drawing.Size(668, 268);
            this.lvwResults.TabIndex = 9;
            this.lvwResults.UseCompatibleStateImageBehavior = false;
            this.lvwResults.View = System.Windows.Forms.View.Details;
            //
            // colResultName
            //
            this.colResultName.Text = "Tên";
            this.colResultName.Width = 220;
            //
            // colResultLocation
            //
            this.colResultLocation.Text = "Vị trí";
            this.colResultLocation.Width = 230;
            //
            // colResultSize
            //
            this.colResultSize.Text = "Kích thước";
            this.colResultSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colResultSize.Width = 100;
            //
            // colResultModified
            //
            this.colResultModified.Text = "Ngày sửa";
            this.colResultModified.Width = 118;
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(584, 532);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 10;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // SearchForm
            //
            this.AcceptButton = this.btnSearch;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(700, 580);
            this.Controls.Add(this.lblKeyword);
            this.Controls.Add(this.txtKeyword);
            this.Controls.Add(this.lblRootFolder);
            this.Controls.Add(this.txtRootFolder);
            this.Controls.Add(this.btnBrowseRootFolder);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.btnCancelSearch);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lvwResults);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tìm kiếm";
            this.grpOptions.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
