namespace FileExplorerApp.Models
{
    /// <summary>
    /// Ket qua doi ten CUA MOT MUC trong mot lan goi FileService.BatchRename -
    /// dung de noi goi (VD: BatchRenameForm) biet chinh xac muc nao thanh
    /// cong/bi bo qua/loi va vi sao, thay vi chi co MOT OperationResult chung
    /// cho ca lo (khong the phan biet duoc muc nao loi giua nhieu muc).
    /// </summary>
    public class BatchRenameItemResult
    {
        /// <summary>Duong dan day du BAN DAU (truoc khi doi ten) cua muc.</summary>
        public string OriginalPath { get; set; }

        /// <summary>
        /// Duong dan day du MOI DU KIEN (tinh tu pattern) cua muc - luon duoc
        /// gan DU Result co la Success hay khong, de noi goi van hien duoc
        /// "ten moi dinh dat" trong thong bao loi (VD: "abc.jpg -> Da ton tai").
        /// </summary>
        public string NewPath { get; set; }

        /// <summary>Ket qua thuc te cua rieng muc nay (xem FileService.BatchRename).</summary>
        public OperationResult Result { get; set; }
    }
}
