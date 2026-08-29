namespace FileExplorerApp.Forms
{
    partial class LogForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.GroupBox grpFilters;
        private System.Windows.Forms.Label lblFilterOperation;
        private System.Windows.Forms.ComboBox cboFilterOperation;
        private System.Windows.Forms.Label lblFilterResult;
        private System.Windows.Forms.ComboBox cboFilterResult;
        private System.Windows.Forms.Label lblFilterFrom;
        private System.Windows.Forms.DateTimePicker dtpFilterFrom;
        private System.Windows.Forms.Label lblFilterTo;
        private System.Windows.Forms.DateTimePicker dtpFilterTo;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.Button btnResetFilter;
        private System.Windows.Forms.ListView lvwLogs;
        private System.Windows.Forms.ColumnHeader colLogTime;
        private System.Windows.Forms.ColumnHeader colLogOperation;
        private System.Windows.Forms.ColumnHeader colLogSource;
        private System.Windows.Forms.ColumnHeader colLogDestination;
        private System.Windows.Forms.ColumnHeader colLogResult;
        private System.Windows.Forms.ColumnHeader colLogItemCount;
        private System.Windows.Forms.ColumnHeader colLogDuration;
        private System.Windows.Forms.ColumnHeader colLogMessage;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClearLogs;
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
            this.grpFilters = new System.Windows.Forms.GroupBox();
            this.btnResetFilter = new System.Windows.Forms.Button();
            this.btnApplyFilter = new System.Windows.Forms.Button();
            this.dtpFilterTo = new System.Windows.Forms.DateTimePicker();
            this.lblFilterTo = new System.Windows.Forms.Label();
            this.dtpFilterFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFilterFrom = new System.Windows.Forms.Label();
            this.cboFilterResult = new System.Windows.Forms.ComboBox();
            this.lblFilterResult = new System.Windows.Forms.Label();
            this.cboFilterOperation = new System.Windows.Forms.ComboBox();
            this.lblFilterOperation = new System.Windows.Forms.Label();
            this.lvwLogs = new System.Windows.Forms.ListView();
            this.colLogTime = new System.Windows.Forms.ColumnHeader();
            this.colLogOperation = new System.Windows.Forms.ColumnHeader();
            this.colLogSource = new System.Windows.Forms.ColumnHeader();
            this.colLogDestination = new System.Windows.Forms.ColumnHeader();
            this.colLogResult = new System.Windows.Forms.ColumnHeader();
            this.colLogItemCount = new System.Windows.Forms.ColumnHeader();
            this.colLogDuration = new System.Windows.Forms.ColumnHeader();
            this.colLogMessage = new System.Windows.Forms.ColumnHeader();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClearLogs = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.grpFilters.SuspendLayout();
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            //
            // grpFilters
            //
            this.grpFilters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.grpFilters.Controls.Add(this.btnResetFilter);
            this.grpFilters.Controls.Add(this.btnApplyFilter);
            this.grpFilters.Controls.Add(this.dtpFilterTo);
            this.grpFilters.Controls.Add(this.lblFilterTo);
            this.grpFilters.Controls.Add(this.dtpFilterFrom);
            this.grpFilters.Controls.Add(this.lblFilterFrom);
            this.grpFilters.Controls.Add(this.cboFilterResult);
            this.grpFilters.Controls.Add(this.lblFilterResult);
            this.grpFilters.Controls.Add(this.cboFilterOperation);
            this.grpFilters.Controls.Add(this.lblFilterOperation);
            this.grpFilters.Location = new System.Drawing.Point(12, 12);
            this.grpFilters.Name = "grpFilters";
            this.grpFilters.Size = new System.Drawing.Size(876, 88);
            this.grpFilters.TabIndex = 0;
            this.grpFilters.TabStop = false;
            this.grpFilters.Text = "Bộ lọc";
            //
            // btnResetFilter
            //
            this.btnResetFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnResetFilter.Location = new System.Drawing.Point(780, 50);
            this.btnResetFilter.Name = "btnResetFilter";
            this.btnResetFilter.Size = new System.Drawing.Size(80, 27);
            this.btnResetFilter.TabIndex = 9;
            this.btnResetFilter.Text = "Đặt lại";
            this.btnResetFilter.Click += new System.EventHandler(this.btnResetFilter_Click);
            //
            // btnApplyFilter
            //
            this.btnApplyFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFilter.Location = new System.Drawing.Point(694, 50);
            this.btnApplyFilter.Name = "btnApplyFilter";
            this.btnApplyFilter.Size = new System.Drawing.Size(80, 27);
            this.btnApplyFilter.TabIndex = 8;
            this.btnApplyFilter.Text = "Lọc";
            this.btnApplyFilter.Click += new System.EventHandler(this.btnApplyFilter_Click);
            //
            // dtpFilterTo
            //
            this.dtpFilterTo.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFilterTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFilterTo.Location = new System.Drawing.Point(462, 52);
            this.dtpFilterTo.Name = "dtpFilterTo";
            this.dtpFilterTo.Size = new System.Drawing.Size(160, 23);
            this.dtpFilterTo.TabIndex = 7;
            //
            // lblFilterTo
            //
            this.lblFilterTo.AutoSize = true;
            this.lblFilterTo.Location = new System.Drawing.Point(462, 32);
            this.lblFilterTo.Name = "lblFilterTo";
            this.lblFilterTo.Size = new System.Drawing.Size(60, 15);
            this.lblFilterTo.TabIndex = 6;
            this.lblFilterTo.Text = "Đến ngày:";
            //
            // dtpFilterFrom
            //
            this.dtpFilterFrom.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFilterFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFilterFrom.Location = new System.Drawing.Point(288, 52);
            this.dtpFilterFrom.Name = "dtpFilterFrom";
            this.dtpFilterFrom.Size = new System.Drawing.Size(160, 23);
            this.dtpFilterFrom.TabIndex = 5;
            //
            // lblFilterFrom
            //
            this.lblFilterFrom.AutoSize = true;
            this.lblFilterFrom.Location = new System.Drawing.Point(288, 32);
            this.lblFilterFrom.Name = "lblFilterFrom";
            this.lblFilterFrom.Size = new System.Drawing.Size(58, 15);
            this.lblFilterFrom.TabIndex = 4;
            this.lblFilterFrom.Text = "Từ ngày:";
            //
            // cboFilterResult
            //
            this.cboFilterResult.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterResult.FormattingEnabled = true;
            this.cboFilterResult.Location = new System.Drawing.Point(148, 52);
            this.cboFilterResult.Name = "cboFilterResult";
            this.cboFilterResult.Size = new System.Drawing.Size(120, 23);
            this.cboFilterResult.TabIndex = 3;
            //
            // lblFilterResult
            //
            this.lblFilterResult.AutoSize = true;
            this.lblFilterResult.Location = new System.Drawing.Point(148, 32);
            this.lblFilterResult.Name = "lblFilterResult";
            this.lblFilterResult.Size = new System.Drawing.Size(53, 15);
            this.lblFilterResult.TabIndex = 2;
            this.lblFilterResult.Text = "Kết quả:";
            //
            // cboFilterOperation
            //
            this.cboFilterOperation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFilterOperation.FormattingEnabled = true;
            this.cboFilterOperation.Location = new System.Drawing.Point(16, 52);
            this.cboFilterOperation.Name = "cboFilterOperation";
            this.cboFilterOperation.Size = new System.Drawing.Size(120, 23);
            this.cboFilterOperation.TabIndex = 1;
            //
            // lblFilterOperation
            //
            this.lblFilterOperation.AutoSize = true;
            this.lblFilterOperation.Location = new System.Drawing.Point(16, 32);
            this.lblFilterOperation.Name = "lblFilterOperation";
            this.lblFilterOperation.Size = new System.Drawing.Size(63, 15);
            this.lblFilterOperation.TabIndex = 0;
            this.lblFilterOperation.Text = "Thao tác:";
            //
            // lvwLogs
            //
            this.lvwLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwLogs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colLogTime,
            this.colLogOperation,
            this.colLogSource,
            this.colLogDestination,
            this.colLogResult,
            this.colLogItemCount,
            this.colLogDuration,
            this.colLogMessage});
            this.lvwLogs.FullRowSelect = true;
            this.lvwLogs.GridLines = true;
            this.lvwLogs.HideSelection = false;
            this.lvwLogs.Location = new System.Drawing.Point(12, 106);
            this.lvwLogs.Name = "lvwLogs";
            this.lvwLogs.Size = new System.Drawing.Size(876, 422);
            this.lvwLogs.TabIndex = 1;
            this.lvwLogs.UseCompatibleStateImageBehavior = false;
            this.lvwLogs.View = System.Windows.Forms.View.Details;
            //
            // colLogTime
            //
            this.colLogTime.Text = "Thời gian";
            this.colLogTime.Width = 130;
            //
            // colLogOperation
            //
            this.colLogOperation.Text = "Thao tác";
            this.colLogOperation.Width = 90;
            //
            // colLogSource
            //
            this.colLogSource.Text = "Nguồn";
            this.colLogSource.Width = 180;
            //
            // colLogDestination
            //
            this.colLogDestination.Text = "Đích";
            this.colLogDestination.Width = 180;
            //
            // colLogResult
            //
            this.colLogResult.Text = "Kết quả";
            this.colLogResult.Width = 90;
            //
            // colLogItemCount
            //
            this.colLogItemCount.Text = "Số mục";
            this.colLogItemCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colLogItemCount.Width = 60;
            //
            // colLogDuration
            //
            this.colLogDuration.Text = "Thời lượng";
            this.colLogDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.colLogDuration.Width = 80;
            //
            // colLogMessage
            //
            this.colLogMessage.Text = "Ghi chú";
            this.colLogMessage.Width = 220;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(12, 540);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(78, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "0 dòng log";
            //
            // btnRefresh
            //
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(568, 534);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 32);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            //
            // btnClearLogs
            //
            this.btnClearLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearLogs.Location = new System.Drawing.Point(674, 534);
            this.btnClearLogs.Name = "btnClearLogs";
            this.btnClearLogs.Size = new System.Drawing.Size(120, 32);
            this.btnClearLogs.TabIndex = 4;
            this.btnClearLogs.Text = "Xóa lịch sử";
            this.btnClearLogs.Click += new System.EventHandler(this.btnClearLogs_Click);
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(788, 534);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 32);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // LogForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(900, 578);
            this.Controls.Add(this.grpFilters);
            this.Controls.Add(this.lvwLogs);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnClearLogs);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(760, 420);
            this.Name = "LogForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nhật ký hoạt động";
            this.grpFilters.ResumeLayout(false);
            this.grpFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
