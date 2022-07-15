using Ghostscript.NET;
using ImageMagick;

namespace PoiskIT.Andromeda.Utils
{
    public static class PdfConverter
    {
        // 200 dpi is a healthy default resolution
        private const int PDF_RESOLUTION_DPI = 300;
        private const string OUTPUT_EXTENSION = "jpg";
        private const string OUTPUT_EXTENSION_WDOT = "." + OUTPUT_EXTENSION;
        private const string GHOSTSCRIPT_OUTPUT_PATTERN = ".conv_%d" + OUTPUT_EXTENSION_WDOT;

        public static bool FullyInstalled()
        {
            return GhostscriptVersionInfo.IsGhostscriptInstalled;
        }

        /// <summary>
        /// Converts given PDF file to individual JPG files (1 per each page), 
        /// and puts them all in a subfolder in the same directory with the source.
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFolder"></param>
        /// <returns></returns>
        public static List<byte[]> Convert(string inputFilePath, string? outputFolder = null)
        {
            //var jpegDevice = new GhostscriptJpegDevice(GhostscriptJpegDeviceType.Jpeg)
            //{
            //    GraphicsAlphaBits = GhostscriptImageDeviceAlphaBits.V_4,
            //    TextAlphaBits = GhostscriptImageDeviceAlphaBits.V_4,
            //    ResolutionXY = new GhostscriptImageDeviceResolution(PDF_RESOLUTION_DPI, PDF_RESOLUTION_DPI),
            //    JpegQuality = jpegQuality
            //};
            //jpegDevice.InputFiles.Add(inputFilePath);
            //var fileName = Path.GetFileNameWithoutExtension(inputFilePath);

            //jpegDevice.OutputPath = Path.Combine(outputFolder, fileName + GHOSTSCRIPT_OUTPUT_PATTERN);

            //// GO! GhostScript silently puts all converted pages in the specified folder. There is no other feedback.
            //jpegDevice.Process();

            List<byte[]> imageBytes = new List<byte[]>();
            var settings = new MagickReadSettings();
            // Settings the density to 300 dpi will create an image with a better quality
            settings.Density = new Density(PDF_RESOLUTION_DPI, PDF_RESOLUTION_DPI);
            using (var images = new MagickImageCollection())
            {
                // Add all the pages of the pdf file to the collection
                images.Read(inputFilePath, settings);

                var page = 1;
                foreach (var image in images)
                {
                    // Write page to file that contains the page number
                    if (!String.IsNullOrEmpty(outputFolder))
                        image.Write($"{outputFolder}.Page.{page}.png");
                    imageBytes.Add(image.ToByteArray(MagickFormat.Png32));
                    page++;
                }
            }
            return imageBytes;
        }
    }
}
