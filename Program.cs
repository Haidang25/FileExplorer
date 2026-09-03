using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FileExplorerApp.Forms;

namespace FileExplorerApp
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Bat loi khong duoc xu ly tren toan ung dung. Mac dinh, mot exception
            // khong duoc "catch" o dau ca tren luong giao dien se lam WinForms DONG
            // (crash) toan bo ung dung ngay lap tuc - kha nang nay tang len ro rang
            // voi cac handler "async void" (VD: MainForm.trvFolders_AfterSelect):
            // exception nem ra SAU mot await trong "async void" khong the duoc bat
            // boi try/catch o NGOAI ham do, ma se duoc nem lai thang tren vong lap
            // thong diep cua UI. Dang ky ThreadException (+ UnhandledException cho
            // cac truong hop hiem xay ra ngoai luong giao dien) de hien mot thong
            // bao loi than thien va CHO PHEP UNG DUNG TIEP TUC CHAY, thay vi nguoi
            // dung dot ngot mat toan bo phien lam viec (file dang mo, clipboard...).
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) => HandleUnhandledException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                HandleUnhandledException(e.ExceptionObject as Exception);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Hien thong bao loi cho nguoi dung khi co exception khong duoc xu ly rieng
        /// o noi phat sinh - CHU DICH la de ung dung KHONG crash/dong dot ngot, du
        /// nguyen nhan goc (bug o dau do) van nen duoc sua rieng khi phat hien qua
        /// thong bao nay.
        /// </summary>
        private static void HandleUnhandledException(Exception ex)
        {
            try
            {
                MessageBox.Show(
                    $"Đã xảy ra lỗi ngoài dự kiến:\n{ex?.Message}\n\nỨng dụng sẽ tiếp tục chạy, nhưng nếu lỗi này lặp lại, vui lòng ghi lại thao tác vừa thực hiện để báo lại.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch
            {
                // Khong de viec hien thong bao loi lai gay ra loi khac (VD: khong con
                // UI thread hop le) - im lang bo qua, uu tien khong crash hon la bao
                // duoc het moi truong hop.
            }
        }
    }
}
