using System;

namespace FileExplorerApp.Services
{
    /// <summary>
    /// Nem boi <see cref="DocumentPreviewService"/> khi mot file .docx/.pdf
    /// duoc bao ve bang mat khau (khong doc duoc noi dung de xem truoc, KHAC
    /// voi truong hop file hong/sai dinh dang thong thuong).
    /// </summary>
    /// <remarks>
    /// QUYET DINH THIET KE - VI SAO CO LOP NAY: <see cref="DocumentPreviewService"/>
    /// dung DocumentFormat.OpenXml SDK (.docx) va PdfPig (.pdf) o BEN TRONG,
    /// moi thu vien co MOT CACH RIENG de bao "file bi khoa mat khau" (PdfPig
    /// nem UglyToad.PdfPig.Exceptions.PdfDocumentEncryptedException - mot
    /// loai ngoai le RIENG; OpenXml SDK lai nem DocumentFormat.OpenXml.Packaging.OpenXmlPackageException
    /// VOI THONG DIEP chua "Encrypt" - KHONG co loai ngoai le rieng nhu PdfPig).
    /// TRUOC KHI CO LOP NAY, MainForm.UpdateDocumentPreview phai "using
    /// DocumentFormat.OpenXml.Packaging"/"using UglyToad.PdfPig.Exceptions"
    /// CHI DE bat dung 2 loai ngoai le do - nghia la MOT Form (thuoc lop UI)
    /// lai PHAI BIET ve chi tiet THU VIEN BEN TRONG cua mot Service, VI PHAM
    /// nguyen tac Form CHI duoc goi qua DocumentPreviewService (dong goi -
    /// encapsulation), va neu sau nay doi thu vien PDF/Word khac, MOI Form
    /// dang preview deu phai sua theo, khong chi rieng DocumentPreviewService.
    ///
    /// SUA bang cach: DocumentPreviewService.ExtractWordText/ExtractPdfText
    /// tu BAT rieng 2 loai ngoai le cu the do o BEN TRONG (noi DUY NHAT trong
    /// ca ung dung con "biet" ve OpenXmlPackageException/PdfDocumentEncryptedException),
    /// roi BOC LAI (rethrow) thanh DUY NHAT MOT loai ngoai le CHUNG, TU DINH
    /// NGHIA nay - Form (MainForm) gio CHI can bat DocumentPasswordProtectedException,
    /// KHONG can (va KHONG DUOC) "using" bat ky namespace nao cua OpenXml/PdfPig
    /// nua. Neu sau nay doi thu vien doc PDF/Word khac (VD PdfPig sang
    /// thu vien khac), CHI can sua lai DocumentPreviewService, MainForm
    /// HOAN TOAN KHONG can dong nao.
    ///
    /// Message cua exception nay duoc DocumentPreviewService soan SAN, mo ta
    /// RO dang la Word hay PDF (2 thong diep khac nhau) - Form CHI can hien
    /// nguyen ex.Message cho nguoi dung, KHONG can tu phan biet dinh dang.
    /// </remarks>
    public class DocumentPasswordProtectedException : Exception
    {
        public DocumentPasswordProtectedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
