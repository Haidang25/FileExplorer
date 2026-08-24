using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Cac ham tien ich lien quan den ten file/thu muc va phan loai loai file,
    /// dung chung cho toan bo ung dung (VD: khi doi ten, tao moi, hien thi ListView).
    /// </summary>
    public static class FileHelper
    {
        /// <summary>Ma loi Win32 ERROR_SHARING_VIOLATION (0x20 = 32), tra ve khi mot
        /// tien trinh khac dang mo/khoa file (VD: dang mo trong Word, Notepad++...)
        /// khien thao tac doi ten/di chuyen/xoa khong the thuc hien duoc luc nay.</summary>
        private const int ErrorSharingViolationHResult = unchecked((int)0x80070020);

        /// <summary>
        /// Kiem tra mot IOException co phai do file dang bi khoa boi chuong trinh
        /// khac hay khong (sharing violation), de phan biet voi cac IOException khac
        /// (VD: het dung luong dia, duong dan qua dai) va bao thong bao phu hop hon
        /// (OperationResult.FileInUse) thay vi Failed chung chung.
        /// </summary>
        /// <param name="ex">IOException bat duoc tu thao tac file (Move/Delete/Copy...).</param>
        public static bool IsSharingViolation(IOException ex)
        {
            return ex != null && ex.HResult == ErrorSharingViolationHResult;
        }

        /// <summary>Cac ten bi Windows gioi han khong duoc dat cho file/thu muc.</summary>
        private static readonly string[] ReservedNames =
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        /// <summary>Bang tra loai file theo phan mo rong (khong phan biet hoa/thuong).</summary>
        private static readonly Dictionary<string, string> FileTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Van ban / tai lieu
            [".txt"] = "Van ban (Text)",
            [".doc"] = "Tai lieu Word",
            [".docx"] = "Tai lieu Word",
            [".pdf"] = "Tai lieu PDF",
            [".rtf"] = "Tai lieu RTF",
            [".odt"] = "Tai lieu OpenDocument",

            // Bang tinh / trinh chieu
            [".xls"] = "Bang tinh Excel",
            [".xlsx"] = "Bang tinh Excel",
            [".csv"] = "Bang du lieu CSV",
            [".ppt"] = "Trinh chieu PowerPoint",
            [".pptx"] = "Trinh chieu PowerPoint",

            // Hinh anh
            [".jpg"] = "Hinh anh JPEG",
            [".jpeg"] = "Hinh anh JPEG",
            [".png"] = "Hinh anh PNG",
            [".gif"] = "Hinh anh GIF",
            [".bmp"] = "Hinh anh Bitmap",
            [".svg"] = "Hinh anh Vector (SVG)",
            [".ico"] = "Bieu tuong (Icon)",
            [".webp"] = "Hinh anh WebP",

            // Am thanh / video
            [".mp3"] = "Am thanh MP3",
            [".wav"] = "Am thanh WAV",
            [".flac"] = "Am thanh FLAC",
            [".mp4"] = "Video MP4",
            [".avi"] = "Video AVI",
            [".mkv"] = "Video MKV",
            [".mov"] = "Video MOV",

            // Nen / luu tru
            [".zip"] = "Tep nen ZIP",
            [".rar"] = "Tep nen RAR",
            [".7z"] = "Tep nen 7-Zip",
            [".tar"] = "Tep nen TAR",
            [".gz"] = "Tep nen GZIP",

            // Thuc thi / he thong
            [".exe"] = "Ung dung thuc thi",
            [".msi"] = "Trinh cai dat Windows",
            [".dll"] = "Thu vien lien ket dong (DLL)",
            [".bat"] = "Tep lenh Batch",
            [".ps1"] = "Tep lenh PowerShell",

            // Ma nguon / du lieu
            [".cs"] = "Ma nguon C#",
            [".html"] = "Trang web HTML",
            [".htm"] = "Trang web HTML",
            [".css"] = "Tep style CSS",
            [".js"] = "Ma nguon JavaScript",
            [".json"] = "Du lieu JSON",
            [".xml"] = "Du lieu XML",
            [".sql"] = "Tep lenh SQL",
        };

        /// <summary>
        /// Kiem tra ten file/thu muc co hop le tren Windows khong: khong rong,
        /// khong chua ky tu cam, khong phai ten bi gioi han (CON, PRN...),
        /// khong ket thuc bang khoang trang hoac dau cham, va khong vuot do dai cho phep.
        /// </summary>
        /// <param name="fileName">Ten file/thu muc can kiem tra (chi ten, khong bao gom duong dan).</param>
        public static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            // Do dai toi da cho mot thanh phan ten tren NTFS la 255 ky tu.
            if (fileName.Length > 255)
                return false;

            // Khong duoc ket thuc bang khoang trang hoac dau cham (Windows tu dong bo di,
            // co the gay nham lan hoac loi khi tao file).
            if (fileName.EndsWith(" ") || fileName.EndsWith("."))
                return false;

            // Khong chua ky tu khong hop le trong ten file (\ / : * ? " < > | va ky tu dieu khien).
            char[] invalidChars = Path.GetInvalidFileNameChars();
            if (fileName.Any(c => invalidChars.Contains(c)))
                return false;

            // Khong duoc la ten thiet bi dac biet cua Windows (bo qua phan mo rong khi so sanh).
            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (ReservedNames.Contains(nameWithoutExtension, StringComparer.OrdinalIgnoreCase))
                return false;

            return true;
        }

        /// <summary>Nhom icon rieng cho file tren ListView, dua tren phan mo rong (xem GetFileIconCategory).</summary>
        public enum FileIconCategory
        {
            /// <summary>Khong khop nhom nao rieng - dung icon file trung tinh mac dinh.</summary>
            Generic,
            Image,
            Document,
            Spreadsheet,
            Archive,
            Media,
            Code
        }

        /// <summary>Bang tra nhom icon theo phan mo rong (khong phan biet hoa/thuong).</summary>
        private static readonly Dictionary<string, FileIconCategory> FileIconMap = new Dictionary<string, FileIconCategory>(StringComparer.OrdinalIgnoreCase)
        {
            // Hinh anh
            [".jpg"] = FileIconCategory.Image,
            [".jpeg"] = FileIconCategory.Image,
            [".png"] = FileIconCategory.Image,
            [".gif"] = FileIconCategory.Image,
            [".bmp"] = FileIconCategory.Image,
            [".svg"] = FileIconCategory.Image,
            [".ico"] = FileIconCategory.Image,
            [".webp"] = FileIconCategory.Image,

            // Van ban / tai lieu
            [".txt"] = FileIconCategory.Document,
            [".doc"] = FileIconCategory.Document,
            [".docx"] = FileIconCategory.Document,
            [".pdf"] = FileIconCategory.Document,
            [".rtf"] = FileIconCategory.Document,
            [".odt"] = FileIconCategory.Document,

            // Bang tinh / trinh chieu (gom chung nhom voi bang tinh, khac hinh dang voi Document).
            [".xls"] = FileIconCategory.Spreadsheet,
            [".xlsx"] = FileIconCategory.Spreadsheet,
            [".csv"] = FileIconCategory.Spreadsheet,
            [".ppt"] = FileIconCategory.Spreadsheet,
            [".pptx"] = FileIconCategory.Spreadsheet,

            // Nen / luu tru
            [".zip"] = FileIconCategory.Archive,
            [".rar"] = FileIconCategory.Archive,
            [".7z"] = FileIconCategory.Archive,
            [".tar"] = FileIconCategory.Archive,
            [".gz"] = FileIconCategory.Archive,

            // Am thanh / video
            [".mp3"] = FileIconCategory.Media,
            [".wav"] = FileIconCategory.Media,
            [".flac"] = FileIconCategory.Media,
            [".mp4"] = FileIconCategory.Media,
            [".avi"] = FileIconCategory.Media,
            [".mkv"] = FileIconCategory.Media,
            [".mov"] = FileIconCategory.Media,

            // Ma nguon / thuc thi
            [".cs"] = FileIconCategory.Code,
            [".html"] = FileIconCategory.Code,
            [".htm"] = FileIconCategory.Code,
            [".css"] = FileIconCategory.Code,
            [".js"] = FileIconCategory.Code,
            [".json"] = FileIconCategory.Code,
            [".xml"] = FileIconCategory.Code,
            [".sql"] = FileIconCategory.Code,
            [".exe"] = FileIconCategory.Code,
            [".msi"] = FileIconCategory.Code,
            [".dll"] = FileIconCategory.Code,
            [".bat"] = FileIconCategory.Code,
            [".ps1"] = FileIconCategory.Code,
        };

        /// <summary>
        /// Xac dinh nhom icon rieng cho mot file dua tren phan mo rong, dung de chon
        /// ImageKey tren lvwFiles (xem MainForm.GetFileIconKey) - tach rieng voi
        /// GetFileType() vi muc dich khac nhau: GetFileType() tra ve chuoi hien thi
        /// chi tiet cho cot "Loai" (VD: "Hinh anh JPEG"), con ham nay chi gom vao
        /// mot trong 6 nhom hinh dang icon rong hon (VD: ca .jpg/.png/.gif deu chung
        /// mot icon "Image") de khong phai ve rieng hang chuc icon cho tung dinh dang.
        /// </summary>
        /// <param name="path">Duong dan hoac ten file (VD: "a.jpg", "C:\\a.jpg").</param>
        public static FileIconCategory GetFileIconCategory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return FileIconCategory.Generic;

            string extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
                return FileIconCategory.Generic;

            return FileIconMap.TryGetValue(extension, out FileIconCategory category)
                ? category
                : FileIconCategory.Generic;
        }

        /// <summary>
        /// Xac dinh loai file de hien thi (VD: "Hinh anh JPEG", "Tai lieu Word"...)
        /// dua tren phan mo rong. Nhan vao duong dan hoac chi ten file/phan mo rong.
        /// Neu la thu muc thi tra ve "Thu muc tap tin".
        /// </summary>
        /// <param name="path">Duong dan, ten file hoac phan mo rong (VD: "a.txt", ".txt", "C:\\a.txt").</param>
        public static string GetFileType(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "Khong xac dinh";

            if (Directory.Exists(path))
                return "Thu muc tap tin";

            string extension = Path.GetExtension(path);
            if (string.IsNullOrEmpty(extension))
                return "Tap tin";

            if (FileTypeMap.TryGetValue(extension, out string type))
                return type;

            // Khong co trong bang tra: tra ve dang "XYZ File" nhu Windows Explorer,
            // VD ".abc" -> "Tap tin ABC".
            string ext = extension.TrimStart('.').ToUpperInvariant();
            return $"Tap tin {ext}";
        }
    }
}
