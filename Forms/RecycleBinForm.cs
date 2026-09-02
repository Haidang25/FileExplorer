using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh xem noi dung Recycle Bin (menu Cong cu > Thung rac), doc du
    /// lieu tu <see cref="RecycleBinService.GetRecycleBinItems"/>. Cho phep
    /// khoi phuc mot/nhieu muc da chon ve vi tri goc, hoac don trong toan bo
    /// Thung rac.
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE: mirror cau truc cua LogForm (doc TOAN BO danh
    /// sach mot lan vao _allItems, ve len ListView, "Lam moi" doc lai tu dia)
    /// de nhat quan voi Form xem-danh-sach-chi-doc khac da co trong ung dung,
    /// thay vi tao mot khuon mau moi.
    ///
    /// QUYET DINH THIET KE - CHAY BAT DONG BO TREN THREAD STA RIENG (KHONG
    /// dung Task.Run/threadpool thong thuong): RecycleBinService.GetRecycleBinItems/
    /// RestoreFromRecycleBin/EmptyRecycleBin goi Shell32 COM automation
    /// (Shell.Application) - loai COM nay yeu cau chay tren mot thread STA
    /// (Single-Threaded Apartment) co message pump, giong UI thread cua
    /// WinForms, KHONG phai MTA (Multi-Threaded Apartment) cua threadpool
    /// thong thuong (Task.Run) - goi tren MTA co the nem loi hoac cham/treo
    /// do phai marshal qua apartment khac. Vi vay dung RunOnStaThreadAsync
    /// (tao MOT Thread rieng, dat ApartmentState.STA, chay xong roi thoat)
    /// cho MOI lan goi vao RecycleBinService, thay vi goi truc tiep tren UI
    /// thread (nguyen nhan ung dung bi "đơ" truoc khi sua: Thung rac cua
    /// Windows co the co hang tram/nghin muc, moi muc can nhieu lan goi COM
    /// qua late binding (Type.InvokeMember) de doc Name/ExtendedProperty/
    /// IsFolder/Size - tong thoi gian co the len den vai chuc giay, chay
    /// dong bo NGAY TRONG constructor/Click handler tren UI thread se lam
    /// toan bo cua so (ke ca thanh tieu de) khong phan hoi cho den khi xong).
    /// </remarks>
    public partial class RecycleBinForm : Form
    {
        private readonly RecycleBinService _recycleBinService = new RecycleBinService();
        private readonly RecycleBinItemComparer _sorter = new RecycleBinItemComparer();
        private List<RecycleBinItemModel> _allItems = new List<RecycleBinItemModel>();

        public RecycleBinForm()
        {
            InitializeComponent();
            ApplyTheme();

            // Gan ListViewItemSorter TRUOC khi nap du lieu - lvwItems se tu
            // giu thu tu sap xep MOI LAN Items.Add duoc goi (WinForms tu lam
            // dieu nay khi ListViewItemSorter da duoc gan, xem PopulateListView),
            // khong can goi rieng lvwItems.Sort() sau khi nap, giong cach
            // MainForm.LoadListViewFiles dang lam voi _listViewSorter.
            lvwItems.ListViewItemSorter = _sorter;
            UpdateColumnSortIndicators();

            // Fire-and-forget (khong await trong constructor - constructor
            // khong the la async) - Form se hien NGAY (do ShowDialog goi sau
            // khi constructor tra ve), con LoadItemsAsync tiep tuc chay ngam
            // va tu cap nhat lvwItems/lblStatus khi xong. LoadItemsAsync da
            // tu bat toan bo Exception ben trong (xem than ham), nen KHONG co
            // rui ro "unobserved task exception" khi bo qua ket qua Task nhu
            // the nay.
            _ = LoadItemsAsync();
        }

        /// <summary>
        /// Click vao header cot cua lvwItems: sap xep lai danh sach theo cot
        /// do, click lai CUNG mot cot se doi chieu tang/giam - giong hanh vi
        /// da dung cho lvwFiles cua MainForm (xem Helpers/ListViewItemComparer.cs),
        /// nhung dung comparer RIENG (RecycleBinItemComparer, xem cuoi file) vi
        /// cot cua RecycleBinForm hoan toan khac MainForm.
        /// </summary>
        private void lvwItems_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            _sorter.SetSortColumn(e.Column);
            lvwItems.Sort();
            UpdateColumnSortIndicators();
        }

        /// <summary>
        /// Ten goc (khong co mui ten) cua tung cot tren lvwItems - dung de
        /// dung lai khi ve/xoa mui ten sap xep, giong quy uoc
        /// MainForm.ColumnBaseNames.
        /// </summary>
        private static readonly string[] ColumnBaseNames = { "Tên", "Vị trí gốc", "Ngày xóa", "Kích thước", "Loại" };

        /// <summary>
        /// Cap nhat lai Text cua ca 5 ColumnHeader tren lvwItems de hien mui
        /// ten chi chieu sap xep (▲ tang dan, ▼ giam dan) o cot dang duoc
        /// dung de sap xep - xem giai thich chi tiet tai
        /// MainForm.UpdateColumnSortIndicators (cung ly do/cach lam).
        /// </summary>
        private void UpdateColumnSortIndicators()
        {
            var columns = new[] { colName, colOriginalPath, colDeletedDate, colSize, colType };

            for (int i = 0; i < columns.Length; i++)
            {
                string arrow = i == _sorter.SortColumn
                    ? (_sorter.Descending ? " ▼" : " ▲")
                    : string.Empty;

                columns[i].Text = ColumnBaseNames[i] + arrow;
            }
        }

        /// <summary>
        /// Ap dung AppTheme cho lvwItems va 2 nut hanh dong chinh (btnRestore/
        /// btnEmptyRecycleBin), giong quy uoc da dung tai LogForm.ApplyTheme.
        /// </summary>
        private void ApplyTheme()
        {
            lvwItems.BackColor = AppTheme.Surface;
            lvwItems.ForeColor = AppTheme.TextPrimary;
            lvwItems.BorderStyle = BorderStyle.FixedSingle;

            btnRestore.FlatStyle = FlatStyle.Flat;
            btnRestore.FlatAppearance.BorderColor = AppTheme.Accent;
            btnRestore.BackColor = AppTheme.Accent;
            btnRestore.ForeColor = System.Drawing.Color.White;

            btnEmptyRecycleBin.FlatStyle = FlatStyle.Flat;
            btnEmptyRecycleBin.FlatAppearance.BorderColor = AppTheme.Error;
            btnEmptyRecycleBin.BackColor = AppTheme.Error;
            btnEmptyRecycleBin.ForeColor = System.Drawing.Color.White;
        }

        /// <summary>
        /// Chay mot ham CHI CO KET QUA (khong can tham so) tren MOT Thread
        /// STA rieng roi tra ve Task<T> hoan thanh khi ham do xong - dung
        /// chung cho ca 3 thao tac goi RecycleBinService (doc danh sach/khoi
        /// phuc/don trong), xem giai thich ly do can STA tai remarks dau lop.
        /// Loi (Exception) ben trong action se duoc chuyen sang Task loi
        /// (qua TaskCompletionSource.SetException) de noi await tiep tuc bat
        /// duoc bang try/catch thong thuong, khong bi "nuot" tren thread rieng.
        /// </summary>
        private static Task<T> RunOnStaThreadAsync<T>(Func<T> action)
        {
            var tcs = new TaskCompletionSource<T>();
            var thread = new Thread(() =>
            {
                try
                {
                    tcs.SetResult(action());
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
            return tcs.Task;
        }

        /// <summary>
        /// Bat/tat trang thai "dang xu ly": khoa 3 nut hanh dong (Lam mới/
        /// Khôi phục/Dọn trống - KHONG khoa btnClose, de nguoi dung van dong
        /// duoc Form neu thao tac cham bat thuong, VD Thung rac qua nhieu
        /// muc), doi con chuot thanh hinh cho (UseWaitCursor) va cap nhat
        /// lblStatus voi thong bao dang lam gi.
        /// </summary>
        private void SetBusyState(bool busy, string statusText = null)
        {
            btnRefresh.Enabled = !busy;
            btnRestore.Enabled = !busy;
            btnEmptyRecycleBin.Enabled = !busy;
            UseWaitCursor = busy;

            if (statusText != null)
                lblStatus.Text = statusText;
        }

        /// <summary>
        /// Doc lai TOAN BO danh sach tu RecycleBinService.GetRecycleBinItems()
        /// (tren thread STA rieng - xem RunOnStaThreadAsync) roi ve lai
        /// lvwItems - goi luc mo Form va sau moi thao tac thay doi noi dung
        /// Thung rac (Khoi phuc/Don trong) de danh sach hien thi luon khop
        /// voi thuc te.
        /// </summary>
        private async Task LoadItemsAsync()
        {
            SetBusyState(true, "Đang tải danh sách Thùng rác...");

            List<RecycleBinItemModel> items;
            try
            {
                items = await RunOnStaThreadAsync(() => _recycleBinService.GetRecycleBinItems());
            }
            catch (Exception)
            {
                // GetRecycleBinItems() ban than da tu bat loi va tra ve danh
                // sach rong (xem RecycleBinService) - nhanh catch nay chi de
                // phong xa (defensive) truoc bat ky loi phat sinh ngoai du
                // kien khac (VD tao Thread that bai).
                items = new List<RecycleBinItemModel>();
            }

            // Nguoi dung co the da dong Form trong luc dang tai (btnClose
            // khong bi khoa - xem SetBusyState) - kiem tra IsDisposed truoc
            // khi dong bat ky control nao de tranh ObjectDisposedException.
            if (IsDisposed)
                return;

            _allItems = items;
            PopulateListView(_allItems);

            // Danh sach rong CO THE la vi Thung rac thuc su dang rong (binh
            // thuong), nhung CUNG CO THE la vi GetRecycleBinItems() gap loi
            // COM ma tu nuot (theo thiet ke, xem RecycleBinService.GetRecycleBinItems) -
            // hien LastError (neu co) de nguoi dung/nguoi ho tro biet chinh
            // xac ly do, thay vi chi thay "0 mục" mac dinh gay nham la Thung
            // rac dang rong that su.
            if (_allItems.Count == 0 && _recycleBinService.LastError != null)
            {
                lblStatus.Text = $"Không đọc được Thùng rác (lỗi: {_recycleBinService.LastError})";
            }

            SetBusyState(false);
        }

        /// <summary>
        /// Ve lai lvwItems tu danh sach RecycleBinItemModel, va cap nhat
        /// lblStatus voi tong so muc + tong dung luong dang hien thi.
        /// </summary>
        private void PopulateListView(List<RecycleBinItemModel> items)
        {
            lvwItems.BeginUpdate();
            lvwItems.Items.Clear();

            long totalSize = 0;
            foreach (RecycleBinItemModel model in items)
            {
                var listItem = new ListViewItem(model.Name);
                listItem.SubItems.Add(model.OriginalPath ?? string.Empty);
                listItem.SubItems.Add(FormatHelper.FormatDate(model.DeletedDate));
                listItem.SubItems.Add(model.IsDirectory ? string.Empty : FormatHelper.FormatSize(model.Size));
                listItem.SubItems.Add(model.IsDirectory ? "Thư mục" : "Tệp");
                // Luu ca model goc vao Tag - btnRestore_Click can OriginalPath
                // CHINH XAC (khong phai chuoi da dinh dang lai) de goi
                // RestoreFromRecycleBin.
                listItem.Tag = model;

                lvwItems.Items.Add(listItem);
                totalSize += model.Size;
            }

            lvwItems.EndUpdate();
            lblStatus.Text = $"{items.Count} mục - Tổng dung lượng: {FormatHelper.FormatSize(totalSize)}";
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await LoadItemsAsync();
        }

        /// <summary>
        /// Khoi phuc TOAN BO cac muc dang duoc chon (lvwItems.SelectedItems) ve
        /// vi tri goc - cho phep chon nhieu muc cung luc (lvwItems.MultiSelect =
        /// true, xem Designer.cs) de khoi phuc theo lo, khong bat nguoi dung
        /// phai lam tung muc mot khi can khoi phuc nhieu file/thu muc. Toan bo
        /// vong lap goi RestoreFromRecycleBin chay tren MOT thread STA rieng
        /// (RunOnStaThreadAsync) de khong lam "đơ" giao dien, xem remarks dau lop.
        /// </summary>
        private async void btnRestore_Click(object sender, EventArgs e)
        {
            if (lvwItems.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Vui lòng chọn ít nhất một mục để khôi phục.",
                    "Khôi phục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var modelsToRestore = new List<RecycleBinItemModel>();
            foreach (ListViewItem selected in lvwItems.SelectedItems)
                modelsToRestore.Add((RecycleBinItemModel)selected.Tag);

            SetBusyState(true, $"Đang khôi phục {modelsToRestore.Count} mục...");

            int successCount = 0;
            List<string> failedNames;
            try
            {
                (successCount, failedNames) = await RunOnStaThreadAsync(() =>
                {
                    int success = 0;
                    var failed = new List<string>();

                    foreach (RecycleBinItemModel model in modelsToRestore)
                    {
                        OperationResult result = _recycleBinService.RestoreFromRecycleBin(model.OriginalPath);
                        if (result == OperationResult.Success)
                            success++;
                        else
                            failed.Add(model.Name);
                    }

                    return (success, failed);
                });
            }
            catch (Exception)
            {
                failedNames = new List<string>();
                foreach (RecycleBinItemModel model in modelsToRestore)
                    failedNames.Add(model.Name);
            }

            // Doc lai danh sach NGAY sau khi khoi phuc (du thanh cong mot phan
            // hay toan bo) - cac muc da khoi phuc thanh cong khong con trong
            // Thung rac nua, phai bien mat khoi lvwItems ngay, khong doi nguoi
            // dung tu bam "Lam mới". LoadItemsAsync tu goi SetBusyState(false)
            // khi xong nen khong can goi lai o day.
            await LoadItemsAsync();

            if (IsDisposed)
                return;

            if (failedNames.Count == 0)
            {
                MessageBox.Show(
                    this,
                    $"Đã khôi phục {successCount} mục về vị trí ban đầu.",
                    "Khôi phục",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                // Ap dung ErrorHandler tap trung (xem Helpers/ErrorHandler.cs)
                // thay MessageBox.Show rai rac - kem huong dan khoi phuc thu
                // cong vi RecycleBinService.RestoreFromRecycleBin co the khong
                // tim thay verb "Khôi phục" tren mot so phien ban/ngon ngu
                // Windows (xem remarks tai RecycleBinService.RestoreFromRecycleBin).
                ErrorHandler.Show(
                    this,
                    $"Khôi phục thành công {successCount} mục. Không thể khôi phục {failedNames.Count} mục:\n" +
                    string.Join(", ", failedNames) +
                    "\n\nCó thể vị trí gốc không còn tồn tại, hoặc phiên bản Windows hiện tại không hỗ trợ khôi phục tự động - " +
                    "bạn có thể mở Thùng rác (Recycle Bin) của Windows Explorer để khôi phục các mục này thủ công.",
                    "Khôi phục");
            }
        }

        /// <summary>
        /// Don sach toan bo Recycle Bin (xoa vinh vien) sau khi nguoi dung xac
        /// nhan - day la thao tac KHONG THE HOAN TAC (khac voi DeleteToRecycleBin
        /// thong thuong, cac muc nay se KHONG con cach nao khoi phuc lai duoc
        /// nua), nen bat buoc hoi lai truoc, cung tinh than voi
        /// LogForm.btnClearLogs_Click.
        /// </summary>
        private async void btnEmptyRecycleBin_Click(object sender, EventArgs e)
        {
            if (_allItems.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Thùng rác đang trống.",
                    "Dọn trống thùng rác",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                this,
                $"Bạn có chắc chắn muốn xóa VĨNH VIỄN toàn bộ {_allItems.Count} mục trong Thùng rác?\nHành động này KHÔNG THỂ hoàn tác.",
                "Dọn trống thùng rác",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            SetBusyState(true, "Đang dọn trống Thùng rác...");

            OperationResult result;
            try
            {
                result = await RunOnStaThreadAsync(() => _recycleBinService.EmptyRecycleBin());
            }
            catch (Exception)
            {
                result = OperationResult.Failed;
            }

            await LoadItemsAsync();

            if (IsDisposed)
                return;

            if (result == OperationResult.Success)
            {
                MessageBox.Show(
                    this,
                    "Đã dọn trống Thùng rác.",
                    "Dọn trống thùng rác",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                ErrorHandler.Show(
                    this,
                    "Không thể dọn trống Thùng rác. Vui lòng thử lại sau.",
                    "Dọn trống thùng rác");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// IComparer dung cho lvwItems.ListViewItemSorter - sap xep theo cot
        /// duoc click, ho tro doi chieu tang/giam khi click lai cung mot cot.
        /// Viet RIENG cho RecycleBinForm (khong tai su dung
        /// Helpers/ListViewItemComparer.cs cua MainForm) vi cot va quy uoc
        /// hoan toan khac (5 cot: Tên/Vị trí gốc/Ngày xóa/Kích thước/Loại,
        /// va KHONG can quy uoc "thư mục luôn trước" cua MainForm).
        /// </summary>
        /// <remarks>
        /// QUYET DINH THIET KE - DOC GIA TRI GOC TU item.Tag (RecycleBinItemModel),
        /// KHONG so sanh chuoi da dinh dang hien thi tren SubItems: neu so
        /// sanh truc tiep chuoi "Ngày xóa" (dang "dd/MM/yyyy HH:mm" tu
        /// FormatHelper.FormatDate) hoac "Kích thước" (dang "12.34 MB" tu
        /// FormatHelper.FormatSize), thu tu sap xep se SAI hoan toan so voi
        /// thoi gian/dung luong THAT (VD ngay "31/01/2026" bi xep TRUOC
        /// "05/02/2026" vi so sanh chuoi thay ky tu '3' > '0'; "9 KB" bi xep
        /// SAU "10 KB" vi ky tu '9' > '1') - day chinh la ly do can mot
        /// comparer rieng thay vi de ListView tu sap xep chuoi mac dinh.
        /// PopulateListView da luu san CA MODEL goc vao ListViewItem.Tag
        /// (khong chi luu tung gia tri rieng vao SubItems[i].Tag nhu
        /// ListViewItemComparer cua MainForm) nen doc lai truc tiep
        /// DeletedDate/Size/IsDirectory tu do la du, khong can them Tag rieng
        /// cho tung SubItem.
        ///
        /// Mac dinh sap xep theo Ngày xóa GIẢM DẦN (moi xoa gan day nhat len
        /// dau) - giong thu tu Windows Explorer thuong hien trong Recycle Bin
        /// thuc, thay vi de thu tu ngau nhien tuy Shell32 tra ve (Shell.Application.Items()
        /// khong dam bao mot thu tu ro rang nao).
        /// </remarks>
        private class RecycleBinItemComparer : IComparer
        {
            /// <summary>Chi so cot dang duoc dung de sap xep (2 = Ngày xóa, mac dinh).</summary>
            private int _sortColumn = 2;

            /// <summary>True (mac dinh): sap xep giam dan; False: tang dan.</summary>
            private bool _descending = true;

            /// <summary>
            /// Chon lai cot de sap xep - neu la CUNG cot vua sap xep truoc do
            /// thi doi chieu (tang &lt;-&gt; giam), giong hanh vi click header
            /// cua Windows Explorer; neu la cot KHAC thi luon bat dau lai tu
            /// tang dan.
            /// </summary>
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

            /// <summary>Cot dang duoc dung de sap xep - dung cho RecycleBinForm ve lai mui ten chi huong sap xep tren header.</summary>
            public int SortColumn => _sortColumn;

            /// <summary>True neu dang sap xep giam dan - dung cho RecycleBinForm ve lai mui ten chi huong sap xep tren header.</summary>
            public bool Descending => _descending;

            public int Compare(object x, object y)
            {
                var modelX = (x as ListViewItem)?.Tag as RecycleBinItemModel;
                var modelY = (y as ListViewItem)?.Tag as RecycleBinItemModel;
                if (modelX == null || modelY == null)
                    return 0;

                int result;
                switch (_sortColumn)
                {
                    case 1: // Vị trí gốc
                        result = string.Compare(modelX.OriginalPath, modelY.OriginalPath, StringComparison.OrdinalIgnoreCase);
                        break;

                    case 2: // Ngày xóa - so sanh gia tri DateTime goc, KHONG phai chuoi da dinh dang.
                        result = modelX.DeletedDate.CompareTo(modelY.DeletedDate);
                        break;

                    case 3: // Kích thước - so sanh gia tri long goc, KHONG phai chuoi da dinh dang.
                        result = modelX.Size.CompareTo(modelY.Size);
                        break;

                    case 4: // Loại (Thư mục/Tệp)
                        result = modelX.IsDirectory.CompareTo(modelY.IsDirectory);
                        break;

                    case 0: // Tên
                    default:
                        result = string.Compare(modelX.Name, modelY.Name, StringComparison.OrdinalIgnoreCase);
                        break;
                }

                return _descending ? -result : result;
            }
        }
    }
}
