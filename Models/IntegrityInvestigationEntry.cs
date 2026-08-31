using System;

namespace FileExplorerApp.Models
{
    /// <summary>
    /// Mot dong trong BAO CAO DIEU TRA toan ven (investigation report) - KHAC
    /// voi LogEntryModel (nhat ky thao tac file/thu muc THONG THUONG nhu Copy/
    /// Move/Delete). Moi IntegrityInvestigationEntry tuong ung VOI DUNG MOT vi
    /// pham toan ven ma IntegrityService da phat hien (xem
    /// IntegrityViolationEventArgs ben Services\IntegrityService.cs) - phuc vu
    /// muc dich DIEU TRA SAU NAY (ai dang dang nhap Windows luc xay ra, hash
    /// truoc/sau la gi, xay ra luc nao, tai duong dan nao), khac voi
    /// LogEntryModel von ghi lai KET QUA thao tac cua CHINH nguoi dung dang
    /// dung ung dung (Copy/Move/Delete... nguoi dung tu tay thuc hien).
    /// </summary>
    /// <remarks>
    /// VI SAO MODEL RIENG (khong tai su dung/mo rong LogEntryModel): LogService
    /// hien tai (xem "QUYET DINH THIET KE" tai Services\LogService.cs) da CO
    /// SAN mot dinh dang CSV 9 cot CO DINH (LogFileHeader), voi TryParseLogLine
    /// KIEM TRA CUNG "fields.Length != 9" nhu mot dieu kien hop le - nhoi them
    /// cot (VD HashBefore/HashAfter/UserName) vao LogEntryModel se PHA VO tinh
    /// tuong thich nguoc voi moi file log.csv da ton tai tren may nguoi dung
    /// (dong cu chi co 9 cot se bi TryParseLogLine tu choi vi khong con khop so
    /// cot moi). Ngoai ra, hai khai niem nay VE BAN CHAT khac nhau: LogEntryModel
    /// la nhat ky THAO TAC (Operation/Source/Destination/ItemCount/Duration -
    /// gan voi HANH DONG nguoi dung chu dong lam), con IntegrityInvestigationEntry
    /// la BANG CHUNG PHAP Y cho MOT vi pham he thong TU PHAT HIEN duoc (gan voi
    /// SU KIEN bat thuong xay ra, khong phai hanh dong chu dong) - tron 2 khai
    /// niem vao chung mot dong CSV se lam mot so cot LUON RONG tuy theo loai
    /// dong (VD Operation se luon rong voi dong dieu tra, HashBefore/HashAfter
    /// se luon rong voi dong nhat ky thao tac), gay kho hieu khi mo file xem
    /// thu cong. Vi vay LogService duoc "mo rong" (theo dung yeu cau) bang mot
    /// SCHEMA/FILE RIENG danh cho bao cao dieu tra, KHONG dung chung file/cot
    /// voi log.csv hien co - xem GetInvestigationReportFilePath/
    /// InvestigationReportFileHeader tai LogService.cs.
    /// </remarks>
    public class IntegrityInvestigationEntry
    {
        /// <summary>Thoi diem (UTC) vi pham duoc IntegrityService phat hien - xem IntegrityViolationEventArgs.DetectedAtUtc.</summary>
        public DateTime Timestamp { get; set; }

        /// <summary>Duong dan day du cua file lien quan den vi pham.</summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Loai vi pham, luu duoi dang CHUOI TEN ENUM (VD "ContentModified",
        /// "FileMissing", "UnexpectedNewFile") - xem IntegrityViolationType.
        /// Luu la string (khong phai tham chieu truc tiep enum cua Services)
        /// de Models KHONG can phu thuoc nguoc lai Services (Models hien tai
        /// la tang thap nhat, khong using FileExplorerApp.Services) - giu
        /// dung nguyen tac phan tang da co cua du an.
        /// </summary>
        public string ViolationType { get; set; }

        /// <summary>
        /// Hash SHA-256 TRUOC khi thay doi (hash mong doi/hash goc theo
        /// baseline) - co the rong neu khong ap dung (VD UnexpectedNewFile:
        /// chua tung co hash "truoc" nao ca).
        /// </summary>
        public string HashBefore { get; set; }

        /// <summary>
        /// Hash SHA-256 SAU khi thay doi (hash thuc te tai thoi diem phat
        /// hien) - co the rong neu khong ap dung (VD FileMissing: file khong
        /// con de hash).
        /// </summary>
        public string HashAfter { get; set; }

        /// <summary>
        /// Ten tai khoan Windows dang dang nhap TAI THOI DIEM ghi dong bao cao
        /// nay (Environment.UserName) - phuc vu dieu tra "ai dang su dung may
        /// luc phat hien vi pham". LUU Y: day la nguoi dang dang nhap Windows
        /// TREN MAY DANG CHAY IntegrityService, KHONG chac chan la nguoi THUC
        /// SU gay ra thay doi (VD: thay doi co the den tu mot tien trinh chay
        /// nen, mot ung dung khac, hoac truy cap mang tu xa vao file dang
        /// giam sat) - gia tri nay la MOT DAU MOI dieu tra, khong phai bang
        /// chung ket luan tuyet doi.
        /// </summary>
        public string UserName { get; set; }

        public IntegrityInvestigationEntry()
        {
        }

        public IntegrityInvestigationEntry(DateTime timestamp, string filePath, string violationType, string hashBefore, string hashAfter, string userName)
        {
            Timestamp = timestamp;
            FilePath = filePath;
            ViolationType = violationType;
            HashBefore = hashBefore;
            HashAfter = hashAfter;
            UserName = userName;
        }
    }
}
