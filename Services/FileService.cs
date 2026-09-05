using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Khung lop xu ly cac thao tac lien quan den file (tao, doi ten, xoa,
    /// di chuyen, sao chep, mo file, lay thong tin/danh sach file...).
    /// Cac phuong thuc hien tai chi la khai bao (signature) + TODO, can trien khai
    /// logic thuc te ben trong. Su dung cung Models (FileItemModel, OperationResult,
    /// FileOperationType) va Helpers (FileHelper, PermissionHelper) da co san.
    /// </summary>
    public class FileService
    {
        // TODO: co the tiem (inject) them cac service khac neu can, VD:
        // - mot service ghi log (su dung LogEntryModel) de ghi lai moi thao tac

        public FileService()
        {
            // TODO: khoi tao cac phu thuoc (dependency) neu co, hien tai chua can.
        }

        /// <summary>
        /// Kiem tra file co ton tai hay khong.
        /// </summary>
        /// <param name="filePath">Duong dan file can kiem tra.</param>
        public bool FileExists(string filePath)
        {
            // TODO: kiem tra filePath hop le + File.Exists(filePath)
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay thong tin chi tiet cua mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file.</param>
        /// <returns>FileItemModel chua thong tin file.</returns>
        public FileItemModel GetFileInfo(string filePath)
        {
            // TODO: dung FileItemModel.FromPath(filePath), co the ket hop
            // FileHelper.GetFileType(filePath) neu can hien thi loai file.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lay danh sach cac file (khong bao gom thu muc con) truc tiep trong mot thu muc.
        /// </summary>
        /// <param name="folderPath">Duong dan thu muc chua cac file.</param>
        /// <param name="includeHidden">
        /// True: lay ca file an/he thong (Hidden/System). False: bo qua cac file do.
        /// </param>
        /// <returns>
        /// Danh sach FileItemModel (IsDirectory = false), sap xep theo ten. Danh sach
        /// rong (khong phai null) neu folderPath khong ton tai hoac khong co quyen doc.
        /// </returns>
        public List<FileItemModel> GetFiles(string folderPath, bool includeHidden = true)
        {
            var files = new List<FileItemModel>();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return files;

            try
            {
                var fileInfos = new DirectoryInfo(folderPath)
                    .EnumerateFiles()
                    .OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase);

                foreach (FileInfo fileInfo in fileInfos)
                {
                    try
                    {
                        if (!includeHidden)
                        {
                            bool isHiddenOrSystem = fileInfo.Attributes.HasFlag(FileAttributes.Hidden)
                                || fileInfo.Attributes.HasFlag(FileAttributes.System);
                            if (isHiddenOrSystem)
                                continue;
                        }

                        files.Add(FileItemModel.FromFileInfo(fileInfo));
                    }
                    catch (UnauthorizedAccessException) { /* Khong doc duoc file nay - bo qua rieng no. */ }
                    catch (IOException) { /* VD: file dang bi khoa boi ung dung khac. */ }
                }
            }
            catch (UnauthorizedAccessException) { /* Khong co quyen liet ke thu muc - tra ve danh sach rong. */ }
            catch (IOException) { /* Thu muc nam tren o dia vua thao ra, duong dan mang bi ngat... */ }

            return files;
        }

        /// <summary>
        /// Lay danh sach TOAN BO cac muc (ca thu muc con VA file) truc tiep trong mot
        /// thu muc, thu muc liet ke truoc roi den file - giong thu tu hien thi cua
        /// Windows Explorer va lvwFiles trong MainForm. Ket hop ca FolderService (cho
        /// phan thu muc con) va FileService (cho phan file) vao mot loi goi duy nhat,
        /// tranh MainForm phai tu viet lai logic duyet + loc an/he thong o 2 noi.
        /// </summary>
        /// <param name="path">Duong dan thu muc can liet ke noi dung.</param>
        /// <param name="includeHidden">
        /// True: lay ca muc an/he thong (Hidden/System). False: bo qua cac muc do,
        /// dung khi nguoi dung tat tuy chon "Hien file/thu muc an".
        /// </param>
        /// <returns>
        /// Danh sach FileItemModel: cac thu muc con (IsDirectory = true) truoc, sap
        /// xep theo ten, roi den cac file (IsDirectory = false), cung sap xep theo
        /// ten. Danh sach rong (khong phai null) neu path khong ton tai hoac khong
        /// co quyen doc - khong nem exception ra ngoai.
        /// </returns>
        public List<FileItemModel> GetItems(string path, bool includeHidden = true)
        {
            var items = new List<FileItemModel>();

            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return items;

            var folderService = new FolderService();

            foreach (FolderItemModel folder in folderService.GetSubFolders(path, includeHidden))
            {
                items.Add(new FileItemModel
                {
                    Name = folder.Name,
                    FullPath = folder.FullPath,
                    ParentPath = folder.ParentPath,
                    Extension = string.Empty,
                    IsDirectory = true,
                    Size = 0,
                    CreatedDate = folder.CreatedDate,
                    ModifiedDate = folder.ModifiedDate,
                    LastAccessedDate = folder.ModifiedDate, // FolderItemModel khong luu rieng LastAccessedDate.
                    Attributes = folder.Attributes
                });
            }

            items.AddRange(GetFiles(path, includeHidden));

            return items;
        }

        /// <summary>
        /// Tao file moi (rong) ben trong mot thu muc.
        /// </summary>
        /// <param name="parentPath">Duong dan thu muc chua file moi.</param>
        /// <param name="fileName">Ten file moi (bao gom phan mo rong).</param>
        public OperationResult CreateFile(string parentPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(parentPath) || !Directory.Exists(parentPath))
                return OperationResult.NotFound;

            if (!FileHelper.IsValidFileName(fileName))
                return OperationResult.Failed;

            string fullPath = Path.Combine(parentPath, fileName);

            // Kiem tra do dai duong dan TRUOC KHI lam bat ky dieu gi khac - xem giai
            // thich chi tiet tai FolderService.CreateFolder (cung ky thuat, tranh
            // PermissionHelper.HasWritePermission() bao nham AccessDenied do chinh
            // file tam kiem tra quyen ghi cua no cung vuot MAX_PATH).
            if (FileHelper.IsPathTooLong(fullPath))
                return OperationResult.PathTooLong;

            if (File.Exists(fullPath) || Directory.Exists(fullPath))
                return OperationResult.Skipped; // Da ton tai muc trung ten.

            if (!PermissionHelper.HasWritePermission(parentPath))
                return OperationResult.AccessDenied;

            try
            {
                using (File.Create(fullPath))
                {
                    // Chi can tao file rong - dong ngay sau khi tao.
                }
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (PathTooLongException)
            {
                return OperationResult.PathTooLong;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Doi ten mot muc bat ky - tu dong nhan biet la file hay thu muc de goi
        /// ham xu ly tuong ung (RenameFile o day, hoac FolderService.RenameFolder),
        /// giup noi goi (VD: MainForm) khong can tu kiem tra Directory.Exists roi
        /// re nhanh giua _fileService/_folderService nhu truoc.
        /// </summary>
        /// <param name="path">Duong dan hien tai cua file hoac thu muc.</param>
        /// <param name="newName">Ten moi (chi ten, khong bao gom duong dan; voi file thi bao gom phan mo rong).</param>
        /// <returns>
        /// OperationResult.NotFound neu path khong ton tai (ca file lan thu muc);
        /// cac ket qua khac giong RenameFile/FolderService.RenameFolder.
        /// </returns>
        public OperationResult Rename(string path, string newName)
        {
            if (string.IsNullOrWhiteSpace(path))
                return OperationResult.NotFound;

            if (Directory.Exists(path))
                return new FolderService().RenameFolder(path, newName);

            if (File.Exists(path))
                return RenameFile(path, newName);

            return OperationResult.NotFound;
        }

        /// <summary>
        /// Doi ten mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file hien tai.</param>
        /// <param name="newName">Ten moi (bao gom phan mo rong, khong bao gom duong dan).</param>
        public OperationResult RenameFile(string filePath, string newName)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return OperationResult.NotFound;

            if (!FileHelper.IsValidFileName(newName))
                return OperationResult.Failed;

            string directory = Path.GetDirectoryName(filePath);
            string newPath = Path.Combine(directory ?? string.Empty, newName);

            // Xem chu thich tuong tu tai FolderService.CreateFolder - kiem tra do
            // dai TRUOC PermissionHelper.HasWritePermission() de tranh bao nham
            // AccessDenied.
            if (FileHelper.IsPathTooLong(newPath))
                return OperationResult.PathTooLong;

            if (File.Exists(newPath) || Directory.Exists(newPath))
                return OperationResult.Skipped; // Da co muc trung ten moi.

            if (!PermissionHelper.HasWritePermission(directory))
                return OperationResult.AccessDenied;

            try
            {
                File.Move(filePath, newPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (PathTooLongException)
            {
                return OperationResult.PathTooLong;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // File dang duoc mo/khoa boi chuong trinh khac (VD: dang mo trong
                // Word, Notepad++...) - tach rieng voi Failed de bao thong bao cu
                // the hon, huong dan nguoi dung dong chuong trinh do roi thu lai.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Nhan dien token dang "{ten_token}" hoac "{ten_token:dinh_dang}" trong
        /// mot mau ten (pattern) doi ten hang loat, VD: "{name}", "{n:000}".
        /// Dung chung cho GenerateBatchRenameName - tach rieng thanh field static
        /// (thay vi tao moi Regex mot lan cho moi file) vi Regex duoc bien dich
        /// (compile) mot lan roi dung lai nhieu lan se nhanh hon dang ke khi
        /// doi ten hang tram/nghin file.
        /// </summary>
        private static readonly Regex BatchRenameTokenRegex = new Regex(@"\{(name|ext|n|date)(?::([^}]+))?\}", RegexOptions.IgnoreCase);

        /// <summary>
        /// Sinh ten file/thu muc moi cho MOT duong dan theo mau ten (pattern),
        /// dung chung boi FileService.BatchRename (thuc su doi ten tren dia) va
        /// BatchRenameForm (xem truoc) - DAM BAO ca hai LUON tra ve CUNG MOT
        /// ket qua cho cung dau vao, tranh truong hop nguoi dung thay preview
        /// mot dang nhung ap dung lai ra ten khac.
        ///
        /// Token ho tro trong pattern:
        /// - {name}: ten goc, KHONG gom phan mo rong (Path.GetFileNameWithoutExtension).
        /// - {ext}: phan mo rong goc, KEM dau cham (VD ".jpg"); thu muc thuong
        ///   khong co phan mo rong nen se la chuoi rong.
        /// - {n} hoac {n:000}: so thu tu (bat dau tu 1, theo <paramref name="index"/>
        ///   nguoi goi truyen vao - phan sau dau ":" quyet dinh do rong dem so 0
        ///   dau, VD {n:000} -> "001", "002"...).
        /// - {date} hoac {date:yyyyMMdd}: ngay gio hien tai, phan sau dau ":" la
        ///   chuoi dinh dang DateTime tuy chinh.
        ///
        /// Neu pattern KHONG chua token {ext}, phan mo rong goc se duoc TU DONG
        /// noi vao cuoi ten moi - tranh nguoi dung vo tinh lam mat phan mo rong
        /// (VD go "{name}_backup" van ra "abc_backup.jpg" chu khong mat ".jpg").
        /// Cac ky tu khong hop le trong ten file (Path.GetInvalidFileNameChars)
        /// duoc thay bang "_" de ten moi luon la mot ten file hop le.
        /// </summary>
        /// <param name="originalPath">Duong dan day du hien tai (file hoac thu muc).</param>
        /// <param name="pattern">Mau ten, xem cac token ho tro o tren.</param>
        /// <param name="index">Vi tri (bat dau tu 0) cua muc nay trong danh sach dang doi ten hang loat - quyet dinh gia tri token {n}.</param>
        /// <returns>Ten moi (khong bao gom duong dan). Tra ve ten goc neu pattern rong hoac ket qua thay the ra chuoi rong.</returns>
        public static string GenerateBatchRenameName(string originalPath, string pattern, int index)
        {
            string originalName = Path.GetFileName(originalPath);
            if (string.IsNullOrWhiteSpace(pattern))
                return originalName;

            string extension = Path.GetExtension(originalPath) ?? string.Empty;

            string result = BatchRenameTokenRegex.Replace(pattern, match =>
            {
                string token = match.Groups[1].Value.ToLowerInvariant();
                string format = match.Groups[2].Success ? match.Groups[2].Value : null;

                switch (token)
                {
                    case "name":
                        return Path.GetFileNameWithoutExtension(originalPath);
                    case "ext":
                        return extension;
                    case "n":
                        {
                            int width = string.IsNullOrEmpty(format) ? 1 : format.Length;
                            return (index + 1).ToString().PadLeft(width, '0');
                        }
                    case "date":
                        return DateTime.Now.ToString(string.IsNullOrEmpty(format) ? "yyyyMMdd" : format);
                    default:
                        return match.Value;
                }
            });

            bool patternHasExtensionToken = pattern.IndexOf("{ext}", StringComparison.OrdinalIgnoreCase) >= 0
                || Regex.IsMatch(pattern, @"\{ext:", RegexOptions.IgnoreCase);
            if (!patternHasExtensionToken && !string.IsNullOrEmpty(extension))
                result += extension;

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
                result = result.Replace(invalidChar, '_');

            return string.IsNullOrEmpty(result) ? originalName : result;
        }

        /// <summary>
        /// Tra ve danh sach cac KY TU KHONG HOP LE (Path.GetInvalidFileNameChars())
        /// xuat hien trong PHAN VAN BAN CO DINH (literal) cua mot mau ten (pattern)
        /// doi ten hang loat - tuc PHAN NGOAI cac token {name}/{ext}/{n[:dinh_dang]}/
        /// {date[:dinh_dang]} (xem GenerateBatchRenameName). Chi PHAN LITERAL nguoi
        /// dung TU GO truc tiep (VD dau ":", "?", "*"...) moi duoc kiem tra - gia
        /// tri THAY THE cua cac token LUON hop le (lay tu ten file da co san tren
        /// dia, so thu tu, hoac chuoi ngay gio da dinh dang qua DateTime.ToString,
        /// KHONG the chua ky tu nhu "*"/"?"), nen khong can kiem tra phan do.
        /// </summary>
        /// <remarks>
        /// Dung de CANH BAO NGAY khi nguoi dung go pattern (xem
        /// BatchRenameForm.UpdatePreview/txtPattern_TextChanged) va CHAN xem
        /// truoc/xac nhan doi ten cho toi khi sua lai, THAY VI de
        /// GenerateBatchRenameName AM THAM thay the ky tu khong hop le bang "_"
        /// nhu truoc (nguoi dung khong duoc bao truoc, ten cuoi cung tren dia
        /// khac voi nhung gi ho go trong pattern ma khong hay biet).
        ///
        /// Dung CUNG mot Regex (BatchRenameTokenRegex) voi GenerateBatchRenameName
        /// de "cat bo" dung phan token TRUOC KHI kiem tra - dam bao 2 ham LUON
        /// thong nhat ve viec dau la token/dau la literal, tranh truong hop
        /// mot cai coi la token con cai kia lai coi la ky tu thuong.
        /// </remarks>
        /// <param name="pattern">Mau ten can kiem tra.</param>
        /// <returns>
        /// Danh sach cac ky tu khong hop le, KHONG TRUNG LAP, theo dung thu tu
        /// xuat hien LAN DAU trong pattern - danh sach RONG (khong phai null)
        /// neu pattern hop le hoac rong.
        /// </returns>
        public static List<char> GetInvalidPatternLiteralChars(string pattern)
        {
            var result = new List<char>();
            if (string.IsNullOrEmpty(pattern))
                return result;

            string literalOnly = BatchRenameTokenRegex.Replace(pattern, string.Empty);
            char[] invalidChars = Path.GetInvalidFileNameChars();

            foreach (char c in literalOnly)
            {
                if (Array.IndexOf(invalidChars, c) >= 0 && !result.Contains(c))
                    result.Add(c);
            }

            return result;
        }

        /// <summary>
        /// Sinh MOT ten tam (khong bao gio trung voi bat ky file/thu muc nao
        /// dang co, ke ca cac muc KHAC dang duoc doi ten trong CUNG mot lan
        /// goi BatchRename) - dua tren Guid nen xac suat trung la khong dang
        /// ke; van thu lai vai lan (kiem tra File.Exists/Directory.Exists)
        /// truoc khi danh cuoc vao Guid, giong tinh than "kiem tra truoc khi
        /// tin tuong" da dung o cac noi khac trong ung dung.
        /// </summary>
        private static string GenerateUniqueTempName(string originalPath)
        {
            string directory = Path.GetDirectoryName(originalPath) ?? string.Empty;
            string extension = Path.GetExtension(originalPath) ?? string.Empty;

            for (int attempt = 0; attempt < 5; attempt++)
            {
                string candidateName = "~batchrename_" + Guid.NewGuid().ToString("N") + extension;
                string candidatePath = Path.Combine(directory, candidateName);
                if (!File.Exists(candidatePath) && !Directory.Exists(candidatePath))
                    return candidateName;
            }

            // Cuc ky khong the xay ra (5 Guid lien tiep deu trung mot muc co
            // san) - noi them Ticks de chac chan khac voi 5 lan thu tren.
            return "~batchrename_" + Guid.NewGuid().ToString("N") + "_" + DateTime.Now.Ticks + extension;
        }

        /// <summary>
        /// Kiem tra TRUOC (khong dong vao dia gi ca) xem lan doi ten hang loat
        /// nay co dan den TRUNG TEN thuc su hay khong - dung cho CA BatchRenameForm
        /// (goi truoc khi hien hop thoai xac nhan, de bao loi va DUNG HAN neu co
        /// trung ten, thay vi cu xac nhan roi doi "do dang") LAN BatchRename()
        /// (goi lai o dau ham do lam lop bao ve thu 2, phong khi co loi goi khac
        /// trong tuong lai khong di qua BatchRenameForm).
        ///
        /// Phan biet 2 loai trung ten THAT SU can chan (KHONG bao gom truong hop
        /// "hoan doi ten cho nhau" giua cac muc trong LO - VD A muon lay ten cua
        /// B, B muon lay ten khac - day la truong hop HOP LE, da duoc BatchRename
        /// tu xu ly dung qua co che doi tam 2 giai doan, KHONG duoc coi la xung
        /// dot o day):
        /// - Hai muc TRONG CUNG LO cho ra CUNG MOT ten dich (VD pattern khong co
        ///   token {n} nen tat ca deu thanh cung mot ten) - chi mot trong hai co
        ///   the giu duoc ten do, muc con lai CHAC CHAN se bi Skipped neu cu doi.
        /// - Ten dich trung voi MOT FILE/THU MUC DA CO SAN tren dia nhung KHONG
        ///   thuoc lo dang doi ten nay (vi lo se tu "nhuong" het ten GOC cua minh
        ///   o Giai doan 1 truoc khi doi vao ten dich, nen trung voi CHINH ten
        ///   goc cua MOT MUC KHAC trong lo la binh thuong, khong tinh la xung dot).
        /// </summary>
        /// <param name="paths">Danh sach duong dan can doi ten - GIONG HET danh sach se truyen cho BatchRename().</param>
        /// <param name="pattern">Mau ten - GIONG HET pattern se truyen cho BatchRename().</param>
        /// <returns>
        /// Danh sach mo ta (tieng Viet, de hien truc tiep cho nguoi dung) tung
        /// xung dot phat hien duoc; rong neu khong co xung dot nao (an toan de
        /// tien hanh doi ten). Muc bi NotFound (da bi xoa/di chuyen tu truoc)
        /// KHONG duoc tinh la xung dot o day - BatchRename() se tu bao rieng.
        /// </returns>
        public List<string> ValidateBatchRenameConflicts(List<string> paths, string pattern)
        {
            var conflicts = new List<string>();
            if (paths == null || paths.Count == 0)
                return conflicts;

            var targetPaths = new string[paths.Count];
            var validIndexes = new List<int>();

            for (int i = 0; i < paths.Count; i++)
            {
                string originalPath = paths[i];
                if (string.IsNullOrWhiteSpace(originalPath) || (!File.Exists(originalPath) && !Directory.Exists(originalPath)))
                    continue; // NotFound - BatchRename() tu bao rieng, khong tinh la trung ten o day.

                string targetName = GenerateBatchRenameName(originalPath, pattern, i);
                string directory = Path.GetDirectoryName(originalPath) ?? string.Empty;
                targetPaths[i] = Path.Combine(directory, targetName);
                validIndexes.Add(i);
            }

            // Loai 1: nhieu muc TRONG CUNG LO cho ra CUNG MOT duong dan dich (so
            // sanh CA duong dan, khong chi ten - de dung ngay ca khi cac muc
            // trong lo nam o nhieu thu muc khac nhau).
            var reportedTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var duplicateGroups = validIndexes
                .GroupBy(i => targetPaths[i], StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicateGroups)
            {
                string targetName = Path.GetFileName(group.Key);
                string sourceNames = string.Join(", ", group.Select(i => "\"" + Path.GetFileName(paths[i]) + "\""));
                conflicts.Add($"{sourceNames} đều bị đổi thành cùng một tên \"{targetName}\"");
                reportedTargets.Add(group.Key);
            }

            // Loai 2: ten dich trung voi mot muc DA CO SAN tren dia nhung KHONG
            // thuoc lo nay (xem giai thich chi tiet o <summary>).
            var originalPathSet = new HashSet<string>(
                paths.Where(p => !string.IsNullOrWhiteSpace(p)), StringComparer.OrdinalIgnoreCase);

            foreach (int i in validIndexes)
            {
                string target = targetPaths[i];
                if (reportedTargets.Contains(target))
                    continue; // Da bao o Loai 1 o tren, tranh bao trung 2 lan cho cung 1 ten dich.

                bool existsOnDisk = File.Exists(target) || Directory.Exists(target);
                bool isOwnOriginalPath = string.Equals(target, paths[i], StringComparison.OrdinalIgnoreCase);
                bool isAnotherItemsOriginalPathInBatch = originalPathSet.Contains(target);

                if (existsOnDisk && !isOwnOriginalPath && !isAnotherItemsOriginalPathInBatch)
                {
                    conflicts.Add(
                        $"\"{Path.GetFileName(paths[i])}\" -> \"{Path.GetFileName(target)}\": " +
                        "đã có sẵn một mục khác cùng tên tại vị trí đó");
                    reportedTargets.Add(target);
                }
            }

            return conflicts;
        }

        /// <summary>
        /// Doi ten hang loat danh sach file/thu muc theo MOT mau ten (pattern)
        /// dung chung - xem GenerateBatchRenameName de biet cac token ho tro
        /// ({name}, {ext}, {n}/{n:000}, {date}).
        ///
        /// XU LY QUA 2 GIAI DOAN de xu ly DUNG cac truong hop trung ten PHAT
        /// SINH GIUA CAC MUC TRONG CUNG LO (VD: muc A duoc doi thanh ten ma
        /// muc B dang giu, roi B moi duoc doi sang ten khac trong CUNG lan
        /// goi nay - hoan doi ten cho nhau, hoac dai hon: A->ten cua B, B->ten
        /// cua C, C->ten cua A):
        ///
        /// - Giai doan 1 (doi tam): TOAN BO muc hop le duoc doi sang MOT TEN
        ///   TAM rieng (xem GenerateUniqueTempName) NGAY TRONG CHINH thu muc
        ///   cua no. Vi ten tam la Guid nen KHONG BAO GIO trung voi ten cua
        ///   bat ky muc nao khac (ca file/thu muc co san tren dia lan ten GOC
        ///   cua CAC MUC KHAC trong lo nay) - sau giai doan nay, TOAN BO cac
        ///   muc trong lo deu da "nhuong lai" ten goc cua minh, nen khong con
        ///   muc nao trong lo co the lam "vuong" ten dich cua mot muc khac
        ///   nua, du khong can biet truoc thu tu xu ly phu thuoc nhau nhu the
        ///   nao.
        /// - Giai doan 2 (doi vao ten dich): tu ten tam, doi tiep sang ten
        ///   dich THAT (tinh tu pattern) THEO DUNG THU TU trong
        ///   <paramref name="paths"/> - luc nay neu hai muc vo tinh (hoac do
        ///   pattern) cho ra CUNG mot ten dich, muc xu ly TRUOC se chiem duoc
        ///   ten do, muc SAU se tu dong bi Skipped (dung y voi canh bao to do
        ///   o BatchRenameForm khi phat hien ten moi trung nhau trong xem
        ///   truoc) - day la xung dot THAT (chi mot file co the mang mot ten),
        ///   khong con lien quan gi den viec sap xep thu tu xu ly nua.
        ///
        /// Ca hai giai doan deu goi lai Rename() (ham da co san, dung chung
        /// voi doi ten tu F2 tren MainForm) de tai su dung toan bo kiem tra an
        /// toan (ten khong hop le, khong co quyen ghi, file dang bi khoa...),
        /// thay vi tu goi File.Move/Directory.Move rieng.
        ///
        /// Neu giai doan 2 khong thanh cong cho mot muc (VD: trung ten voi
        /// mot file/thu muc KHONG thuoc lo nay, hoac vua bi khoa giua luc dang
        /// xu ly), ham se CO GANG doi muc do TRA LAI ten goc (best-effort, bo
        /// qua ket qua cua lan doi lai nay) de nguoi dung khong bi "mat dau"
        /// file duoi mot ten tam ky la - truong hop cuc hiem ca lan doi lai
        /// nay cung khong thanh cong (VD: mat quyen ghi dung luc do), muc do
        /// se tam thoi con mang ten dang "~batchrename_..." tren dia; chi bao
        /// OperationResult loi thuc te, khong co dau hieu rieng cho tinh huong
        /// nay vi qua hiem va nguoi dung van co the tim/doi ten lai thu cong.
        /// </summary>
        /// <param name="paths">Danh sach duong dan (file hoac thu muc) can doi ten, THEO DUNG THU TU muon dung de danh so token {n}.</param>
        /// <param name="pattern">Mau ten, xem GenerateBatchRenameName.</param>
        /// <returns>
        /// Danh sach ket qua CUNG DO DAI VA CUNG THU TU voi <paramref name="paths"/>
        /// - moi phan tu gom duong dan goc, duong dan moi DU KIEN (du thanh
        /// cong hay khong), va OperationResult thuc te cua rieng muc do.
        /// </returns>
        public List<BatchRenameItemResult> BatchRename(List<string> paths, string pattern)
        {
            var itemResults = new List<BatchRenameItemResult>();
            if (paths == null)
                return itemResults;

            var targetNames = new string[paths.Count];
            var tempPaths = new string[paths.Count];

            // Buoc 1: tinh ten dich cho TUNG muc TRUOC KHI dong vao dia gi ca
            // - dam bao gia tri token {n} luon theo DUNG vi tri trong danh
            // sach goc nguoi dung da chon, khong bi anh huong boi thu tu xu
            // ly thuc te o cac giai doan sau.
            for (int i = 0; i < paths.Count; i++)
            {
                string originalPath = paths[i];
                var itemResult = new BatchRenameItemResult { OriginalPath = originalPath };
                itemResults.Add(itemResult);

                if (string.IsNullOrWhiteSpace(originalPath) || (!File.Exists(originalPath) && !Directory.Exists(originalPath)))
                {
                    itemResult.NewPath = originalPath;
                    itemResult.Result = OperationResult.NotFound;
                    continue;
                }

                targetNames[i] = GenerateBatchRenameName(originalPath, pattern, i);
                string directory = Path.GetDirectoryName(originalPath);
                itemResult.NewPath = Path.Combine(directory ?? string.Empty, targetNames[i]);
            }

            // KIEM TRA TRUNG TEN TRUOC KHI DONG VAO DIA (defense-in-depth):
            // BatchRenameForm.btnApply_Click DA tu goi ValidateBatchRenameConflicts()
            // rieng truoc khi goi ham nay, de hien thong bao loi CHI TIET (liet
            // ke tung cap ten trung) va DUNG LAI HOAN TOAN truoc khi nguoi dung
            // kip xac nhan - nhung kiem tra LAI o day (ngay ben trong BatchRename)
            // de dam bao BAT KY LOI GOI NAO KHAC trong tuong lai (khong di qua
            // BatchRenameForm) cung KHONG THE vo tinh doi ten "do dang" (mot vai
            // muc da doi thanh cong, cac muc trung ten con lai bi Skipped) - neu
            // phat hien trung ten, DUNG NGAY o day, KHONG thuc hien Giai doan 1/2
            // ben duoi (tuc KHONG dong vao dia bat ky thay doi nao ca - "rollback"
            // o day don gian la CHUA TUNG DOI GI ca, khong can hoan tac).
            List<string> conflicts = ValidateBatchRenameConflicts(paths, pattern);
            if (conflicts.Count > 0)
            {
                foreach (BatchRenameItemResult itemResult in itemResults)
                {
                    if (itemResult.Result != OperationResult.NotFound)
                        itemResult.Result = OperationResult.Skipped;
                }
                return itemResults;
            }

            // Giai doan 1: doi tam TOAN BO muc hop le - xem giai thich chi
            // tiet o phan <summary> cua ham.
            for (int i = 0; i < paths.Count; i++)
            {
                if (itemResults[i].Result == OperationResult.NotFound)
                    continue;

                string tempName = GenerateUniqueTempName(paths[i]);
                OperationResult tempResult = Rename(paths[i], tempName);
                if (tempResult != OperationResult.Success)
                {
                    // Khong doi tam duoc (VD: dang bi khoa, khong co quyen) -
                    // muc nay van con NGUYEN o ten GOC, bao dung ket qua thuc
                    // te va KHONG dua sang giai doan 2.
                    itemResults[i].Result = tempResult;
                    continue;
                }

                string directory = Path.GetDirectoryName(paths[i]);
                tempPaths[i] = Path.Combine(directory ?? string.Empty, tempName);
            }

            // Giai doan 2: doi tu ten tam sang ten dich THAT, THEO DUNG THU TU
            // trong paths - xem giai thich chi tiet o phan <summary> cua ham.
            for (int i = 0; i < paths.Count; i++)
            {
                if (tempPaths[i] == null)
                    continue; // Da bi NotFound hoac loi ngay o Giai doan 1.

                OperationResult finalResult = Rename(tempPaths[i], targetNames[i]);
                if (finalResult == OperationResult.Success)
                {
                    itemResults[i].Result = OperationResult.Success;
                }
                else
                {
                    // Co gang tra lai TEN GOC (best-effort - xem giai thich o
                    // phan <summary> cua ham) roi bao dung ket qua loi thuc te.
                    string originalName = Path.GetFileName(paths[i]);
                    Rename(tempPaths[i], originalName);
                    itemResults[i].Result = finalResult;
                }
            }

            return itemResults;
        }

        /// <summary>
        /// Xoa mot file.
        /// </summary>
        /// <param name="filePath">Duong dan file can xoa.</param>
        /// <param name="permanent">
        /// True: xoa vinh vien (File.Delete, khong the khoi phuc). False: chuyen vao
        /// Recycle Bin - nen goi RecycleBinService.DeleteToRecycleBin() truc tiep cho
        /// truong hop nay (xem giai thich tuong tu trong FolderService.DeleteFolder).
        /// </param>
        public OperationResult DeleteFile(string filePath, bool permanent = false)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                return OperationResult.NotFound;

            if (!permanent)
            {
                throw new NotSupportedException(
                    "FileService.DeleteFile(permanent: false) chua duoc ho tro - " +
                    "hay dung RecycleBinService.DeleteToRecycleBin() de chuyen vao Thung rac.");
            }

            string directory = Path.GetDirectoryName(filePath);
            if (!PermissionHelper.HasWritePermission(directory))
                return OperationResult.AccessDenied;

            try
            {
                // Go co ReadOnly truoc (giong Windows Explorer) - File.Delete() se nem
                // UnauthorizedAccessException neu file dang co thuoc tinh nay, du da co
                // quyen ghi len thu muc cha (ReadOnly la co rieng cua tung file, khac
                // voi quyen NTFS cua thu muc chua no).
                FileHelper.ClearReadOnlyAttribute(filePath);
                File.Delete(filePath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Xoa vinh vien (khong qua Recycle Bin, KHONG THE KHOI PHUC) mot muc bat ky -
        /// tu dong nhan biet la file hay thu muc de goi ham xu ly tuong ung, giong
        /// cach Rename() da lam. Dung cho hanh dong Shift+Delete tren lvwFiles (giong
        /// Windows Explorer: Delete thuong = chuyen vao Thung rac qua RecycleBinService,
        /// Shift+Delete = xoa thang, bo qua Thung rac).
        /// </summary>
        /// <param name="path">Duong dan file hoac thu muc can xoa vinh vien.</param>
        /// <returns>
        /// OperationResult.NotFound neu path khong ton tai (ca file lan thu muc);
        /// cac ket qua khac giong DeleteFile/FolderService.DeleteFolder (voi permanent = true).
        /// </returns>
        public OperationResult DeletePermanently(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return OperationResult.NotFound;

            if (Directory.Exists(path))
                return new FolderService().DeleteFolder(path, permanent: true);

            if (File.Exists(path))
                return DeleteFile(path, permanent: true);

            return OperationResult.NotFound;
        }

        /// <summary>
        /// Di chuyen file sang vi tri khac.
        /// </summary>
        /// <param name="sourcePath">Duong dan file nguon.</param>
        /// <param name="destinationPath">Duong dan file dich (bao gom ten file).</param>
        public OperationResult MoveFile(string sourcePath, string destinationPath)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return OperationResult.NotFound;

            // Xem chu thich tuong tu tai FolderService.CreateFolder - kiem tra do
            // dai TRUOC PermissionHelper.HasWritePermission() de tranh bao nham
            // AccessDenied.
            if (FileHelper.IsPathTooLong(destinationPath))
                return OperationResult.PathTooLong;

            if (File.Exists(destinationPath) || Directory.Exists(destinationPath))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationDir = Path.GetDirectoryName(destinationPath);
            if (!PermissionHelper.HasWritePermission(destinationDir))
                return OperationResult.AccessDenied;

            if (FileHelper.IsOnDifferentDrive(sourcePath, destinationPath))
            {
                // File.Move() khong ho tro di chuyen truc tiep giua 2 o dia khac nhau
                // - tu dong chuyen sang Copy roi Delete nguon, giong hanh vi Windows
                // Explorer khi keo-tha giua 2 o (nguoi dung khong can biet co su khac
                // biet nay). Tach rieng buoc Copy va Delete de: neu Copy loi thi chua
                // dong gi den nguon (an toan); neu Copy thanh cong nhung Delete nguon
                // loi (VD: file nguon vua bi khoa ngay luc do) thi bao PartialSuccess
                // thay vi Success (da co ban sao o dich, nhung ban goc van con o
                // nguon - khac voi Move thuc su, nguoi dung can biet de tu xoa sau).
                try
                {
                    File.Copy(sourcePath, destinationPath, overwrite: false);
                }
                catch (UnauthorizedAccessException)
                {
                    return OperationResult.AccessDenied;
                }
                catch (PathTooLongException)
                {
                    return OperationResult.PathTooLong;
                }
                catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
                {
                    return OperationResult.FileInUse;
                }
                catch (IOException)
                {
                    return OperationResult.Failed;
                }

                try
                {
                    File.Delete(sourcePath);
                    return OperationResult.Success;
                }
                catch (IOException)
                {
                    return OperationResult.PartialSuccess;
                }
                catch (UnauthorizedAccessException)
                {
                    return OperationResult.PartialSuccess;
                }
            }

            try
            {
                File.Move(sourcePath, destinationPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (PathTooLongException)
            {
                return OperationResult.PathTooLong;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // File dang bi khoa boi chuong trinh khac - tach rieng voi Failed de
                // bao thong bao cu the hon, giong da lam voi RenameFile/DeleteFile.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Kich thuoc buffer (byte) dung cho moi luot doc/ghi trong CopyFileAsync -
        /// 1 MB giup giam so lan goi ReadAsync/WriteAsync voi file lon, nhung van du
        /// nho de khong chiem qua nhieu bo nho khi Copy nhieu file cung luc (Paste
        /// nhieu muc, xem mnuEditPaste_Click).
        /// </summary>
        private const int CopyBufferSize = 1024 * 1024;

        /// <summary>
        /// Sao chep file sang vi tri khac (dong bo) - giu lai de tuong thich voi cac
        /// noi goi cu/don gian chua can async; ben trong chi goi CopyFileAsync roi
        /// cho ket qua ngay (.GetAwaiter().GetResult()) thay vi viet lai logic File.Copy.
        /// Uu tien dung CopyFileAsync() truc tiep o cac noi co the await (VD: UI).
        /// </summary>
        /// <param name="sourcePath">Duong dan file nguon.</param>
        /// <param name="destinationPath">Duong dan file dich (bao gom ten file).</param>
        /// <param name="overwrite">True neu cho phep ghi de file dich da ton tai.</param>
        public OperationResult CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            return CopyFileAsync(sourcePath, destinationPath, overwrite).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Sao chep file sang vi tri khac theo kieu bat dong bo, tu doc/ghi bang
        /// FileStream + buffer (thay vi File.Copy noi bo cua .NET) de: (1) khong chan
        /// (block) UI thread trong luc copy file lon - MainForm co the await ham nay
        /// ngay trong mnuEditPaste_Click va van phan hoi duoc cac tuong tac khac; (2)
        /// tu kiem soat vong lap doc/ghi tung buffer de co the bao cao tien do qua
        /// tham so progress ben duoi.
        /// </summary>
        /// <param name="sourcePath">Duong dan file nguon.</param>
        /// <param name="destinationPath">Duong dan file dich (bao gom ten file).</param>
        /// <param name="overwrite">True neu cho phep ghi de file dich da ton tai.</param>
        /// <param name="progress">
        /// IProgress&lt;T&gt; (thuong la Progress&lt;long&gt; tao tren UI thread) de bao cao
        /// so byte da doc/ghi xong CUA RIENG file nay (luy ke, khong phai delta) sau
        /// moi lan doc/ghi mot buffer - noi goi (VD: FolderService khi copy ca thu
        /// muc, hoac MainForm khi copy mot file don le) tu quy doi gia tri nay sang
        /// FileOperationProgress phu hop voi ngu canh cua minh. Bo qua (null) neu
        /// khong can theo doi tien do.
        /// </param>
        /// <param name="cancellationToken">
        /// Token de huy giua chung (VD: nguoi dung bam nut Huy tren CopyProgressForm).
        /// Duoc truyen thang vao ReadAsync/WriteAsync de dap ung huy ngay giua vong
        /// lap doc/ghi buffer, khong phai cho copy xong het file moi kiem tra. Neu bi
        /// huy, file dich (dang do dang, chua nguyen ven) se duoc xoa bo (best-effort)
        /// truoc khi tra ve OperationResult.Cancelled - xem catch (OperationCanceledException) ben duoi.
        /// </param>
        public async Task<OperationResult> CopyFileAsync(
            string sourcePath, string destinationPath, bool overwrite = false,
            IProgress<long> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return OperationResult.NotFound;

            // Xem chu thich tuong tu tai FolderService.CreateFolder - kiem tra do
            // dai TRUOC PermissionHelper.HasWritePermission() de tranh bao nham
            // AccessDenied.
            if (FileHelper.IsPathTooLong(destinationPath))
                return OperationResult.PathTooLong;

            if (!overwrite && (File.Exists(destinationPath) || Directory.Exists(destinationPath)))
                return OperationResult.Skipped; // Da co muc trung ten tai dich.

            string destinationDir = Path.GetDirectoryName(destinationPath);
            if (!PermissionHelper.HasWritePermission(destinationDir))
                return OperationResult.AccessDenied;

            try
            {
                // FileMode.Create: tao moi hoac ghi de (truncate) file dich neu da ton
                // tai va overwrite == true - tuong duong hanh vi File.Copy(overwrite: true).
                using (FileStream sourceStream = new FileStream(
                    sourcePath, FileMode.Open, FileAccess.Read, FileShare.Read,
                    CopyBufferSize, useAsync: true))
                using (FileStream destinationStream = new FileStream(
                    destinationPath, FileMode.Create, FileAccess.Write, FileShare.None,
                    CopyBufferSize, useAsync: true))
                {
                    byte[] buffer = new byte[CopyBufferSize];
                    int bytesRead;
                    long totalBytesWritten = 0;
                    while ((bytesRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        await destinationStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                        totalBytesWritten += bytesRead;
                        progress?.Report(totalBytesWritten);
                    }
                }

                // File.Copy() tu dong sao chep ca cac thuoc tinh (VD: ReadOnly, Hidden)
                // va thoi gian tao/sua doi cua file nguon; tu doc/ghi bang FileStream thi
                // khong tu lam dieu nay, nen can gan lai thu cong de giu hanh vi giong nhau.
                try
                {
                    File.SetAttributes(destinationPath, File.GetAttributes(sourcePath));
                    File.SetCreationTimeUtc(destinationPath, File.GetCreationTimeUtc(sourcePath));
                    File.SetLastWriteTimeUtc(destinationPath, File.GetLastWriteTimeUtc(sourcePath));
                }
                catch (IOException) { /* Khong quan trong bang viec da copy xong noi dung - bo qua rieng loi nay. */ }
                catch (UnauthorizedAccessException) { /* VD: khong doi duoc thuoc tinh tren dich (mang, o dia chi doc) - bo qua. */ }

                return OperationResult.Success;
            }
            catch (OperationCanceledException)
            {
                // Xoa file dich dang do dang copy (moi co mot phan noi dung, chua
                // nguyen ven) - tranh de lai file "rac" nua vien nua sau khi Huy,
                // giong hanh vi Windows Explorer khi bam Cancel giua luc copy.
                try
                {
                    if (File.Exists(destinationPath))
                        File.Delete(destinationPath);
                }
                catch (IOException) { /* Khong xoa duoc file rac - bo qua, khong quan trong bang viec da huy theo yeu cau. */ }
                catch (UnauthorizedAccessException) { /* Tuong tu. */ }

                return OperationResult.Cancelled;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (PathTooLongException)
            {
                return OperationResult.PathTooLong;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // Nguon dang bi khoa boi ung dung khac (VD: dang mo trong Word) - tach
                // rieng voi Failed, giong da lam voi RenameFile/DeleteFile/MoveFile.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Mo file bang ung dung mac dinh cua he thong.
        /// </summary>
        /// <param name="filePath">Duong dan file can mo.</param>
        public OperationResult OpenFile(string filePath)
        {
            // TODO: kiem tra File.Exists(filePath), dung Process.Start(new ProcessStartInfo(filePath)
            // { UseShellExecute = true }) va bat try/catch Win32Exception (VD: khong co ung dung mo).
            throw new NotImplementedException();
        }
    }
}
