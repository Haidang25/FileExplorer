namespace FileExplorerApp.Forms
{
    partial class IntegrityToastForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlAccent;
        private System.Windows.Forms.Label lblIcon;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblFilePath;
        private System.Windows.Forms.Timer tmrAutoClose;

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
            this.components = new System.ComponentModel.Container();
            this.pnlAccent = new System.Windows.Forms.Panel();
            this.lblIcon = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.tmrAutoClose = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            //
            // pnlAccent
            //
            this.pnlAccent.BackColor = FileExplorerApp.Helpers.AppTheme.Error;
            this.pnlAccent.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlAccent.Location = new System.Drawing.Point(0, 0);
            this.pnlAccent.Name = "pnlAccent";
            this.pnlAccent.Size = new System.Drawing.Size(6, 92);
            this.pnlAccent.TabIndex = 0;
            //
            // lblIcon
            //
            this.lblIcon.Font = new System.Drawing.Font("Segoe UI", 20F);
            this.lblIcon.ForeColor = FileExplorerApp.Helpers.AppTheme.Error;
            this.lblIcon.Location = new System.Drawing.Point(16, 16);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(48, 48);
            this.lblIcon.TabIndex = 1;
            this.lblIcon.Text = "⚠";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTitle
            //
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(70, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(240, 20);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Phát hiện tệp bị sửa";
            //
            // lblFilePath
            //
            this.lblFilePath.AutoEllipsis = true;
            this.lblFilePath.Location = new System.Drawing.Point(70, 36);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(240, 44);
            this.lblFilePath.TabIndex = 3;
            this.lblFilePath.Text = "(đường dẫn tệp)";
            this.lblFilePath.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            //
            // tmrAutoClose
            //
            this.tmrAutoClose.Interval = 5000;
            this.tmrAutoClose.Tick += new System.EventHandler(this.tmrAutoClose_Tick);
            //
            // IntegrityToastForm
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Surface;
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.ClientSize = new System.Drawing.Size(320, 92);
            this.ControlBox = false;
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Controls.Add(this.lblFilePath);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblIcon);
            this.Controls.Add(this.pnlAccent);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "IntegrityToastForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "IntegrityToastForm";
            this.TopMost = true;
            this.ResumeLayout(false);
        }

        #endregion
    }
}
