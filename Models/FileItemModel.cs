using System;
using System.IO;
using FileExplorerApp.Helpers;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Mo hinh du lieu cho mot muc (file hoac thu muc) hien thi trong
    /// giao dien quan ly tep tin: ten, duong dan, kich thuoc, thoi gian,
    /// thuoc tinh va cac co trang thai lien quan.
    /// </summary>
    public class FileItemModel
    {
        /// <summary>Ten file/thu muc (khong bao gom duong dan).</summary>
        public string Name { get; set; }

        /// <summary>Duong dan day du toi file/thu muc.</summary>
        public string FullPath { get; set; }

        /// <summary>Duong dan thu muc cha chua muc nay.</summary>
        public string ParentPath { get; set; }

        /// <summary>Phan mo rong cua file (VD: ".txt"). Rong voi thu muc.</summary>
        public string Extension { get; set; }

        /// <summary>True neu la thu muc, false neu la file.</summary>
        public bool IsDirectory { get; set; }

        /// <summary>Kich thuoc file tinh theo byte. Bang 0 voi thu muc (tru khi tinh tong).</summary>
        public long Size { get; set; }

        /// <summary>Thoi gian tao.</summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>Thoi gian sua doi gan nhat.</summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>Thoi gian truy cap gan nhat.</summary>
        public DateTime LastAccessedDate { get; set; }

        /// <summary>Thuoc tinh he thong file goc (FileAttributes) cua muc.</summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>True neu muc bi an (Hidden).</summary>
        public bool IsHidden => Attributes.HasFlag(FileAttributes.Hidden);

        /// <summary>True neu muc chi doc (ReadOnly).</summary>
        public bool IsReadOnly => Attributes.HasFlag(FileAttributes.ReadOnly);

        /// <summary>True neu muc la file/thu muc he thong (System).</summary>
        public bool IsSystem => Attributes.HasFlag(FileAttributes.System);

        /// <summary>True neu muc la duong dan tat (Reparse point / shortcut thu muc, symlink...).</summary>
        public bool IsSystemLink => Attributes.HasFlag(FileAttributes.ReparsePoint);

        /// <summary>
        /// Kich thuoc da dinh dang de hien thi (VD: "1.25 MB").
        /// Voi thu muc mac dinh tra ve rong tru khi Size duoc gan (VD: tinh tong dung luong).
        /// </summary>
        public string SizeFormatted => IsDirectory && Size == 0 ? string.Empty : FormatHelper.FormatSize(Size);

        public FileItemModel()
        {
        }

        /// <summary>
        /// Tao FileItemModel tu mot FileInfo (file cu the tren dia).
        /// </summary>
        public static FileItemModel FromFileInfo(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

            return new FileItemModel
            {
                Name = fileInfo.Name,
                FullPath = fileInfo.FullName,
                ParentPath = fileInfo.DirectoryName,
                Extension = fileInfo.Extension,
                IsDirectory = false,
                Size = fileInfo.Exists ? fileInfo.Length : 0,
                CreatedDate = fileInfo.CreationTime,
                ModifiedDate = fileInfo.LastWriteTime,
                LastAccessedDate = fileInfo.LastAccessTime,
                Attributes = fileInfo.Attributes
            };
        }

        /// <summary>
        /// Tao FileItemModel tu mot DirectoryInfo (thu muc cu the tren dia).
        /// </summary>
        public static FileItemModel FromDirectoryInfo(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));

            return new FileItemModel
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                ParentPath = directoryInfo.Parent?.FullName,
                Extension = string.Empty,
                IsDirectory = true,
                Size = 0,
                CreatedDate = directoryInfo.CreationTime,
                ModifiedDate = directoryInfo.LastWriteTime,
                LastAccessedDate = directoryInfo.LastAccessTime,
                Attributes = directoryInfo.Attributes
            };
        }

        /// <summary>
        /// Tao FileItemModel tu duong dan bat ky, tu nhan biet la file hay thu muc.
        /// </summary>
        public static FileItemModel FromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path khong duoc rong.", nameof(path));

            if (Directory.Exists(path))
                return FromDirectoryInfo(new DirectoryInfo(path));

            if (File.Exists(path))
                return FromFileInfo(new FileInfo(path));

            throw new FileNotFoundException("Khong tim thay file hoac thu muc.", path);
        }

        public override string ToString()
        {
            return IsDirectory ? $"[DIR] {Name}" : $"{Name} ({SizeFormatted})";
        }
    }
}
