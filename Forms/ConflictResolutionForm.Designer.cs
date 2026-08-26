namespace FileExplorerApp.Forms
{
    partial class ConflictResolutionForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.TextBox txtNewName;
        private System.Windows.Forms.Button btnOverwrite;
        private System.Windows.Forms.Button btnRename;
        private System.Windows.Forms.Button btnSkip;
        private System.Windows.Forms.CheckBox chkApplyToAll;

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
            this.lblMessage = new System.Windows.Forms.Label();
            this.txtNewName = new System.Windows.Forms.TextBox();
            this.btnOverwrite = new System.Windows.Forms.Button();
            this.btnRename = new System.Windows.Forms.Button();
            this.btnSkip = new System.Windows.Forms.Button();
            this.chkApplyToAll = new System.Windows.Forms.CheckBox();
            this.components = new System.ComponentModel.Container();
            this.SuspendLayout();
            //
            // lblMessage
            //
            this.lblMessage.Location = new System.Drawing.Point(16, 16);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(408, 48);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "lblMessage";
            //
            // txtNewName
            //
            this.txtNewName.Location = new System.Drawing.Point(16, 72);
            this.txtNewName.Name = "txtNewName";
            this.txtNewName.Size = new System.Drawing.Size(408, 23);
            this.txtNewName.TabIndex = 1;
            //
            // chkApplyToAll
            //
            this.chkApplyToAll.Location = new System.Drawing.Point(16, 104);
            this.chkApplyToAll.Name = "chkApplyToAll";
            this.chkApplyToAll.Size = new System.Drawing.Size(408, 24);
            this.chkApplyToAll.TabIndex = 2;
            this.chkApplyToAll.Text = "Áp dụng cho tất cả các mục trùng tên còn lại";
            //
            // btnOverwrite
            //
            this.btnOverwrite.Location = new System.Drawing.Point(16, 144);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(128, 32);
            this.btnOverwrite.TabIndex = 3;
            this.btnOverwrite.Text = "Ghi đè";
            this.btnOverwrite.Click += new System.EventHandler(this.btnOverwrite_Click);
            //
            // btnRename
            //
            this.btnRename.Location = new System.Drawing.Point(152, 144);
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(128, 32);
            this.btnRename.TabIndex = 4;
            this.btnRename.Text = "Đổi tên";
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            //
            // btnSkip
            //
            this.btnSkip.Location = new System.Drawing.Point(296, 144);
            this.btnSkip.Name = "btnSkip";
            this.btnSkip.Size = new System.Drawing.Size(128, 32);
            this.btnSkip.TabIndex = 5;
            this.btnSkip.Text = "Bỏ qua";
            this.btnSkip.Click += new System.EventHandler(this.btnSkip_Click);
            //
            // ConflictResolutionForm
            //
            this.AcceptButton = this.btnRename;
            this.CancelButton = this.btnSkip;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(440, 196);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.txtNewName);
            this.Controls.Add(this.chkApplyToAll);
            this.Controls.Add(this.btnOverwrite);
            this.Controls.Add(this.btnRename);
            this.Controls.Add(this.btnSkip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConflictResolutionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Mục đã tồn tại";
            this.ResumeLayout(false);
        }

        #endregion
    }
}
