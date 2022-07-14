using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using PoiskIT.Andromeda.Settings;
using PoiskIT.Andromeda.Settings.Filters.EmguCv;
using System.Diagnostics;

namespace PoiskIT.Andromeda.Ocr
{
    public class EmguCvRecognizer : IRecognizer
    {
        private static readonly object Lock = new object();
        private readonly Emgu.CV.OCR.Tesseract engine;
        private readonly Options options;
        private readonly FilterManager<Mat> filters;
        private bool _disposed;
        private bool _debug = false;
        private string _log = string.Empty;

        public EmguCvRecognizer(Options ocrOptions, bool debug = false)
        {
            if (ocrOptions == null)
                ocrOptions = Options.Default;
            options = ocrOptions;
            _debug = debug;
            filters = new FilterManager<Mat>();
            string langsStr = String.Join("+", options.Languages);
            engine = new Emgu.CV.OCR.Tesseract(SetQuality(options.Quality), langsStr, OcrEngineMode.LstmOnly);
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

        public string Recognize(FileInfo imageInfo)
        {
            if (imageInfo == null)
                throw new ArgumentNullException("imageFile");

            var recognizeSw = new Stopwatch();
            var filtersSw = new Stopwatch();
            string resultText = String.Empty;

            var source = CvInvoke.Imread(imageInfo.FullName, ImreadModes.Grayscale); // Will be Dispose in FilterManager
            if (source == null)
                throw new ArgumentNullException(nameof(imageInfo));

            engine.PageSegMode = PageSegMode.SingleBlock;
            AddFilters();
            filtersSw.Start();
            var resultMat = filters.Processing(source);
            filtersSw.Stop();
            if (resultMat == null)
                throw new NullReferenceException("resultMat can't be null");
            engine.SetImage(resultMat);
            recognizeSw.Start();
            if (engine.Recognize() == 0)
                resultText = engine.GetUTF8Text();
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

            var recognizeSw = new Stopwatch();
            var filtersSw = new Stopwatch();
            string resultText = String.Empty;

            using (var bitmap = new Mat())
            {
                engine.PageSegMode = PageSegMode.SingleBlock;
                AddFilters();
                CvInvoke.Imdecode(image, ImreadModes.Grayscale, bitmap);
                engine.SetImage(bitmap);
                recognizeSw.Start();
                if (engine.Recognize() == 0)
                    resultText = engine.GetUTF8Text();
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

        public void Dispose()
        {
            if (_disposed)
                return;
            filters.Dispose();
            engine.Dispose();
            _disposed = true;
        }

        public string Log => _log;
    }
}
