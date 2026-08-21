using System;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Mo hinh du lieu cho mot muc dang co trong Recycle Bin (Thung rac):
    /// ten, duong dan goc truoc khi bi xoa, thoi gian xoa va kich thuoc.
    /// </summary>
    public class RecycleBinItemModel
    {
        /// <summary>Ten file/thu muc.</summary>
        public string Name { get; set; }

        /// <summary>Duong dan goc truoc khi bi xoa.</summary>
        public string OriginalPath { get; set; }

        /// <summary>Thoi gian bi xoa (chuyen vao thung rac).</summary>
        public DateTime DeletedDate { get; set; }

        /// <summary>Kich thuoc (byte).</summary>
        public long Size { get; set; }

        /// <summary>True neu muc goc la thu muc.</summary>
        public bool IsDirectory { get; set; }

        public override string ToString() => Name;
    }
}
