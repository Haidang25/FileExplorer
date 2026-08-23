using System.Drawing;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Bang mau dung chung cho toan bo ung dung, theo tai lieu tham chieu
    /// "00_He_Thong_Mau_Sac.md". Day la bang mau DUY NHAT cho ca 5 man hinh
    /// (MainForm, PropertiesForm, SearchForm, LogForm, SettingsForm) — moi noi
    /// can mau deu phai lay tu day, khong tu dinh nghia rieng, de dam bao dong
    /// bo giua Light/Dark mode va giua cac form.
    ///
    /// Nguyen tac: cung mot tong tim lam diem nhan thuong hieu o ca hai che do,
    /// chi khac nhau o mau nen/mau chu de dam bao tuong phan dung chuan.
    /// </summary>
    public static class AppTheme
    {
        /// <summary>Bang mau — Light Mode.</summary>
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

        /// <summary>Bang mau — Dark Mode.</summary>
        public static class Dark
        {
            /// <summary>Nen chinh cua Form. #0D0F1A</summary>
            public static readonly Color Background = ColorTranslator.FromHtml("#0D0F1A");

            /// <summary>Nen be mat: MenuStrip, ToolStrip, StatusStrip, GroupBox... #14172A</summary>
            public static readonly Color Surface = ColorTranslator.FromHtml("#14172A");

            /// <summary>Vien control, duong ke phan cach. #2A2E45</summary>
            public static readonly Color Border = ColorTranslator.FromHtml("#2A2E45");

            /// <summary>Chu chinh: ten tep, noi dung chinh. #F3F4F6</summary>
            public static readonly Color TextPrimary = ColorTranslator.FromHtml("#F3F4F6");

            /// <summary>Chu phu: ngay thang, chu thich, placeholder. #9CA3AF</summary>
            public static readonly Color TextSecondary = ColorTranslator.FromHtml("#9CA3AF");

            /// <summary>
            /// Diem nhan (accent): nut chinh, icon logo, vien focus.
            /// Sang hon ban Light (#7C6FF0) de noi ro tren nen toi.
            /// </summary>
            public static readonly Color Accent = ColorTranslator.FromHtml("#7C6FF0");

            /// <summary>Nen dong dang duoc chon trong ListView. #2D2A55</summary>
            public static readonly Color SelectedRow = ColorTranslator.FromHtml("#2D2A55");

            /// <summary>Trang thai thanh cong (✓). #34D399</summary>
            public static readonly Color Success = ColorTranslator.FromHtml("#34D399");

            /// <summary>Trang thai loi/that bai (✕). #F87171</summary>
            public static readonly Color Error = ColorTranslator.FromHtml("#F87171");
        }

        /// <summary>
        /// Che do giao dien dang duoc ap dung cho toan ung dung. Gia tri khoi tao
        /// duoc doc tu Properties.Settings.Default.IsDarkMode (nguoi dung chon o
        /// SettingsForm, nhom "Giao dien", va duoc luu lai giua cac lan chay).
        ///
        /// Khi doi gia tri nay luc dang chay (SettingsForm.btnSave_Click), goi lai
        /// MainForm.ApplyTheme() (va ApplyTheme() cua cac Form dang mo khac, neu
        /// co) de giao dien cap nhat ngay, vi cac Form da mo truoc do khong tu
        /// dong ve lai khi thuoc tinh nay thay doi.
        /// </summary>
        public static bool IsDarkMode { get; set; } = FileExplorerApp.Properties.Settings.Default.IsDarkMode;

        /// <summary>Nen chinh theo che do hien tai (Light/Dark).</summary>
        public static Color Background => IsDarkMode ? Dark.Background : Light.Background;

        /// <summary>Nen be mat theo che do hien tai (Light/Dark).</summary>
        public static Color Surface => IsDarkMode ? Dark.Surface : Light.Surface;

        /// <summary>Mau vien theo che do hien tai (Light/Dark).</summary>
        public static Color Border => IsDarkMode ? Dark.Border : Light.Border;

        /// <summary>Mau chu chinh theo che do hien tai (Light/Dark).</summary>
        public static Color TextPrimary => IsDarkMode ? Dark.TextPrimary : Light.TextPrimary;

        /// <summary>Mau chu phu theo che do hien tai (Light/Dark).</summary>
        public static Color TextSecondary => IsDarkMode ? Dark.TextSecondary : Light.TextSecondary;

        /// <summary>Mau diem nhan (accent) theo che do hien tai (Light/Dark).</summary>
        public static Color Accent => IsDarkMode ? Dark.Accent : Light.Accent;

        /// <summary>Mau nen dong duoc chon theo che do hien tai (Light/Dark).</summary>
        public static Color SelectedRow => IsDarkMode ? Dark.SelectedRow : Light.SelectedRow;

        /// <summary>Mau trang thai thanh cong theo che do hien tai (Light/Dark).</summary>
        public static Color Success => IsDarkMode ? Dark.Success : Light.Success;

        /// <summary>Mau trang thai loi theo che do hien tai (Light/Dark).</summary>
        public static Color Error => IsDarkMode ? Dark.Error : Light.Error;
    }
}
