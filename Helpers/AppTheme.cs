using System.Drawing;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Bang mau dung chung cho toan bo ung dung, theo tai lieu tham chieu
    /// "00_He_Thong_Mau_Sac.md". Day la bang mau DUY NHAT cho ca 5 man hinh
    /// (MainForm, PropertiesForm, SearchForm, LogForm, SettingsForm) — moi noi
    /// can mau deu phai lay tu day, khong tu dinh nghia rieng, de dam bao dong
    /// bo giua cac form.
    ///
    /// GHI CHU: truoc day co them mot bang mau Dark Mode (theo yeu cau nguoi
    /// dung "bỏ đi phần giao diện dark mode") - da BO, chi con lai DUY NHAT
    /// bang mau Light ben duoi, cac thuoc tinh (Background/Surface/...) gio
    /// tra thang gia tri Light tuong ung, khong con doc/ghi
    /// Properties.Settings.Default.IsDarkMode nua.
    /// </summary>
    public static class AppTheme
    {
        /// <summary>Bang mau duy nhat cua ung dung.</summary>
        public static class Light
        {
            /// <summary>Nen chinh cua Form. #F5F6FA</summary>
            public static readonly Color Background = ColorTranslator.FromHtml("#F5F6FA");

            /// <summary>Nen be mat: MenuStrip, ToolStrip, StatusStrip, GroupBox... #FFFFFF</summary>
            public static readonly Color Surface = ColorTranslator.FromHtml("#FFFFFF");

            /// <summary>Vien control, duong ke phan cach. #D8DAE3</summary>
            public static readonly Color Border = ColorTranslator.FromHtml("#D8DAE3");

            /// <summary>Chu chinh: ten tep, noi dung chinh. #1F2230</summary>
            public static readonly Color TextPrimary = ColorTranslator.FromHtml("#1F2230");

            /// <summary>Chu phu: ngay thang, chu thich, placeholder. #6B7280</summary>
            public static readonly Color TextSecondary = ColorTranslator.FromHtml("#6B7280");

            /// <summary>Diem nhan (accent): nut chinh, icon logo, vien focus. #6C5CE7</summary>
            public static readonly Color Accent = ColorTranslator.FromHtml("#6C5CE7");

            /// <summary>Nen dong dang duoc chon trong ListView. #EDE9FE</summary>
            public static readonly Color SelectedRow = ColorTranslator.FromHtml("#EDE9FE");

            /// <summary>Trang thai thanh cong (✓). #16A34A</summary>
            public static readonly Color Success = ColorTranslator.FromHtml("#16A34A");

            /// <summary>Trang thai loi/that bai (✕). #DC2626</summary>
            public static readonly Color Error = ColorTranslator.FromHtml("#DC2626");
        }

        /// <summary>Nen chinh cua Form.</summary>
        public static Color Background => Light.Background;

        /// <summary>Nen be mat: MenuStrip, ToolStrip, StatusStrip, GroupBox...</summary>
        public static Color Surface => Light.Surface;

        /// <summary>Mau vien control, duong ke phan cach.</summary>
        public static Color Border => Light.Border;

        /// <summary>Mau chu chinh: ten tep, noi dung chinh.</summary>
        public static Color TextPrimary => Light.TextPrimary;

        /// <summary>Mau chu phu: ngay thang, chu thich, placeholder.</summary>
        public static Color TextSecondary => Light.TextSecondary;

        /// <summary>Mau diem nhan (accent): nut chinh, icon logo, vien focus.</summary>
        public static Color Accent => Light.Accent;

        /// <summary>Mau nen dong duoc chon trong ListView.</summary>
        public static Color SelectedRow => Light.SelectedRow;

        /// <summary>Mau trang thai thanh cong (✓).</summary>
        public static Color Success => Light.Success;

        /// <summary>Mau trang thai loi/that bai (✕).</summary>
        public static Color Error => Light.Error;
    }
}
