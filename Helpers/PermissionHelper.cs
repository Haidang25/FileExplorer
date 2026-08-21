using System;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Cac ham tien ich kiem tra quyen truy cap (doc/ghi) tren thu muc,
    /// dung truoc khi thuc hien cac thao tac nhu tao file, copy, doi ten...
    /// de bao loi som va ro rang cho nguoi dung.
    /// </summary>
    public static class PermissionHelper
    {
        /// <summary>
        /// Kiem tra thu muc co the ghi duoc hay khong bang cach thu tao va xoa
        /// mot file tam trong do. Day la cach kiem tra chinh xac nhat vi phan anh
        /// dung quyen thuc te (ke ca cac truong hop ACL phuc tap, o dia chi doc/USB
        /// bao ve, folder OneDrive dang dong bo...), nhung co tac dong phu (tao/xoa file tam).
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc can kiem tra.</param>
        public static bool CanWriteByTest(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return false;

            string testFile = Path.Combine(folderPath, $"~write_test_{Guid.NewGuid():N}.tmp");
            try
            {
                using (File.Create(testFile, 1, FileOptions.DeleteOnClose))
                {
                    // File duoc tao va se tu xoa khi dong (DeleteOnClose) - khong can xu ly gi them.
                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                // Phong truong hop DeleteOnClose khong duoc ho tro tren mot so he thong file dac biet.
                try
                {
                    if (File.Exists(testFile))
                        File.Delete(testFile);
                }
                catch
                {
                    // Bo qua: khong the xoa file tam thi khong lam sap ket qua kiem tra chinh.
                }
            }
        }

        /// <summary>
        /// Kiem tra quyen ghi dua tren danh sach ACL (Access Control List) cua thu muc,
        /// khong tao/xoa file thuc te. Nhanh hon va khong co tac dong phu, nhung co the
        /// khong phan anh dung 100% mot so truong hop dac biet (VD: o dia chi doc vat ly,
        /// chinh sach nhom (Group Policy), OneDrive dang dong bo...).
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc can kiem tra.</param>
        public static bool HasWriteAccessByAcl(string folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return false;

            try
            {
                var directoryInfo = new DirectoryInfo(folderPath);
                DirectorySecurity security = directoryInfo.GetAccessControl();
                AuthorizationRuleCollection rules = security.GetAccessRules(true, true, typeof(SecurityIdentifier));

                var currentUser = WindowsIdentity.GetCurrent();
                var principal = new WindowsPrincipal(currentUser);

                bool allowWrite = false;

                foreach (FileSystemAccessRule rule in rules)
                {
                    if (!(rule.IdentityReference is SecurityIdentifier sid))
                        continue;

                    // Chi xet cac rule ap dung cho user hien tai hoac nhom ma user thuoc ve.
                    bool appliesToCurrentUser = currentUser.User != null && currentUser.User.Equals(sid);
                    bool appliesToGroup = !appliesToCurrentUser && principal.IsInRole(sid);
                    if (!appliesToCurrentUser && !appliesToGroup)
                        continue;

                    bool grantsWrite = (rule.FileSystemRights & (FileSystemRights.WriteData | FileSystemRights.CreateFiles)) != 0;
                    if (!grantsWrite)
                        continue;

                    if (rule.AccessControlType == AccessControlType.Deny)
                        return false; // Deny luon uu tien hon Allow.

                    if (rule.AccessControlType == AccessControlType.Allow)
                        allowWrite = true;
                }

                return allowWrite;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
            catch (Exception)
            {
                // Loi khong xac dinh khi doc ACL (VD: he thong file khong ho tro ACL Windows)
                // -> khong the khang dinh, coi nhu khong co quyen de an toan.
                return false;
            }
        }

        /// <summary>
        /// Kiem tra quyen ghi vao thu muc - phuong thuc tong quat, khuyen nghi dung
        /// truoc khi thuc hien thao tac ghi (tao file, copy, doi ten...).
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc can kiem tra.</param>
        /// <param name="useRealTest">
        /// True (mac dinh): kiem tra bang cach thu tao file thuc te (chinh xac nhat).
        /// False: chi kiem tra qua ACL, khong co tac dong phu tren he thong file.
        /// </param>
        public static bool HasWritePermission(string folderPath, bool useRealTest = true)
        {
            return useRealTest ? CanWriteByTest(folderPath) : HasWriteAccessByAcl(folderPath);
        }
    }
}
