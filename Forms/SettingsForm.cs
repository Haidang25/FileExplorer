using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using FileExplorerApp.Helpers;
using FileExplorerApp.Properties;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Man hinh Cai dat, mo tu MainForm.mnuToolsSettings_Click. Cac lua chon duoc
    /// luu qua Properties.Settings.Default de nho lai giua cac lan chay.
    ///
    /// Nhom "Giao dien" (Light/Dark) va "Hien thi" (hien tep an, che do xem mac
    /// dinh) co tac dung ngay: MainForm doc lai gia tri nay va goi ApplyTheme()/
    /// dong bo lai UI sau khi dong hop thoai nay voi DialogResult.OK.
    ///
    /// Nhom "Giam sat thu muc" (FileMonitorService) va "Nhat ky" (LogService)
    /// deu da co tac dung THUC TE:
    /// - chkAutoRefresh/numWatcherDelay: MainForm doc lai 2 gia tri nay ngay
    ///   sau khi dong hop thoai voi DialogResult.OK (mnuToolsSettings_Click) de
    ///   bat/tat FileMonitorService va cap nhat Interval cua timer debounce,
    ///   khong can khoi dong lai ung dung - xem MainForm.RestartFolderMonitoring/
    ///   InitializeFolderMonitoring.
    /// - chkEnableLog: LogService.WriteLog doc truc tiep Settings.Default.
    ///   LogEnabled moi lan ghi (khong can MainForm chuyen tiep gia tri).
    /// </summary>
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            ApplyTheme();
            LoadSettings();
        }

        /// <summary>Nap gia tri hien tai tu Properties.Settings.Default len cac control.</summary>
        private void LoadSettings()
        {
            rbDark.Checked = Settings.Default.IsDarkMode;
            rbLight.Checked = !Settings.Default.IsDarkMode;

            chkShowHidden.Checked = Settings.Default.ShowHiddenFiles;

            switch ((View)Settings.Default.DefaultViewMode)
            {
                case View.LargeIcon:
                    rbLargeIcon.Checked = true;
                    break;
                case View.List:
                    rbList.Checked = true;
                    break;
                default:
                    rbDetails.Checked = true;
                    break;
            }

            chkAutoRefresh.Checked = Settings.Default.AutoRefreshEnabled;
            numWatcherDelay.Value = Math.Min(numWatcherDelay.Maximum,
                Math.Max(numWatcherDelay.Minimum, Settings.Default.WatcherDelayMs));

            chkEnableLog.Checked = Settings.Default.LogEnabled;
            txtLogPath.Text = Environment.ExpandEnvironmentVariables(Settings.Default.LogPath);
        }

        /// <summary>Ap dung AppTheme cho toan bo control cua SettingsForm.</summary>
        private void ApplyTheme()
        {
            this.BackColor = AppTheme.Background;
            this.ForeColor = AppTheme.TextPrimary;

            foreach (GroupBox box in new[] { groupBoxTheme, groupBoxDisplay, groupBoxWatcher, groupBoxLog })
            {
                box.ForeColor = AppTheme.TextSecondary;
            }

            txtLogPath.BackColor = AppTheme.Surface;
            txtLogPath.ForeColor = AppTheme.TextPrimary;
            txtLogPath.BorderStyle = BorderStyle.FixedSingle;

            numWatcherDelay.BackColor = AppTheme.Surface;
            numWatcherDelay.ForeColor = AppTheme.TextPrimary;

            foreach (Button btn in new[] { btnOpenLogFolder, btnCancel })
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderColor = AppTheme.Border;
                btn.BackColor = AppTheme.Surface;
                btn.ForeColor = AppTheme.TextPrimary;
            }

            // Nut Luu la nut nhan manh (Accept), to noi bat bang mau Accent giong
            // "Lọc"/"Tìm kiếm" trong cac mockup khac.
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.FlatAppearance.BorderColor = AppTheme.Accent;
            btnSave.BackColor = AppTheme.Accent;
            btnSave.ForeColor = System.Drawing.Color.White;
        }

        /// <summary>Luu lua chon vao Properties.Settings.Default va dong voi DialogResult.OK.</summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings.Default.IsDarkMode = rbDark.Checked;
            Settings.Default.ShowHiddenFiles = chkShowHidden.Checked;

            if (rbLargeIcon.Checked)
            {
                Settings.Default.DefaultViewMode = (int)View.LargeIcon;
            }
            else if (rbList.Checked)
            {
                Settings.Default.DefaultViewMode = (int)View.List;
            }
            else
            {
                Settings.Default.DefaultViewMode = (int)View.Details;
            }

            Settings.Default.AutoRefreshEnabled = chkAutoRefresh.Checked;
            Settings.Default.WatcherDelayMs = (int)numWatcherDelay.Value;
            Settings.Default.LogEnabled = chkEnableLog.Checked;

            try
            {
                Settings.Default.Save();
            }
            catch (Exception ex) when (ex is ConfigurationErrorsException || ex is IOException || ex is UnauthorizedAccessException)
            {
                // RA SOAT try-catch: Settings.Default.Save() ghi file user.config
                // ra dia (thuong trong AppData) - TRUOC DAY khong co try-catch
                // nao ca, nen mot file user.config chi doc (read-only)/bi khoa/
                // mat quyen ghi se lam CRASH CA UNG DUNG chi vi bam nut Luu trong
                // hop thoai Cai dat. Bao loi ro rang va KHONG dong hop thoai (giu
                // DialogResult mac dinh la None) de nguoi dung biet cai dat CHUA
                // duoc luu va co the thu lai, giong cach btnOpenLogFolder_Click
                // ben duoi da bao loi cho nguoi dung.
                MessageBox.Show(this, "Không thể lưu cài đặt: " + ex.Message, "Cài đặt",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Cap nhat ngay AppTheme trong bo nho de MainForm/cac Form mo sau do
            // dung dung mau vua chon, khong can khoi dong lai ung dung.
            AppTheme.IsDarkMode = Settings.Default.IsDarkMode;

            this.DialogResult = DialogResult.OK;
        }

        /// <summary>Mo thu muc chua log bang Explorer, tao thu muc truoc neu chua co.</summary>
        private void btnOpenLogFolder_Click(object sender, EventArgs e)
        {
            try
            {
                string path = Environment.ExpandEnvironmentVariables(Settings.Default.LogPath);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                Process.Start("explorer.exe", path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể mở thư mục nhật ký: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
