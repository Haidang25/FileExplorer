using System.Drawing;
using System.Windows.Forms;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Renderer dung chung cho MenuStrip/ToolStrip/StatusStrip/ContextMenuStrip, to
    /// mau theo AppTheme. Chi dung ProfessionalColorTable — mot API WinForms co san
    /// (khong tu ve GraphicsPath/bo goc/do bong), dung theo dung nguyen tac trong
    /// "00_He_Thong_Mau_Sac.md" muc 3 (chi doi mau qua thuoc tinh co san).
    ///
    /// Cach dung: gan cho tung ToolStrip/MenuStrip/StatusStrip/ContextMenuStrip:
    ///     mnsMain.Renderer = new AppThemeRenderer();
    /// </summary>
    public class AppThemeRenderer : ToolStripProfessionalRenderer
    {
        public AppThemeRenderer() : base(new AppThemeColorTable())
        {
        }

        /// <summary>Ve duong vien mong 1px thay cho vien 3D mac dinh.</summary>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            using (Pen pen = new Pen(AppTheme.Border))
            {
                int y = e.AffectedBounds.Height - 1;
                e.Graphics.DrawLine(pen, 0, y, e.AffectedBounds.Width, y);
            }
        }

        /// <summary>Luon dung mau chu chinh cua AppTheme cho item text.</summary>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = AppTheme.TextPrimary;
            base.OnRenderItemText(e);
        }
    }

    /// <summary>
    /// Bang mau phang (khong gradient nhieu diem dung) cho ToolStripProfessionalRenderer,
    /// lay toan bo tu AppTheme de dam bao dong bo voi phan con lai cua ung dung.
    /// </summary>
    public class AppThemeColorTable : ProfessionalColorTable
    {
        public override Color MenuStripGradientBegin => AppTheme.Surface;
        public override Color MenuStripGradientEnd => AppTheme.Surface;

        public override Color ToolStripGradientBegin => AppTheme.Surface;
        public override Color ToolStripGradientMiddle => AppTheme.Surface;
        public override Color ToolStripGradientEnd => AppTheme.Surface;

        public override Color ImageMarginGradientBegin => AppTheme.Surface;
        public override Color ImageMarginGradientMiddle => AppTheme.Surface;
        public override Color ImageMarginGradientEnd => AppTheme.Surface;

        public override Color StatusStripGradientBegin => AppTheme.Surface;
        public override Color StatusStripGradientEnd => AppTheme.Surface;

        public override Color ToolStripDropDownBackground => AppTheme.Surface;
        public override Color ToolStripBorder => AppTheme.Border;
        public override Color MenuBorder => AppTheme.Border;

        public override Color MenuItemSelected => AppTheme.SelectedRow;
        public override Color MenuItemSelectedGradientBegin => AppTheme.SelectedRow;
        public override Color MenuItemSelectedGradientEnd => AppTheme.SelectedRow;
        public override Color MenuItemPressedGradientBegin => AppTheme.SelectedRow;
        public override Color MenuItemPressedGradientEnd => AppTheme.SelectedRow;
        public override Color MenuItemBorder => AppTheme.Accent;

        public override Color ButtonSelectedHighlight => AppTheme.SelectedRow;
        public override Color ButtonSelectedBorder => AppTheme.Accent;
        public override Color ButtonPressedHighlight => AppTheme.SelectedRow;
        public override Color ButtonPressedBorder => AppTheme.Accent;
        public override Color ButtonCheckedHighlight => AppTheme.SelectedRow;
        public override Color ButtonCheckedGradientBegin => AppTheme.SelectedRow;
        public override Color ButtonCheckedGradientEnd => AppTheme.SelectedRow;

        public override Color SeparatorDark => AppTheme.Border;
        public override Color SeparatorLight => AppTheme.Border;

        public override Color OverflowButtonGradientBegin => AppTheme.Surface;
        public override Color OverflowButtonGradientMiddle => AppTheme.Surface;
        public override Color OverflowButtonGradientEnd => AppTheme.Surface;

        public override Color GripDark => AppTheme.Border;
        public override Color GripLight => AppTheme.Surface;
    }
}
