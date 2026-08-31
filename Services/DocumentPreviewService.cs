using System;
using System.IO;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Doc va trich xuat NOI DUNG VAN BAN THUAN (plain text) tu cac file van
    /// phong de PHUC VU XEM TRUOC (preview) - hien tai chi ho tro Word
    /// (.docx) qua <see cref="ExtractWordText"/>. Cac dinh dang khac (VD
    /// .pdf qua goi PdfPig da co san trong packages.config, .pptx) se bo
    /// sung sau neu can, moi dinh dang mot ham ExtractXxxText rieng - KHONG
    /// co gang dung MOT ham chung cho tat ca dinh dang, vi cau truc/thu vien
    /// doc cua tung dinh dang qua khac nhau (OpenXml SDK cho .docx/.pptx,
    /// PdfPig cho .pdf...) de dung chung logic ma khong lam kho hieu.
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE - DUNG DocumentFormat.OpenXml SDK (goi NuGet chinh
    /// thuc cua Microsoft, da duoc CAI SAN trong project - xem packages.config)
    /// THAY VI tu doc file .docx nhu mot file .zip roi tu parse XML thu cong:
    /// ban dau ham nay duoc viet theo huong tu doc (chi dung System.IO.Compression
    /// + System.Xml.Linq da co san trong .NET Framework, khong can them goi
    /// NuGet nao) - VE MAT KY THUAT van chay dung, nhung sau khi phat hien
    /// project NAY DA CO SAN goi DocumentFormat.OpenXml (va DocumentFormat.OpenXml.Framework)
    /// trong packages.config/thu muc packages\ (chuan bi cho tinh nang xem
    /// truoc noi dung tep dang duoc xay dung song song trong MainForm - xem
    /// spcFilesPreview/pnlPreview/txtPreview trong MainForm.Designer.cs), nen
    /// chuyen sang dung THANG SDK nay de:
    /// - Tranh 2 CACH KHAC NHAU cung lam MOT VIEC (doc .docx) ton tai song
    ///   song trong project - neu sau nay can doc THEM thong tin khac cua
    ///   tai lieu (VD thuoc tinh tai lieu, bang, hinh anh...), SDK chinh thuc
    ///   co san day du API cho viec do, con cach tu parse XML se phai tu viet
    ///   them rat nhieu cho tung truong hop.
    /// - SDK xu ly dung cac truong hop bien (edge case) cua chuan Office Open
    ///   XML ma tu viet co the bo sot (VD w:fldSimple/w:fldChar cho noi dung
    ///   dong (field) nhu so trang tu dong, cac phan tu it gap khac).
    ///
    /// LOP .NET FRAMEWORK CUA 2 GOI OpenXml/OpenXml.Framework (thu muc "net46"
    /// trong lib\ - xem ghi chu tai FileExplorerApp.csproj) tu dong dung
    /// WindowsBase.dll (co san trong .NET Framework, KHONG phai mot goi
    /// NuGet rieng tren Framework) de xu ly phan "goi zip OPC" ben duoi -
    /// khong can bat ky ma nao trong lop nay tu dong cham vao System.IO.Packaging,
    /// SDK da lo lieu het qua WordprocessingDocument.Open.
    /// </remarks>
    public class DocumentPreviewService
    {
        /// <summary>
        /// Trich xuat toan bo noi dung VAN BAN THUAN tu mot file Word (.docx),
        /// tra ve dang MOT CHUOI, cac DOAN VAN (paragraph) duoc noi voi nhau
        /// bang Environment.NewLine - dung cho muc dich XEM TRUOC noi dung
        /// tai lieu ma KHONG CAN mo Word.
        /// </summary>
        /// <remarks>
        /// CACH DOC: WordprocessingDocument.Open(filePath, isEditable: false)
        /// mo tai lieu CHI DE DOC (khong sua doi, tranh khoa file/vo tinh
        /// thay doi noi dung goc chi vi dang xem truoc) - MainDocumentPart.Document.Body
        /// la goc cua THAN CHINH tai lieu (KHAC voi header/footer/footnote...
        /// nam o cac phan (part) rieng khac trong .docx, CHUA duoc doc o day,
        /// xem <remarks> dau lop). Voi MOI doan van (Paragraph), gop noi dung
        /// TAT CA phan tu con lien quan theo THU TU XUAT HIEN (Descendants()
        /// tra ve dung thu tu tai lieu - document order):
        /// - Text (w:t): noi dung van ban thuc su, LAY NGUYEN VAN (.Text).
        /// - TabChar (w:tab): mot ky tu tab (Word luu tab nhu MOT PHAN TU
        ///   RIENG, khong phai ky tu '\t' ben trong Text) - chuyen thanh '\t'
        ///   de giu dung y dinh trinh bay cua nguoi soan.
        /// - Break/CarriageReturn (w:br/w:cr - ngat dong THU CONG trong CUNG
        ///   mot doan van, VD Shift+Enter - khac voi ngat DOAN VAN thong
        ///   thuong giua 2 Paragraph) - chuyen thanh Environment.NewLine de
        ///   giu dung cho xuong dong nguoi soan chu dich dat.
        /// Cac phan tu khac (VD RunProperties chua thong tin dinh dang font/
        /// mau - KHONG phai text) bi BO QUA hoan toan - dung dinh huong "chi
        /// lay van ban thuan" da neu o remarks dau lop.
        /// </remarks>
        /// <param name="filePath">Duong dan file .docx can doc.</param>
        /// <returns>
        /// Toan bo van ban, cac doan van cach nhau boi Environment.NewLine.
        /// Tra ve chuoi RONG (khong phai null) neu tai lieu hop le nhung
        /// KHONG co noi dung (VD file .docx moi tao, chua go gi).
        /// </returns>
        /// <exception cref="ArgumentException">filePath rong hoac chi chua khoang trang.</exception>
        /// <exception cref="FileNotFoundException">File khong ton tai tai duong dan chi dinh.</exception>
        /// <exception cref="OpenXmlPackageException">
        /// File ton tai nhung KHONG phai file .docx hop le (khong dung cau
        /// truc Office Open XML/goi OPC, VD nguoi dung doi ten mot file .txt
        /// thanh .docx, hoac file bi hong) - SDK OpenXml tu kiem tra va nem
        /// loi nay (hoac loi con cua no) khi cau truc goi khong hop le.
        /// </exception>
        /// <exception cref="IOException">Loi doc file (VD dang bi khoa boi chuong trinh khac, VD dang mo trong Word).</exception>
        /// <exception cref="UnauthorizedAccessException">Khong du quyen doc file.</exception>
        public string ExtractWordText(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Đường dẫn file không được rỗng.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Không tìm thấy file cần đọc.", filePath);

            using (WordprocessingDocument document = WordprocessingDocument.Open(filePath, isEditable: false))
            {
                Body body = document.MainDocumentPart?.Document?.Body;
                if (body == null)
                    return string.Empty; // Tai lieu hop le nhung khong co than chinh - coi nhu rong, khong nem loi.

                var resultBuilder = new StringBuilder();
                bool isFirstParagraph = true;

                foreach (Paragraph paragraph in body.Elements<Paragraph>())
                {
                    if (!isFirstParagraph)
                    {
                        resultBuilder.Append(Environment.NewLine);
                    }
                    isFirstParagraph = false;

                    AppendParagraphText(paragraph, resultBuilder);
                }

                return resultBuilder.ToString();
            }
        }

        /// <summary>
        /// Gop noi dung van ban cua MOT doan van vao builder, theo dung thu
        /// tu xuat hien trong tai lieu - xem "CACH DOC" o remarks tai
        /// ExtractWordText de biet chi tiet vi sao xu ly rieng Text/TabChar/
        /// Break/CarriageReturn.
        /// </summary>
        private static void AppendParagraphText(Paragraph paragraph, StringBuilder resultBuilder)
        {
            foreach (OpenXmlElement element in paragraph.Descendants())
            {
                if (element is Text textElement)
                {
                    resultBuilder.Append(textElement.Text);
                }
                else if (element is TabChar)
                {
                    resultBuilder.Append('\t');
                }
                else if (element is Break || element is CarriageReturn)
                {
                    resultBuilder.Append(Environment.NewLine);
                }
            }
        }
    }
}
