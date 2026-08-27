namespace FileExplorerApp.Models
{
    /// <summary>
    /// Tien do cua mot thao tac Copy dang chay (file don le hoac ca cay thu muc),
    /// dung lam kieu du lieu cho <see cref="System.IProgress{T}"/> khi FileService/
    /// FolderService bao cao nguoc ve noi goi (VD: MainForm) trong luc thao tac
    /// dang thuc hien bat dong bo.
    ///
    /// Noi goi thuong dung <see cref="System.Progress{T}"/> (implementation mac
    /// dinh cua IProgress&lt;T&gt;) duoc tao TREN UI THREAD - Progress&lt;T&gt; tu dong
    /// ghi nho SynchronizationContext luc do va luon Post() callback ve dung thread
    /// ay moi khi Report() duoc goi, du Report() thuc su duoc goi tu thread nao (VD:
    /// tu ben trong vong lap doc/ghi cua CopyFileAsync sau khi da ConfigureAwait(false)).
    /// Nho vay handler co the cap nhat truc tiep tspProgress/tsslStatus ma khong can
    /// tu Invoke/BeginInvoke thu cong.
    /// </summary>
    public class FileOperationProgress
    {
        /// <summary>Ten (khong bao gom duong dan) cua file dang duoc sao chep luc bao cao.</summary>
        public string CurrentFileName { get; set; }

        /// <summary>So file da sao chep xong (thanh cong hoac bi bo qua do loi) tinh den luc bao cao.</summary>
        public int FilesCompleted { get; set; }

        /// <summary>
        /// Tong so file can sao chep cua CA THAO TAC (VD: toan bo file trong mot cay
        /// thu muc). Bang 1 khi dang sao chep mot file don le (khong phai thu muc).
        /// </summary>
        public int TotalFiles { get; set; }

        /// <summary>So byte da doc/ghi xong CUA RIENG file dang duoc sao chep (CurrentFileName).</summary>
        public long CurrentFileBytesTransferred { get; set; }

        /// <summary>Tong dung luong (byte) CUA RIENG file dang duoc sao chep.</summary>
        public long CurrentFileTotalBytes { get; set; }

        /// <summary>
        /// Phan tram hoan thanh uoc tinh (0-100) tren TOAN BO thao tac: cac file da
        /// xong tinh du 100%, cong them ti le hoan thanh rieng cua file dang copy do
        /// dang (theo byte), roi chia deu cho TotalFiles - giup thanh tien do tang
        /// muot qua tung file thay vi nhay cung tung 1/TotalFiles mot.
        ///
        /// Tra ve 0 neu TotalFiles &lt;= 0 (chua xac dinh duoc tong so file).
        /// </summary>
        public int PercentComplete
        {
            get
            {
                if (TotalFiles <= 0)
                    return 0;

                double currentFileFraction = CurrentFileTotalBytes > 0
                    ? (double)CurrentFileBytesTransferred / CurrentFileTotalBytes
                    : 0d;

                double overallFraction = (FilesCompleted + currentFileFraction) / TotalFiles;
                int percent = (int)(overallFraction * 100);

                if (percent < 0) return 0;
                if (percent > 100) return 100;
                return percent;
            }
        }
    }
}
