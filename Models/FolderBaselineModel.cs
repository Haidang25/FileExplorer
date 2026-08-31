using System;
using System.Collections.Generic;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// "Anh chup" (snapshot) baseline cua toan bo file trong MOT thu muc tai
    /// thoi diem bat dau giam sat thu muc do - day la "moc 0" ma cac lan kiem
    /// tra sau nay se so sanh voi hien trang thu muc de phat hien thay doi
    /// (sua noi dung/xoa/them moi). Xem Services/BaselineService.cs (noi tao/
    /// luu/doc lai model nay) de biet co che chi tiet: dinh dang luu, vi tri
    /// luu tren dia, va cach anh xa 1-1 giua mot thu muc va file baseline
    /// cua no.
    /// </summary>
    public class FolderBaselineModel
    {
        /// <summary>Duong dan day du cua thu muc goc duoc giam sat.</summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// True neu baseline nay bao gom ca file trong CAC THU MUC CON (de
        /// quy) cua FolderPath, false neu chi tinh file nam TRUC TIEP trong
        /// FolderPath. Luu lai gia tri nay (khong chi dung luc tao) de lan
        /// kiem tra sau con biet dung PHAM VI can quet lai cho khop voi
        /// baseline da luu, tranh so sanh lech pham vi (VD: baseline chi lay
        /// file cap 1 nhung lan kiem tra sau lai quet ca thu muc con).
        /// </summary>
        public bool IncludeSubdirectories { get; set; }

        /// <summary>
        /// Thoi diem (UTC) baseline nay duoc tao - la "thoi diem 0" cua phien
        /// giam sat, hien thi cho nguoi dung biet ho dang so sanh voi trang
        /// thai thu muc tu luc nao.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; }

        /// <summary>
        /// Danh sach ban ghi baseline cua tung file duoc tim thay trong
        /// FolderPath (va thu muc con, neu IncludeSubdirectories) tai thoi
        /// diem tao baseline - xem <see cref="FileBaselineEntry"/>. Thu muc
        /// con (chinh no, khong phai file BEN TRONG no) KHONG duoc dua vao
        /// danh sach nay - baseline chi quan tam NOI DUNG file (thu muc
        /// khong co "noi dung" de hash/so sanh, giong nguyen tac
        /// DuplicateService.FindDuplicateFiles da ap dung).
        /// </summary>
        public List<FileBaselineEntry> Entries { get; set; } = new List<FileBaselineEntry>();
    }
}
