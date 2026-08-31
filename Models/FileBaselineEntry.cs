using System;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Ban ghi baseline cua MOT file: trang thai "goc" cua no tai thoi diem
    /// bat dau giam sat mot thu muc - dung lam moc so sanh sau nay de phat
    /// hien file bi SUA NOI DUNG, XOA, hoac phat hien file MOI phat sinh
    /// (khong co trong baseline) - xem <see cref="FolderBaselineModel"/> va
    /// Services/BaselineService.cs (noi tao/luu/doc lai cac ban ghi nay).
    /// </summary>
    public class FileBaselineEntry
    {
        /// <summary>Duong dan day du cua file tai thoi diem lay baseline.</summary>
        public string FilePath { get; set; }

        /// <summary>Kich thuoc file (byte) tai thoi diem lay baseline.</summary>
        public long Size { get; set; }

        /// <summary>
        /// Thoi gian sua doi cuoi (UTC, KHONG dung gio dia phuong nhu
        /// FileItemModel.ModifiedDate - xem BaselineService.CreateBaselineAsync)
        /// tai thoi diem lay baseline.
        ///
        /// Muc dich chinh: lam "kiem tra nhanh" TRUOC khi hash lai mot file de
        /// so sanh voi baseline - neu ca Size VA LastWriteTimeUtc cua file HIEN
        /// TAI deu khop voi baseline, co the KET LUAN LUON file chua thay doi
        /// ma khong can doc lai toan bo noi dung de hash (rat tot kem I/O+CPU
        /// voi file lon/thu muc nhieu file) - CHI can hash lai khi it nhat MOT
        /// trong hai gia tri nay khac biet, hoac khi noi goi can do chinh xac
        /// tuyet doi bat ke Size/LastWriteTimeUtc (VD: kiem tra dinh ky, it xay
        /// ra, chap nhan cham hon de khong bo sot truong hop hiem gap: noi
        /// dung file bi thay the nhung Size VA LastWriteTimeUtc deu duoc co
        /// tinh "phuc hoi" giong het gia tri cu - kha thi neu ke tan cong kiem
        /// soat duoc ca dong ho he thong, nhung nam ngoai pham vi de doa thong
        /// thuong ma tinh nang nay huong toi).
        /// </summary>
        public DateTime LastWriteTimeUtc { get; set; }

        /// <summary>
        /// Hash SHA-256 (chuoi hex chu thuong) noi dung file tai thoi diem lay
        /// baseline - xem HashHelper.ComputeSha256Async va ghi chu tai do ve
        /// ly do chon SHA-256 (khong phai MD5 nhu DuplicateService dung de tim
        /// file trung lap) cho muc dich giam sat/phat hien thay doi noi dung.
        /// </summary>
        public string Hash { get; set; }
    }
}
