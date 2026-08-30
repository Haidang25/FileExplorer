using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileExplorerApp.Helpers
{
    /// <summary>
    /// Ham tien ich tinh hash noi dung file (MD5) - dung cho
    /// DuplicateService.FindDuplicateFiles (so sanh noi dung cac file cung kich
    /// thuoc de xac dinh trung lap thuc su) va bat cu noi nao khac can so
    /// sanh nhanh noi dung 2 file.
    /// </summary>
    /// <remarks>
    /// QUAN TRONG: doc file THEO STREAM (tung khoi nho, mac dinh 80 KB/lan qua
    /// bien BufferSize), KHONG doc toan bo file vao mot mang byte[] roi moi
    /// hash (VD: File.ReadAllBytes(path) roi MD5.ComputeHash(bytes)) - cach do
    /// se cap phat mot vung nho BANG DUNG KICH THUOC FILE, gay
    /// OutOfMemoryException hoac lam ung dung giat/cham han voi file dung
    /// luong lon (VD: file video/ISO hang GB) du RAM may co the du. Bang cach
    /// doc tung khoi nho va nap dan vao thuat toan hash (HashAlgorithm.
    /// TransformBlock), bo nho tieu thu LUON CO DINH bang BufferSize bat ke
    /// file lon bao nhieu.
    ///
    /// Chon MD5 (khong phai SHA256) vi muc dich CHI la SO SANH NHANH xem 2
    /// file co giong noi dung hay khong (FindDuplicateFiles), khong phai bao
    /// mat/chu ky so - MD5 tinh nhanh hon SHA256 dang ke voi file lon, va do
    /// va cham (collision) gia tao ngau nhien cua MD5 (kem an toan cho muc
    /// dich mat ma hoc) khong dang ngai o day vi FindDuplicateFiles da loc
    /// truoc theo Size (2 file kich thuoc khac nhau chac chan khac hash),
    /// nen chi con so sanh giua cac file CUNG kich thuoc - xac suat va cham MD5
    /// tinh co giua 2 file ngau nhien cung kich thuoc la vo cung nho trong
    /// thuc te su dung cua mot ung dung quan ly file ca nhan.
    /// </remarks>
    public static class HashHelper
    {
        /// <summary>
        /// Kich thuoc moi khoi doc tu file (byte) - 80 KB (81920), gia tri
        /// thuong duoc khuyen dung cho cac vong lap doc/ghi Stream trong .NET
        /// (du lon de giam so lan goi Read/giam overhead syscall, du nho de
        /// khong anh huong dang ke den bo nho ke ca khi co nhieu file duoc
        /// hash gan nhu dong thoi).
        /// </summary>
        private const int BufferSize = 81920;

        /// <summary>
        /// Tinh hash MD5 cua noi dung mot file, tra ve dang chuoi hex chu
        /// thuong (VD: "d41d8cd98f00b204e9800998ecf8427e") - dinh dang pho
        /// bien nhat de so sanh/hien thi, de doi chieu bang mat neu can (VD:
        /// so voi ket qua lenh certutil -hashfile ngoai dong lenh).
        /// </summary>
        /// <param name="filePath">Duong dan file can tinh hash.</param>
        /// <param name="cancellationToken">
        /// Cho phep huy giua chung khi dang hash file rat lon (VD: nguoi dung
        /// dong SearchForm/huy tim file trung lap dang chay) - duoc kiem tra
        /// GIUA MOI KHOI doc (khong chi truoc khi bat dau), nen viec huy co
        /// hieu luc gan nhu ngay lap tuc ke ca voi file dung luong lon, khong
        /// phai doi doc xong toan bo file moi duoc huy.
        /// </param>
        /// <exception cref="FileNotFoundException">File khong ton tai.</exception>
        /// <exception cref="IOException">Loi doc file (VD: dang bi khoa boi chuong trinh khac).</exception>
        /// <exception cref="UnauthorizedAccessException">Khong du quyen doc file.</exception>
        /// <exception cref="OperationCanceledException">cancellationToken duoc huy giua luc dang hash.</exception>
        public static string ComputeMd5(string filePath, CancellationToken cancellationToken = default)
        {
            // FileShare.Read (khong phai FileShare.None) - cho phep cac chuong
            // trinh/thao tac KHAC doc file nay cung luc (VD: nguoi dung mo file
            // xem trong luc FindDuplicateFiles dang chay), chi can dam bao
            // KHONG CO AI GHI trong luc dang hash (tranh hash mot noi dung "nua
            // cu nua moi" khong nhat quan).
            using (var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.SequentialScan)) // Doc TUAN TU tu dau den cuoi (dung voi cach hash hoat dong) - goi y he dieu hanh toi uu cache/prefetch phu hop, khac voi doc ngau nhien (RandomAccess).
            {
                return ComputeMd5(stream, cancellationToken);
            }
        }

        /// <summary>
        /// Tinh hash MD5 tu mot Stream bat ky (khong chi FileStream) - tach
        /// rieng overload nay de co the tai su dung voi cac nguon du lieu
        /// khac ngoai file thuc te tren dia neu can sau nay (VD: MemoryStream
        /// trong unit test, hoac mot Stream doc tu nguon khac).
        /// </summary>
        /// <param name="stream">Stream can tinh hash - doc tu vi tri hien tai (Position) cho den het.</param>
        /// <param name="cancellationToken">Xem ComputeMd5(string, CancellationToken).</param>
        public static string ComputeMd5(Stream stream, CancellationToken cancellationToken)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (MD5 md5 = MD5.Create())
            {
                byte[] buffer = new byte[BufferSize];
                int bytesRead;

                // Doc TUNG KHOI (toi da BufferSize byte/lan) va nap dan vao MD5
                // qua TransformBlock - day la diem MAU CHOT lam cho ham nay
                // "theo stream" thuc su: tai moi thoi diem, bo nho chi giu
                // DUNG MOT khoi BufferSize (80 KB), khong bao gio giu toan bo
                // noi dung file trong RAM cung luc, du file dung luong bao
                // nhieu GB.
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                }

                // TransformFinalBlock BAT BUOC phai duoc goi (voi mang rong o
                // day, vi da xu ly het du lieu qua TransformBlock o tren) de
                // MD5 hoan tat tinh toan noi bo va cho phep doc thuoc tinh
                // Hash - thieu buoc nay se nem loi khi truy cap md5.Hash.
                md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                return ToHexString(md5.Hash);
            }
        }

        /// <summary>
        /// Ban khong dong bo (async) cua ComputeMd5(string, CancellationToken) -
        /// dung ReadAsync thay vi Read de KHONG chan (block) luong dang goi
        /// (VD: luong UI, neu FindDuplicateFiles duoc goi tu mot Task.Run rieng
        /// nhung ban than ham hash lai muon tan dung I/O bat dong bo them mot
        /// tang nua) trong luc cho he dieu hanh doc du lieu tu dia - dac biet
        /// huu ich voi o dia mang/o cham, noi thoi gian cho I/O co the dang ke.
        /// </summary>
        /// <param name="filePath">Duong dan file can tinh hash.</param>
        /// <param name="cancellationToken">Xem ComputeMd5(string, CancellationToken).</param>
        public static async Task<string> ComputeMd5Async(string filePath, CancellationToken cancellationToken = default)
        {
            using (var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                BufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan))
            {
                using (MD5 md5 = MD5.Create())
                {
                    byte[] buffer = new byte[BufferSize];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        md5.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    }

                    md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                    return ToHexString(md5.Hash);
                }
            }
        }

        /// <summary>
        /// Chuyen mang byte hash thanh chuoi hex chu thuong (VD: {0x0F, 0xA1}
        /// -> "0fa1") - dung StringBuilder + "x2" (2 chu so hex, co dem 0 o
        /// dau neu can) thay vi BitConverter.ToString(...).Replace("-", "")
        /// de tranh cap phat chuoi trung gian co dau gach ngang khong can
        /// thiet truoc khi Replace.
        /// </summary>
        private static string ToHexString(byte[] hash)
        {
            var builder = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
