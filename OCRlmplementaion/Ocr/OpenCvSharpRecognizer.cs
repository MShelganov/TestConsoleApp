using OpenCvSharp;
using PoiskIT.Andromeda.Settings;

namespace PoiskIT.Andromeda.Ocr
{
    public class OpenCvSharpRecognizer : IRecognizer
    {
        private static readonly object Lock = new object();
        private readonly OpenCvSharp.Text.OCRTesseract engine;
        private readonly Options options;
        private bool _disposed;
        public OpenCvSharpRecognizer(Options ocrOptions, bool debug = false)
        {
            if (ocrOptions == null)
                ocrOptions = Options.Default;
            options = ocrOptions;
            string langsStr = String.Join("+", options.Languages);
            engine = OpenCvSharp.Text.OCRTesseract.Create(Config.TRAINED_DATA_PATH, langsStr, null, 1, 3);

        }
    
        #region Filters
        private void Denoising(Mat src, Mat dst)
        {
            if (!options.IsDenoising)
                dst = src;
            else
                Cv2.FastNlMeansDenoising(src, dst);
        }
        private void Filter2D(Mat src, Mat dst)
        {
            if (!options.IsFilter2D)
                dst = src;
            //else
            //    Cv2.Filter2D(src, dst);
        }
        private void GaussianWeighted(Mat src, Mat dst, Mat weighted)
        {
            if (!options.IsGaussianWeighted)
                weighted = src;
            else
            {
                Cv2.GaussianBlur(src, dst, new OpenCvSharp.Size(5, 5), 0);
                Cv2.AddWeighted(src, 1.5, dst, -.5, 0, weighted);
            }
        }
        private void Bilateral(Mat src, Mat dst)
        {
            if (!options.IsBilateral)
                dst = src;
            else
                Cv2.BilateralFilter(src, dst, 15, 80, 80); // 5, 10, 2
        }
        #endregion Filters

        /// <summary>
        /// Aligning images with template and keypoint matching .
        /// </summary>
        /// <param name="image">Our input photo/scan of a form. Should be identical to the template image.</param>
        /// <param name="template">The template form image.</param>
        /// <param name="maxFeatures">Places an upper bound on the number of candidate keypoint regions to consider.</param>
        /// <param name="keepPrecent">Designates the percentage of keypoint matches to keep, effectively allowing us to eliminate noisy keypoint matching results</param>
        private void AlighImageByTemp(Mat image, Mat template, int maxFeatures = 500, float keepPrecent = .2f)
        {
            using (var orb = ORB.Create(maxFeatures))
            using (var descImage = new Mat())
            using (var descTemplate = new Mat())
            {
                KeyPoint[] keyPointsImage, keyPointsTemp;
                orb.DetectAndCompute(image, null, out keyPointsImage, descImage);
                orb.DetectAndCompute(template, null, out keyPointsTemp, descTemplate);

                var matcher = new BFMatcher(NormTypes.Hamming);
                var matches = matcher.Match(descImage, descTemplate);
                // todo
            }
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            engine.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// The OpenCV library has an OCRTesseract class which gives more information other than text
        /// such as the location of text on the image and confidence score which can be useful.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Recognize(FileInfo pathFile)
        {
            string result = string.Empty;
            OpenCvSharp.Rect[] textLocations;
            string?[] componentTexts;
            float[] confidences;
            // Always remember to release Mat instances! The using syntax is useful.
            using (var img = Cv2.ImRead(pathFile.FullName, ImreadModes.Grayscale)) // Should I use Grayscale instant?
            //using (var kernel = new Mat(3, 3, MatType.CV_32F, ))
            //using (var denoise = new Mat())
            //using (var blur = new Mat())
            //using (var weighted = new Mat())
            //using (var bilateral = new Mat())
            {
                //Denoising(img, denoise);
                //Filter2D(denoise, );
                //GaussianWeighted(denoise, blur, weighted);
                //Bilateral(weighted, bilateral);
                //Cv2.Sobel();
                engine.Run(img, out result, out textLocations, out componentTexts, out confidences, OpenCvSharp.Text.ComponentLevels.TextLine);
            }

            return result;
        }

        public void Recognize(FileInfo pathFile, string saveFile)
        {
            string result = Recognize(pathFile);
            var fileName = String.Format("{0}\\{1:yyyy-MM-dd hh_mm_ss_fftt}.txt", saveFile, DateTime.Now);
            File.WriteAllText(fileName, result, System.Text.Encoding.Unicode);
        }

        public string Log
        {
            get => "";
        }
    }
}
