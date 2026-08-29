using System;
using System.Globalization;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Mo hinh du lieu cho mot dong nhat ky (log) thao tac file/folder: thoi
    /// gian, loai thao tac, nguon, dich, ket qua thuc hien va vai thong tin bo
    /// sung (so luong muc, thoi gian thuc hien) huu ich voi thao tac hang loat.
    /// </summary>
    /// <remarks>
    /// Thiet ke: CO Y THUC dung lai 2 enum da co san trong toan ung dung thay vi
    /// tu dinh nghia enum rieng cho log:
    /// - <see cref="FileOperationType"/> (FileOperationType.cs) cho Operation -
    ///   day chinh la enum FileService/FolderService/MainForm dung de phan loai
    ///   hanh dong nguoi dung (Copy/Move/Delete/Rename/CreateFile/CreateFolder/
    ///   Cut/Paste/Compress/Extract/Open/Search/Other). Dung chung nghia la noi
    ///   goi (VD: FileService.CopyFileAsync) co the truyen thang gia tri co san,
    ///   khong can anh xa qua mot enum "rieng cho log" nhu LogOperationType cu
    ///   (chi co 7 gia tri, thieu Cut/Paste/Compress/Extract/Search so voi
    ///   FileOperationType) - tranh 2 enum song song de bieu dien cung mot khai
    ///   niem, de nham lan va de bi lech nhau ve sau khi them loai thao tac moi.
    /// - <see cref="OperationResult"/> (OperationResult.cs) cho Result - day la
    ///   enum ket qua CHI TIET ma FileService/FolderService da tra ve san
    ///   (Success/PartialSuccess/Failed/Cancelled/Skipped/AccessDenied/NotFound/
    ///   FileInUse/InvalidDestination). LogResult cu chi co 3 gia tri
    ///   (Success/Failed/Cancelled) se lam mat thong tin khi ghi log (VD: khong
    ///   phan biet duoc AccessDenied voi Failed thong thuong) va lai can anh xa
    ///   thu cong tu OperationResult ve LogResult moi lan ghi.
    /// Nho vay, tao mot LogEntryModel tu ket qua mot thao tac chi la
    /// "new LogEntryModel(FileOperationType.Copy, src, dest, result)" voi result
    /// chinh la OperationResult da co, khong can chuyen doi gi them.
    /// </remarks>
    public class LogEntryModel
    {
        /// <summary>
        /// Ma dinh danh duy nhat cua dong log (Guid) - dung khi can tham chieu/
        /// xoa/cap nhat MOT dong cu the (VD: mot dong log hien tren ListView cua
        /// LogForm) ma khong phu thuoc vao vi tri (index) cua no trong danh sach,
        /// vi danh sach co the duoc sap xep/loc lai bat cu luc nao.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>Thoi gian thao tac dien ra.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Loai thao tac - dung lai <see cref="FileOperationType"/> (xem ghi chu
        /// tren dau lop) thay vi mot enum rieng cho log.
        /// </summary>
        public FileOperationType Operation { get; set; }

        /// <summary>Duong dan nguon (file/folder bi tac dong).</summary>
        public string Source { get; set; }

        /// <summary>
        /// Duong dan dich. Null/rong voi cac thao tac khong co dich nhu Delete,
        /// Rename (dung Message de ghi ten moi thay vi Destination), Open, Search.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Ket qua thuc hien - dung lai <see cref="OperationResult"/> (xem ghi
        /// chu tren dau lop) thay vi mot enum rieng cho log.
        /// </summary>
        public OperationResult Result { get; set; }

        /// <summary>
        /// Thong tin bo sung dang tu do: ly do that bai (VD: thong diep ngoai
        /// le), ten moi khi Rename, tu khoa khi Search, hoac bat ky chi tiet nao
        /// khac khong hop voi Source/Destination.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// So luong muc (file/thu muc) lien quan den thao tac nay - mac dinh 1
        /// (mot file/thu muc don le). Voi thao tac hang loat (VD: nguoi dung
        /// chon 20 file roi Xoa/Sao chep cung luc), ghi 1 dong log DUY NHAT voi
        /// ItemCount = 20 thay vi 20 dong log rieng le trung lap gan het thong
        /// tin - de doc lai lich su hon, va tranh phinh to file log qua nhanh.
        /// </summary>
        public int ItemCount { get; set; } = 1;

        /// <summary>
        /// Thoi gian thuc hien thao tac (tu luc bat dau den luc ket thuc, ke ca
        /// that bai/bi huy). Null neu khong do (VD: thao tac tuc thi nhu Rename,
        /// hoac noi goi khong can bao cao thoi gian). Huu ich de nhan ra thao tac
        /// bat thuong cham (VD: sao chep qua mang, thu muc rat lon).
        /// </summary>
        public TimeSpan? Duration { get; set; }

        public LogEntryModel()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }

        public LogEntryModel(FileOperationType operation, string source, string destination, OperationResult result, string message = null, int itemCount = 1, TimeSpan? duration = null)
            : this()
        {
            Operation = operation;
            Source = source;
            Destination = destination;
            Result = result;
            Message = message;
            ItemCount = itemCount;
            Duration = duration;
        }

        /// <summary>
        /// Bieu dien dong log o dang chuoi ngan gon, tien cho hien thi hoac ghi
        /// ra file text/CSV. VD:
        /// [2026-08-21 14:30:00] Copy | Nguồn: C:\a.txt | Đích: D:\a.txt | Success (1 mục, 0.42s)
        /// </summary>
        public override string ToString()
        {
            var destPart = string.IsNullOrEmpty(Destination) ? "-" : Destination;
            var line = $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Operation} | Nguồn: {Source} | Đích: {destPart} | {Result}";

            var extras = string.Empty;
            if (ItemCount != 1)
                extras += $"{ItemCount.ToString(CultureInfo.InvariantCulture)} mục";
            if (Duration.HasValue)
                extras += (extras.Length > 0 ? ", " : string.Empty) + $"{Duration.Value.TotalSeconds.ToString("0.##", CultureInfo.InvariantCulture)}s";
            if (extras.Length > 0)
                line += $" ({extras})";

            if (!string.IsNullOrEmpty(Message))
            {
                line += $" | {Message}";
            }
            return line;
        }
    }
}
