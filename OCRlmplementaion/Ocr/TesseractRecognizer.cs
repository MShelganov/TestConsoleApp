using Tesseract;
using PoiskIT.Andromeda.Settings;

namespace PoiskIT.Andromeda.Ocr
{
    public class TesseractRecognizer : IRecognizer
    {
        private static readonly object Lock = new object();
        private readonly TesseractEngine engine;
        private readonly Options options;
        private bool _disposed;

        public TesseractRecognizer(Options ocrOptions, bool debug = false)
        {
            if (ocrOptions == null)
                ocrOptions = Options.Default;
            options = ocrOptions;
            string langsStr = String.Join("+", options.Languages);
            engine = new TesseractEngine(Config.TRAINED_DATA_PATH, langsStr, EngineMode.Default);
        }

        public string Recognize(FileInfo imageFile)
        {
            string result = string.Empty;
            try
            {
                // Simple "eng" or multiply languages "jpn+eng"
                using (var img = Pix.LoadFromFile(imageFile.FullName))
                using (var page = engine.Process(img))
                    result = page.GetText();
            }
            catch (Exception e)
            {
                throw;
            }
            return result;
        }

        public void Recognize(FileInfo pathFile, string saveFile)
        {
            string result = Recognize(pathFile);
            var fileName = String.Format("{0}\\{1:yyyy-MM-dd hh_mm_ss_fftt}.txt", saveFile, DateTime.Now);
            File.WriteAllText(fileName, result, System.Text.Encoding.Unicode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public string Parsing(byte[] binary)
        {
            string result = string.Empty;
            try
            {
                //using (var tesseractEngine = new TesseractEngine(Config.TRAINED_DATA_PATH, Language.ToString(), EngineMode.Default))
                //using (var img = Pix.LoadFromMemory(binary))
                //using (var page = tesseractEngine.Process(img))
                //    result = page.GetText();
            }
            catch (Exception e)
            {
                throw;
            }
            return result;
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            engine.Dispose();
            _disposed = true;
        }
        public string Log
        {
            get => "";
        }
    }
}
