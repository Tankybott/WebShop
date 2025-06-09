using DinkToPdf;
using DinkToPdf.Contracts;
using Utility.Common.Interfaces;

namespace Utility.Pdf
{
    public class PdfFileGenerator : IPdfFileGenerator
    {
        private readonly IConverter _converter;
        private readonly IFileService _fileService;
        private readonly IFileNameCreator _fileNameCreator;
        private readonly IPathCreator _pathCreator;

        public PdfFileGenerator(
            IConverter converter,
            IFileService fileService,
            IFileNameCreator fileNameCreator,
            IPathCreator pathCreator)
        {
            _converter = converter;
            _fileService = fileService;
            _fileNameCreator = fileNameCreator;
            _pathCreator = pathCreator;
        }

        public async Task<string> GeneratePdfFromHtmlAsync(string html)
        {
            var fileName = _fileNameCreator.CreateFileName("pdf");
            var tempDir = _pathCreator.CombinePaths(_pathCreator.GetRootPath(), "temp");
            var outputPath = _pathCreator.CombinePaths(tempDir, fileName);

            if (!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            var doc = new HtmlToPdfDocument
            {
                GlobalSettings = new GlobalSettings
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            byte[] pdfBytes = _converter.Convert(doc);

            if (pdfBytes == null || pdfBytes.Length == 0)
            {
                throw new Exception("PDF generation failed: byte array is empty.");
            }

            await using var memoryStream = new MemoryStream(pdfBytes);
            await _fileService.CreateFileAsync(memoryStream, outputPath);

            return outputPath;
        }
    }
}
