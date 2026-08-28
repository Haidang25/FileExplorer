using System;
using System.Collections;
using System.Windows.Forms;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// IComparer dung cho ListView.ListViewItemSorter - sap xep cac ListViewItem cua
    /// lvwFiles (MainForm) theo mot cot duoc chi dinh (click vao header cot), ho tro
    /// doi chieu tang/giam khi click lai cung mot cot. Dung cho ListView co
    /// ImageKey/Tag danh dau thu muc (xem MainForm.LoadListViewFiles: item cua thu
    /// muc dung ImageKey "folder") - LUON dat thu muc len truoc file bat ke sap xep
    /// theo cot nao hay chieu nao, giong hanh vi cua Windows Explorer.
    /// </summary>
    /// <remarks>
    /// Cach dung (trong MainForm):
    /// 1. Khoi tao mot instance va gan cho lvwFiles.ListViewItemSorter.
    /// 2. Trong lvwFiles_ColumnClick, goi SetSortColumn(e.Column) roi lvwFiles.Sort().
    ///
    /// Ve du lieu de so sanh: voi cot Ten/Loai, so sanh truc tiep chuoi hien thi
    /// (Text/SubItems[i].Text). Voi cot Kich thuoc/Ngay sua, KHONG so sanh chuoi da
    /// dinh dang (SizeFormatted, FormatHelper.FormatDate) vi se cho ket qua sai (VD:
    /// "1 KB" so sanh chuoi lon hon "20 KB"; ngay dang "dd/MM/yyyy" sap xep chuoi se
    /// sai thu tu thang/nam) - ma doc gia tri goc (long cho Size, DateTime cho
    /// ModifiedDate) duoc luu san trong SubItems[i].Tag boi MainForm.LoadListViewFiles().
    /// </remarks>
    public class ListViewItemComparer : IComparer
    {
        /// <summary>
        /// ImageKey dung de danh dau mot ListViewItem la thu muc (xem
        /// MainForm.LoadListViewFiles: new ListViewItem(entry.Name, "folder")) -
        /// dung de luon xep thu muc len truoc file, khong phu thuoc cot/chieu sap xep.
        /// </summary>
        private const string FolderImageKey = "folder";

        /// <summary>Chi so cot dang duoc dung de sap xep (0 = Ten, mac dinh).</summary>
        private int _sortColumn;

        /// <summary>True: sap xep giam dan; False (mac dinh): tang dan.</summary>
        private bool _descending;

        /// <summary>
        /// Chon lai cot de sap xep - neu la CUNG cot vua sap xep truoc do thi doi
        /// chieu (tang &lt;-&gt; giam), giong hanh vi click header cua Windows
        /// Explorer; neu la cot KHAC thi luon bat dau lai tu tang dan.
        /// </summary>
        /// <param name="columnIndex">Chi so cot vua duoc click (ColumnClickEventArgs.Column).</param>
        public void SetSortColumn(int columnIndex)
        {
            if (_sortColumn == columnIndex)
            {
                _descending = !_descending;
            }
            else
            {
                _sortColumn = columnIndex;
                _descending = false;
            }
        }

        /// <summary>Cot dang duoc dung de sap xep - dung cho MainForm ve lai mui ten chi huong sap xep tren header (neu can).</summary>
        public int SortColumn => _sortColumn;

        /// <summary>True neu dang sap xep giam dan - dung cho MainForm ve lai mui ten chi huong sap xep tren header (neu can).</summary>
        public bool Descending => _descending;

        public int Compare(object x, object y)
        {
            var itemX = x as ListViewItem;
            var itemY = y as ListViewItem;
            if (itemX == null || itemY == null)
                return 0;

            bool isDirectoryX = itemX.ImageKey == FolderImageKey;
            bool isDirectoryY = itemY.ImageKey == FolderImageKey;

            // Thu muc luon truoc file, KHONG phu thuoc cot/chieu dang sap xep - giu
            // dung "quy uoc" cua Windows Explorer du nguoi dung sap theo Ten, Kich
            // thuoc, Loai hay Ngay sua.
            if (isDirectoryX != isDirectoryY)
                return isDirectoryX ? -1 : 1;

            int result = CompareByColumn(itemX, itemY, _sortColumn);
            return _descending ? -result : result;
        }

        /// <summary>
        /// So sanh 2 item CUNG LOAI (ca 2 la thu muc, hoac ca 2 la file) theo gia tri
        /// cua cot columnIndex - luon theo chieu TANG DAN (dao chieu giam dan da
        /// duoc xu ly rieng o Compare()).
        /// </summary>
        private static int CompareByColumn(ListViewItem itemX, ListViewItem itemY, int columnIndex)
        {
            // SubItems[0] la chinh Text cua ListViewItem (cot Ten) - cac cot con lai
            // (Kich thuoc, Loai, Ngay sua) la SubItems[1], [2], [3] tuong ung thu tu
            // khai bao trong MainForm.Designer.cs (colName, colSize, colType, colModified).
            switch (columnIndex)
            {
                case 1: // Kich thuoc - so sanh theo gia tri long goc (Tag), khong theo chuoi da dinh dang.
                    return CompareTagAs<long>(itemX, itemY, columnIndex);

                case 3: // Ngay sua - so sanh theo gia tri DateTime goc (Tag).
                    return CompareTagAs<DateTime>(itemX, itemY, columnIndex);

                case 0: // Ten
                case 2: // Loai
                default:
                    string textX = columnIndex < itemX.SubItems.Count ? itemX.SubItems[columnIndex].Text : string.Empty;
                    string textY = columnIndex < itemY.SubItems.Count ? itemY.SubItems[columnIndex].Text : string.Empty;
                    return string.Compare(textX, textY, StringComparison.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Doc SubItems[columnIndex].Tag cua ca 2 item, ep ve kieu T (IComparable) roi
        /// so sanh - neu Tag bi thieu/khong dung kieu (VD: du lieu cu chua duoc gan
        /// Tag) thi coi nhu 0 muc do bang nhau, tranh nem exception lam gay ca qua
        /// trinh Sort() cua ListView.
        /// </summary>
        private static int CompareTagAs<T>(ListViewItem itemX, ListViewItem itemY, int columnIndex) where T : IComparable
        {
            object tagX = columnIndex < itemX.SubItems.Count ? itemX.SubItems[columnIndex].Tag : null;
            object tagY = columnIndex < itemY.SubItems.Count ? itemY.SubItems[columnIndex].Tag : null;

            if (tagX is T valueX && tagY is T valueY)
                return valueX.CompareTo(valueY);

            return 0;
        }
    }
}
