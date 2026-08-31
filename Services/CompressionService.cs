using System;
using System.IO;
using System.IO.Compression;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Xu ly cac thao tac NEN/GIAI NEN thu muc va file - hien tai chi co
    /// <see cref="CompressFolder"/> (nen mot thu muc thanh file .zip). Dung
    /// ZipFile (namespace System.IO.Compression, assembly System.IO.Compression.FileSystem -
    /// xem ghi chu tai FileExplorerApp.csproj) THAY VI tu duyet ZipArchive/
    /// DeflateStream thu cong: ZipFile.CreateFromDirectory da lo lieu day du
    /// viec DUYET DE QUY toan bo cay thu muc con VA giu dung cau truc thu
    /// muc tuong doi ben trong file .zip, khong can tu viet lai logic do.
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
    }
}
