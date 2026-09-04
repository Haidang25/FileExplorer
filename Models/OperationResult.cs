namespace FileExplorerApp.Models
{
    /// <summary>
    /// Ket qua thuc hien mot thao tac file/thu muc (tra ve tu lop Services).
    /// Dung chung cho toan bo ung dung khi can bao cao trang thai thao tac,
    /// bao gom ca ghi vao <see cref="LogEntryModel"/>.
    /// </summary>
    public enum OperationResult
    {
        /// <summary>Thao tac thanh cong hoan toan.</summary>
        Success,

        /// <summary>Thao tac thanh cong mot phan (VD: sao chep 8/10 file, 2 file loi).</summary>
        PartialSuccess,

        /// <summary>Thao tac that bai.</summary>
        Failed,

        /// <summary>Nguoi dung huy thao tac giua chung.</summary>
        Cancelled,

        /// <summary>Thao tac bi bo qua (VD: file trung ten va nguoi dung chon Skip).</summary>
        Skipped,

        /// <summary>Khong du quyen truy cap de thuc hien thao tac.</summary>
        AccessDenied,

        /// <summary>Khong tim thay file/thu muc de thuc hien thao tac.</summary>
        NotFound,

        /// <summary>
        /// File dang bi khoa (mo/su dung) boi mot chuong trinh khac nen khong the
        /// doi ten/di chuyen/xoa duoc luc nay (VD: dang mo trong Word, Notepad++...).
        /// Tach rieng voi Failed de bao thong bao cu the, huong dan nguoi dung dong
        /// chuong trinh dang giu file roi thu lai, thay vi bao loi chung chung.
        /// </summary>
        FileInUse,

        /// <summary>
        /// Vi tri dich khong hop le doi voi thao tac nay - cu the la khi di
        /// chuyen/sao chep mot thu muc vao CHINH NO hoac vao MOT THU MUC CON cua
        /// chinh no (VD: keo "C:\A" vao "C:\A\B"), se gay de quy vo han hoac loi he
        /// thong kho hieu neu khong chan truoc. Tach rieng voi Skipped/Failed de bao
        /// thong bao cu the, giup nguoi dung hieu dung ly do bi chan.
        /// </summary>
        InvalidDestination,

        /// <summary>
        /// File .zip bi hong (du lieu ben trong khong toan ven, VD tai xuong do
        /// dang/dia loi) hoac khong dung dinh dang Zip (VD doi ten mot file khac
        /// thanh .zip) - CompressionService.ExtractZip/ExtractZipAsync tra ve gia
        /// tri nay khi mo file .zip nem InvalidDataException. Tach rieng voi Failed
        /// de bao thong bao cu the ("tệp .zip bị hỏng/sai định dạng") thay vi loi
        /// chung chung, giup nguoi dung hieu NGAY nguyen nhan nam o CHINH file .zip
        /// nguon (VD: tai lai tu nguon khac) chu khong phai o thu muc dich/quyen
        /// truy cap.
        /// </summary>
        CorruptedArchive,

        /// <summary>
        /// Duong dan day du (thu muc/file dich, sau khi ket hop ten moi/vi tri dich)
        /// vuot qua gioi han do dai duong dan cua Windows (MAX_PATH = 260 ky tu -
        /// xem FileHelper.MaxPathLength) - cac ham he thong file cua .NET Framework
        /// (Directory.CreateDirectory, File.Move, Directory.Move...) nem
        /// PathTooLongException (mot lop con cua IOException) trong truong hop nay.
        /// Tach rieng voi Failed/AccessDenied de bao thong bao cu the ("đường dẫn
        /// quá dài") - truoc day PathTooLongException bi cac catch (IOException)
        /// chung dang co san "nuot" thanh Failed chung chung (hoac te hon, bi
        /// PermissionHelper.CanWriteByTest hieu SAI thanh AccessDenied, vi ham do tu
        /// tao mot file tam voi ten dai ben trong thu muc dich de kiem tra quyen
        /// ghi, va chinh file tam do lai la thu vuot qua 260 ky tu truoc ca khi thao
        /// tac thuc su duoc goi) - nguoi dung khong biet duoc nguyen nhan THAT SU la
        /// do do dai duong dan, khong phai do quyen truy cap.
        /// </summary>
        PathTooLong
    }
}
