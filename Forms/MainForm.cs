using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            // TODO: goi FolderService.CreateFolder(duongDanHienTai, tenMoi) roi lam moi ListView.
        }

        private void mnuFileNewFile_Click(object sender, EventArgs e)
        {
            // TODO: goi FileService.CreateFile(duongDanHienTai, tenMoi) roi lam moi ListView.
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Menu Chinh sua (Edit)

        private void mnuEditCut_Click(object sender, EventArgs e)
        {
            // TODO: luu danh sach muc dang chon (FileItemModel/FolderItemModel) vao
            // bien tam clipboard cua ung dung, danh dau la thao tac Cut.
        }

        private void mnuEditCopy_Click(object sender, EventArgs e)
        {
            // TODO: tuong tu Cut nhung danh dau la thao tac Copy (khong xoa nguon khi dan).
        }

        private void mnuEditPaste_Click(object sender, EventArgs e)
        {
            // TODO: tuy trang thai da luu (Cut/Copy) goi FileService/FolderService.MoveFile/CopyFile
            // hoac MoveFolder/CopyFolder toi thu muc dang mo, roi lam moi ListView.
        }

        private void mnuEditDelete_Click(object sender, EventArgs e)
        {
            // TODO: hoi xac nhan, goi RecycleBinService.MoveToRecycleBin cho tung muc dang chon
            // (hoac FileService/FolderService.DeleteFile/DeleteFolder neu Shift+Delete - xoa vinh vien).
        }

        private void mnuEditRename_Click(object sender, EventArgs e)
        {
            // TODO: cho phep sua ten truc tiep tren ListView (label edit) hoac hop thoai nhap ten,
            // sau do goi FileService.RenameFile / FolderService.RenameFolder.
        }

        #endregion

        #region Menu Xem (View)

        private void mnuViewRefresh_Click(object sender, EventArgs e)
        {
            // TODO: nap lai noi dung thu muc dang mo (FileService.GetFiles + FolderService.GetSubFolders).
        }

        private void mnuViewShowHidden_Click(object sender, EventArgs e)
        {
            // TODO: luu trang thai mnuViewShowHidden.Checked (VD: vao Properties.Settings),
            // loc/hien lai cac muc co IsHidden theo trang thai moi.
        }

        #endregion

        #region Menu Cong cu (Tools)

        private void mnuToolsSearch_Click(object sender, EventArgs e)
        {
            // TODO: mo form/hop thoai tim kiem, dung SearchService de tim va hien ket qua.
        }

        private void mnuToolsRecycleBin_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem noi dung Thung rac, dung RecycleBinService.GetRecycleBinItems.
        }

        private void mnuToolsLogs_Click(object sender, EventArgs e)
        {
            // TODO: mo man hinh xem lich su thao tac, dung LogService.GetLogs.
        }

        #endregion

        #region Menu Tro giup (Help)

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            // TODO: hien hop thoai/Form gioi thieu ung dung (ten, phien ban tu AssemblyInfo, tac gia).
            MessageBox.Show(
                "SFileManager\nPhien ban 1.0.0.0",
                "Gioi thieu",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        #endregion
    }
}
