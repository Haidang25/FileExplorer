using System;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Cac ham tien ich dung de dinh dang du lieu hien thi (dung luong, v.v.)
    /// dung chung cho toan bo ung dung.
    /// </summary>
    public static class FormatHelper
    {
        private static readonly string[] SizeUnits = { "B", "KB", "MB", "GB", "TB", "PB" };

        /// <summary>
        /// Dinh dang dung luong (byte) sang chuoi de doc, tu dong chon don vi
        /// phu hop (B, KB, MB, GB, TB, PB), co so 1024.
        /// VD: 1536 -> "1.5 KB", 1073741824 -> "1 GB".
        /// </summary>
        /// <param name="bytes">So byte can dinh dang.</param>
        /// <param name="decimals">So chu so sau dau thap phan (mac dinh 2).</param>
        public static string FormatSize(long bytes, int decimals = 2)
        {
            if (bytes < 0)
                return "0 B";

            double size = bytes;
            int unitIndex = 0;

            while (size >= 1024 && unitIndex < SizeUnits.Length - 1)
            {
                size /= 1024;
                unitIndex++;
            }

            // Don vi B khong can phan thap phan.
            string format = unitIndex == 0 ? "0" : "0." + new string('#', Math.Max(decimals, 0));
            return $"{size.ToString(format)} {SizeUnits[unitIndex]}";
        }

        /// <summary>
        /// Nhu <see cref="FormatSize(long, int)"/> nhung nhan vao so nguyen khong dau (ulong),
        /// thuong dung khi lay dung luong tu API he thong (VD: DriveInfo).
        /// </summary>
        public static string FormatSize(ulong bytes, int decimals = 2)
        {
            return FormatSize((long)Math.Min(bytes, long.MaxValue), decimals);
        }
    }
}
