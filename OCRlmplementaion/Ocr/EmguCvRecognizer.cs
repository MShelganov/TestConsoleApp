using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
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

        public string Recognize(FileInfo imageFile)
        {
            if (imageFile == null)
                throw new ArgumentNullException("imageFile");

            var recognizeSw = new Stopwatch();
            var filtersSw = new Stopwatch();
            string resultText = String.Empty;
            if (imageFile.Extension == ".pdf")
            {
                using (var pdfReader = new PDFRenderer(imageFile.FullName, SetQuality(options.Quality), false))
                using (var resfd = new Mat(2480, 3508, DepthType.Default, 0)) // 300 dpi
                using (Pix imgPix = new Pix(resfd))
                {
                    bool success = engine.ProcessPage(imgPix, 1, "img", null, 100000, pdfReader);
                    if (success)
                    {
                        engine.SetImage(imgPix);
                        if (engine.Recognize() == 0)
                            resultText = engine.GetUTF8Text();
                    }
                    else
                        throw new Exception("[Err] ProcessPage PDFRenderer");
                }
            }

            var source = CvInvoke.Imread(imageFile.FullName, ImreadModes.Grayscale); // Will be Dispose in FilterManager
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

        public void Recognize(FileInfo pathFile, string saveFile)
        {
            string result = Recognize(pathFile);
            var fileName = String.Format("{0}\\{1}_{2:yyyy-MM-dd hh_mm_ss_fftt}.txt", saveFile, pathFile.Name.Split('.')[0], DateTime.Now);
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
