namespace FileExplorerApp.Models
{
    /// <summary>
    /// Hanh dong nguoi dung chon tren ConflictResolutionForm khi Paste gap mot muc
    /// da trung ten voi mot muc co san trong thu muc dich.
    /// </summary>
    public enum ConflictAction
    {
        /// <summary>Nguoi dung dong dialog (nhan X hoac Esc) - coi nhu Bo qua muc nay va dung het thao tac Paste con lai.</summary>
        Cancel,

        /// <summary>Ghi de len muc da co tai dich bang muc dang duoc dan.</summary>
        Overwrite,

        /// <summary>Dan voi mot ten moi (khac ten da trung) - xem ConflictResolutionForm.NewName.</summary>
        Rename,

        /// <summary>Bo qua rieng muc nay, tiep tuc dan cac muc con lai trong danh sach.</summary>
        Skip
    }
}
