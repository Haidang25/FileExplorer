namespace FileExplorerApp.Models
{
    /// <summary>
    /// Ket qua thuc hien mot thao tac file/thu muc (tra ve tu lop Services).
    /// Dung chung cho toan bo ung dung khi can bao cao trang thai thao tac,
    /// bao gom ca ghi vao <see cref="LogEntryModel"/>.
    /// </summary>
    public enum OperationResult
    {
        /// <summary>Thao tac thanh cong hoan toan.</summary>
        Success,

        /// <summary>Thao tac thanh cong mot phan (VD: sao chep 8/10 file, 2 file loi).</summary>
        PartialSuccess,

        /// <summary>Thao tac that bai.</summary>
        Failed,

        /// <summary>Nguoi dung huy thao tac giua chung.</summary>
        Cancelled,

        /// <summary>Thao tac bi bo qua (VD: file trung ten va nguoi dung chon Skip).</summary>
        Skipped,

        /// <summary>Khong du quyen truy cap de thuc hien thao tac.</summary>
        AccessDenied,

        /// <summary>Khong tim thay file/thu muc de thuc hien thao tac.</summary>
        NotFound
    }
}
