using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using FileExplorerApp.Models;
using FileExplorerApp.Services;

namespace FileExplorerApp.Forms
{
    /// <summary>
    /// Cua so chinh cua ung dung. Chua MenuStrip (Tep/Chinh sua/Xem/Cong cu/Tro giup)
    /// va se la noi chua TreeView/ListView duyet thu muc trong cac buoc tiep theo.
    /// Cac handler menu hien tai chi la khung (TODO), can noi voi cac Services da co
    /// (FileService, FolderService, SearchService, RecycleBinService, LogService).
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly FolderService _folderService = new FolderService();
        private readonly FileService _fileService = new FileService();
        private readonly RecycleBinService _recycleBinService = new RecycleBinService();

        // TODO: thay bang duong dan dang duoc chon tren TreeView/ListView khi da co
        // giao dien dieu huong thuc te. Tam thoi mac dinh la Desktop de New Folder/New File
        // co noi de tao.
        private string _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        // "Clipboard" noi bo cua ung dung cho Cut/Copy/Paste: danh sach duong dan
        // day du da chon, va co dang la Cut (true, se xoa nguon sau khi dan) hay
        // Copy (false, giu nguyen nguon) hay khong.
        private List<string> _clipboardPaths = new List<string>();
        private bool _clipboardIsCut;

        // Che do hien thi hien tai (tuong ung System.Windows.Forms.View de gan truc tiep
        // cho ListView.View khi da co ListView). Mac dinh la Details, giong Windows Explorer.
        private View _currentViewMode = View.Details;

        // True neu dang hien thi ca file/thu muc an (IsHidden). Mac dinh la false.
        private bool _showHiddenItems;

        public MainForm()
        {
            InitializeComponent();
            this.Text = "SFileManager";
            // Dung icon da gan cho file .exe (ApplicationIcon) lam icon cua form,
            // khong phu thuoc duong dan tuong doi luc chay.
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        }

        #region Menu Tep (File)

        private void mnuFileNewFolder_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox(
                "Nhap ten thu muc moi:", "Tao thu muc moi", "New Folder");

            if (string.IsNullOrWhiteSpace(name))
                return; // Nguoi dung bam Cancel hoac de trong.

            OperationResult result = _folderService.CreateFolder(_currentPath, name);
            ShowOperationResultMessage(result, $"tao thu muc \"{name}\"");

            if (result == OperationResult.Success)
            {
                // TODO: goi lai ham lam moi ListView/TreeView khi da co (VD: LoadCurrentFolder()).
                mnuViewRefresh_Click(sender, e);
            }
        }

        private void mnuFileNewFile_Click(object sender, EventArgs e)
        {
            string name = Interaction.InputBox(
                "Nhap ten file moi (bao gom phan mo rong, VD: moi.txt):", "Tao file moi", "New File.txt");

            if (string.IsNullOrWhiteSpace(name))
                return; // Nguoi dung bam Cancel hoac de trong.

            OperationResult result = _fileService.CreateFile(_currentPath, name);
            ShowOperationResultMessage(result, $"tao file \"{name}\"");

            if (result == OperationResult.Success)
            {
                // TODO: goi lai ham lam moi ListView/TreeView khi da co (VD: LoadCurrentFolder()).
                mnuViewRefresh_Click(sender, e);
            }
        }

        /// <summary>
        /// Hien thong bao phu hop voi ket qua tra ve tu Services, dung chung cho
        /// cac thao tac tao/doi ten/xoa/di chuyen/sao chep file va thu muc.
        /// </summary>
        /// <param name="result">Ket qua thao tac.</param>
        /// <param name="actionDescription">Mo ta ngan gon thao tac da thuc hien (VD: "tao thu muc \"abc\"").</param>
        private void ShowOperationResultMessage(OperationResult result, string actionDescription)
        {
            switch (result)
            {
                case OperationResult.Success:
                    MessageBox.Show($"Da {actionDescription} thanh cong.", "Thong bao",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                case OperationResult.Skipped:
                    MessageBox.Show($"Khong the {actionDescription}: da co muc trung ten trong thu muc nay.",
                        "Canh bao", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

                case OperationResult.AccessDenied:
                    MessageBox.Show($"Khong the {actionDescription}: khong du quyen truy cap thu muc nay.",
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case OperationResult.NotFound:
                    MessageBox.Show($"Khong the {actionDescription}: khong tim thay thu muc dich.",
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                default:
                    MessageBox.Show($"Khong the {actionDescription}: ten khong hop le hoac co loi xay ra.",
                        "Loi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Menu Chinh sua (Edit)

        /// <summary>
        /// Lay danh sach duong dan day du dang duoc chon tren giao dien.
        /// </summary>
        /// <remarks>
        /// TODO: hien tai MainForm chua co ListView/TreeView duyet noi dung thu muc,
        /// nen luon tra ve danh sach rong. Khi da xay giao dien duyet file, thay phan
        /// than ham nay bang viec doc tu (VD) listViewFiles.SelectedItems va tra ve
        /// FullPath cua tung FileItemModel/FolderItemModel tuong ung.
        /// </remarks>
        private List<string> GetSelectedPaths()
        {
            return new List<string>();
        }

        private void mnuEditCut_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chua chon muc nao de cat.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _clipboardPaths = selected;
            _clipboardIsCut = true;
        }

        private void mnuEditCopy_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chua chon muc nao de sao chep.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            _clipboardPaths = selected;
            _clipboardIsCut = false;
        }

        private void mnuEditPaste_Click(object sender, EventArgs e)
        {
            if (_clipboardPaths.Count == 0)
            {
                MessageBox.Show("Chua co gi trong clipboard de dan (hay Cut/Copy truoc).",
                    "Thong bao", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (string sourcePath in _clipboardPaths)
            {
                string name = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(_currentPath, name);
                bool isDirectory = Directory.Exists(sourcePath);

                OperationResult result;
                if (isDirectory)
                {
                    result = _clipboardIsCut
                        ? _folderService.MoveFolder(sourcePath, destinationPath)
                        : _folderService.CopyFolder(sourcePath, destinationPath);
                }
                else
                {
                    result = _clipboardIsCut
                        ? _fileService.MoveFile(sourcePath, destinationPath)
                        : _fileService.CopyFile(sourcePath, destinationPath);
                }

                ShowOperationResultMessage(result, $"dan \"{name}\"");
            }

            if (_clipboardIsCut)
            {
                // Sau khi Cut + Paste xong thi clipboard het gia tri (giong Windows Explorer).
                _clipboardPaths = new List<string>();
            }

            mnuViewRefresh_Click(sender, e);
        }

        private void mnuEditDelete_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count == 0)
            {
                MessageBox.Show("Chua chon muc nao de xoa.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Ban co chac muon chuyen {selected.Count} muc da chon vao Thung rac?",
                "Xac nhan xoa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            foreach (string path in selected)
            {
                OperationResult result = _recycleBinService.MoveToRecycleBin(path);
                ShowOperationResultMessage(result, $"xoa \"{Path.GetFileName(path)}\"");
            }

            // TODO: neu nguoi dung giu Shift khi bam Delete (hoac chon muc "Xoa vinh vien"),
            // goi FileService.DeleteFile/FolderService.DeleteFolder voi permanent = true thay
            // vi RecycleBinService.MoveToRecycleBin.

            mnuViewRefresh_Click(sender, e);
        }

        private void mnuEditRename_Click(object sender, EventArgs e)
        {
            List<string> selected = GetSelectedPaths();
            if (selected.Count != 1)
            {
                MessageBox.Show("Vui long chon dung mot muc de doi ten.", "Thong bao",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = selected[0];
            string oldName = Path.GetFileName(path);
            string newName = Interaction.InputBox("Nhap ten moi:", "Doi ten", oldName);

            if (string.IsNullOrWhiteSpace(newName) || newName == oldName)
                return; // Nguoi dung bam Cancel, de trong, hoac khong doi gi.

            bool isDirectory = Directory.Exists(path);
            OperationResult result = isDirectory
                ? _folderService.RenameFolder(path, newName)
                : _fileService.RenameFile(path, newName);

            ShowOperationResultMessage(result, $"doi ten \"{oldName}\" thanh \"{newName}\"");

            if (result == OperationResult.Success)
            {
                mnuViewRefresh_Click(sender, e);
            }
        }

        private void mnuEditSelectAll_Click(object sender, EventArgs e)
        {
            // TODO: goi listViewFiles.SelectAll() (hoac tuong duong) khi da co ListView
            // hien thi noi dung thu muc. Hien tai chua co giao dien liet ke nen chua
            // co gi de chon.
        }

        #endregion

        #region Menu Xem (View)

        private void mnuViewRefresh_Click(object sender, EventArgs e)
        {
            // TODO: nap lai noi dung thu muc dang mo (FileService.GetFiles + FolderService.GetSubFolders).
        }

        private void mnuViewShowHidden_Click(object sender, EventArgs e)
        {
            _showHiddenItems = mnuViewShowHidden.Checked;

            // TODO: luu trang thai _showHiddenItems (VD: vao Properties.Settings) de nho
            // giua cac lan mo ung dung, roi loc/hien lai cac muc co IsHidden tren ListView
            // theo trang thai moi (mnuViewRefresh_Click(sender, e)).
            mnuViewRefresh_Click(sender, e);
        }

        /// <summary>
        /// Chon mot che do hien thi (Large Icon/Small Icon/List/Details), bo chon
        /// 3 che do con lai (hanh xu nhu radio button) va luu vao _currentViewMode.
        /// </summary>
        /// <param name="mode">Che do hien thi vua duoc chon.</param>
        /// <param name="selectedItem">Muc menu tuong ung voi mode (se duoc danh dau Checked).</param>
        private void SetViewMode(View mode, ToolStripMenuItem selectedItem)
        {
            foreach (ToolStripMenuItem item in mnuViewMode.DropDownItems.OfType<ToolStripMenuItem>())
            {
                item.Checked = item == selectedItem;
            }

            _currentViewMode = mode;

            // TODO: gan truc tiep cho ListView khi da co giao dien duyet file, VD:
            // listViewFiles.View = _currentViewMode;
        }

        private void mnuViewModeLargeIcon_Click(object sender, EventArgs e)
        {
            SetViewMode(View.LargeIcon, mnuViewModeLargeIcon);
        }

        private void mnuViewModeSmallIcon_Click(object sender, EventArgs e)
        {
            SetViewMode(View.SmallIcon, mnuViewModeSmallIcon);
        }

        private void mnuViewModeList_Click(object sender, EventArgs e)
        {
            SetViewMode(View.List, mnuViewModeList);
        }

        private void mnuViewModeDetails_Click(object sender, EventArgs e)
        {
            SetViewMode(View.Details, mnuViewModeDetails);
        }

        #endregion

        #region Menu Cong cu (Tools)

        private void mnuToolsSearch_Click(object sender, EventArgs e)
        {
            // TODO: mo form/hop thoai tim kiem, dung SearchService de tim va hien ket qua.
        }

        private void mnuToolsFindDuplicates_Click(object sender, EventArgs e)
        {
            // TODO: mo form hien thi tien trinh + ket qua, goi
            // SearchService.FindDuplicateFiles(_currentPath) (nen chay tren luong rieng
            // hoac async vi co the mat thoi gian voi thu muc lon), sau do hien tung
            // nhom file trung lap de nguoi dung chon xoa bot ban trung.
        }

        private void mnuToolsRecycleBin_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem noi dung Thung rac, dung RecycleBinService.GetRecycleBinItems.
        }

        private void mnuToolsLogs_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem lich su thao tac, dung LogService.GetLogs.
        }

        private void mnuToolsSettings_Click(object sender, EventArgs e)
        {
            // TODO: mo Form cai dat (VD: Forms/SettingsForm) cho phep chinh:
            // - Che do xem mac dinh (_currentViewMode), co hien file an mac dinh (_showHiddenItems)
            // - Thu muc mac dinh khi mo ung dung (thay _currentPath)
            // - Xoa vinh vien hay chuyen vao Thung rac khi Delete
            // - Vi tri/gioi han dung luong file log (LogService)
            // Cac lua chon nen luu qua Properties.Settings.Default de nho giua cac lan chay.
        }

        #endregion

        #region Menu Tro giup (Help)

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string productName = GetAssemblyAttribute<AssemblyProductAttribute>(assembly)?.Product
                ?? assembly.GetName().Name;
            string version = assembly.GetName().Version?.ToString() ?? "1.0.0.0";
            string copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(assembly)?.Copyright;
            string company = GetAssemblyAttribute<AssemblyCompanyAttribute>(assembly)?.Company;
            string description = GetAssemblyAttribute<AssemblyDescriptionAttribute>(assembly)?.Description;

            var lines = new List<string>
            {
                productName,
                $"Phiên bản {version}"
            };

            if (!string.IsNullOrWhiteSpace(description))
                lines.Add(description);

            if (!string.IsNullOrWhiteSpace(company))
                lines.Add(company);

            if (!string.IsNullOrWhiteSpace(copyright))
                lines.Add(copyright);

            MessageBox.Show(
                string.Join(Environment.NewLine, lines),
                "Giới thiệu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Doc mot custom attribute cap assembly (VD: AssemblyProductAttribute,
        /// AssemblyCopyrightAttribute...) de hien thi trong hop thoai About, tranh
        /// phai ghi cung (hardcode) thong tin phien ban/ten san pham trong code.
        /// </summary>
        private static T GetAssemblyAttribute<T>(Assembly assembly) where T : Attribute
        {
            return Attribute.GetCustomAttribute(assembly, typeof(T)) as T;
        }

        #endregion
    }
}
