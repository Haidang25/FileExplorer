namespace FileExplorerApp.Models
{
    /// <summary>
    /// Ket qua cua CompressionService.CompressFolderAsync/ExtractZipAsync - mo
    /// rong OperationResult don thuan bang kich thuoc (byte) TRUOC/SAU thao tac,
    /// dung de ghi log chi tiet hon (xem MainForm.cmsCompressToZip_Click/
    /// cmsExtractHere_Click goi LogOperationResult voi extraNote mo ta 2 kich
    /// thuoc nay).
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE: tra ve mot class rieng (khong dung ValueTuple/out
    /// parameter) - giong quy uoc cac model nho khac trong ung dung
    /// (FileOperationProgress, LogEntryModel...), de ten tung truong tu giai
    /// thich duoc y nghia (SizeBeforeBytes/SizeAfterBytes) thay vi Item1/Item2
    /// vo nghia cua tuple. Khong the dung out/ref parameter vi CompressFolderAsync/
    /// ExtractZipAsync la async Task&lt;T&gt; - C# khong cho phep out/ref tren
    /// tham so cua phuong thuc async.
    ///
    /// SizeBeforeBytes/SizeAfterBytes CHI duoc tinh chinh xac (khac 0) khi Result
    /// == OperationResult.Success - voi cac ket qua khac (Failed/Cancelled/
    /// Skipped/...), thao tac chua hoan tat nen "kich thuoc sau" (va doi khi ca
    /// "kich thuoc truoc" trong truong hop that bai ngay tu buoc validate, VD
    /// NotFound) khong co y nghia ro rang - giu ca 2 = 0, giong cach
    /// BuildOperationResultMessage cua MainForm cung khong can kich thuoc de mo
    /// ta cac ket qua that bai.
    /// </remarks>
    public class CompressionOperationResult
    {
        /// <summary>Ket qua thuc hien (Success/Failed/Cancelled/...) - xem OperationResult.</summary>
        public OperationResult Result { get; set; }

        /// <summary>
        /// Kich thuoc (byte) TRUOC thao tac: tong dung luong TAT CA file trong thu
        /// muc nguon (voi CompressFolderAsync) hoac dung luong file .zip nguon
        /// (voi ExtractZipAsync). Chi chinh xac khi Result == Success.
        /// </summary>
        public long SizeBeforeBytes { get; set; }

        /// <summary>
        /// Kich thuoc (byte) SAU thao tac: dung luong file .zip ket qua (voi
        /// CompressFolderAsync) hoac tong dung luong da giai nen ra thu muc dich
        /// (voi ExtractZipAsync). Chi chinh xac khi Result == Success.
        /// </summary>
        public long SizeAfterBytes { get; set; }

        public CompressionOperationResult(OperationResult result, long sizeBeforeBytes = 0, long sizeAfterBytes = 0)
        {
            Result = result;
            SizeBeforeBytes = sizeBeforeBytes;
            SizeAfterBytes = sizeAfterBytes;
        }
    }
}
