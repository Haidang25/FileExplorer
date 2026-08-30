namespace FileExplorerApp.Forms
{
    partial class BatchRenameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblPatternCaption;
        private System.Windows.Forms.TextBox txtPattern;
        private System.Windows.Forms.Label lblPatternHint;
        private System.Windows.Forms.ListView lvwPreview;
        private System.Windows.Forms.ColumnHeader colOldName;
        private System.Windows.Forms.ColumnHeader colNewName;
        private System.Windows.Forms.Button btnApply;
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
            this.lblPatternCaption = new System.Windows.Forms.Label();
            this.txtPattern = new System.Windows.Forms.TextBox();
            this.lblPatternHint = new System.Windows.Forms.Label();
            this.lvwPreview = new System.Windows.Forms.ListView();
            this.colOldName = new System.Windows.Forms.ColumnHeader();
            this.colNewName = new System.Windows.Forms.ColumnHeader();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblPatternCaption
            //
            this.lblPatternCaption.AutoSize = true;
            this.lblPatternCaption.Location = new System.Drawing.Point(16, 16);
            this.lblPatternCaption.Name = "lblPatternCaption";
            this.lblPatternCaption.Size = new System.Drawing.Size(89, 15);
            this.lblPatternCaption.TabIndex = 0;
            this.lblPatternCaption.Text = "Mẫu tên mới:";
            //
            // txtPattern
            //
            this.txtPattern.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPattern.Location = new System.Drawing.Point(16, 36);
            this.txtPattern.Name = "txtPattern";
            this.txtPattern.Size = new System.Drawing.Size(628, 23);
            this.txtPattern.TabIndex = 1;
            this.txtPattern.TextChanged += new System.EventHandler(this.txtPattern_TextChanged);
            //
            // lblPatternHint
            //
            this.lblPatternHint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPatternHint.Location = new System.Drawing.Point(16, 64);
            this.lblPatternHint.Name = "lblPatternHint";
            this.lblPatternHint.Size = new System.Drawing.Size(628, 34);
            this.lblPatternHint.TabIndex = 2;
            this.lblPatternHint.Text = "Hỗ trợ: {name} tên gốc, {ext} phần mở rộng, {n} hoặc {n:000} số thứ tự, {date} h" +
    "oặc {date:yyyyMMdd} ngày hiện tại.";
            //
            // lvwPreview
            //
            this.lvwPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwPreview.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colOldName,
            this.colNewName});
            this.lvwPreview.FullRowSelect = true;
            this.lvwPreview.GridLines = true;
            this.lvwPreview.HideSelection = false;
            this.lvwPreview.Location = new System.Drawing.Point(16, 104);
            this.lvwPreview.Name = "lvwPreview";
            this.lvwPreview.Size = new System.Drawing.Size(628, 340);
            this.lvwPreview.TabIndex = 3;
            this.lvwPreview.UseCompatibleStateImageBehavior = false;
            this.lvwPreview.View = System.Windows.Forms.View.Details;
            //
            // colOldName
            //
            this.colOldName.Text = "Tên hiện tại";
            this.colOldName.Width = 300;
            //
            // colNewName
            //
            this.colNewName.Text = "Tên mới";
            this.colNewName.Width = 300;
            //
            // btnApply
            //
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(16, 454);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(160, 30);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Đổi tên (sắp có)";
            //
            // btnClose
            //
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(544, 454);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 30);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Đóng";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            //
            // BatchRenameForm
            //
            this.AcceptButton = this.btnClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(660, 500);
            this.Controls.Add(this.lblPatternCaption);
            this.Controls.Add(this.txtPattern);
            this.Controls.Add(this.lblPatternHint);
            this.Controls.Add(this.lvwPreview);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnClose);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(520, 380);
            this.Name = "BatchRenameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Đổi tên hàng loạt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
