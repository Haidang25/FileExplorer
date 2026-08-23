using System;
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
    /// hien chi luu gia tri cho hop thoai nay — ban than 2 Service do trong du
    /// an van con la khung (TODO/NotImplementedException), nen luu y cac tuy
    /// chon nay CHUA co tac dung thuc te cho toi khi 2 Service duoc trien khai.
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

            Settings.Default.Save();

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
