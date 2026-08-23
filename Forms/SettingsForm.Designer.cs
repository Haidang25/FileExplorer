namespace FileExplorerApp.Forms
{
    partial class SettingsForm
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

        private System.Windows.Forms.GroupBox groupBoxTheme;
        private System.Windows.Forms.RadioButton rbLight;
        private System.Windows.Forms.RadioButton rbDark;

        private System.Windows.Forms.GroupBox groupBoxDisplay;
        private System.Windows.Forms.CheckBox chkShowHidden;
        private System.Windows.Forms.CheckBox chkShowExtension;
        private System.Windows.Forms.Label lblViewMode;
        private System.Windows.Forms.RadioButton rbDetails;
        private System.Windows.Forms.RadioButton rbLargeIcon;
        private System.Windows.Forms.RadioButton rbList;

        private System.Windows.Forms.GroupBox groupBoxWatcher;
        private System.Windows.Forms.CheckBox chkAutoRefresh;
        private System.Windows.Forms.Label lblWatcherDelay;
        private System.Windows.Forms.NumericUpDown numWatcherDelay;
        private System.Windows.Forms.Label lblWatcherDelayUnit;

        private System.Windows.Forms.GroupBox groupBoxLog;
        private System.Windows.Forms.CheckBox chkEnableLog;
        private System.Windows.Forms.Label lblLogPath;
        private System.Windows.Forms.TextBox txtLogPath;
        private System.Windows.Forms.Button btnOpenLogFolder;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBoxTheme = new System.Windows.Forms.GroupBox();
            this.rbLight = new System.Windows.Forms.RadioButton();
            this.rbDark = new System.Windows.Forms.RadioButton();
            this.groupBoxDisplay = new System.Windows.Forms.GroupBox();
            this.chkShowHidden = new System.Windows.Forms.CheckBox();
            this.chkShowExtension = new System.Windows.Forms.CheckBox();
            this.lblViewMode = new System.Windows.Forms.Label();
            this.rbDetails = new System.Windows.Forms.RadioButton();
            this.rbLargeIcon = new System.Windows.Forms.RadioButton();
            this.rbList = new System.Windows.Forms.RadioButton();
            this.groupBoxWatcher = new System.Windows.Forms.GroupBox();
            this.chkAutoRefresh = new System.Windows.Forms.CheckBox();
            this.lblWatcherDelay = new System.Windows.Forms.Label();
            this.numWatcherDelay = new System.Windows.Forms.NumericUpDown();
            this.lblWatcherDelayUnit = new System.Windows.Forms.Label();
            this.groupBoxLog = new System.Windows.Forms.GroupBox();
            this.chkEnableLog = new System.Windows.Forms.CheckBox();
            this.lblLogPath = new System.Windows.Forms.Label();
            this.txtLogPath = new System.Windows.Forms.TextBox();
            this.btnOpenLogFolder = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBoxTheme.SuspendLayout();
            this.groupBoxDisplay.SuspendLayout();
            this.groupBoxWatcher.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWatcherDelay)).BeginInit();
            this.groupBoxLog.SuspendLayout();
            this.SuspendLayout();
            //
            // groupBoxTheme
            //
            this.groupBoxTheme.Controls.Add(this.rbLight);
            this.groupBoxTheme.Controls.Add(this.rbDark);
            this.groupBoxTheme.Location = new System.Drawing.Point(12, 12);
            this.groupBoxTheme.Name = "groupBoxTheme";
            this.groupBoxTheme.Size = new System.Drawing.Size(460, 56);
            this.groupBoxTheme.TabIndex = 0;
            this.groupBoxTheme.TabStop = false;
            this.groupBoxTheme.Text = "GIAO DIỆN";
            //
            // rbLight
            //
            this.rbLight.AutoSize = true;
            this.rbLight.Location = new System.Drawing.Point(16, 24);
            this.rbLight.Name = "rbLight";
            this.rbLight.Size = new System.Drawing.Size(80, 20);
            this.rbLight.TabIndex = 0;
            this.rbLight.Text = "Sáng (Light)";
            this.rbLight.UseVisualStyleBackColor = true;
            //
            // rbDark
            //
            this.rbDark.AutoSize = true;
            this.rbDark.Checked = true;
            this.rbDark.Location = new System.Drawing.Point(160, 24);
            this.rbDark.Name = "rbDark";
            this.rbDark.Size = new System.Drawing.Size(78, 20);
            this.rbDark.TabIndex = 1;
            this.rbDark.TabStop = true;
            this.rbDark.Text = "Tối (Dark)";
            this.rbDark.UseVisualStyleBackColor = true;
            //
            // groupBoxDisplay
            //
            this.groupBoxDisplay.Controls.Add(this.chkShowHidden);
            this.groupBoxDisplay.Controls.Add(this.chkShowExtension);
            this.groupBoxDisplay.Controls.Add(this.lblViewMode);
            this.groupBoxDisplay.Controls.Add(this.rbDetails);
            this.groupBoxDisplay.Controls.Add(this.rbLargeIcon);
            this.groupBoxDisplay.Controls.Add(this.rbList);
            this.groupBoxDisplay.Location = new System.Drawing.Point(12, 80);
            this.groupBoxDisplay.Name = "groupBoxDisplay";
            this.groupBoxDisplay.Size = new System.Drawing.Size(460, 118);
            this.groupBoxDisplay.TabIndex = 1;
            this.groupBoxDisplay.TabStop = false;
            this.groupBoxDisplay.Text = "HIỂN THỊ";
            //
            // chkShowHidden
            //
            this.chkShowHidden.AutoSize = true;
            this.chkShowHidden.Location = new System.Drawing.Point(16, 24);
            this.chkShowHidden.Name = "chkShowHidden";
            this.chkShowHidden.Size = new System.Drawing.Size(150, 20);
            this.chkShowHidden.TabIndex = 0;
            this.chkShowHidden.Text = "Hiện tệp ẩn (Hidden files)";
            this.chkShowHidden.UseVisualStyleBackColor = true;
            //
            // chkShowExtension
            //
            this.chkShowExtension.AutoSize = true;
            this.chkShowExtension.Location = new System.Drawing.Point(16, 48);
            this.chkShowExtension.Name = "chkShowExtension";
            this.chkShowExtension.Size = new System.Drawing.Size(170, 20);
            this.chkShowExtension.TabIndex = 1;
            this.chkShowExtension.Text = "Hiện phần mở rộng tệp";
            this.chkShowExtension.UseVisualStyleBackColor = true;
            //
            // lblViewMode
            //
            this.lblViewMode.AutoSize = true;
            this.lblViewMode.Location = new System.Drawing.Point(16, 78);
            this.lblViewMode.Name = "lblViewMode";
            this.lblViewMode.Size = new System.Drawing.Size(140, 20);
            this.lblViewMode.TabIndex = 2;
            this.lblViewMode.Text = "Chế độ xem mặc định:";
            //
            // rbDetails
            //
            this.rbDetails.AutoSize = true;
            this.rbDetails.Checked = true;
            this.rbDetails.Location = new System.Drawing.Point(160, 78);
            this.rbDetails.Name = "rbDetails";
            this.rbDetails.Size = new System.Drawing.Size(66, 20);
            this.rbDetails.TabIndex = 3;
            this.rbDetails.TabStop = true;
            this.rbDetails.Text = "Chi tiết";
            this.rbDetails.UseVisualStyleBackColor = true;
            //
            // rbLargeIcon
            //
            this.rbLargeIcon.AutoSize = true;
            this.rbLargeIcon.Location = new System.Drawing.Point(240, 78);
            this.rbLargeIcon.Name = "rbLargeIcon";
            this.rbLargeIcon.Size = new System.Drawing.Size(118, 20);
            this.rbLargeIcon.TabIndex = 4;
            this.rbLargeIcon.Text = "Biểu tượng lớn";
            this.rbLargeIcon.UseVisualStyleBackColor = true;
            //
            // rbList
            //
            this.rbList.AutoSize = true;
            this.rbList.Location = new System.Drawing.Point(370, 78);
            this.rbList.Name = "rbList";
            this.rbList.Size = new System.Drawing.Size(80, 20);
            this.rbList.TabIndex = 5;
            this.rbList.Text = "Danh sách";
            this.rbList.UseVisualStyleBackColor = true;
            //
            // groupBoxWatcher
            //
            this.groupBoxWatcher.Controls.Add(this.chkAutoRefresh);
            this.groupBoxWatcher.Controls.Add(this.lblWatcherDelay);
            this.groupBoxWatcher.Controls.Add(this.numWatcherDelay);
            this.groupBoxWatcher.Controls.Add(this.lblWatcherDelayUnit);
            this.groupBoxWatcher.Location = new System.Drawing.Point(12, 206);
            this.groupBoxWatcher.Name = "groupBoxWatcher";
            this.groupBoxWatcher.Size = new System.Drawing.Size(460, 78);
            this.groupBoxWatcher.TabIndex = 2;
            this.groupBoxWatcher.TabStop = false;
            this.groupBoxWatcher.Text = "GIÁM SÁT THƯ MỤC";
            //
            // chkAutoRefresh
            //
            this.chkAutoRefresh.AutoSize = true;
            this.chkAutoRefresh.Location = new System.Drawing.Point(16, 24);
            this.chkAutoRefresh.Name = "chkAutoRefresh";
            this.chkAutoRefresh.Size = new System.Drawing.Size(220, 20);
            this.chkAutoRefresh.TabIndex = 0;
            this.chkAutoRefresh.Text = "Tự động cập nhật khi có thay đổi";
            this.chkAutoRefresh.UseVisualStyleBackColor = true;
            //
            // lblWatcherDelay
            //
            this.lblWatcherDelay.AutoSize = true;
            this.lblWatcherDelay.Location = new System.Drawing.Point(16, 50);
            this.lblWatcherDelay.Name = "lblWatcherDelay";
            this.lblWatcherDelay.Size = new System.Drawing.Size(100, 20);
            this.lblWatcherDelay.TabIndex = 1;
            this.lblWatcherDelay.Text = "Độ trễ cập nhật:";
            //
            // numWatcherDelay
            //
            this.numWatcherDelay.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            this.numWatcherDelay.Location = new System.Drawing.Point(150, 48);
            this.numWatcherDelay.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numWatcherDelay.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numWatcherDelay.Name = "numWatcherDelay";
            this.numWatcherDelay.Size = new System.Drawing.Size(80, 26);
            this.numWatcherDelay.TabIndex = 2;
            this.numWatcherDelay.Value = new decimal(new int[] { 500, 0, 0, 0 });
            //
            // lblWatcherDelayUnit
            //
            this.lblWatcherDelayUnit.AutoSize = true;
            this.lblWatcherDelayUnit.Location = new System.Drawing.Point(236, 50);
            this.lblWatcherDelayUnit.Name = "lblWatcherDelayUnit";
            this.lblWatcherDelayUnit.Size = new System.Drawing.Size(30, 20);
            this.lblWatcherDelayUnit.TabIndex = 3;
            this.lblWatcherDelayUnit.Text = "ms";
            //
            // groupBoxLog
            //
            this.groupBoxLog.Controls.Add(this.chkEnableLog);
            this.groupBoxLog.Controls.Add(this.lblLogPath);
            this.groupBoxLog.Controls.Add(this.txtLogPath);
            this.groupBoxLog.Controls.Add(this.btnOpenLogFolder);
            this.groupBoxLog.Location = new System.Drawing.Point(12, 292);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new System.Drawing.Size(460, 100);
            this.groupBoxLog.TabIndex = 3;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "NHẬT KÝ";
            //
            // chkEnableLog
            //
            this.chkEnableLog.AutoSize = true;
            this.chkEnableLog.Location = new System.Drawing.Point(16, 24);
            this.chkEnableLog.Name = "chkEnableLog";
            this.chkEnableLog.Size = new System.Drawing.Size(160, 20);
            this.chkEnableLog.TabIndex = 0;
            this.chkEnableLog.Text = "Ghi nhật ký thao tác";
            this.chkEnableLog.UseVisualStyleBackColor = true;
            //
            // lblLogPath
            //
            this.lblLogPath.AutoSize = true;
            this.lblLogPath.Location = new System.Drawing.Point(16, 56);
            this.lblLogPath.Name = "lblLogPath";
            this.lblLogPath.Size = new System.Drawing.Size(100, 20);
            this.lblLogPath.TabIndex = 1;
            this.lblLogPath.Text = "Vị trí lưu log:";
            //
            // txtLogPath
            //
            this.txtLogPath.Location = new System.Drawing.Point(120, 54);
            this.txtLogPath.Name = "txtLogPath";
            this.txtLogPath.ReadOnly = true;
            this.txtLogPath.Size = new System.Drawing.Size(240, 26);
            this.txtLogPath.TabIndex = 2;
            //
            // btnOpenLogFolder
            //
            this.btnOpenLogFolder.Location = new System.Drawing.Point(368, 52);
            this.btnOpenLogFolder.Name = "btnOpenLogFolder";
            this.btnOpenLogFolder.Size = new System.Drawing.Size(80, 28);
            this.btnOpenLogFolder.TabIndex = 3;
            this.btnOpenLogFolder.Text = "Mở thư mục";
            this.btnOpenLogFolder.UseVisualStyleBackColor = true;
            this.btnOpenLogFolder.Click += new System.EventHandler(this.btnOpenLogFolder_Click);
            //
            // btnSave
            //
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(316, 406);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 30);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Lưu";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(397, 406);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 30);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            //
            // SettingsForm
            //
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 450);
            this.Controls.Add(this.groupBoxTheme);
            this.Controls.Add(this.groupBoxDisplay);
            this.Controls.Add(this.groupBoxWatcher);
            this.Controls.Add(this.groupBoxLog);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cài đặt";
            this.groupBoxTheme.ResumeLayout(false);
            this.groupBoxTheme.PerformLayout();
            this.groupBoxDisplay.ResumeLayout(false);
            this.groupBoxDisplay.PerformLayout();
            this.groupBoxWatcher.ResumeLayout(false);
            this.groupBoxWatcher.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numWatcherDelay)).EndInit();
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
