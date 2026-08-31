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

        /// <summary>
        /// Toan bo vi pham toan ven da ghi nhan (LogService.GetInvestigationEntries,
        /// da sap xep gan nhat truoc) - hien tren tab "Vi phạm toàn vẹn"
        /// (lvwViolations). KHAC HAN _allLogs/_currentFilteredLogs (nhat ky
        /// thao tac thong thuong) - day la ly do yeu cau truoc do ("cong cu
        /// xem nhat ky thi khong ghi lai") xay ra: lvwLogs CHI hien _allLogs,
        /// KHONG he lien quan den danh sach nay - can mot ListView/tab RIENG
        /// (xem LogForm.Designer.cs: tabsLog/tabOperationLog/tabViolations)
        /// de nguoi dung thay duoc vi pham NGAY TRONG ung dung, khong bat
        /// buoc phai "Xuất báo cáo điều tra" ra file moi xem duoc.
        /// </summary>
        private List<IntegrityInvestigationEntry> _allViolations = new List<IntegrityInvestigationEntry>();

        public LogForm()
        {
            InitializeComponent();
            ApplyTheme();
            InitializeFilterOptions();
            LoadLogs();
            LoadViolations();
        }

        /// <summary>
        /// Ap dung AppTheme cho cum control tab "Nhật ký thao tác"/"Vi phạm
        /// toàn vẹn" (tabsLog/tabOperationLog/tabViolations/lvwViolations/
        /// lvwLogs) va 2 nut bao cao dieu tra (btnVerifyReport/
        /// btnExportInvestigationReport) - toan bo cum nay duoc THEM MOI trong
        /// cung mot lan sua doi (yeu cau "cong cu xem nhat ky thi khong ghi
        /// lai") va CHUA TUNG duoc to mau theo AppTheme, chi Form.BackColor/
        /// ForeColor o Designer.cs la theo AppTheme - cac control con van giu
        /// mau he thong (system color) mac dinh, se hien SAI (VD lvwViolations
        /// nen trang giua Form nen toi o Dark Mode) va KHONG DONG BO giua 2
        /// tab (mot tab da to theo AppTheme rieng le trong .cs nhu mau chu do
        /// ContentModified/FileMissing - xem GetViolationForeColor/lvwViolations -
        /// tab con lai hoan toan mac dinh).
        /// </summary>
        /// <remarks>
        /// KHONG dong theo grpFilters/btnRefresh/btnExportCsv/btnClearLogs/
        /// btnClose - CAC CONTROL DO co truoc tinh nang toan ven nay, ngoai
        /// pham vi yeu cau "control canh bao toan ven moi them"; theo cung
        /// mau cho toan bo LogForm (neu can) nen la mot yeu cau rieng de
        /// khong lam thay doi pham vi ngoai du dinh.
        ///
        /// Goi 1 LAN trong constructor (giong BatchRenameForm.ApplyTheme) -
        /// LogForm la hop thoai modal (ShowDialog), khong can cap nhat theme
        /// "song" giua luc dang mo.
        /// </remarks>
        private void ApplyTheme()
        {
            tabsLog.BackColor = AppTheme.Surface;

            tabOperationLog.BackColor = AppTheme.Surface;
            tabOperationLog.ForeColor = AppTheme.TextPrimary;
            tabViolations.BackColor = AppTheme.Surface;
            tabViolations.ForeColor = AppTheme.TextPrimary;

            lvwLogs.BackColor = AppTheme.Surface;
            lvwLogs.ForeColor = AppTheme.TextPrimary;
            lvwLogs.BorderStyle = BorderStyle.FixedSingle;

            lvwViolations.BackColor = AppTheme.Surface;
            lvwViolations.ForeColor = AppTheme.TextPrimary;
            lvwViolations.BorderStyle = BorderStyle.FixedSingle;

            // btnVerifyReport/btnExportInvestigationReport: 2 nut HANH DONG
            // CHINH cua tinh nang bao cao dieu tra (giong btnGo/btnApply da
            // dung mau Accent lam noi bat o MainForm/BatchRenameForm).
            btnVerifyReport.FlatStyle = FlatStyle.Flat;
            btnVerifyReport.FlatAppearance.BorderColor = AppTheme.Accent;
            btnVerifyReport.BackColor = AppTheme.Accent;
            btnVerifyReport.ForeColor = System.Drawing.Color.White;

            btnExportInvestigationReport.FlatStyle = FlatStyle.Flat;
            btnExportInvestigationReport.FlatAppearance.BorderColor = AppTheme.Accent;
            btnExportInvestigationReport.BackColor = AppTheme.Accent;
            btnExportInvestigationReport.ForeColor = System.Drawing.Color.White;
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

            // CHI cap nhat lblStatus khi tab "Nhật ký thao tác" dang duoc xem -
            // tranh de dong chu "N dòng log" GHI DE len dong chu "N vi phạm
            // ghi nhận" neu nguoi dung dang o tab kia luc LoadLogs() chay
            // (VD: btnRefresh_Click lam moi CA HAI tab cung luc - xem
            // tabsLog_SelectedIndexChanged).
            if (tabsLog.SelectedTab == tabOperationLog)
            {
                lblStatus.Text = $"{entries.Count} dòng log";
            }
        }

        /// <summary>
        /// Doc lai TOAN BO danh sach vi pham toan ven tu
        /// LogService.GetInvestigationEntries() (da sap xep gan nhat truoc)
        /// roi ve lai lvwViolations - cau truc song song voi LoadLogs()/
        /// PopulateListView o tren, danh cho tab "Vi phạm toàn vẹn".
        /// </summary>
        private void LoadViolations()
        {
            _allViolations = _logService.GetInvestigationEntries();
            PopulateViolationsListView(_allViolations);
        }

        /// <summary>
        /// Ve lai lvwViolations tu danh sach IntegrityInvestigationEntry, va
        /// cap nhat lblStatus (CHI khi tab "Vi phạm toàn vẹn" dang duoc xem -
        /// xem ghi chu tuong tu tai PopulateListView).
        /// </summary>
        /// <remarks>
        /// Thoi gian hien LOCAL (ToLocalTime) va loai vi pham DICH sang tieng
        /// Viet (LogService.TranslateViolationType) - DUNG Y HET cach
        /// ExportInvestigationReport dinh dang file xuat ra (xem
        /// LogService.FormatInvestigationDisplayRow), de nguoi dung thay
        /// CUNG MOT thong tin du xem truc tiep trong ung dung hay mo file da
        /// xuat, khong bi lech nhau (VD: mot noi hien UTC, noi kia hien gio
        /// dia phuong se gay nham lan).
        /// </remarks>
        private void PopulateViolationsListView(List<IntegrityInvestigationEntry> entries)
        {
            lvwViolations.BeginUpdate();
            lvwViolations.Items.Clear();

            foreach (IntegrityInvestigationEntry entry in entries)
            {
                var item = new ListViewItem(entry.Timestamp.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture));
                item.SubItems.Add(entry.FilePath ?? string.Empty);
                item.SubItems.Add(LogService.TranslateViolationType(entry.ViolationType));
                item.SubItems.Add(string.IsNullOrEmpty(entry.HashBefore) ? "-" : entry.HashBefore);
                item.SubItems.Add(string.IsNullOrEmpty(entry.HashAfter) ? "-" : entry.HashAfter);
                item.SubItems.Add(entry.UserName ?? string.Empty);

                // To mau ContentModified/FileMissing bang AppTheme.Error - day
                // la 2 loai vi pham NGHIEM TRONG (mat/sua noi dung), giong
                // quy uoc PopulateListView dung cho OperationResult that bai o
                // tren. UnexpectedNewFile giu mau thuong (TextPrimary) vi ban
                // than IntegrityService.cs da ghi chu day CO THE la hoat dong
                // binh thuong (file moi hop le), khong chac chan la van de.
                item.ForeColor = entry.ViolationType == "ContentModified" || entry.ViolationType == "FileMissing"
                    ? AppTheme.Error
                    : AppTheme.TextPrimary;

                lvwViolations.Items.Add(item);
            }

            lvwViolations.EndUpdate();

            if (tabsLog.SelectedTab == tabViolations)
            {
                lblStatus.Text = $"{entries.Count} vi phạm ghi nhận";
            }
        }

        /// <summary>
        /// Khi nguoi dung chuyen qua lai giua 2 tab, cap nhat lai lblStatus
        /// cho DUNG voi noi dung dang xem (dung lai danh sach da co san trong
        /// bo nho - _currentFilteredLogs/_allViolations - KHONG doc lai tu
        /// dia moi lan chuyen tab, giu dung nguyen tac "chi doc lai tu dia
        /// khi bam Lam moi" da neu tai remarks dau lop).
        /// </summary>
        private void tabsLog_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabsLog.SelectedTab == tabOperationLog)
            {
                lblStatus.Text = $"{_currentFilteredLogs.Count} dòng log";
            }
            else if (tabsLog.SelectedTab == tabViolations)
            {
                lblStatus.Text = $"{_allViolations.Count} vi phạm ghi nhận";
            }
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

        /// <summary>
        /// Lam moi CA HAI tab (nhat ky thao tac VA vi pham toan ven) cung
        /// luc, du chi mot nut "Lam mới" duy nhat o Form (khong tach rieng 2
        /// nut lam moi cho 2 tab) - don gian hon cho nguoi dung, va chi phi
        /// doc lai 2 file CSV don gian nay khong dang ke ke ca khi khong can
        /// thiet (VD dang o tab kia).
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadLogs();
            LoadViolations();
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
                // Ap dung ErrorHandler tap trung (xem Helpers/ErrorHandler.cs)
                // thay MessageBox.Show rai rac.
                ErrorHandler.Show(
                    this,
                    "Không thể xóa file lịch sử (có thể đang được mở bởi chương trình khác). Vui lòng thử lại sau.");
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
                    // Ap dung ErrorHandler tap trung (xem Helpers/ErrorHandler.cs)
                    // thay MessageBox.Show rai rac.
                    ErrorHandler.Show(
                        this,
                        "Không thể ghi file CSV (có thể tệp đang được mở bởi chương trình khác, hoặc không đủ quyền ghi vào vị trí đã chọn):",
                        ex);
                }
            }
        }

        /// <summary>
        /// Xuat BAO CAO DIEU TRA TOAN VEN (integrity investigation report) -
        /// KHAC HAN voi btnExportCsv_Click ben tren (xuat nguyen ven NHAT KY
        /// THAO TAC dang loc tren lvwLogs). Day la NUT DUY NHAT trong toan bo
        /// ung dung goi toi LogService.ExportInvestigationReport - truoc khi
        /// them nut nay, tinh nang xuat bao cao dieu tra (dinh dang tieng
        /// Viet + hash SHA-256 kem theo, xem LogService.cs) DA CO SAN trong
        /// LogService nhung KHONG CO CACH NAO nguoi dung kich hoat duoc tu
        /// giao dien - bam "Xuất CSV" chi xuat NHAT KY THAO TAC thong thuong
        /// (dinh dang cu, header tieng Anh) chu KHONG PHAI bao cao dieu tra,
        /// day chinh la ly do bao cao xuat ra "chua dung dinh dang da noi".
        /// </summary>
        /// <remarks>
        /// KHONG dung _currentFilteredLogs/lvwLogs o day - bao cao dieu tra
        /// la mot NGUON DU LIEU HOAN TOAN KHAC (LogService.GetInvestigationEntries,
        /// ghi nhan tu IntegrityService qua MainForm.IntegrityService_IntegrityViolationDetected),
        /// khong lien quan gi den danh sach dang hien tren LogForm (nhat ky
        /// thao tac Copy/Move/Delete...) - vi vay kiem tra "co du lieu de
        /// xuat khong" phai goi GetInvestigationEntries() RIENG, khong the
        /// tai su dung _currentFilteredLogs.Count.
        /// </remarks>
        private void btnExportInvestigationReport_Click(object sender, EventArgs e)
        {
            if (_logService.GetInvestigationEntries().Count == 0)
            {
                MessageBox.Show(
                    this,
                    "Chưa ghi nhận vi phạm toàn vẹn nào để xuất báo cáo (cần bật giám sát toàn vẹn một thư mục trước - menu Công cụ > Giám sát toàn vẹn thư mục này).",
                    "Xuất báo cáo điều tra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            using (var saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Tệp CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*";
                saveDialog.DefaultExt = "csv";
                saveDialog.AddExtension = true;
                saveDialog.FileName = $"bao_cao_dieu_tra_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

                if (saveDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                OperationResult result = _logService.ExportInvestigationReport(saveDialog.FileName);
                if (result == OperationResult.Success)
                {
                    // Bao ro CA hai file (bao cao + hash .sha256 di kem tu
                    // dong) de nguoi dung biet ho nhan duoc 2 file, khong chi
                    // 1 - xem LogService.WriteReportHashFile/GetReportHashFilePath.
                    string hashFilePath = LogService.GetReportHashFilePath(saveDialog.FileName);
                    MessageBox.Show(
                        this,
                        $"Đã xuất báo cáo điều tra ra:\n{saveDialog.FileName}\n\nHash SHA-256 của báo cáo (để đối chiếu sau này) đã lưu kèm tại:\n{hashFilePath}",
                        "Xuất báo cáo điều tra",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    // Ap dung ErrorHandler tap trung (xem Helpers/ErrorHandler.cs)
                    // thay MessageBox.Show rai rac.
                    ErrorHandler.Show(
                        this,
                        "Không thể xuất báo cáo điều tra (có thể tệp đích đang được mở bởi chương trình khác, đường dẫn không hợp lệ, hoặc không đủ quyền ghi vào vị trí đã chọn).");
                }
            }
        }

        /// <summary>
        /// Xac thuc mot bao cao dieu tra DA XUAT TU TRUOC: cho nguoi dung
        /// chon LAI file CSV bao cao (VD file da nhan duoc tu
        /// btnExportInvestigationReport_Click, co the o bat ky dau, khong
        /// nhat thiet con trong ung dung nay), tinh lai hash SHA-256 HIEN TAI
        /// cua file do va so sanh voi hash da luu trong file .sha256 di kem
        /// luc xuat (LogService.VerifyExportedReportHash) - day chinh la
        /// chuc nang "doi chieu sau nay" ma tinh hash luc xuat (yeu cau
        /// truoc) huong toi, gio duoc dua LEN GIAO DIEN de nguoi dung tu bam
        /// kiem tra ma khong can dong lenh/cong cu ngoai.
        /// </summary>
        /// <remarks>
        /// Dung OpenFileDialog (khong phai thao tac tren _currentFilteredLogs/
        /// lvwLogs) vi bao cao can xac thuc la MOT FILE DOC LAP tren dia,
        /// KHONG con lien quan gi den danh sach dang hien trong LogForm tai
        /// thoi diem xac thuc (co the da xuat tu rat lau truoc, hoac tu mot
        /// may khac roi mang ve day de kiem tra).
        /// </remarks>
        private void btnVerifyReport_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Filter = "Tệp CSV (*.csv)|*.csv|Tất cả tệp (*.*)|*.*";
                openDialog.Title = "Chọn báo cáo điều tra cần xác thực";

                if (openDialog.ShowDialog(this) != DialogResult.OK)
                    return;

                ReportHashVerificationResult result = _logService.VerifyExportedReportHash(openDialog.FileName);

                switch (result)
                {
                    case ReportHashVerificationResult.Match:
                        MessageBox.Show(
                            this,
                            $"Báo cáo còn NGUYÊN VẸN - hash SHA-256 hiện tại khớp với hash đã lưu lúc xuất.\n\n{openDialog.FileName}",
                            "Xác thực báo cáo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        break;

                    case ReportHashVerificationResult.Mismatch:
                        // Icon Warning (khong phai Error) - day la mot PHAT HIEN can
                        // nguoi dung chu y, khong phai loi thao tac cua chinh ung
                        // dung (giong quy uoc IntegrityService dung cho ContentModified).
                        MessageBox.Show(
                            this,
                            $"CẢNH BÁO: báo cáo ĐÃ BỊ THAY ĐỔI kể từ lúc xuất (hash SHA-256 hiện tại KHÔNG khớp với hash đã lưu).\n\n{openDialog.FileName}",
                            "Xác thực báo cáo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        break;

                    case ReportHashVerificationResult.HashFileNotFound:
                        MessageBox.Show(
                            this,
                            $"Không tìm thấy file hash (.sha256) đi kèm báo cáo này - có thể báo cáo được xuất từ phiên bản ứng dụng cũ (trước khi có tính năng lưu hash), hoặc file .sha256 đã bị xóa/di chuyển riêng khỏi báo cáo.\n\nCần đặt cạnh báo cáo file:\n{LogService.GetReportHashFilePath(openDialog.FileName)}",
                            "Xác thực báo cáo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                        break;

                    case ReportHashVerificationResult.ReportFileNotFound:
                        // Ap dung ErrorHandler tap trung (xem Helpers/ErrorHandler.cs)
                        // thay MessageBox.Show rai rac - giu nguyen tieu de rieng
                        // "Xác thực báo cáo" (khac tieu de mac dinh "Lỗi").
                        ErrorHandler.Show(this, "Không tìm thấy file báo cáo đã chọn.", "Xác thực báo cáo");
                        break;

                    default: // ReportHashVerificationResult.Error
                        ErrorHandler.Show(
                            this,
                            "Không thể xác thực báo cáo (có thể tệp đang bị khóa bởi chương trình khác, hoặc không đủ quyền đọc).");
                        break;
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
