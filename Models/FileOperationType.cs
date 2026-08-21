namespace FileExplorerApp.Models
{
    /// <summary>
    /// Cac loai thao tac co the thuc hien tren file/thu muc trong ung dung.
    /// Dung chung cho lop Services (thuc thi thao tac) va cac noi can phan loai
    /// hanh dong nguoi dung (VD: menu chuot phai, thanh cong cu).
    /// </summary>
    public enum FileOperationType
    {
        /// <summary>Sao chep file/thu muc.</summary>
        Copy,

        /// <summary>Di chuyen file/thu muc.</summary>
        Move,

        /// <summary>Xoa file/thu muc.</summary>
        Delete,

        /// <summary>Doi ten file/thu muc.</summary>
        Rename,

        /// <summary>Tao file moi.</summary>
        CreateFile,

        /// <summary>Tao thu muc moi.</summary>
        CreateFolder,

        /// <summary>Cat (chuan bi di chuyen) file/thu muc vao clipboard.</summary>
        Cut,

        /// <summary>Dan file/thu muc tu clipboard.</summary>
        Paste,

        /// <summary>Nen file/thu muc thanh file luu tru (VD: .zip).</summary>
        Compress,

        /// <summary>Giai nen file luu tru.</summary>
        Extract,

        /// <summary>Mo file bang ung dung mac dinh hoac mo thu muc.</summary>
        Open,

        /// <summary>Tim kiem file/thu muc theo ten hoac dieu kien.</summary>
        Search,

        /// <summary>Thao tac khac khong thuoc cac loai tren.</summary>
        Other
    }
}
