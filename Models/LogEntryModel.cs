using System;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Loai thao tac duoc ghi log (copy, di chuyen, xoa, doi ten, tao moi...).
    /// </summary>
    public enum LogOperationType
    {
        Copy,
        Move,
        Delete,
        Rename,
        Create,
        Open,
        Other
    }

    /// <summary>
    /// Ket qua cua thao tac duoc ghi log.
    /// </summary>
    public enum LogResult
    {
        Success,
        Failed,
        Cancelled
    }

    /// <summary>
    /// Mo hinh du lieu cho mot dong nhat ky (log) thao tac file/folder:
    /// thoi gian, loai thao tac, nguon, dich va ket qua thuc hien.
    /// </summary>
    public class LogEntryModel
    {
        /// <summary>Thoi gian thao tac dien ra.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Loai thao tac (Copy, Move, Delete, Rename, Create, Open...).</summary>
        public LogOperationType Operation { get; set; }

        /// <summary>Duong dan nguon (file/folder bi tac dong).</summary>
        public string Source { get; set; }

        /// <summary>
        /// Duong dan dich. Co the null/empty voi cac thao tac khong co dich
        /// nhu Delete hoac Open.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>Ket qua thuc hien thao tac.</summary>
        public LogResult Result { get; set; }

        /// <summary>
        /// Thong tin bo sung khi thao tac thanh cong hoac ly do khi that bai
        /// (VD: thong bao loi ngoai le).
        /// </summary>
        public string Message { get; set; }

        public LogEntryModel()
        {
            Timestamp = DateTime.Now;
        }

        public LogEntryModel(LogOperationType operation, string source, string destination, LogResult result, string message = null)
        {
            Timestamp = DateTime.Now;
            Operation = operation;
            Source = source;
            Destination = destination;
            Result = result;
            Message = message;
        }

        /// <summary>
        /// Bieu dien dong log o dang chuoi ngan gon, tien cho hien thi hoac ghi ra file text.
        /// VD: [2026-08-21 14:30:00] Copy | Nguon: C:\a.txt | Dich: D:\a.txt | Success
        /// </summary>
        public override string ToString()
        {
            var destPart = string.IsNullOrEmpty(Destination) ? "-" : Destination;
            var line = $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] {Operation} | Nguon: {Source} | Dich: {destPart} | {Result}";
            if (!string.IsNullOrEmpty(Message))
            {
                line += $" | {Message}";
            }
            return line;
        }
    }
}
