using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.FileIO;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Xu ly cac thao tac lien quan den Recycle Bin (Thung rac): chuyen
    /// file/thu muc vao thung rac (thay vi xoa vinh vien), xem danh sach,
    /// khoi phuc ve vi tri goc, va don trong toan bo thung rac.
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE - CACH TRUY CAP RECYCLE BIN: .NET Framework khong
    /// co API dung san (managed) cho GetRecycleBinItems/RestoreFromRecycleBin -
    /// chi Microsoft.VisualBasic.FileIO.FileSystem co san 2 ham DeleteFile/
    /// DeleteDirectory voi RecycleOption.SendToRecycleBin (dung cho
    /// DeleteToRecycleBin ben duoi). Vi vay 2 ham con lai phai dung Shell32
    /// COM automation (Shell.Application, NameSpace(10) = CSIDL_BITBUCKET)
    /// qua LATE BINDING (Type.InvokeMember) thay vi "dynamic" - tranh phai
    /// them reference Microsoft.CSharp.dll vao csproj chi de dung 2 ham nay,
    /// va tranh rui ro "dynamic" bien dich khac nhau giua trinh bien dich
    /// (VD mcs dung de kiem tra o day) so voi Visual Studio/csc thuc te.
    ///
    /// EmptyRecycleBin dung P/Invoke SHEmptyRecycleBinW (shell32.dll) thay vi
    /// Shell32 automation - day la API on dinh, KHONG phu thuoc ngon ngu
    /// hien thi (khac voi cach tim "verb Khoi phuc" o RestoreFromRecycleBin
    /// ben duoi, buoc phai doi ten hien thi cua verb vi Shell32 automation
    /// khong co ten "canonical" khong doi ngon ngu cho verb nay).
    /// </remarks>
    public class RecycleBinService
    {
        /// <summary>CSIDL_BITBUCKET - ma thu muc dac biet "Recycle Bin" dung cho Shell.Application.NameSpace.</summary>
        private const int CSIDL_BITBUCKET = 10;

        [DllImport("shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHEmptyRecycleBinW(IntPtr hwnd, string pszRootPath, uint dwFlags);

        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        /// <summary>
        /// HRESULT rieng ma mot so phien ban Windows tra ve tu SHEmptyRecycleBinW
        /// khi Thung rac DA RONG SAN (khong phai loi thuc su) - can coi day cung
        /// la thanh cong, tranh bao "That bai" sai cho truong hop nay.
        /// </summary>
        private const int E_UNEXPECTED = unchecked((int)0x8000FFFF);

        public RecycleBinService()
        {
        }

        /// <summary>
        /// Xoa mot file hoac thu muc bang cach chuyen vao Recycle Bin (khong xoa
        /// vinh vien - nguoi dung van co the khoi phuc lai qua RestoreFromRecycleBin
        /// hoac tu mo Recycle Bin cua Windows).
        /// </summary>
        /// <param name="path">Duong dan file/thu muc can xoa (chuyen vao thung rac).</param>
        public OperationResult DeleteToRecycleBin(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return OperationResult.Failed;

            try
            {
                if (Directory.Exists(path))
                {
                    // Go co ReadOnly cho toan bo file con truoc (Shell API thuong tu xu
                    // ly duoc file ReadOnly khi chuyen vao Thung rac, nhung van lam de
                    // dam bao nhat quan voi FileService/FolderService.DeleteFolder).
                    ClearReadOnlyAttributeRecursive(path);
                    FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    return OperationResult.Success;
                }

                if (File.Exists(path))
                {
                    try { FileHelper.ClearReadOnlyAttribute(path); }
                    catch (UnauthorizedAccessException) { /* De FileSystem.DeleteFile tu bao loi cu the hon. */ }
                    catch (IOException) { /* Tuong tu. */ }

                    FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    return OperationResult.Success;
                }

                return OperationResult.NotFound;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (OperationCanceledException)
            {
                // Nguoi dung bam Cancel tren hop thoai loi (neu UIOption khac OnlyErrorDialogs).
                return OperationResult.Cancelled;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // File (hoac mot file nao do ben trong thu muc) dang bi chuong trinh
                // khac khoa - tach rieng voi Failed de bao thong bao cu the hon, giong
                // da lam voi FileService.RenameFile/DeleteFile.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Go co ReadOnly cho toan bo file (de quy qua tat ca thu muc con) ben trong
        /// mot thu muc truoc khi chuyen vao Thung rac - xem giai thich tuong tu trong
        /// FolderService.ClearReadOnlyAttributeRecursive (khong tai su dung truc tiep
        /// duoc vi ham do la private cua class khac).
        /// </summary>
        private static void ClearReadOnlyAttributeRecursive(string folderPath)
        {
            try
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    try { FileHelper.ClearReadOnlyAttribute(filePath); }
                    catch (UnauthorizedAccessException) { /* Bo qua rieng file nay - DeleteDirectory se tu bao loi cu the hon sau. */ }
                    catch (IOException) { /* VD: file dang bi khoa boi ung dung khac. */ }
                }

                foreach (string subDir in Directory.GetDirectories(folderPath))
                {
                    ClearReadOnlyAttributeRecursive(subDir);
                }
            }
            catch (UnauthorizedAccessException) { /* Khong liet ke duoc thu muc nay - bo qua, de DeleteDirectory tu bao loi. */ }
            catch (IOException) { /* Tuong tu. */ }
        }

        /// <summary>
        /// Khoi tao doi tuong Shell.Application (COM automation) qua ProgID -
        /// dung chung cho GetRecycleBinItems/RestoreFromRecycleBin ben duoi.
        /// Tra ve null neu khong the tao duoc (VD moi truong khong co Shell32
        /// automation, cuc hiem tren Windows thuong).
        /// </summary>
        private static object CreateShellApplication()
        {
            Type shellType = Type.GetTypeFromProgID("Shell.Application");
            return shellType != null ? Activator.CreateInstance(shellType) : null;
        }

        /// <summary>
        /// Goi mot METHOD COM qua late binding (Type.InvokeMember) - dung thay
        /// "dynamic" xuyen suot lop nay, xem giai thich tai remarks dau lop.
        /// </summary>
        private static object InvokeCom(object target, string name, params object[] args)
        {
            return target.GetType().InvokeMember(
                name, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, target, args);
        }

        /// <summary>Doc mot PROPERTY COM qua late binding - xem InvokeCom o tren.</summary>
        private static object GetComProperty(object target, string name, params object[] args)
        {
            return target.GetType().InvokeMember(
                name, BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, target, args);
        }

        /// <summary>
        /// Doc ExtendedProperty cua mot FolderItem Shell32 (VD
        /// "System.Recycle.DeletedFrom", "System.Recycle.DateDeleted") - boc
        /// trong try/catch rieng vi mot so phien ban Windows/loai muc co the
        /// khong co san mot property cu the, ne muon ca viec doc toan bo muc
        /// do bi loai khoi danh sach chi vi thieu MOT property phu.
        /// </summary>
        private static object TryGetExtendedProperty(object item, string propertyName)
        {
            try
            {
                return InvokeCom(item, "ExtendedProperty", propertyName);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Lay danh sach cac muc hien co trong Recycle Bin, qua Shell32 COM
        /// automation (Shell.Application.NameSpace(10)). Tra ve danh sach
        /// RONG (khong nem Exception) neu khong the doc duoc Recycle Bin vi
        /// bat ky ly do gi - day la mot man hinh CHI XEM (read-only), khong
        /// nen lam RecycleBinForm bi loi mo khong duoc chi vi mot phien ban
        /// Windows dac biet nao do tra ve COM loi.
        /// </summary>
        public List<RecycleBinItemModel> GetRecycleBinItems()
        {
            var result = new List<RecycleBinItemModel>();
            object shell = null;

            try
            {
                shell = CreateShellApplication();
                if (shell == null)
                    return result;

                object recycleBin = InvokeCom(shell, "NameSpace", CSIDL_BITBUCKET);
                if (recycleBin == null)
                    return result;

                object items = InvokeCom(recycleBin, "Items");
                int count = Convert.ToInt32(GetComProperty(items, "Count"));

                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        object item = GetComProperty(items, "Item", i);
                        result.Add(MapShellItemToModel(recycleBin, item));
                    }
                    catch (Exception)
                    {
                        // Bo qua rieng muc nay (VD khong doc duoc mot property) -
                        // khong de mot muc loi lam mat toan bo danh sach.
                    }
                }
            }
            catch (Exception)
            {
                // Khong the truy cap Recycle Bin qua Shell32 automation - tra ve
                // danh sach da co (co the rong) thay vi nem loi ra ngoai.
            }
            finally
            {
                if (shell != null)
                    Marshal.ReleaseComObject(shell);
            }

            return result;
        }

        /// <summary>
        /// Chuyen mot FolderItem (Shell32) trong Recycle Bin sang
        /// RecycleBinItemModel - dung chung cho GetRecycleBinItems va
        /// RestoreFromRecycleBin (khi can doi khop OriginalPath).
        /// </summary>
        private static RecycleBinItemModel MapShellItemToModel(object recycleBinFolder, object item)
        {
            string name = Convert.ToString(GetComProperty(item, "Name"));

            // "System.Recycle.DeletedFrom" tra ve THU MUC goc (khong bao gom ten
            // file) - ghep voi Name de co OriginalPath day du. Day la property
            // ON DINH qua cac phien ban Windows (khac cot "Vi tri goc" hien thi
            // qua GetDetailsOf, thu tu cot co the khac nhau giua cac phien ban).
            object deletedFromObj = TryGetExtendedProperty(item, "System.Recycle.DeletedFrom");
            string deletedFrom = deletedFromObj as string;
            string originalPath = !string.IsNullOrEmpty(deletedFrom)
                ? Path.Combine(deletedFrom, name)
                : name;

            DateTime deletedDate;
            object dateDeletedObj = TryGetExtendedProperty(item, "System.Recycle.DateDeleted");
            if (dateDeletedObj is DateTime dt)
                deletedDate = dt;
            else if (dateDeletedObj != null && DateTime.TryParse(Convert.ToString(dateDeletedObj), out DateTime parsed))
                deletedDate = parsed;
            else
                deletedDate = DateTime.MinValue;

            bool isDirectory = Convert.ToBoolean(GetComProperty(item, "IsFolder"));

            long size = 0;
            if (!isDirectory)
            {
                // "System.Size" chi co y nghia cho FILE - thu muc trong Recycle Bin
                // khong tinh dung luong theo property nay (Shell32 tra ve 0 hoac
                // gia tri khong dang tin cho thu muc), giu Size = 0 cho thu muc
                // thay vi hien mot con so sai lech cho nguoi dung.
                object sizeObj = TryGetExtendedProperty(item, "System.Size");
                if (sizeObj != null)
                {
                    try { size = Convert.ToInt64(sizeObj); }
                    catch (Exception) { size = 0; }
                }
            }

            return new RecycleBinItemModel
            {
                Name = name,
                OriginalPath = originalPath,
                DeletedDate = deletedDate,
                Size = size,
                IsDirectory = isDirectory
            };
        }

        /// <summary>
        /// Khoi phuc mot muc da bi xoa tu Recycle Bin ve vi tri ban dau, bang
        /// cach tim FolderItem co OriginalPath trung khop roi goi verb
        /// "Khoi phuc"/"Restore" cua Shell32 tren muc do (tuong duong bam
        /// chuot phai > Restore trong Windows Explorer).
        /// </summary>
        /// <param name="originalPath">Duong dan goc cua muc truoc khi bi xoa (Name ghep voi thu muc chua no).</param>
        /// <remarks>
        /// QUYET DINH THIET KE - TIM VERB THEO TEN HIEN THI: Shell32 automation
        /// (FolderItemVerbs) KHONG co "ten canonical" khong doi ngon ngu cho
        /// verb Restore (khac IContextMenu/IFileOperation cap thap hon, phuc
        /// tap hon nhieu de goi qua interop) - vi vay phai doi ten hien thi
        /// (co the la "Restore", "Khôi phục", v.v. tuy ngon ngu Windows dang
        /// dung) chua tu khoa "restore"/"khoi phuc"/"khôi phục". Neu Windows
        /// dang chay o mot ngon ngu khac (VD Windows tieng Phap/tieng Trung)
        /// ma khong khop duoc tu khoa nao, ham nay tra ve Failed - nguoi dung
        /// van co the tu mo Recycle Bin cua Windows de khoi phuc thu cong
        /// (xem thong bao huong dan tai RecycleBinForm.btnRestore_Click).
        /// </remarks>
        public OperationResult RestoreFromRecycleBin(string originalPath)
        {
            if (string.IsNullOrWhiteSpace(originalPath))
                return OperationResult.Failed;

            object shell = null;

            try
            {
                shell = CreateShellApplication();
                if (shell == null)
                    return OperationResult.Failed;

                object recycleBin = InvokeCom(shell, "NameSpace", CSIDL_BITBUCKET);
                if (recycleBin == null)
                    return OperationResult.Failed;

                object items = InvokeCom(recycleBin, "Items");
                int count = Convert.ToInt32(GetComProperty(items, "Count"));

                object matchedItem = null;
                for (int i = 0; i < count; i++)
                {
                    object item = GetComProperty(items, "Item", i);
                    RecycleBinItemModel model;
                    try
                    {
                        model = MapShellItemToModel(recycleBin, item);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (string.Equals(model.OriginalPath, originalPath, StringComparison.OrdinalIgnoreCase))
                    {
                        matchedItem = item;
                        break;
                    }
                }

                if (matchedItem == null)
                    return OperationResult.NotFound;

                object verbs = InvokeCom(matchedItem, "Verbs");
                int verbCount = Convert.ToInt32(GetComProperty(verbs, "Count"));

                for (int i = 0; i < verbCount; i++)
                {
                    object verb = GetComProperty(verbs, "Item", i);
                    string verbName = Convert.ToString(GetComProperty(verb, "Name")) ?? string.Empty;
                    verbName = verbName.Replace("&", string.Empty);

                    if (verbName.IndexOf("restore", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        verbName.IndexOf("khôi phục", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        verbName.IndexOf("khoi phuc", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        InvokeCom(verb, "DoIt");
                        return OperationResult.Success;
                    }
                }

                // Khong tim thay verb Restore (VD ngon ngu he thong khac, xem remarks).
                return OperationResult.Failed;
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }
            finally
            {
                if (shell != null)
                    Marshal.ReleaseComObject(shell);
            }
        }

        /// <summary>
        /// Tinh tong dung luong hien tai cua Recycle Bin (byte), cong tong Size
        /// cua tat ca muc tu GetRecycleBinItems() - don gian hon P/Invoke
        /// SHQueryRecycleBin rieng, va da du chinh xac cho muc dich hien thi
        /// (xem RecycleBinForm.lblStatus).
        /// </summary>
        public long GetRecycleBinSize()
        {
            long total = 0;
            foreach (RecycleBinItemModel item in GetRecycleBinItems())
                total += item.Size;
            return total;
        }

        /// <summary>Kiem tra Recycle Bin co dang rong hay khong.</summary>
        public bool IsRecycleBinEmpty()
        {
            return GetRecycleBinItems().Count == 0;
        }

        /// <summary>
        /// Don sach toan bo Recycle Bin (xoa vinh vien tat ca cac muc dang co),
        /// qua P/Invoke SHEmptyRecycleBinW - khong hien hop thoai xac nhan/tien
        /// trinh cua Windows (SHERB_NOCONFIRMATION/SHERB_NOPROGRESSUI/SHERB_NOSOUND)
        /// vi RecycleBinForm.btnEmptyRecycleBin_Click da tu hoi xac nhan truoc
        /// bang MessageBox cua chinh ung dung (nhat quan voi
        /// LogForm.btnClearLogs_Click), khong can Windows hoi lai lan nua.
        /// </summary>
        public OperationResult EmptyRecycleBin()
        {
            try
            {
                int hr = SHEmptyRecycleBinW(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);

                // hr == 0 (S_OK): thanh cong. E_UNEXPECTED duoc mot so phien ban
                // Windows tra ve khi Thung rac DA RONG SAN truoc do - coi la
                // thanh cong (khong phai loi thuc su), xem hang so E_UNEXPECTED.
                if (hr == 0 || hr == E_UNEXPECTED)
                    return OperationResult.Success;

                return OperationResult.Failed;
            }
            catch (Exception)
            {
                return OperationResult.Failed;
            }
        }
    }
}
