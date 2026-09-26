using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Phong to giao dien (co chu, icon, kich thuoc control) cho TOAN BO ung dung
    /// tu MOT cho duy nhat - muc dich: khi trinh chieu do an qua may chieu, nguoi
    /// xem ngoi xa van doc ro. Moi Form goi <see cref="Apply"/> ngay sau
    /// InitializeComponent().
    ///
    /// Cach hoat dong: cac Form deu dung AutoScaleMode.Font, nen khi doi Font cua
    /// Form, WinForms TU DONG gian kich thuoc/vi tri moi control con theo ty le
    /// co chu moi / co chu cu. Cac control co Font RIENG (dam, Consolas...) va
    /// ToolStrip (khong ke thua Font cua Form) duoc phong to them bang tay.
    /// </summary>
    public static class UiScale
    {
        /// <summary>
        /// Co chu (pt) khi chay ung dung. Cac Form duoc thiet ke o 9pt, dat 12pt
        /// = phong to ~1.33 lan. Muon to/nho hon chi can doi so nay (VD 11F, 13F).
        /// </summary>
        public const float FontSize = 12F;

        /// <summary>Co chu cac Form duoc thiet ke trong Designer.</summary>
        public const float DesignFontSize = 9F;

        /// <summary>Ty le phong to so voi thiet ke goc.</summary>
        public static float Factor => FontSize / DesignFontSize;

        /// <summary>Phong to mot gia tri pixel theo <see cref="Factor"/>.</summary>
        public static int Scale(int value) => (int)Math.Round(value * Factor);

        /// <summary>Phong to toan bo mot Form (goi ngay sau InitializeComponent).</summary>
        public static void Apply(Form form)
        {
            if (form == null)
                return;

            float factor = FontSize / form.Font.SizeInPoints;
            if (factor <= 1.01f)
                return; // Form da du to (hoac da duoc phong to roi) - khong lam gi.

            // 1. Ghi lai cac control/ToolStripItem co Font RIENG (khac Font cua cha)
            //    TRUOC khi doi Font cua Form - sau khi doi se khong phan biet duoc nua.
            var explicitFonts = new List<KeyValuePair<Control, Font>>();
            var explicitItemFonts = new List<KeyValuePair<ToolStripItem, Font>>();
            CollectExplicitFonts(form, explicitFonts, explicitItemFonts);

            // 2. Doi Font cua Form -> AutoScaleMode.Font tu gian layout cua moi control.
            form.Font = new Font("Segoe UI", FontSize, form.Font.Style);

            // 3. Phong to cac Font rieng theo ty le so voi thiet ke goc (9pt -> FontSize).
            foreach (var pair in explicitFonts)
                pair.Key.Font = ScaleFont(pair.Value, Factor);
            foreach (var pair in explicitItemFonts)
                pair.Key.Font = ScaleFont(pair.Value, Factor);

            // 4. Icon tren ToolStrip va do rong cot ListView (khong tu gian theo Font).
            ScaleToolStripsAndColumns(form, Factor);

            // 5. Khong de cua so vuot qua man hinh (VD may chieu 1024x768).
            FitToScreen(form);
        }

        /// <summary>
        /// Phong to mot ContextMenuStrip (menu chuot phai) - khong nam trong cay
        /// Controls cua Form nen <see cref="Apply"/> khong tu tim thay.
        /// </summary>
        public static void ApplyToContextMenu(ContextMenuStrip menu)
        {
            if (menu == null)
                return;

            float factor = FontSize / menu.Font.SizeInPoints;
            if (factor <= 1.01f)
                return;

            menu.Font = ScaleFont(menu.Font, factor);
            menu.ImageScalingSize = new Size(Scale(menu.ImageScalingSize.Width), Scale(menu.ImageScalingSize.Height));
        }

        private static void CollectExplicitFonts(Control parent,
            List<KeyValuePair<Control, Font>> controls, List<KeyValuePair<ToolStripItem, Font>> items)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is ToolStrip || !child.Font.Equals(parent.Font))
                    controls.Add(new KeyValuePair<Control, Font>(child, child.Font));

                if (child is ToolStrip toolStrip)
                {
                    foreach (ToolStripItem item in toolStrip.Items)
                    {
                        if (!item.Font.Equals(toolStrip.Font))
                            items.Add(new KeyValuePair<ToolStripItem, Font>(item, item.Font));
                    }
                }

                CollectExplicitFonts(child, controls, items);
            }
        }

        private static void ScaleToolStripsAndColumns(Control parent, float factor)
        {
            foreach (Control child in parent.Controls)
            {
                if (child is ToolStrip toolStrip)
                {
                    toolStrip.ImageScalingSize = new Size(
                        (int)Math.Round(toolStrip.ImageScalingSize.Width * factor),
                        (int)Math.Round(toolStrip.ImageScalingSize.Height * factor));
                }
                else if (child is ListView listView)
                {
                    foreach (ColumnHeader column in listView.Columns)
                        column.Width = (int)Math.Round(column.Width * factor);
                }

                ScaleToolStripsAndColumns(child, factor);
            }
        }

        private static void FitToScreen(Form form)
        {
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            if (form.Width > workingArea.Width)
                form.Width = workingArea.Width;
            if (form.Height > workingArea.Height)
                form.Height = workingArea.Height;
        }

        private static Font ScaleFont(Font font, float factor)
        {
            return new Font(font.FontFamily, font.SizeInPoints * factor, font.Style, GraphicsUnit.Point);
        }
    }
}
