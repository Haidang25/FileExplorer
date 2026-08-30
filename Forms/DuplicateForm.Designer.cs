namespace FileExplorerApp.Forms
{
    partial class DuplicateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblRootFolderCaption;
        private System.Windows.Forms.Label lblRootFolderValue;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnCancelScan;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ListView lvwDuplicates;
        private System.Windows.Forms.ColumnHeader colDupName;
        private System.Windows.Forms.ColumnHeader colDupLocation;
        private System.Windows.Forms.ColumnHeader colDupSize;
        private System.Windows.Forms.ColumnHeader colDupModified;
        private System.Windows.Forms.Button btnDeleteSelected;
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
            this.lblRootFolderCaption = new System.Windows.Forms.Label();
            this.lblRootFolderValue = new System.Windows.Forms.Label();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.btnScan = new System.Windows.Forms.Button();
            this.btnCancelScan = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lvwDuplicates = new System.Windows.Forms.ListView();
            this.colDupName = new System.Windows.Forms.ColumnHeader();
            this.colDupLocation = new System.Windows.Forms.ColumnHeader();
            this.colDupSize = new System.Windows.Forms.ColumnHeader();
            this.colDupModified = new System.Windows.Forms.ColumnHeader();
            this.btnDeleteSelected = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            //
            // lblRootFolderCaption
            //
            this.lblRootFolderCaption.AutoSize = true;
            this.lblRootFolderCaption.Location = new System.Drawing.Point(16, 16);
            this.lblRootFolderCaption.Name = "lblRootFolderCaption";
            this.lblRootFolderCaption.Size = new System.Drawing.Size(85, 15);
            this.lblRootFolderCaption.TabIndex = 0;
            this.lblRootFolderCaption.Text = "Thư mục quét:";
            //
            // lblRootFolderValue
            //
            this.lblRootFolderValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRootFolderValue.AutoEllipsis = true;
            this.lblRootFolderValue.Location = new System.Drawing.Point(107, 16);
            this.lblRootFolderValue.Name = "lblRootFolderValue";
            this.lblRootFolderValue.Size = new System.Drawing.Size(560, 15);
            this.lblRootFolderValue.TabIndex = 1;
            this.lblRootFolderValue.Text = "-";
            //
            // chkRecursive
            //
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Checked = true;
            this.chkRecursive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRecursive.Location = new System.Drawing.Point(16, 42);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(180, 19);
            this.chkRecursive.TabIndex = 2;
            this.chkRecursive.Text = "Quét cả thư mục con";
            //
            // btnScan
            //
            this.btnScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScan.Location = new System.Drawing.Point(478, 38);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(100, 30);
            this.btnScan.TabIndex = 3;
            this.btnScan.Text = "Quét lại";
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            //
            // btnCancelScan
            //
            this.btnCancelScan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelScan.Enabled = false;
            this.btnCancelScan.Location = new System.Drawing.Point(584, 38);
            this.btnCancelScan.Name = "btnCancelScan";
            this.btnCancelScan.Size = new System.Drawing.Size(100, 30);
            this.btnCancelScan.TabIndex = 4;
            this.btnCancelScan.Text = "Hủy";
            this.btnCancelScan.Click += new System.EventHandler(this.btnCancelScan_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Location = new System.Drawing.Point(16, 76);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(668, 20);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Sẵn sàng.";
            //
            // lvwDuplicates
            //
            this.lvwDuplicates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwDuplicates.CheckBoxes = true;
            this.lvwDuplicates.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colDupName,
            this.colDupLocation,
            this.colDupSize,
            this.colDupModified});
            this.lvwDuplicates.FullRowSelect = true;
            this.lvwDuplicates.GridLines = true;
            this.lvwDuplicates.HideSelection = false;
            this.lvwDuplicates.Location = new System.Drawing.Point(16, 102);
            this.lvwDuplicates.Name = "lvwDuplicates";
            this.lvwDuplicates.Size = new System.Drawing.Size(668, 400);
            this.lvwDuplicates.TabIndex = 6;
            this.lvwDuplicates.UseCompatibleStateImageBehavior = false;
            this.lvwDuplicates.View = System.Windows.Forms.View.Details;
            this.lvwDuplicates.DoubleClick += new System.EventHandler(this.lvwDuplicates_DoubleClick);
            //
            // colDupName
            //
            this.colDupName.Text = "Tên";
            this.colDupName.Width = 220;
            //
            // colDupLocation
            //
            this.colDupLocation.Text = "Vị trí";
            this.colDupLocation.Width = 260;
            //
            // colDupSize
            //
            this.colDupSize.Text = "Kích thước";
            this.colDupSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colDupSize.Width = 90;
            //
            // colDupModified
            //
            this.colDupModified.Text = "Ngày sửa";
            this.colDupModified.Width = 96;
            //
            // btnDeleteSelected
            //
            this.btnDeleteSelected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteSelected.Location = new System.Drawing.Point(16, 510);
            this.btnDeleteSelected.Name = "btnDeleteSelected";
            this.btnDeleteSelected.Size = new System.Drawing.Size(180, 32);
            this.btnDeleteSelected.TabIndex = 8;
            this.btnDeleteSelected.Text = "Xóa tệp đã chọn";
            this.btnDeleteSelected.Click += new System.EventHandler(this.btnDeleteSelected_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(584, 510);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 7;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // DuplicateForm
            //
            this.AcceptButton = this.btnScan;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(700, 554);
            this.Controls.Add(this.lblRootFolderCaption);
            this.Controls.Add(this.lblRootFolderValue);
            this.Controls.Add(this.chkRecursive);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnCancelScan);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lvwDuplicates);
            this.Controls.Add(this.btnDeleteSelected);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(560, 420);
            this.Name = "DuplicateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tìm tệp trùng lặp";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
