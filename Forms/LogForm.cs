using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh xem lich su thao tac (menu Cong cu > Xem nhat ky hoat dong),
    /// doc du lieu tu <see cref="LogService.GetLogs"/>. Cho phep loc theo loai
    /// thao tac, ket qua va khoang thoi gian, va xoa toan bo lich su.
    /// </summary>
    /// <remarks>
    /// Thiet ke: doc TOAN BO danh sach log MOT LAN (LoadLogs) roi loc TREN BO
    /// NHO (ApplyFilter) thay vi goi lai LogService.GetLogs(...) rieng cho tung
    /// tieu chi loc - vi nguoi dung co the doi qua lai nhieu bo loc lien tuc
    /// (VD: doi Thao tac roi doi Ket qua) va file log CSV thuong khong qua lon
    /// (du lieu don gian, ghi them dan theo thoi gian su dung ung dung), nen
    /// doc lai tu dia moi lan doi bo loc la khong can thiet - chi doc lai tu
    /// dia khi nguoi dung bam "Lam moi" (VD: sau khi co thao tac moi duoc ghi
    /// trong luc dang mo LogForm) hoac sau khi "Xoa lich su".
    /// </remarks>
    public partial class LogForm : Form
    {
        private readonly LogService _logService = new LogService();
        private List<LogEntryModel> _allLogs = new List<LogEntryModel>();

        /// <summary>
        /// Danh sach dang HIEN THI tren lvwLogs sau khi ap dung bo loc hien tai -
        /// luu lai rieng (thay vi doc lai tu lvwLogs.Items) de btnExportCsv_Click
        /// xuat DUNG nhung gi nguoi dung dang xem (da loc), khong phai toan bo
        /// _allLogs chua loc - hop ly hon vi nguoi dung thuong loc truoc roi moi
        /// xuat dung phan can (VD: chi xuat cac thao tac That bai trong thang nay).
        /// </summary>
        private List<LogEntryModel> _currentFilteredLogs = new List<LogEntryModel>();

        public LogForm()
        {
            InitializeComponent();
            InitializeFilterOptions();
            LoadLogs();
        }

        /// <summary>
        /// Do cboFilterOperation/cboFilterResult voi lua chon "Tat ca" (mac dinh,
        /// khong loc theo tieu chi do) cong voi tung gia tri cua FileOperationType/
        /// OperationResult - dung ComboBox thay vi CheckedListBox vi day la loc
        /// DON GIA TRI (chon 1 trong cac loai), khong phai chon nhieu cung luc.
        /// </summary>
        private void InitializeFilterOptions()
        {
            cboFilterOperation.Items.Add("Tất cả");
            foreach (FileOperationType operation in Enum.GetValues(typeof(FileOperationType)))
            {
                cboFilterOperation.Items.Add(operation);
            }
            cboFilterOperation.SelectedIndex = 0;

            cboFilterResult.Items.Add("Tất cả");
            foreach (OperationResult result in Enum.GetValues(typeof(OperationResult)))
            {
                cboFilterResult.Items.Add(result);
            }
            cboFilterResult.SelectedIndex = 0;

            // Khoang thoi gian mac dinh: tu dau ngay hom nay truoc 30 ngay den
            // cuoi ngay hien tai - du rong de thay lich su gan day ma khong can
            // nguoi dung tu chinh ngay ngay khi vua mo form, nhung van co the mo
            // rong ra qua khu bang cach tu chinh dtpFilterFrom.
            dtpFilterFrom.Value = DateTime.Now.Date.AddDays(-30);
            dtpFilterTo.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
        }

        /// <summary>
        /// Doc lai TOAN BO danh sach log tu LogService.GetLogs() (da sap xep gan
        /// nhat truoc) roi ap dung bo loc hien tai len danh sach vua doc.
        /// </summary>
        private void LoadLogs()
        {
            _allLogs = _logService.GetLogs();
            ApplyFilter();
        }

        /// <summary>
        /// Loc _allLogs theo Thao tac/Ket qua/khoang thoi gian dang chon tren
        /// grpFilters, roi ve lai lvwLogs voi ket qua loc duoc.
        /// </summary>
        private void ApplyFilter()
        {
            DateTime fromDate = dtpFilterFrom.Value;
            DateTime toDate = dtpFilterTo.Value;

            bool filterByOperation = cboFilterOperation.SelectedIndex > 0;
            bool filterByResult = cboFilterResult.SelectedIndex > 0;
            FileOperationType selectedOperation = filterByOperation ? (FileOperationType)cboFilterOperation.SelectedItem : default;
            OperationResult selectedResult = filterByResult ? (OperationResult)cboFilterResult.SelectedItem : default;

            var filtered = _allLogs.FindAll(entry =>
                entry.Timestamp >= fromDate && entry.Timestamp <= toDate
                && (!filterByOperation || entry.Operation == selectedOperation)
                && (!filterByResult || entry.Result == selectedResult));

            _currentFilteredLogs = filtered;
            PopulateListView(filtered);
        }

        /// <summary>
        /// Ve lai lvwLogs tu danh sach LogEntryModel da loc, va cap nhat
        /// lblStatus voi tong so dong dang hien thi.
        /// </summary>
        private void PopulateListView(List<LogEntryModel> entries)
        {
            lvwLogs.BeginUpdate();
            lvwLogs.Items.Clear();

            foreach (LogEntryModel entry in entries)
            {
                var item = new ListViewItem(entry.Timestamp.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                item.SubItems.Add(entry.Operation.ToString());
                item.SubItems.Add(entry.Source ?? string.Empty);
                item.SubItems.Add(entry.Destination ?? string.Empty);
                item.SubItems.Add(GetResultDisplayText(entry.Result));
                item.SubItems.Add(entry.ItemCount.ToString(CultureInfo.InvariantCulture));
                item.SubItems.Add(entry.Duration.HasValue
                    ? entry.Duration.Value.TotalSeconds.ToString("0.##", CultureInfo.InvariantCulture) + "s"
                    : string.Empty);
                item.SubItems.Add(entry.Message ?? string.Empty);

                // To mau dong theo ket qua (giong tinh than Success/Error cua
                // AppTheme) de nguoi dung luot mat nhanh thay ngay thao tac nao
                // that bai/dang chu y ma khong can doc tung dong.
                item.ForeColor = GetResultColor(entry.Result);

                lvwLogs.Items.Add(item);
            }

            lvwLogs.EndUpdate();
            lblStatus.Text = $"{entries.Count} dòng log";
        }

        /// <summary>
        /// Chuoi hien thi tieng Viet cho tung gia tri OperationResult - de doc
        /// hon ten enum tieng Anh thuan tuy tren giao dien nguoi dung.
        /// </summary>
        private static string GetResultDisplayText(OperationResult result)
        {
            switch (result)
            {
                case OperationResult.Success: return "Thành công";
                case OperationResult.PartialSuccess: return "Một phần";
                case OperationResult.Failed: return "Thất bại";
                case OperationResult.Cancelled: return "Đã hủy";
                case OperationResult.Skipped: return "Bỏ qua";
                case OperationResult.AccessDenied: return "Từ chối truy cập";
                case OperationResult.NotFound: return "Không tìm thấy";
                case OperationResult.FileInUse: return "Tệp đang sử dụng";
                case OperationResult.InvalidDestination: return "Đích không hợp lệ";
                default: return result.ToString();
            }
        }

        /// <summary>
        /// Mau chu cho dong log theo ket qua: Success dung AppTheme.Success, cac
        /// ket qua that bai/bi chan (Failed/AccessDenied/NotFound/FileInUse/
        /// InvalidDestination) dung AppTheme.Error, con lai (PartialSuccess/
        /// Cancelled/Skipped - khong hoan toan thanh cong nhung cung khong han
        /// la loi) giu mau chu thuong (TextPrimary) de khong danh dong voi loi
        /// that su.
        /// </summary>
        private static System.Drawing.Color GetResultColor(OperationResult result)
        {
            switch (result)
            {
                case OperationResult.Success:
                    return AppTheme.Success;
                case OperationResult.Failed:
                case OperationResult.AccessDenied:
                case OperationResult.NotFound:
                case OperationResult.FileInUse:
                case OperationResult.InvalidDestination:
                    return AppTheme.Error;
                default:
                    return AppTheme.TextPrimary;
            }
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        /// <summary>
        /// Dua tat ca bo loc ve trang thai mac dinh (Tat ca thao tac, Tat ca ket
        /// qua, 30 ngay gan nhat) roi ap dung lai ngay - tien loi hon bat nguoi
        /// dung tu tay dat lai tung o mot khi muon xem lai toan bo lich su gan day.
        /// </summary>
        private void btnResetFilter_Click(object sender, EventArgs e)
        {
            cboFilterOperation.SelectedIndex = 0;
            cboFilterResult.SelectedIndex = 0;
            dtpFilterFrom.Value = DateTime.Now.Date.AddDays(-30);
            dtpFilterTo.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            ApplyFilter();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
        }

        /// <summary>
        /// Xoa toan bo lich su log (LogService.ClearLogs) sau khi nguoi dung xac
        /// nhan - day la thao tac KHONG THE HOAN TAC (khong co Thung rac cho
        /// lich su log), nen bat buoc hoi lai truoc, khac voi cac thao tac doc
        /// (Loc/Lam moi) khong can xac nhan.
        /// </summary>
        private void btnClearLogs_Click(object sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                this,
                "Bạn có chắc chắn muốn xóa toàn bộ lịch sử thao tác? Hành động này không thể hoàn tác.",
                "Xóa lịch sử",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (confirm != DialogResult.Yes)
                return;

            OperationResult result = _logService.ClearLogs();
            if (result == OperationResult.Success)
            {
                LoadLogs();
                MessageBox.Show(this, "Đã xóa toàn bộ lịch sử thao tác.", "Xóa lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    this,
                    "Không thể xóa file lịch sử (có thể đang được mở bởi chương trình khác). Vui lòng thử lại sau.",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Xuat danh sach log DANG HIEN THI (da loc - xem _currentFilteredLogs)
        /// ra mot file .csv do nguoi dung tu chon vi tri luu, dung CUNG dinh
        /// dang cot voi file log noi bo (xem LogService.LogFileHeader) de nguoi
        /// dung quen thuoc co the mo lai file xuat ra bang chinh cong cu ho da
        /// dung xem file log goc (VD Excel), va de nhat quan trong toan ung dung.
        /// </summary>
        /// <remarks>
        /// Day la file XUAT RIENG (ban sao), khong phai file log goc dang duoc
        /// LogService ghi/doc - vi vay KHONG can khoa WriteLock: file xuat ra la
        /// mot file MOI, doc lap hoan toan voi file log that dang duoc WriteLog
        /// quan ly, khong co nguy co tranh chap ghi voi cac thao tac khac.
        /// </remarks>
        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (_currentFilteredLogs.Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Không có dòng log nào để xuất (danh sách đang hiển thị rỗng). Hãy điều chỉnh lại bộ lọc nếu cần.",
                    "Xuất CSV",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Tệp CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*";
                saveDialog.DefaultExt = "csv";
                saveDialog.AddExtension = true;
                saveDialog.FileName = $"log_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    ExportToCsv(saveDialog.FileName, _currentFilteredLogs);
                    MessageBox.Show(
                        this,
                        $"Đã xuất {_currentFilteredLogs.Count} dòng log ra:\n{saveDialog.FileName}",
                        "Xuất CSV",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
                {
                    // Khac voi loi ghi log ngam (LogService.WriteLog nuot loi) - day
                    // la thao tac nguoi dung CHU DONG bam va CHO KET QUA, nen phai
                    // bao loi cu the (VD: file dich dang mo trong chuong trinh khac,
                    // khong du quyen ghi vao thu muc dich) de ho biet ma xu ly,
                    // khong duoc im lang nhu WriteLog.
                    MessageBox.Show(
                        this,
                        $"Không thể ghi file CSV (có thể tệp đang được mở bởi chương trình khác, hoặc không đủ quyền ghi vào vị trí đã chọn):\n{ex.Message}",
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Ghi danh sach LogEntryModel ra file CSV tai duong dan chi dinh, dung
        /// LAI dung dinh dang cot va cach escape RFC 4180 voi LogService (xem
        /// LogService.FormatCsvRow/EscapeCsvField) de file xuat ra tuong thich
        /// hoan toan neu can doc lai bang cong cu/ma nguon xu ly file log noi bo.
        /// </summary>
        private static void ExportToCsv(string filePath, List<LogEntryModel> entries)
        {
            using (var writer = new StreamWriter(filePath, false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false)))
            {
                writer.WriteLine(LogService.LogFileHeader);

                foreach (LogEntryModel entry in entries)
                {
                    string[] fields =
                    {
                        entry.Id.ToString(),
                        entry.Timestamp.ToString("o", CultureInfo.InvariantCulture),
                        entry.Operation.ToString(),
                        EscapeCsvField(entry.Source),
                        EscapeCsvField(entry.Destination),
                        entry.Result.ToString(),
                        EscapeCsvField(entry.Message),
                        entry.ItemCount.ToString(CultureInfo.InvariantCulture),
                        entry.Duration.HasValue ? entry.Duration.Value.TotalSeconds.ToString(CultureInfo.InvariantCulture) : string.Empty
                    };

                    writer.WriteLine(string.Join(",", fields));
                }
            }
        }

        /// <summary>
        /// Ban sao cua LogService.EscapeCsvField (private, khong the goi thang tu
        /// day) - GIU NGUYEN cung logic escape RFC 4180 de file xuat ra tuong
        /// thich voi cach GetLogs cua LogService parse lai neu can.
        /// </summary>
        private static string EscapeCsvField(string field)
        {
            if (string.IsNullOrEmpty(field))
                return string.Empty;

            bool needsQuoting = field.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0;
            if (!needsQuoting)
                return field;

            return "\"" + field.Replace("\"", "\"\"") + "\"";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
