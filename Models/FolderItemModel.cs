using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Mo hinh du lieu cho mot thu muc trong cay dieu huong (TreeView):
    /// thong tin co ban, quan he cha-con va trang thai hien thi.
    /// Khac voi <see cref="FileItemModel"/> (dung cho danh sach hien thi noi dung,
    /// gom ca file va thu muc), FolderItemModel tap trung vao cau truc cay thu muc.
    /// </summary>
    public class FolderItemModel
    {
        /// <summary>Ten thu muc (khong bao gom duong dan).</summary>
        public string Name { get; set; }

        /// <summary>Duong dan day du toi thu muc.</summary>
        public string FullPath { get; set; }

        /// <summary>Duong dan thu muc cha. Null/rong neu la thu muc goc (o dia).</summary>
        public string ParentPath { get; set; }

        /// <summary>Thoi gian tao.</summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>Thoi gian sua doi gan nhat.</summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>Thuoc tinh he thong file goc cua thu muc.</summary>
        public FileAttributes Attributes { get; set; }

        /// <summary>True neu thu muc bi an.</summary>
        public bool IsHidden => Attributes.HasFlag(FileAttributes.Hidden);

        /// <summary>True neu thu muc chi doc.</summary>
        public bool IsReadOnly => Attributes.HasFlag(FileAttributes.ReadOnly);

        /// <summary>True neu thu muc he thong.</summary>
        public bool IsSystem => Attributes.HasFlag(FileAttributes.System);

        /// <summary>True neu day la mot o dia (drive) - goc cua cay thu muc.</summary>
        public bool IsDrive { get; set; }

        /// <summary>
        /// Chi co y nghia khi IsDrive = true: true neu o dia dang san sang doc/ghi
        /// (tuong ung DriveInfo.IsReady). False voi o CD/DVD rong, o dia mang mat
        /// ket noi... - van hien trong danh sach/TreeView nhung khong the mo rong
        /// hay dieu huong vao, chi de nguoi dung biet o do co ton tai.
        /// Luon true voi thu muc thong thuong (khong phai o dia).
        /// </summary>
        public bool IsReady { get; set; } = true;

        /// <summary>
        /// Chi co y nghia khi IsDrive = true: loai o dia (Fixed, Removable, CDRom,
        /// Network, Ram...), tuong ung DriveInfo.DriveType. Dung de chon icon rieng
        /// cho tung loai o dia tren TreeView (xem MainForm.LoadTreeViewFolders).
        /// Gia tri mac dinh (DriveType.Unknown) voi thu muc thong thuong.
        /// </summary>
        public DriveType DriveType { get; set; } = DriveType.Unknown;

        /// <summary>
        /// True neu thu muc con it nhat mot thu muc con, dung de hien thi
        /// dau (+) tren TreeView ma khong can nap toan bo cay ngay tu dau (lazy-load).
        /// </summary>
        public bool HasSubFolders { get; set; }

        /// <summary>So luong file truc tiep trong thu muc (khong tinh de quy). -1 neu chua tinh.</summary>
        public int FileCount { get; set; } = -1;

        /// <summary>So luong thu muc con truc tiep (khong tinh de quy). -1 neu chua tinh.</summary>
        public int SubFolderCount { get; set; } = -1;

        /// <summary>Tong dung luong thu muc (byte), tinh de quy. -1 neu chua tinh (de tranh cham do voi thu muc lon).</summary>
        public long TotalSize { get; set; } = -1;

        /// <summary>
        /// Chi co y nghia khi IsDrive = true va IsReady = true: tong dung luong cua
        /// ca o dia (byte), tuong ung DriveInfo.TotalSize. -1 voi thu muc thong
        /// thuong hoac o dia chua san sang (khong doc duoc).
        /// </summary>
        public long DriveTotalSize { get; set; } = -1;

        /// <summary>
        /// Chi co y nghia khi IsDrive = true va IsReady = true: dung luong con trong
        /// nguoi dung con dung duoc (byte), tuong ung DriveInfo.AvailableFreeSpace.
        /// -1 voi thu muc thong thuong hoac o dia chua san sang (khong doc duoc).
        /// </summary>
        public long AvailableFreeSpace { get; set; } = -1;

        /// <summary>Trang thai dang mo/dong tren TreeView (phuc vu UI, khong anh huong du lieu thuc).</summary>
        public bool IsExpanded { get; set; }

        /// <summary>Danh sach thu muc con da duoc nap (rong neu chua nap - dung cho lazy-load).</summary>
        public List<FolderItemModel> SubFolders { get; set; } = new List<FolderItemModel>();

        public FolderItemModel()
        {
        }

        /// <summary>
        /// Tao FolderItemModel tu DirectoryInfo. Chi doc thong tin co ban va kiem tra
        /// nhanh xem co thu muc con hay khong (khong nap toan bo cay - lazy-load).
        /// </summary>
        public static FolderItemModel FromDirectoryInfo(DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null) throw new ArgumentNullException(nameof(directoryInfo));

            bool hasSubFolders = false;
            try
            {
                hasSubFolders = directoryInfo.EnumerateDirectories().Any();
            }
            catch (UnauthorizedAccessException) { /* Khong co quyen doc - coi nhu khong co thu muc con */ }
            catch (IOException) { /* O dia khong san sang (VD: o dia CD rong) */ }

            return new FolderItemModel
            {
                Name = directoryInfo.Name,
                FullPath = directoryInfo.FullName,
                ParentPath = directoryInfo.Parent?.FullName,
                CreatedDate = directoryInfo.CreationTime,
                ModifiedDate = directoryInfo.LastWriteTime,
                Attributes = directoryInfo.Attributes,
                IsDrive = directoryInfo.Parent == null,
                HasSubFolders = hasSubFolders
            };
        }

        /// <summary>Tao FolderItemModel tu duong dan thu muc.</summary>
        public static FolderItemModel FromPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path khong duoc rong.", nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Khong tim thay thu muc: {path}");

            return FromDirectoryInfo(new DirectoryInfo(path));
        }

        /// <summary>
        /// Nap danh sach thu muc con truc tiep (khong de quy) vao SubFolders.
        /// Dung khi nguoi dung mo rong (expand) mot node tren TreeView.
        /// </summary>
        public void LoadSubFolders()
        {
            SubFolders.Clear();
            try
            {
                foreach (var dir in new DirectoryInfo(FullPath).EnumerateDirectories()
                                                                .OrderBy(d => d.Name))
                {
                    SubFolders.Add(FromDirectoryInfo(dir));
                }
                SubFolderCount = SubFolders.Count;
            }
            catch (UnauthorizedAccessException)
            {
                SubFolderCount = 0;
            }
        }

        public override string ToString() => Name;
    }
}
