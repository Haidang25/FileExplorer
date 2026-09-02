namespace FileExplorerApp.Forms
{
    partial class RecycleBinForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.ListView lvwItems;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colOriginalPath;
        private System.Windows.Forms.ColumnHeader colDeletedDate;
        private System.Windows.Forms.ColumnHeader colSize;
        private System.Windows.Forms.ColumnHeader colType;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnEmptyRecycleBin;
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
            this.lvwItems = new System.Windows.Forms.ListView();
            this.colName = new System.Windows.Forms.ColumnHeader();
            this.colOriginalPath = new System.Windows.Forms.ColumnHeader();
            this.colDeletedDate = new System.Windows.Forms.ColumnHeader();
            this.colSize = new System.Windows.Forms.ColumnHeader();
            this.colType = new System.Windows.Forms.ColumnHeader();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.btnEmptyRecycleBin = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            //
            // lvwItems
            //
            this.lvwItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colOriginalPath,
            this.colDeletedDate,
            this.colSize,
            this.colType});
            this.lvwItems.FullRowSelect = true;
            this.lvwItems.GridLines = true;
            this.lvwItems.HideSelection = false;
            this.lvwItems.Location = new System.Drawing.Point(12, 12);
            this.lvwItems.MultiSelect = true;
            this.lvwItems.Name = "lvwItems";
            this.lvwItems.Size = new System.Drawing.Size(1000, 420);
            this.lvwItems.TabIndex = 0;
            this.lvwItems.UseCompatibleStateImageBehavior = false;
            this.lvwItems.View = System.Windows.Forms.View.Details;
            //
            // colName
            //
            this.colName.Text = "Tên";
            this.colName.Width = 220;
            //
            // colOriginalPath
            //
            this.colOriginalPath.Text = "Vị trí gốc";
            this.colOriginalPath.Width = 340;
            //
            // colDeletedDate
            //
            this.colDeletedDate.Text = "Ngày xóa";
            this.colDeletedDate.Width = 140;
            //
            // colSize
            //
            this.colSize.Text = "Kích thước";
            this.colSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colSize.Width = 100;
            //
            // colType
            //
            this.colType.Text = "Loại";
            this.colType.Width = 90;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 444);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(60, 15);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "0 mục";
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(612, 438);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 32);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnRestore
            //
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestore.Location = new System.Drawing.Point(718, 438);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(110, 32);
            this.btnRestore.TabIndex = 3;
            this.btnRestore.Text = "Khôi phục";
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            //
            // btnEmptyRecycleBin
            //
            this.btnEmptyRecycleBin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEmptyRecycleBin.Location = new System.Drawing.Point(834, 438);
            this.btnEmptyRecycleBin.Name = "btnEmptyRecycleBin";
            this.btnEmptyRecycleBin.Size = new System.Drawing.Size(170, 32);
            this.btnEmptyRecycleBin.TabIndex = 4;
            this.btnEmptyRecycleBin.Text = "Dọn trống thùng rác";
            this.btnEmptyRecycleBin.Click += new System.EventHandler(this.btnEmptyRecycleBin_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(1010, 438);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // RecycleBinForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(1124, 482);
            this.Controls.Add(this.lvwItems);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnEmptyRecycleBin);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(700, 360);
            this.Name = "RecycleBinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thùng rác";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
