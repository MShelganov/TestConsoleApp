using OpenCvSharp;
using OpenCvSharp.Text;
using PoiskIT.Andromeda.Settings;
using PoiskIT.Andromeda.Settings.Filters.OpenCvSharp;
using System.Diagnostics;

namespace PoiskIT.Andromeda.Ocr
{
    public class OpenCvSharpRecognizer : IRecognizer
    {
        private static readonly object Lock = new object();
        private readonly OCRTesseract engine;
        private readonly Options options;
        private readonly FilterManager<Mat> filters;
        private bool _debug = false;
        private bool _disposed = false;
        private string _log = String.Empty;
        public OpenCvSharpRecognizer(Options ocrOptions, bool debug = false)
        {
            if (ocrOptions == null)
                ocrOptions = Options.Default;
            options = ocrOptions;
            _debug = debug;
            filters = new FilterManager<Mat>();
            string langsStr = String.Join("+", options.Languages);
            engine = OCRTesseract.Create(SetQuality(options.Quality), langsStr);

        }

        private string SetQuality(QualityEnum quality)
        {
            switch (quality)
            {
                case QualityEnum.def:
                    return Config.TRAINED_DATA_PATH;
                case QualityEnum.fast:
                    return Config.TRAINED_DATA_FAST_PATH;
                case QualityEnum.best:
                    return Config.TRAINED_DATA_BEST_PATH;
            }
            return Config.TRAINED_DATA_PATH;
        }

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

        /// <summary>
        /// The OpenCV library has an OCRTesseract class which gives more information other than text
        /// such as the location of text on the image and confidence score which can be useful.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Recognize(FileInfo imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException("imageFile");

            string resultText = String.Empty;
            OpenCvSharp.Rect[] textLocations;
            string?[] componentTexts;
            float[] confidences;

            var recognizeSw = new Stopwatch();
            var filtersSw = new Stopwatch();

            var source = Cv2.ImRead(imageInfo.FullName, ImreadModes.Grayscale); // Will be Dispose in FilterManager
            if (source == null)
                throw new ArgumentNullException(nameof(imageInfo));

            AddFilters();
            filtersSw.Start();
            var resultMat = filters.Processing(source);
            filtersSw.Stop();
            if (resultMat == null)
                throw new NullReferenceException("resultMat can't be null");
            recognizeSw.Start();
            engine.Run(resultMat, out resultText, out textLocations, out componentTexts, out confidences, OpenCvSharp.Text.ComponentLevels.TextLine);
            recognizeSw.Stop();
            if (_debug)
            {
                _log = String.Format("\n\tFilterd time: {0} sec.", filtersSw.Elapsed.TotalSeconds.ToString());
                _log += String.Format("\n\tRecognize time: {0} sec.", recognizeSw.Elapsed.TotalSeconds.ToString());
            }
            return resultText;
        }

        public void Recognize(FileInfo imageInfo, string pathFile)
        {
            string result = Recognize(imageInfo);
            var fileName = String.Format("{0}\\{1}.{2:yyyy-MM-dd hh_mm_ss_fftt}.txt", pathFile, imageInfo.Name.Split('.')[0], DateTime.Now);
            _log += String.Format("Saved: {0} \n", fileName);
            File.WriteAllText(fileName, result, System.Text.Encoding.Unicode);
        }

        public string Recognize(byte[] image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            string resultText = String.Empty;
            OpenCvSharp.Rect[] textLocations;
            string?[] componentTexts;
            float[] confidences;

            var recognizeSw = new Stopwatch();
            var filtersSw = new Stopwatch();
            AddFilters();
            using (var bitmap = Cv2.ImDecode(image, ImreadModes.Grayscale))
            {
                recognizeSw.Start();
                engine.Run(bitmap, out resultText, out textLocations, out componentTexts, out confidences, OpenCvSharp.Text.ComponentLevels.TextLine);
                recognizeSw.Stop();
            }
            if (_debug)
            {
                _log = String.Format("\n\tFilterd time: {0} sec.", filtersSw.Elapsed.TotalSeconds.ToString());
                _log += String.Format("\n\tRecognize time: {0} sec.", recognizeSw.Elapsed.TotalSeconds.ToString());
            }
            return resultText;
        }

        public void Recognize(byte[] image, string nameFile, string pathFile)
        {
            string result = Recognize(image);
            var fileName = String.Format("{0}\\{1}.txt", pathFile, nameFile, DateTime.Now);
            _log += String.Format("Saved: {0} \n", fileName);
            File.WriteAllText(fileName, result, System.Text.Encoding.Unicode);
        }

        private void AddFilters()
        {
            if (options.IsScaling)
                filters.Add(new Resize());

            if (options.IsDenoising)
                filters.Add(new Denoising());

            if (options.IsGaussianWeighted)
                filters.Add(new GaussianWeighted());

            if (options.IsFilter2D)
                filters.Add(new Filter2D());

            if (options.IsBilateral)
                filters.Add(new Bilateral());
        }

        public string Log
        {
            get => "";
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            filters.Dispose();
            engine.Dispose();
            _disposed = true;
        }
    }
}
