using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Xu ly cac thao tac NEN/GIAI NEN thu muc va file - <see cref="CompressFolder"/>
    /// (nen mot thu muc thanh file .zip) va <see cref="ExtractZip"/> (giai nen mot
    /// file .zip ra thu muc). Dung ZipFile (namespace System.IO.Compression, assembly
    /// System.IO.Compression.FileSystem - xem ghi chu tai FileExplorerApp.csproj)
    /// THAY VI tu duyet ZipArchive/DeflateStream thu cong: ca ZipFile.CreateFromDirectory
    /// lan ZipFile.ExtractToDirectory da lo lieu day du viec DUYET DE QUY toan bo cay
    /// thu muc con VA giu dung cau truc thu muc tuong doi, khong can tu viet lai logic do.
    /// </summary>
    public class CompressionService
    {
        /// <summary>
        /// Nen toan bo mot thu muc (bao gom tat ca thu muc con/file ben
        /// trong, giu nguyen cau truc) thanh MOT file .zip duy nhat tai
        /// zipPath.
        /// </summary>
        /// <remarks>
        /// QUYET DINH THIET KE:
        /// - KHONG tu ghi de (overwrite) file .zip da ton tai tai zipPath -
        ///   ZipFile.CreateFromDirectory tu ban chat se NEM IOException neu
        ///   file dich da co san (khac voi File.Copy co tham so overwrite),
        ///   nen kiem tra TRUOC va tra ve OperationResult.Skipped ro rang
        ///   (giong quy uoc "trung ten tai dich" da dung o FileService.CopyFileAsync
        ///   khi overwrite == false) thay vi de ngoai le chung chung "Failed"
        ///   xay ra ben trong ZipFile.CreateFromDirectory.
        /// - Chan TRUOC truong hop zipPath nam BEN TRONG chinh thu muc path
        ///   dang duoc nen (VD path="C:\A", zipPath="C:\A\out.zip") - neu
        ///   khong chan, ZipFile.CreateFromDirectory se co gang doc CHINH
        ///   file .zip dang duoc no ghi do lam mot phan noi dung can nen vao,
        ///   gay loi IOException kho hieu ("dang duoc su dung boi mot tien
        ///   trinh khac") hoac file .zip ket qua khong dung y muon - tra ve
        ///   OperationResult.InvalidDestination RO RANG hon, giong quy uoc
        ///   da dung o FileService/FolderService khi copy/di chuyen mot thu
        ///   muc vao CHINH NO hoac thu muc con cua no.
        /// - CompressionLevel.Optimal (mac dinh cua overload 2 tham so
        ///   ZipFile.CreateFromDirectory) - can bang hop ly giua ty le nen va
        ///   thoi gian nen, phu hop cho thao tac nen thu cong tu giao dien
        ///   (khac voi nhu cau nen realtime/toc do toi da).
        /// </remarks>
        /// <param name="path">Duong dan thu muc nguon can nen.</param>
        /// <param name="zipPath">Duong dan file .zip dich se duoc tao (bao gom ten file).</param>
        /// <returns>
        /// OperationResult.Success neu nen thanh cong; NotFound neu path
        /// khong phai thu muc hop le; Skipped neu zipPath da ton tai san
        /// (KHONG tu ghi de); InvalidDestination neu zipPath nam ben trong
        /// chinh path; AccessDenied/FileInUse/Failed cho cac loi con lai -
        /// xem <remarks>.
        /// </returns>
        public OperationResult CompressFolder(string path, string zipPath)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return OperationResult.NotFound;

            if (string.IsNullOrWhiteSpace(zipPath))
                return OperationResult.Failed;

            if (File.Exists(zipPath))
                return OperationResult.Skipped; // Da co file .zip trung ten tai dich - xem <remarks>.

            // Chuan hoa ca 2 duong dan (Path.GetFullPath) truoc khi so sanh -
            // tranh truong hop nguoi dung go duong dan tuong doi/co "..", hoac
            // 2 cach viet khac nhau cung tro ve mot vi tri thuc te.
            string normalizedSourcePath = Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string normalizedZipPath = Path.GetFullPath(zipPath);
            if (normalizedZipPath.StartsWith(normalizedSourcePath, StringComparison.OrdinalIgnoreCase))
                return OperationResult.InvalidDestination; // zipPath nam ben trong chinh path - xem <remarks>.

            string destinationDir = Path.GetDirectoryName(normalizedZipPath);
            if (!PermissionHelper.HasWritePermission(destinationDir))
                return OperationResult.AccessDenied;

            try
            {
                ZipFile.CreateFromDirectory(path, zipPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // Mot file BEN TRONG thu muc dang bi khoa boi chuong trinh khac
                // (VD dang mo trong Word) nen ZipFile.CreateFromDirectory khong
                // doc duoc de nen vao - tach rieng voi Failed, giong quy uoc da
                // dung o FileService.CopyFileAsync/RecycleBinService.DeleteToRecycleBin.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }

        /// <summary>
        /// Giai nen toan bo mot file .zip ra thu muc destPath (tu tao neu chua
        /// ton tai), giu nguyen cau truc thu muc con ben trong file .zip.
        /// </summary>
        /// <remarks>
        /// QUYET DINH THIET KE:
        /// - Neu destPath da ton tai VA khong rong, tra ve OperationResult.Skipped
        ///   thay vi giai nen de/tron (merge) vao - ZipFile.ExtractToDirectory (overload
        ///   2 tham so, khong co "overwrite") se NEM IOException ngay khi gap file
        ///   trung ten dau tien ben trong thu muc dich, dan den giai nen DO DANG (mot
        ///   phan noi dung da duoc ghi ra, phan con lai thi khong) - kho hieu hon nhieu
        ///   so voi viec chan tu dau va bao Skipped ro rang, giong quy uoc
        ///   CompressFolder da dung cho truong hop zipPath trung ten.
        /// - Neu destPath trung ten voi mot FILE (khong phai thu muc) da ton tai,
        ///   tra ve OperationResult.InvalidDestination - khong the giai nen zip
        ///   "vao trong" mot file thuong duoc.
        /// - Kiem tra quyen ghi TRUOC khi goi ZipFile.ExtractToDirectory: neu destPath
        ///   chua ton tai (truong hop pho bien nhat), kiem tra quyen ghi tren THU MUC
        ///   CHA cua destPath (giong dung quy uoc da dung o FolderService.CreateFolder/
        ///   MoveFolder cho duong dan dich chua ton tai), vi PermissionHelper.HasWritePermission
        ///   tu ban chat doi hoi thu muc phai co san de thu tao file test ben trong.
        /// </remarks>
        /// <param name="zipPath">Duong dan file .zip nguon can giai nen.</param>
        /// <param name="destPath">Duong dan thu muc dich se chua noi dung giai nen.</param>
        /// <returns>
        /// OperationResult.Success neu giai nen thanh cong; NotFound neu zipPath
        /// khong phai file hop le; Skipped neu destPath da ton tai va khong rong;
        /// InvalidDestination neu destPath trung ten voi mot file; AccessDenied/
        /// FileInUse/Failed cho cac loi con lai - xem <remarks>.
        /// </returns>
        public OperationResult ExtractZip(string zipPath, string destPath)
        {
            if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath))
                return OperationResult.NotFound;

            if (string.IsNullOrWhiteSpace(destPath))
                return OperationResult.Failed;

            string normalizedDestPath = Path.GetFullPath(destPath);

            if (File.Exists(normalizedDestPath))
                return OperationResult.InvalidDestination; // destPath trung ten voi 1 file - xem <remarks>.

            bool destPathExists = Directory.Exists(normalizedDestPath);
            if (destPathExists && Directory.EnumerateFileSystemEntries(normalizedDestPath).Any())
                return OperationResult.Skipped; // Thu muc dich da ton tai va KHONG rong - xem <remarks>.

            // destPath chua ton tai -> kiem tra quyen ghi tren thu muc CHA (giong
            // FolderService.CreateFolder/MoveFolder). destPath da ton tai (rong) ->
            // kiem tra quyen ghi ngay tren chinh no.
            string permissionCheckPath = destPathExists
                ? normalizedDestPath
                : Directory.GetParent(normalizedDestPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(permissionCheckPath))
                return OperationResult.AccessDenied;

            try
            {
                ZipFile.ExtractToDirectory(zipPath, normalizedDestPath);
                return OperationResult.Success;
            }
            catch (UnauthorizedAccessException)
            {
                return OperationResult.AccessDenied;
            }
            catch (InvalidDataException)
            {
                // File .zip bi hong hoac khong dung dinh dang zip - tach rieng voi
                // Failed de nguoi dung biet ro nguyen nhan la o CHINH file .zip nguon,
                // khong phai loi ghi vao thu muc dich.
                return OperationResult.Failed;
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                // Mot file da ton tai trong thu muc dich dang bi khoa boi chuong
                // trinh khac nen khong ghi de duoc - giong quy uoc da dung o
                // CompressFolder/FileService.CopyFileAsync.
                return OperationResult.FileInUse;
            }
            catch (IOException)
            {
                return OperationResult.Failed;
            }
        }
    }
}
