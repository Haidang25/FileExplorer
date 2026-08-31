using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileExplorerApp.Helpers;
using FileExplorerApp.Models;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Xu ly cac thao tac NEN/GIAI NEN thu muc va file. Co 2 CAP DO cho moi thao
    /// tac, giong dung quy uoc da dung o FolderService (CopyFolder/CopyFolderAsync):
    /// - <see cref="CompressFolder"/>/<see cref="ExtractZip"/> (dong bo, KHONG bao
    ///   cao tien do/khong huy duoc): dung ZipFile.CreateFromDirectory/
    ///   ExtractToDirectory (namespace System.IO.Compression, assembly
    ///   System.IO.Compression.FileSystem) - don gian, dung cho truong hop khong
    ///   can theo doi tien do.
    /// - <see cref="CompressFolderAsync"/>/<see cref="ExtractZipAsync"/> (bat dong
    ///   bo, co IProgress&lt;FileOperationProgress&gt; + CancellationToken): TU
    ///   DUYET DE QUY bang ZipArchive/stream thay vi goi thang ZipFile.*, vi
    ///   ZipFile.CreateFromDirectory/ExtractToDirectory lam TRON MOT LUOT (khong
    ///   co diem nao de bao cao tien do tung file hay kiem tra huy giua chung) -
    ///   dung cho thu muc/file .zip LON tu giao dien (menu chuot phai), noi
    ///   MainForm hien CopyProgressForm + tspProgress giong het luong Dan (Paste),
    ///   xem MainForm.cmsCompressToZip_Click/cmsExtractHere_Click.
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
        /// Ban BAT DONG BO cua <see cref="CompressFolder"/> - cung dieu kien
        /// validate/tra ve OperationResult HET giong nhau, nhung TU DUYET DE QUY
        /// cay thu muc bang ZipArchive (ZipArchiveMode.Create) + FileStream.
        /// CopyToAsync thay vi goi thang ZipFile.CreateFromDirectory, de co the:
        /// (1) bao cao tien do qua <paramref name="progress"/> sau MOI file duoc
        /// nen xong, va (2) kiem tra <paramref name="cancellationToken"/> giua
        /// tung file/thu muc con, dung lai gan nhu ngay khi nguoi dung bam Huy
        /// thay vi phai cho nen xong toan bo thu muc (co the rat lau voi thu muc
        /// lon) - giong dung tinh than CopyFolderAsync cua FolderService.
        /// </summary>
        /// <param name="path">Duong dan thu muc nguon can nen.</param>
        /// <param name="zipPath">Duong dan file .zip dich se duoc tao.</param>
        /// <param name="progress">
        /// IProgress&lt;FileOperationProgress&gt; (thuong la Progress&lt;T&gt; tao
        /// tren UI thread) de bao cao tien do (so file da nen / tong so file - xem
        /// FileOperationProgress.PercentComplete). Bo qua (null) neu khong can
        /// theo doi tien do - luc do ham nay khong ton chi phi dem truoc tong so
        /// file (CountFiles).
        /// </param>
        /// <param name="cancellationToken">Token de huy giua chung (VD: nut Huy tren CopyProgressForm).</param>
        /// <returns>
        /// CompressionOperationResult goi lai OperationResult giong het CompressFolder
        /// (cong them Cancelled neu bi huy giua chung), kem SizeBeforeBytes (tong
        /// dung luong thu muc nguon) va SizeAfterBytes (dung luong file .zip ket
        /// qua) - CHI chinh xac khi Result == Success, xem CompressionOperationResult.
        /// </returns>
        public async Task<CompressionOperationResult> CompressFolderAsync(
            string path, string zipPath,
            IProgress<FileOperationProgress> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return new CompressionOperationResult(OperationResult.NotFound);

            if (string.IsNullOrWhiteSpace(zipPath))
                return new CompressionOperationResult(OperationResult.Failed);

            if (File.Exists(zipPath))
                return new CompressionOperationResult(OperationResult.Skipped); // Da co file .zip trung ten tai dich.

            string normalizedSourcePath = Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
            string normalizedZipPath = Path.GetFullPath(zipPath);
            if (normalizedZipPath.StartsWith(normalizedSourcePath, StringComparison.OrdinalIgnoreCase))
                return new CompressionOperationResult(OperationResult.InvalidDestination); // zipPath nam ben trong chinh path.

            string destinationDir = Path.GetDirectoryName(normalizedZipPath);
            if (!PermissionHelper.HasWritePermission(destinationDir))
                return new CompressionOperationResult(OperationResult.AccessDenied);

            // Luon dem so file VA tong dung luong (CountFilesAndSize) truoc khi bat
            // dau - khac CompressFolderAsync ban dau (chi dem khi progress != null):
            // gio can SizeBeforeBytes de ghi log DU KHI khong ai theo doi tien do
            // (progress == null), nen khong the tranh chi phi duyet cay thu muc nay
            // nua. progressState (mutable, cong don FilesCompleted qua de quy) van
            // CHI duoc tao khi progress != null - khong ton chi phi Report() vo ich.
            int totalFiles;
            long sourceSizeBytes;
            CountFilesAndSize(path, out totalFiles, out sourceSizeBytes);

            CompressionProgressState progressState = null;
            if (progress != null)
            {
                progressState = new CompressionProgressState
                {
                    Progress = progress,
                    TotalFiles = totalFiles
                };
            }

            string rootDir = normalizedSourcePath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            try
            {
                using (FileStream zipStream = new FileStream(normalizedZipPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    await CompressDirectoryRecursiveAsync(archive, rootDir, path, progressState, cancellationToken).ConfigureAwait(false);
                }

                // Doc lai dung luong file .zip SAU KHI zipStream/archive da Dispose()
                // (using o tren) - ZipArchive chi flush/ghi Central Directory (phan
                // "muc luc" quyet dinh dung luong cuoi cung) luc Dispose, doc som hon
                // se ra ket qua sai/chua day du.
                long resultSizeBytes = new FileInfo(normalizedZipPath).Length;
                return new CompressionOperationResult(OperationResult.Success, sourceSizeBytes, resultSizeBytes);
            }
            catch (OperationCanceledException)
            {
                // Nguoi dung bam Huy giua chung - xoa file .zip dang do dang. AN
                // TOAN xoa thang (khong can kiem tra lai) vi da chan Skipped o tren
                // neu file .zip nay da ton tai TRUOC do - file .zip hien co, neu
                // co, chac chan do CHINH lan nen nay tao ra.
                try { if (File.Exists(normalizedZipPath)) File.Delete(normalizedZipPath); }
                catch (UnauthorizedAccessException) { /* Khong xoa duoc file rac - bo qua, khong quan trong bang viec da huy theo yeu cau. */ }
                catch (IOException) { /* Tuong tu (VD: dang bi khoa boi tien trinh khac ngay luc do). */ }

                return new CompressionOperationResult(OperationResult.Cancelled);
            }
            catch (UnauthorizedAccessException)
            {
                return new CompressionOperationResult(OperationResult.AccessDenied);
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                return new CompressionOperationResult(OperationResult.FileInUse);
            }
            catch (IOException)
            {
                return new CompressionOperationResult(OperationResult.Failed);
            }
        }

        /// <summary>
        /// Duyet de quy mot thu muc, them tung file thanh 1 ZipArchiveEntry (doc/ghi
        /// bang stream + CopyToAsync, KHONG chan UI thread lau tren 1 file lon) -
        /// dung boi CompressFolderAsync. Them entry thu muc RONG (ket thuc bang "/",
        /// khong co noi dung) cho cac thu muc con khong co file/thu muc con nao ben
        /// trong, giong hanh vi ZipFile.CreateFromDirectory (neu khong, mot thu muc
        /// con hoan toan rong se "bien mat" khoi file .zip vi khong file nao ben
        /// trong no thiet lap duong dan do).
        /// </summary>
        private static async Task CompressDirectoryRecursiveAsync(
            ZipArchive archive, string rootDir, string currentDir,
            CompressionProgressState progressState, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] files = Directory.GetFiles(currentDir);
            string[] subDirs = Directory.GetDirectories(currentDir);

            foreach (string filePath in files)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string entryName = GetRelativeEntryName(rootDir, filePath);
                ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

                try { entry.LastWriteTime = File.GetLastWriteTime(filePath); }
                catch (ArgumentOutOfRangeException) { /* Ngoai khoang Zip ho tro (1980-2107) - bo qua, khong quan trong. */ }

                using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 81920, useAsync: true))
                using (Stream entryStream = entry.Open())
                {
                    await sourceStream.CopyToAsync(entryStream, 81920, cancellationToken).ConfigureAwait(false);
                }

                if (progressState != null)
                {
                    progressState.FilesCompleted++;
                    progressState.Report(Path.GetFileName(filePath));
                }
            }

            foreach (string subDir in subDirs)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await CompressDirectoryRecursiveAsync(archive, rootDir, subDir, progressState, cancellationToken).ConfigureAwait(false);
            }

            if (files.Length == 0 && subDirs.Length == 0 && !string.Equals(
                currentDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar),
                rootDir, StringComparison.OrdinalIgnoreCase))
            {
                string dirEntryName = GetRelativeEntryName(rootDir, currentDir) + "/";
                archive.CreateEntry(dirEntryName);
            }
        }

        /// <summary>
        /// Duong dan tuong doi (dung "/" lam ky tu phan cach - dung chuan dinh dang
        /// Zip tren MOI he dieu hanh, khong phai Path.DirectorySeparatorChar cua
        /// Windows) cua fullPath so voi rootDir, dung lam ZipArchiveEntry.FullName.
        /// </summary>
        private static string GetRelativeEntryName(string rootDir, string fullPath)
        {
            string relative = fullPath.Length > rootDir.Length ? fullPath.Substring(rootDir.Length) : string.Empty;
            relative = relative.TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            return relative.Replace(Path.DirectorySeparatorChar, '/').Replace(Path.AltDirectorySeparatorChar, '/');
        }

        /// <summary>
        /// Dem so luong file VA tong dung luong (byte) cua tat ca file - de quy
        /// qua toan bo thu muc con - dung de uoc tinh CompressionProgressState.
        /// TotalFiles VA SizeBeforeBytes cua CompressionOperationResult TRUOC khi
        /// bat dau nen. Gop 2 viec (dem + cong dung luong) vao MOT LAN duyet cay
        /// thu muc duy nhat (thay vi 2 ham rieng, 2 lan duyet rieng) vi ca 2 gia
        /// tri deu luon can den tu CompressFolderAsync gio da luon tinh
        /// SizeBeforeBytes (khong chi khi progress != null nhu truoc).
        /// </summary>
        private static void CountFilesAndSize(string folderPath, out int fileCount, out long totalBytes)
        {
            fileCount = 0;
            totalBytes = 0;

            try
            {
                foreach (string filePath in Directory.GetFiles(folderPath))
                {
                    fileCount++;
                    try { totalBytes += new FileInfo(filePath).Length; }
                    catch (UnauthorizedAccessException) { /* Bo qua RIENG file nay - chi anh huong do chinh xac cua uoc tinh. */ }
                    catch (IOException) { /* Tuong tu. */ }
                }

                foreach (string subDir in Directory.GetDirectories(folderPath))
                {
                    int subFileCount;
                    long subTotalBytes;
                    CountFilesAndSize(subDir, out subFileCount, out subTotalBytes);
                    fileCount += subFileCount;
                    totalBytes += subTotalBytes;
                }
            }
            catch (UnauthorizedAccessException) { /* Bo qua rieng nhanh nay - chi anh huong do chinh xac cua uoc tinh. */ }
            catch (IOException) { /* Tuong tu. */ }
        }

        /// <summary>
        /// Trang thai dung chung (mutable) cho MOT LAN goi CompressFolderAsync,
        /// cong don so file da nen xong qua nhieu file/thu muc con de tinh
        /// FileOperationProgress tren TOAN BO thu muc dang nen - giong het
        /// FolderService.CopyProgressState. Chi duoc tao khi co noi theo doi
        /// tien do (progress != null trong CompressFolderAsync).
        /// </summary>
        private class CompressionProgressState
        {
            public IProgress<FileOperationProgress> Progress;
            public int TotalFiles;
            public int FilesCompleted;

            public void Report(string currentFileName)
            {
                Progress.Report(new FileOperationProgress
                {
                    CurrentFileName = currentFileName,
                    FilesCompleted = FilesCompleted,
                    TotalFiles = TotalFiles,
                    CurrentFileBytesTransferred = 0,
                    CurrentFileTotalBytes = 0
                });
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

        /// <summary>
        /// Ban BAT DONG BO cua <see cref="ExtractZip"/> - cung dieu kien validate/
        /// tra ve OperationResult HET giong nhau, nhung TU DUYET tung ZipArchiveEntry
        /// (ZipArchiveMode.Read) + FileStream.CopyToAsync thay vi goi thang
        /// ZipFile.ExtractToDirectory, de bao cao tien do va ho tro huy giua chung -
        /// xem <see cref="CompressFolderAsync"/> (ly do tuong tu).
        /// </summary>
        /// <remarks>
        /// Vi TU MO tung ZipArchiveEntry thay vi de ZipFile.ExtractToDirectory tu
        /// lam (ham do NOI BO da tu chan "Zip Slip" - mot entry co FullName chua
        /// "../" co the ghi ra NGOAI destPath), ham nay PHAI TU kiem tra lai dieu
        /// do (xem IsWithinDirectory) truoc khi ghi tung entry - bo qua (khong giai
        /// nen) bat ky entry nao co duong dan dich nam ngoai destPath, tranh mot
        /// file .zip doc hai ghi de len file he thong ngoai y muon.
        /// </remarks>
        /// <param name="zipPath">Duong dan file .zip nguon can giai nen.</param>
        /// <param name="destPath">Duong dan thu muc dich se chua noi dung giai nen.</param>
        /// <param name="progress">Bao cao tien do (so entry da giai nen / tong so entry). Bo qua (null) neu khong can.</param>
        /// <param name="cancellationToken">Token de huy giua chung.</param>
        /// <returns>
        /// CompressionOperationResult goi lai OperationResult giong het ExtractZip
        /// (cong them Cancelled neu bi huy giua chung), kem SizeBeforeBytes (dung
        /// luong file .zip nguon) va SizeAfterBytes (tong dung luong da giai nen
        /// ra) - CHI chinh xac khi Result == Success, xem CompressionOperationResult.
        /// </returns>
        public async Task<CompressionOperationResult> ExtractZipAsync(
            string zipPath, string destPath,
            IProgress<FileOperationProgress> progress = null,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(zipPath) || !File.Exists(zipPath))
                return new CompressionOperationResult(OperationResult.NotFound);

            if (string.IsNullOrWhiteSpace(destPath))
                return new CompressionOperationResult(OperationResult.Failed);

            string normalizedDestPath = Path.GetFullPath(destPath);

            if (File.Exists(normalizedDestPath))
                return new CompressionOperationResult(OperationResult.InvalidDestination);

            bool destPathExistedBefore = Directory.Exists(normalizedDestPath);
            if (destPathExistedBefore && Directory.EnumerateFileSystemEntries(normalizedDestPath).Any())
                return new CompressionOperationResult(OperationResult.Skipped);

            string permissionCheckPath = destPathExistedBefore
                ? normalizedDestPath
                : Directory.GetParent(normalizedDestPath)?.FullName;
            if (!PermissionHelper.HasWritePermission(permissionCheckPath))
                return new CompressionOperationResult(OperationResult.AccessDenied);

            string normalizedDestPathWithSeparator = normalizedDestPath
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

            // Dung luong file .zip nguon - biet duoc ngay (khong can duyet gi them),
            // dung lam SizeBeforeBytes cua CompressionOperationResult.
            long zipSizeBytes = new FileInfo(zipPath).Length;
            long extractedSizeBytes = 0; // Cong don qua tung entry - xem ben duoi.

            try
            {
                Directory.CreateDirectory(normalizedDestPath);

                using (FileStream zipStream = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    int totalEntries = progress != null ? archive.Entries.Count : 0;
                    int entriesCompleted = 0;

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        string destEntryPath = Path.GetFullPath(
                            Path.Combine(normalizedDestPath, entry.FullName.Replace('/', Path.DirectorySeparatorChar)));

                        // "Zip Slip" - xem <remarks>: bo qua entry co duong dan
                        // (sau khi chuan hoa .. neu co) nam NGOAI normalizedDestPath.
                        if (!destEntryPath.StartsWith(normalizedDestPathWithSeparator, StringComparison.OrdinalIgnoreCase))
                            continue;

                        bool isDirectoryEntry = string.IsNullOrEmpty(entry.Name);
                        if (isDirectoryEntry)
                        {
                            Directory.CreateDirectory(destEntryPath);
                        }
                        else
                        {
                            string entryDestDir = Path.GetDirectoryName(destEntryPath);
                            if (!string.IsNullOrEmpty(entryDestDir))
                                Directory.CreateDirectory(entryDestDir);

                            using (Stream entryStream = entry.Open())
                            using (FileStream destStream = new FileStream(
                                destEntryPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 81920, useAsync: true))
                            {
                                await entryStream.CopyToAsync(destStream, 81920, cancellationToken).ConfigureAwait(false);
                            }

                            // entry.Length = dung luong SAU KHI giai nen (uncompressed) cua
                            // rieng entry nay, doc san tu Central Directory cua file .zip -
                            // dung thang gia tri nay de cong don SizeAfterBytes, tranh phai
                            // FileInfo(destEntryPath).Length (mo lai file vua ghi) cho tung
                            // file mot cach khong can thiet.
                            extractedSizeBytes += entry.Length;

                            try { File.SetLastWriteTime(destEntryPath, entry.LastWriteTime.DateTime); }
                            catch (ArgumentOutOfRangeException) { /* Ngoai khoang DateTime hop le - bo qua, khong quan trong. */ }
                            catch (UnauthorizedAccessException) { /* Tuong tu. */ }
                            catch (IOException) { /* Tuong tu. */ }
                        }

                        entriesCompleted++;
                        if (progress != null)
                        {
                            progress.Report(new FileOperationProgress
                            {
                                CurrentFileName = string.IsNullOrEmpty(entry.Name) ? entry.FullName : entry.Name,
                                FilesCompleted = entriesCompleted,
                                TotalFiles = totalEntries,
                                CurrentFileBytesTransferred = 0,
                                CurrentFileTotalBytes = 0
                            });
                        }
                    }
                }

                return new CompressionOperationResult(OperationResult.Success, zipSizeBytes, extractedSizeBytes);
            }
            catch (OperationCanceledException)
            {
                // Nguoi dung bam Huy giua chung - CHI xoa destPath neu chinh lan
                // giai nen nay la nguoi TAO RA no (destPathExistedBefore == false).
                // Neu destPath da ton tai TU TRUOC (VD: mot thu muc rong co san cua
                // nguoi dung, da qua kiem tra Skipped o tren vi rong), KHONG xoa -
                // chi de lai phan noi dung do dang (an toan hon xoa nham thu muc
                // khong phai do thao tac nay tao ra) - giong tinh than thu vi cua
                // FolderService.CopyFolderAsync (luon xoa destinationPath khi huy)
                // NHUNG can them dieu kien nay vi ExtractZipAsync (khac CopyFolderAsync)
                // CHO PHEP destPath da ton tai san (rong) tu truoc khi bat dau.
                if (!destPathExistedBefore)
                {
                    try
                    {
                        if (Directory.Exists(normalizedDestPath))
                            Directory.Delete(normalizedDestPath, recursive: true);
                    }
                    catch (UnauthorizedAccessException) { /* Khong xoa duoc thu muc rac - bo qua. */ }
                    catch (IOException) { /* Tuong tu. */ }
                }

                return new CompressionOperationResult(OperationResult.Cancelled);
            }
            catch (UnauthorizedAccessException)
            {
                return new CompressionOperationResult(OperationResult.AccessDenied);
            }
            catch (InvalidDataException)
            {
                return new CompressionOperationResult(OperationResult.Failed);
            }
            catch (IOException ex) when (FileHelper.IsSharingViolation(ex))
            {
                return new CompressionOperationResult(OperationResult.FileInUse);
            }
            catch (IOException)
            {
                return new CompressionOperationResult(OperationResult.Failed);
            }
        }
    }
}
