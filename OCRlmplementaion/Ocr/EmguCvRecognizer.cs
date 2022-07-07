using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using PoiskIT.Andromeda.Settings;
using PoiskIT.Andromeda.Settings.Filters;
using System.Diagnostics;

namespace PoiskIT.Andromeda.Ocr
{
    public class EmguCvRecognizer : IRecognizer
    {
        private static readonly object Lock = new object();
        private readonly Emgu.CV.OCR.Tesseract engine;
        private readonly Options options;
        private bool _disposed;
        private bool _debug = false;
        private string _log = string.Empty;

        public EmguCvRecognizer(Options ocrOptions, bool debug = false)
        {
            if (ocrOptions == null)
                ocrOptions = Options.Default;
            options = ocrOptions;
            _debug = debug;
            string langsStr = String.Join("+", options.Languages);
            engine = new Emgu.CV.OCR.Tesseract(SetQuality(ocrOptions.Quality), langsStr, OcrEngineMode.LstmOnly);
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

        public string Recognize(string imageFile)
        {
            if (String.IsNullOrEmpty(imageFile))
                throw new ArgumentNullException("imageFile");

            var recognizeSw = new Stopwatch();
            var filtersSw = new Stopwatch();
            string resultText = String.Empty;

            var source = CvInvoke.Imread(imageFile, ImreadModes.Grayscale); // Will be Dispose in FilterManager
            using (var filters = new FilterManager())
            {
                engine.PageSegMode = PageSegMode.SingleBlock;
                if (options.IsScaling)
                    filters.Add(new Resize());

                if (options.IsDenoising)
                    filters.Add(new Denoising());

                if (options.IsGaussianWeighted)
                    filters.Add(new GaussianWeighted());

                if (options.IsFilter2D)
                    filters.Add(new Filter2D());

                //if (options.IsBilateral)
                //    filters.Add(new Bilateral());

                filtersSw.Start();
                var resultMat = filters.Processing(source);
                filtersSw.Stop();
                if (resultMat == null)
                    throw new NullReferenceException("resultMat can't be null");
                engine.SetImage(resultMat);
                recognizeSw.Start();
                resultText = engine.GetUTF8Text();
                recognizeSw.Stop();
            }
            if (_debug)
            {
                _log = String.Format("Filterd time: {0} sec.\n", filtersSw.Elapsed.TotalSeconds.ToString());
                _log += String.Format("Recognize time: {0} sec.\n", recognizeSw.Elapsed.TotalSeconds.ToString());
            }
            return resultText;
        }

        public void Recognize(string pathFile, string saveFile)
        {
            FileInfo info = new FileInfo(pathFile);
            string result = Recognize(pathFile);
            var fileName = String.Format("{0}\\{1}_{2:yyyy-MM-dd hh_mm_ss_fftt}.txt", saveFile, info.Name.Split('.')[0], DateTime.Now);
            _log += String.Format("Saved: {0} \n", fileName);
            File.WriteAllText(fileName, result, System.Text.Encoding.Unicode);
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            engine.Dispose();
            _disposed = true;
        }

        public string Log => _log;
    }
}
