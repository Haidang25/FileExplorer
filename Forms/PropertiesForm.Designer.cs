namespace FileExplorerApp.Forms
{
    partial class PropertiesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        // Hang tren cung: icon + ten muc (file/thu muc dang xem thuoc tinh).
        private System.Windows.Forms.PictureBox picIcon;
        private System.Windows.Forms.Label lblName;

        // Duong ke phan cach - dung Panel mong (cao 1px, mau AppTheme.Border) thay
        // vi Label/GroupBox, giong cach mot so noi khac trong project da lam.
        private System.Windows.Forms.Panel pnlSeparatorTop;
        private System.Windows.Forms.Panel pnlSeparatorBottom;

        // Cac cap "nhan - gia tri" hien thong tin chi tiet, bo cuc giong tab
        // "General" cua hop thoai Properties trong Windows Explorer. lblXxx (co
        // hau to Caption) la ten truong (in dam, mau chu phu), lblXxxValue la gia
        // tri thuc te (se duoc gan trong constructor/code phia sau, hien tai chi
        // dat san chuoi placeholder giong ten bien de de nhan biet luc thiet ke).
        private System.Windows.Forms.Label lblTypeCaption;
        private System.Windows.Forms.Label lblTypeValue;
        private System.Windows.Forms.Label lblLocationCaption;
        private System.Windows.Forms.Label lblLocationValue;
        private System.Windows.Forms.Label lblSizeCaption;
        private System.Windows.Forms.Label lblSizeValue;
        private System.Windows.Forms.Label lblContentsCaption;
        private System.Windows.Forms.Label lblContentsValue;
        private System.Windows.Forms.Label lblCreatedCaption;
        private System.Windows.Forms.Label lblCreatedValue;
        private System.Windows.Forms.Label lblModifiedCaption;
        private System.Windows.Forms.Label lblModifiedValue;
        private System.Windows.Forms.Label lblAccessedCaption;
        private System.Windows.Forms.Label lblAccessedValue;

        // Nhom thuoc tinh co the bat/tat (Chi doc, An) - giong 2 checkbox duy nhat
        // ma hop thoai Properties cua Windows cho phep sua truc tiep tren tab General.
        private System.Windows.Forms.GroupBox grpAttributes;
        private System.Windows.Forms.CheckBox chkReadOnly;
        private System.Windows.Forms.CheckBox chkHidden;
        private System.Windows.Forms.CheckBox chkSystem;
        private System.Windows.Forms.CheckBox chkArchive;

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;

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
            this.picIcon = new System.Windows.Forms.PictureBox();
            this.lblName = new System.Windows.Forms.Label();
            this.pnlSeparatorTop = new System.Windows.Forms.Panel();
            this.lblTypeCaption = new System.Windows.Forms.Label();
            this.lblTypeValue = new System.Windows.Forms.Label();
            this.lblLocationCaption = new System.Windows.Forms.Label();
            this.lblLocationValue = new System.Windows.Forms.Label();
            this.lblSizeCaption = new System.Windows.Forms.Label();
            this.lblSizeValue = new System.Windows.Forms.Label();
            this.lblContentsCaption = new System.Windows.Forms.Label();
            this.lblContentsValue = new System.Windows.Forms.Label();
            this.lblCreatedCaption = new System.Windows.Forms.Label();
            this.lblCreatedValue = new System.Windows.Forms.Label();
            this.lblModifiedCaption = new System.Windows.Forms.Label();
            this.lblModifiedValue = new System.Windows.Forms.Label();
            this.lblAccessedCaption = new System.Windows.Forms.Label();
            this.lblAccessedValue = new System.Windows.Forms.Label();
            this.pnlSeparatorBottom = new System.Windows.Forms.Panel();
            this.grpAttributes = new System.Windows.Forms.GroupBox();
            this.chkReadOnly = new System.Windows.Forms.CheckBox();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.chkSystem = new System.Windows.Forms.CheckBox();
            this.chkArchive = new System.Windows.Forms.CheckBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.components = new System.ComponentModel.Container();
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).BeginInit();
            this.grpAttributes.SuspendLayout();
            this.SuspendLayout();
            //
            // picIcon
            //
            this.picIcon.Location = new System.Drawing.Point(16, 16);
            this.picIcon.Name = "picIcon";
            this.picIcon.Size = new System.Drawing.Size(32, 32);
            this.picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picIcon.TabIndex = 0;
            this.picIcon.TabStop = false;
            //
            // lblName
            //
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblName.Location = new System.Drawing.Point(60, 16);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(336, 32);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "lblName";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlSeparatorTop
            //
            this.pnlSeparatorTop.BackColor = FileExplorerApp.Helpers.AppTheme.Border;
            this.pnlSeparatorTop.Location = new System.Drawing.Point(16, 60);
            this.pnlSeparatorTop.Name = "pnlSeparatorTop";
            this.pnlSeparatorTop.Size = new System.Drawing.Size(380, 1);
            this.pnlSeparatorTop.TabIndex = 2;
            //
            // lblTypeCaption
            //
            this.lblTypeCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTypeCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblTypeCaption.Location = new System.Drawing.Point(16, 76);
            this.lblTypeCaption.Name = "lblTypeCaption";
            this.lblTypeCaption.Size = new System.Drawing.Size(100, 20);
            this.lblTypeCaption.TabIndex = 3;
            this.lblTypeCaption.Text = "Loại:";
            //
            // lblTypeValue
            //
            this.lblTypeValue.Location = new System.Drawing.Point(120, 76);
            this.lblTypeValue.Name = "lblTypeValue";
            this.lblTypeValue.Size = new System.Drawing.Size(276, 20);
            this.lblTypeValue.TabIndex = 4;
            this.lblTypeValue.Text = "lblTypeValue";
            //
            // lblLocationCaption
            //
            this.lblLocationCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLocationCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblLocationCaption.Location = new System.Drawing.Point(16, 100);
            this.lblLocationCaption.Name = "lblLocationCaption";
            this.lblLocationCaption.Size = new System.Drawing.Size(100, 20);
            this.lblLocationCaption.TabIndex = 5;
            this.lblLocationCaption.Text = "Vị trí:";
            //
            // lblLocationValue
            //
            // AutoEllipsis: duong dan thu muc cha co the rat dai (nhieu cap thu
            // muc) - hien "..." o giua thay vi de tran/xuong dong, giong cach
            // Windows Explorer rut gon duong dan dai trong hop thoai Properties.
            this.lblLocationValue.AutoEllipsis = true;
            this.lblLocationValue.Location = new System.Drawing.Point(120, 100);
            this.lblLocationValue.Name = "lblLocationValue";
            this.lblLocationValue.Size = new System.Drawing.Size(276, 20);
            this.lblLocationValue.TabIndex = 6;
            this.lblLocationValue.Text = "lblLocationValue";
            //
            // lblSizeCaption
            //
            this.lblSizeCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSizeCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblSizeCaption.Location = new System.Drawing.Point(16, 124);
            this.lblSizeCaption.Name = "lblSizeCaption";
            this.lblSizeCaption.Size = new System.Drawing.Size(100, 20);
            this.lblSizeCaption.TabIndex = 7;
            this.lblSizeCaption.Text = "Kích thước:";
            //
            // lblSizeValue
            //
            this.lblSizeValue.Location = new System.Drawing.Point(120, 124);
            this.lblSizeValue.Name = "lblSizeValue";
            this.lblSizeValue.Size = new System.Drawing.Size(276, 20);
            this.lblSizeValue.TabIndex = 8;
            this.lblSizeValue.Text = "lblSizeValue";
            //
            // lblContentsCaption
            //
            this.lblContentsCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblContentsCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblContentsCaption.Location = new System.Drawing.Point(16, 148);
            this.lblContentsCaption.Name = "lblContentsCaption";
            this.lblContentsCaption.Size = new System.Drawing.Size(100, 20);
            this.lblContentsCaption.TabIndex = 9;
            this.lblContentsCaption.Text = "Nội dung:";
            //
            // lblContentsValue
            //
            this.lblContentsValue.Location = new System.Drawing.Point(120, 148);
            this.lblContentsValue.Name = "lblContentsValue";
            this.lblContentsValue.Size = new System.Drawing.Size(276, 20);
            this.lblContentsValue.TabIndex = 10;
            this.lblContentsValue.Text = "lblContentsValue";
            //
            // lblCreatedCaption
            //
            this.lblCreatedCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCreatedCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblCreatedCaption.Location = new System.Drawing.Point(16, 172);
            this.lblCreatedCaption.Name = "lblCreatedCaption";
            this.lblCreatedCaption.Size = new System.Drawing.Size(100, 20);
            this.lblCreatedCaption.TabIndex = 11;
            this.lblCreatedCaption.Text = "Ngày tạo:";
            //
            // lblCreatedValue
            //
            this.lblCreatedValue.Location = new System.Drawing.Point(120, 172);
            this.lblCreatedValue.Name = "lblCreatedValue";
            this.lblCreatedValue.Size = new System.Drawing.Size(276, 20);
            this.lblCreatedValue.TabIndex = 12;
            this.lblCreatedValue.Text = "lblCreatedValue";
            //
            // lblModifiedCaption
            //
            this.lblModifiedCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblModifiedCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblModifiedCaption.Location = new System.Drawing.Point(16, 196);
            this.lblModifiedCaption.Name = "lblModifiedCaption";
            this.lblModifiedCaption.Size = new System.Drawing.Size(100, 20);
            this.lblModifiedCaption.TabIndex = 13;
            this.lblModifiedCaption.Text = "Ngày sửa đổi:";
            //
            // lblModifiedValue
            //
            this.lblModifiedValue.Location = new System.Drawing.Point(120, 196);
            this.lblModifiedValue.Name = "lblModifiedValue";
            this.lblModifiedValue.Size = new System.Drawing.Size(276, 20);
            this.lblModifiedValue.TabIndex = 14;
            this.lblModifiedValue.Text = "lblModifiedValue";
            //
            // lblAccessedCaption
            //
            this.lblAccessedCaption.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAccessedCaption.ForeColor = FileExplorerApp.Helpers.AppTheme.TextSecondary;
            this.lblAccessedCaption.Location = new System.Drawing.Point(16, 220);
            this.lblAccessedCaption.Name = "lblAccessedCaption";
            this.lblAccessedCaption.Size = new System.Drawing.Size(100, 20);
            this.lblAccessedCaption.TabIndex = 15;
            this.lblAccessedCaption.Text = "Ngày truy cập:";
            //
            // lblAccessedValue
            //
            this.lblAccessedValue.Location = new System.Drawing.Point(120, 220);
            this.lblAccessedValue.Name = "lblAccessedValue";
            this.lblAccessedValue.Size = new System.Drawing.Size(276, 20);
            this.lblAccessedValue.TabIndex = 16;
            this.lblAccessedValue.Text = "lblAccessedValue";
            //
            // pnlSeparatorBottom
            //
            this.pnlSeparatorBottom.BackColor = FileExplorerApp.Helpers.AppTheme.Border;
            this.pnlSeparatorBottom.Location = new System.Drawing.Point(16, 252);
            this.pnlSeparatorBottom.Name = "pnlSeparatorBottom";
            this.pnlSeparatorBottom.Size = new System.Drawing.Size(380, 1);
            this.pnlSeparatorBottom.TabIndex = 17;
            //
            // grpAttributes
            //
            this.grpAttributes.Controls.Add(this.chkReadOnly);
            this.grpAttributes.Controls.Add(this.chkHidden);
            this.grpAttributes.Controls.Add(this.chkSystem);
            this.grpAttributes.Controls.Add(this.chkArchive);
            this.grpAttributes.Location = new System.Drawing.Point(16, 268);
            this.grpAttributes.Name = "grpAttributes";
            this.grpAttributes.Size = new System.Drawing.Size(380, 112);
            this.grpAttributes.TabIndex = 18;
            this.grpAttributes.TabStop = false;
            this.grpAttributes.Text = "Thuộc tính";
            //
            // chkReadOnly
            //
            this.chkReadOnly.Location = new System.Drawing.Point(16, 28);
            this.chkReadOnly.Name = "chkReadOnly";
            this.chkReadOnly.Size = new System.Drawing.Size(170, 24);
            this.chkReadOnly.TabIndex = 0;
            this.chkReadOnly.Text = "Chỉ đọc (Read-only)";
            //
            // chkHidden
            //
            this.chkHidden.Location = new System.Drawing.Point(192, 28);
            this.chkHidden.Name = "chkHidden";
            this.chkHidden.Size = new System.Drawing.Size(170, 24);
            this.chkHidden.TabIndex = 1;
            this.chkHidden.Text = "Ẩn (Hidden)";
            //
            // chkSystem
            //
            this.chkSystem.Location = new System.Drawing.Point(16, 56);
            this.chkSystem.Name = "chkSystem";
            this.chkSystem.Size = new System.Drawing.Size(170, 24);
            this.chkSystem.TabIndex = 2;
            this.chkSystem.Text = "Hệ thống (System)";
            //
            // chkArchive
            //
            this.chkArchive.Location = new System.Drawing.Point(192, 56);
            this.chkArchive.Name = "chkArchive";
            this.chkArchive.Size = new System.Drawing.Size(170, 24);
            this.chkArchive.TabIndex = 3;
            this.chkArchive.Text = "Lưu trữ (Archive)";
            //
            // btnOK
            //
            this.btnOK.Location = new System.Drawing.Point(140, 392);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(84, 32);
            this.btnOK.TabIndex = 19;
            this.btnOK.Text = "OK";
            //
            // btnCancel
            //
            this.btnCancel.Location = new System.Drawing.Point(228, 392);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 32);
            this.btnCancel.TabIndex = 20;
            this.btnCancel.Text = "Hủy";
            //
            // btnApply
            //
            this.btnApply.Location = new System.Drawing.Point(316, 392);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(80, 32);
            this.btnApply.TabIndex = 21;
            this.btnApply.Text = "Áp dụng";
            //
            // PropertiesForm
            //
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = FileExplorerApp.Helpers.AppTheme.Background;
            this.ClientSize = new System.Drawing.Size(412, 440);
            this.Controls.Add(this.picIcon);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.pnlSeparatorTop);
            this.Controls.Add(this.lblTypeCaption);
            this.Controls.Add(this.lblTypeValue);
            this.Controls.Add(this.lblLocationCaption);
            this.Controls.Add(this.lblLocationValue);
            this.Controls.Add(this.lblSizeCaption);
            this.Controls.Add(this.lblSizeValue);
            this.Controls.Add(this.lblContentsCaption);
            this.Controls.Add(this.lblContentsValue);
            this.Controls.Add(this.lblCreatedCaption);
            this.Controls.Add(this.lblCreatedValue);
            this.Controls.Add(this.lblModifiedCaption);
            this.Controls.Add(this.lblModifiedValue);
            this.Controls.Add(this.lblAccessedCaption);
            this.Controls.Add(this.lblAccessedValue);
            this.Controls.Add(this.pnlSeparatorBottom);
            this.Controls.Add(this.grpAttributes);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = FileExplorerApp.Helpers.AppTheme.TextPrimary;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thuộc tính";
            ((System.ComponentModel.ISupportInitialize)(this.picIcon)).EndInit();
            this.grpAttributes.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
