using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
// REFACTOR - dong goi thu vien: them using nay VAO DAY (truoc kia nam ben
// MainForm) - DocumentPreviewService gio la noi DUY NHAT trong toan ung
// dung con "biet" ve UglyToad.PdfPig.Exceptions/DocumentFormat.OpenXml.Packaging.OpenXmlPackageException,
// xem DocumentPasswordProtectedException.cs de biet chi tiet.
using UglyToad.PdfPig.Exceptions;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Doc va trich xuat NOI DUNG VAN BAN THUAN (plain text) tu cac file van
    /// phong de PHUC VU XEM TRUOC (preview) - ho tro Word (.docx) qua
    /// <see cref="ExtractWordText"/> va PDF qua <see cref="ExtractPdfText"/>.
    /// Dinh dang khac (VD .pptx) se bo sung sau neu can, moi dinh dang mot
    /// ham ExtractXxxText rieng - KHONG co gang dung MOT ham chung cho tat
    /// ca dinh dang, vi cau truc/thu vien doc cua tung dinh dang qua khac
    /// nhau (OpenXml SDK cho .docx/.pptx, PdfPig cho .pdf...) de dung chung
    /// logic ma khong lam kho hieu.
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
        /// <exception cref="DocumentPasswordProtectedException">
        /// File .docx duoc bao ve bang mat khau - xem <see cref="DocumentPasswordProtectedException"/>
        /// de biet ly do co loai ngoai le rieng nay (thay vi de OpenXmlPackageException
        /// cua SDK lot ra ngoai truc tiep).
        /// </exception>
        /// <remarks>
        /// SUA LOI (phat hien khi chuan bi test "preview tep Word co bang bieu"):
        /// PHIEN BAN TRUOC chi doc body.Elements&lt;Paragraph&gt;() - CHI lay
        /// cac Paragraph la CON TRUC TIEP cua w:body. Trong file .docx, mot
        /// BANG (w:tbl) cung la CON TRUC TIEP cua body, nhung cac doan van
        /// BEN TRONG bang lai nam sau 2 cap long nhau (w:tbl > w:tr > w:tc >
        /// w:p) - KHONG phai con truc tiep cua body - nen bi BO SOT HOAN TOAN,
        /// nghia la mot tai lieu co bang se bi mat trang toan bo noi dung
        /// trong bang khi xem truoc. SUA bang cach di qua TUNG PHAN TU CON
        /// TRUC TIEP cua body theo dung THU TU xuat hien (body.Elements() -
        /// khong ep kieu Paragraph ngay) - phan tu nao la Paragraph thi doc
        /// nhu cu, phan tu nao la Table thi goi them AppendTableText de doc
        /// rieng cau truc bang (xem ham do). Cac loai phan tu khac cua body
        /// (VD SectionProperties - thong tin trang/le, khong phai noi dung)
        /// van duoc BO QUA nhu truoc.
        /// </remarks>
        public string ExtractWordText(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Đường dẫn file không được rỗng.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Không tìm thấy file cần đọc.", filePath);

            try
            {
                using (WordprocessingDocument document = WordprocessingDocument.Open(filePath, isEditable: false))
                {
                    Body body = document.MainDocumentPart?.Document?.Body;
                    if (body == null)
                        return string.Empty; // Tai lieu hop le nhung khong co than chinh - coi nhu rong, khong nem loi.

                    var resultBuilder = new StringBuilder();
                    bool isFirstBlock = true;

                    // Di qua TUNG PHAN TU CON TRUC TIEP cua body theo dung thu tu
                    // xuat hien trong tai lieu - KHONG chi loc rieng Paragraph nhu
                    // truoc, de khong bo sot Table (xem <remarks> cua ham nay).
                    foreach (OpenXmlElement blockElement in body.Elements())
                    {
                        if (blockElement is Paragraph paragraph)
                        {
                            if (!isFirstBlock)
                            {
                                resultBuilder.Append(Environment.NewLine);
                            }
                            isFirstBlock = false;

                            AppendParagraphText(paragraph, resultBuilder);
                        }
                        else if (blockElement is Table table)
                        {
                            if (!isFirstBlock)
                            {
                                resultBuilder.Append(Environment.NewLine);
                            }
                            isFirstBlock = false;

                            AppendTableText(table, resultBuilder);
                        }
                        // Cac loai phan tu khac cua body (VD SectionProperties -
                        // thong tin trang/le, khong phai noi dung) - BO QUA.
                    }

                    return resultBuilder.ToString();
                }
            }
            catch (OpenXmlPackageException ex) when (ex.Message.IndexOf("Encrypt", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                // REFACTOR - dong goi thu vien: bat rieng truong hop mat khau
                // NGAY TAI DAY (noi DUY NHAT trong ung dung con "biet" ve
                // OpenXmlPackageException, xem <see cref="DocumentPasswordProtectedException"/>)
                // va boc lai thanh mot loai ngoai le CHUNG, KHONG thuoc ve bat
                // ky thu vien cu the nao - de Form goi ham nay (MainForm) KHONG
                // CAN "using DocumentFormat.OpenXml.Packaging" chi de bat truong
                // hop nay nua (truoc day PHAI lam vay, vi pham dong goi).
                throw new DocumentPasswordProtectedException(
                    "Tệp Word này được bảo vệ bằng mật khẩu, không thể xem trước nội dung.", ex);
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

        /// <summary>
        /// Gop noi dung van ban cua MOT bang (Table) vao builder - moi HANG
        /// (TableRow) tren MOT dong, cac O (TableCell) trong cung mot hang
        /// duoc noi voi nhau bang ky tu tab ('\t') de giu duoc cam giac
        /// "cot" khi xem truoc dang van ban thuan (khong the ve khung bang
        /// thuc su trong mot o TextBox thuan van ban).
        /// </summary>
        /// <remarks>
        /// MOI O co the chua NHIEU doan van (Paragraph) - noi cac doan van
        /// TRONG CUNG MOT O bang mot khoang trang (khong dung newline, vi
        /// newline se lam lech hang/cot khi xem duoi dang bang van ban tho).
        /// GIOI HAN DA BIET (chap nhan duoc cho muc dich XEM TRUOC): mot O
        /// bang co the chua BANG LONG BEN TRONG (nested table) - truong hop
        /// nay HIEM GAP trong thuc te va KHONG duoc doc de tranh de quy phuc
        /// tap khong can thiet cho tinh nang xem truoc; noi dung o do se bi
        /// bo qua (chi lay Paragraph truc tiep cua o, khong lay Table long
        /// trong o).
        /// </remarks>
        private static void AppendTableText(Table table, StringBuilder resultBuilder)
        {
            bool isFirstRow = true;

            foreach (TableRow row in table.Elements<TableRow>())
            {
                if (!isFirstRow)
                {
                    resultBuilder.Append(Environment.NewLine);
                }
                isFirstRow = false;

                bool isFirstCell = true;
                foreach (TableCell cell in row.Elements<TableCell>())
                {
                    if (!isFirstCell)
                    {
                        resultBuilder.Append('\t');
                    }
                    isFirstCell = false;

                    bool isFirstParagraphInCell = true;
                    foreach (Paragraph cellParagraph in cell.Elements<Paragraph>())
                    {
                        if (!isFirstParagraphInCell)
                        {
                            resultBuilder.Append(' ');
                        }
                        isFirstParagraphInCell = false;

                        AppendParagraphText(cellParagraph, resultBuilder);
                    }
                }
            }
        }

        /// <summary>
        /// Trich xuat noi dung VAN BAN THUAN tu mot file PDF, tra ve THEO
        /// TUNG TRANG (KHONG gop thanh mot chuoi duy nhat nhu ExtractWordText) -
        /// dung yeu cau "doc noi dung van ban theo tung trang PDF". Phan tu
        /// tai chi so i (0-based) trong danh sach tra ve la noi dung van ban
        /// cua TRANG SO (i + 1) trong file PDF (PDF danh so trang tu 1, xem
        /// <returns> ben duoi).
        /// </summary>
        /// <remarks>
        /// QUYET DINH THIET KE - DUNG PdfPig (goi NuGet da co san trong
        /// packages.config, KHONG phai .NET Framework tu co san nhu
        /// System.IO.Compression/System.Xml.Linq da dung cho .docx): dinh
        /// dang PDF (khac han Office Open XML cua .docx - KHONG phai zip/XML)
        /// can mot bo PARSER PDF THUC SU (doc cau truc object/cross-reference
        /// table/content stream, giai ma font de biet MA BYTE nao ung voi KY
        /// TU nao...) - tu viet lai tu dau la KHONG THUC TE, nen dung PdfPig
        /// (thu vien PDF thuan .NET pho bien, KHONG can phu thuoc native/
        /// COM nhu mot so thu vien PDF khac, phu hop de chay ben trong ung
        /// dung WinForms nay).
        ///
        /// Moi Page (UglyToad.PdfPig.Content.Page) da co san thuoc tinh .Text
        /// - CHINH LA toan bo van ban thuan cua trang do, PdfPig da tu lam
        /// het viec ghep cac ky tu/tu/dong theo dung vi tri hinh hoc tren
        /// trang (PDF khong luu "doan van" nhu .docx, chi luu VI TRI VE TUNG
        /// KY TU/CHUOI ky tu tren trang - PdfPig tu suy luan thu tu doc hop
        /// ly tu cac vi tri do) - vi vay KHONG can tu ghep noi dung tu cac
        /// phan tu con nho hon (Letter/Word) nhu cach ExtractWordText phai tu
        /// lam voi Text/TabChar/Break cua .docx, dung truc tiep .Text la du
        /// cho muc dich xem truoc.
        ///
        /// SAP XEP LAI THEO Page.Number (OrderBy) - PHONG XA du document.GetPages()
        /// tren ly thuyet da tra ve dung thu tu trang, sap xep lai RO RANG
        /// THEO SO TRANG (khong dua vao thu tu enumerate ngam dinh) giup ket
        /// qua LUON dung voi thu tu trang thuc te, ke ca neu mot phien ban
        /// PdfPig sau nay thay doi thu tu enumerate ben trong.
        /// </remarks>
        /// <param name="filePath">Duong dan file .pdf can doc.</param>
        /// <returns>
        /// Danh sach van ban theo trang, THEO DUNG THU TU trang trong file
        /// (phan tu 0 = trang 1, phan tu 1 = trang 2, v.v.) - danh sach RONG
        /// (khong phai null) neu PDF hop le nhung khong co trang nao (truong
        /// hop rat hiem). Mot trang KHONG co van ban (VD trang toan hinh anh
        /// quet - scan, chua qua OCR) se co phan tu tuong ung la CHUOI RONG,
        /// KHONG bi bo qua khoi danh sach - giu dung so luong phan tu BANG SO
        /// TRANG THUC TE de chi so (index) luon khop voi so trang.
        /// </returns>
        /// <exception cref="ArgumentException">filePath rong hoac chi chua khoang trang.</exception>
        /// <exception cref="FileNotFoundException">File khong ton tai tai duong dan chi dinh.</exception>
        /// <exception cref="UglyToad.PdfPig.Exceptions.PdfDocumentFormatException">
        /// File ton tai nhung KHONG phai file PDF hop le/dung cau truc (VD
        /// nguoi dung doi ten mot file khac thanh .pdf, hoac file bi hong) -
        /// PdfPig tu kiem tra va nem loi nay khi cau truc PDF khong hop le.
        /// </exception>
        /// <exception cref="IOException">Loi doc file (VD dang bi khoa boi chuong trinh khac).</exception>
        /// <exception cref="UnauthorizedAccessException">Khong du quyen doc file.</exception>
        /// <exception cref="DocumentPasswordProtectedException">
        /// File .pdf duoc bao ve bang mat khau - xem <see cref="DocumentPasswordProtectedException"/>
        /// de biet ly do co loai ngoai le rieng nay (thay vi de
        /// UglyToad.PdfPig.Exceptions.PdfDocumentEncryptedException cua PdfPig
        /// lot ra ngoai truc tiep).
        /// </exception>
        public List<string> ExtractPdfText(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Đường dẫn file không được rỗng.", nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Không tìm thấy file cần đọc.", filePath);

            try
            {
                using (PdfDocument document = PdfDocument.Open(filePath))
                {
                    return document.GetPages()
                        .OrderBy(page => page.Number)
                        .Select(page => page.Text ?? string.Empty)
                        .ToList();
                }
            }
            catch (PdfDocumentEncryptedException ex)
            {
                // REFACTOR - dong goi thu vien: xem ghi chu tuong tu tai
                // ExtractWordText/catch (OpenXmlPackageException...) o tren -
                // cung ly do, ap dung cho PdfPig thay OpenXml SDK.
                throw new DocumentPasswordProtectedException(
                    "Tệp PDF này được bảo vệ bằng mật khẩu, không thể xem trước nội dung.", ex);
            }
        }
    }
}
