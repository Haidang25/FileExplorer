using System;
using System.Windows.Forms;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Diem TAP TRUNG DUY NHAT de hien thi hop thoai LOI (error) cho nguoi
    /// dung trong toan bo ung dung - thay cho viec moi Form tu goi rieng
    /// MessageBox.Show(..., MessageBoxIcon.Error) voi cach viet/tham so khac
    /// nhau (truoc khi co lop nay, ra soat toan ung dung cho thay: co noi
    /// QUEN truyen "this" lam owner (dialog moc len KHONG DUNG giua man hinh,
    /// khong nam tren dung Form cha - VD MainForm khi mo file/nhap duong dan
    /// sai, SettingsForm.btnOpenLogFolder_Click), co noi noi thong diep loi
    /// va ex.Message bang dau ":\n" (xuong dong), co noi lai noi bang ": "
    /// (cung dong) - KHONG NHAT QUAN giua cac Form).
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE - PHAM VI CUA LOP NAY: CHI phu trach HIEN THI loi
    /// (dung MessageBoxIcon.Error, MessageBoxButtons.OK) - KHONG phu trach
    /// GHI LOG loi (do la trach nhiem cua LogService, moi noi goi ErrorHandler
    /// van tu quyet dinh co ghi log hay khong, giong nhu truoc day) va KHONG
    /// thay the cac hop thoai XAC NHAN (Yes/No)/THONG BAO (Information)/
    /// CANH BAO (Warning) - nhung loai nay KHONG phai "hien thi loi" theo dung
    /// nghia cua yeu cau, va da duoc tung Form tu quan ly hop ly (VD hop
    /// thoai xac nhan xoa lich su trong LogForm.btnClearLogs_Click).
    ///
    /// 2 overload cua Show tuong ung 2 tinh huong thuc te da gap trong ung
    /// dung:
    /// - Show(owner, userMessage, title): LOI KHONG PHAT SINH TU EXCEPTION -
    ///   VD nguoi dung nhap duong dan thu muc khong ton tai (MainForm.btnGo_Click),
    ///   hoac mot Service tra ve OperationResult/enum bao that bai ma KHONG
    ///   kem Exception cu the (VD LogForm.btnExportInvestigationReport_Click
    ///   khi ExportInvestigationReport tra ve khac Success).
    /// - Show(owner, userMessage, ex, title): LOI TU MOT Exception da duoc
    ///   bat (catch) - noi THEM ex.Message vao SAU userMessage, CACH NHAU
    ///   boi MOT DAU XUONG DONG (chon dinh dang nay vi day la cach da duoc
    ///   dung o NHIEU noi nhat truoc khi co lop nay, VD PropertiesForm/MainForm/
    ///   LogForm.btnExportCsv_Click - ap dung dinh dang nay CHUNG CHO TOAN BO
    ///   ung dung, thay 2 noi con lai (SettingsForm) dang noi bang ": " sang
    ///   cung dinh dang nay). userMessage NEN ket thuc bang dau ':' de cau
    ///   hoan chinh doc tu nhien khi ex.Message duoc noi xuong dong duoi.
    ///
    /// "owner" (IWin32Window - thuong la "this" cua Form dang goi) LA THAM SO
    /// BAT BUOC (khong co gia tri mac dinh) - CO CHU DICH bat loi truyen thieu
    /// owner NGAY LUC VIET CODE (thieu tham so se khong compile duoc) thay vi
    /// de sot mot Form quen truyen "this" giong 2 truong hop da phat hien khi
    /// ra soat (MainForm dong file/nhap duong dan sai, SettingsForm.btnOpenLogFolder_Click) -
    /// hop thoai loi KHONG co owner se hien GIUA MAN HINH (khong nam tren dung
    /// Form cha dang mo) va KHONG bi chan (disable) cung voi Form cha, de
    /// nguoi dung vo tinh bam nham Form cha trong khi hop thoai loi dang mo.
    /// </remarks>
    public static class ErrorHandler
    {
        /// <summary>Tieu de mac dinh cho hop thoai loi khi noi goi khong can mot tieu de rieng.</summary>
        private const string DefaultTitle = "Lỗi";

        /// <summary>
        /// Hien hop thoai loi voi MOT thong diep da soan san (KHONG kem chi
        /// tiet ky thuat tu Exception) - dung cho loi phat hien qua kiem tra
        /// dieu kien/gia tri tra ve, khong phai tu catch.
        /// </summary>
        /// <param name="owner">
        /// Form dang hien thi hop thoai nay (thuong la "this") - BAT BUOC, xem
        /// <see cref="ErrorHandler"/> remarks ve ly do khong co gia tri mac dinh.
        /// </param>
        /// <param name="userMessage">Thong diep loi bang tieng Viet, de nguoi dung hieu duoc, KHONG chua thuat ngu ky thuat.</param>
        /// <param name="title">Tieu de hop thoai - mac dinh "Lỗi" neu khong can mot tieu de rieng phu hop hon ngu canh (VD "Xác thực báo cáo").</param>
        public static void Show(IWin32Window owner, string userMessage, string title = DefaultTitle)
        {
            MessageBox.Show(owner, userMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Hien hop thoai loi cho MOT Exception da bat duoc (catch) - noi
        /// them ex.Message vao sau userMessage, xem "2 overload" o remarks
        /// dau lop de biet quy uoc dinh dang.
        /// </summary>
        /// <param name="owner">Form dang hien thi hop thoai nay (thuong la "this") - BAT BUOC.</param>
        /// <param name="userMessage">
        /// Thong diep loi bang tieng Viet mo ta NGAN GON dieu vua that bai (VD
        /// "Không thể mở file:") - NEN ket thuc bang dau ':' de doc tu nhien
        /// cung ex.Message duoc noi xuong dong ngay duoi.
        /// </param>
        /// <param name="ex">
        /// Exception da bat duoc - ex.Message duoc noi THEM vao sau userMessage
        /// de nguoi dung/nguoi ho tro ky thuat biet chi tiet cu the (VD "tệp
        /// đang được sử dụng bởi một quy trình khác"). Cho phep null (khi do
        /// chi hien userMessage, tuong duong goi overload khong co ex) de cac
        /// noi goi dang co bien Exception san (VD trong mot catch chung xu ly
        /// nhieu nhanh) khong can tu kiem tra null truoc khi goi.
        /// </param>
        /// <param name="title">Tieu de hop thoai - mac dinh "Lỗi".</param>
        public static void Show(IWin32Window owner, string userMessage, Exception ex, string title = DefaultTitle)
        {
            string fullMessage = ex != null ? $"{userMessage}\n{ex.Message}" : userMessage;
            MessageBox.Show(owner, fullMessage, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
