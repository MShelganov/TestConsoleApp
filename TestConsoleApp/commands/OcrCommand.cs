using PoiskIT.Andromeda.interfases;
using PoiskIT.Andromeda.Metrics;
using PoiskIT.Andromeda.Ocr;
using PoiskIT.Andromeda.Settings;
using PoiskIT.Andromeda.Utils;
using Serilog;
using System.Diagnostics;

namespace PoiskIT.Andromeda.commands
{
    // ocr -qd -lru -r
    internal class OcrCommand : BaseCommand, ICommand
    {
        private readonly string openPath = @"/data/pdf/";
        private readonly string savePath = @"/data/pdf/texts";
        //private readonly string openPath = @"\\PC-MSHELGANOV\ocr\pdf\";
        //private readonly string savePath = @"\\PC-MSHELGANOV\ocr\pdf\texts";
        //private readonly string openPath = @"G:\temp\pdf\";
        //private readonly string savePath = @"G:\temp\pdf\texts";
        private delegate void EngineExecs<T>(Options op, string path) where T : IRecognizer;
        private Dictionary<string, EngineExecs<IRecognizer>> engines;
        public override string Name => "ocr";

        public override string Description => "Tesseract ocr test.";


        public OcrCommand()
        {
            engines = new Dictionary<string, EngineExecs<IRecognizer>>();
            engines.Add("tess", OcrReaderExec<TesseractRecognizer>);
            engines.Add("sharp", OcrReaderExec<OpenCvSharpRecognizer>);
            engines.Add("emgu", OcrReaderExec<EmguCvRecognizer>);
        }

        public override void Execute(string[]? subcommand = null)
        {
            Log.Information("Execute OCR Task!");
            try
            {
                var op = Options.Best;

                string engineKey = "emgu";
                if (subcommand != null)
                {
                    engineKey = subcommand.Intersect(engines.Keys).FirstOrDefault("emgu");
                    string? lang = subcommand.Where(x => x.StartsWith("-l")).FirstOrDefault();
                    SetLanguage(lang, op);
                    Log.Information("Using languages: {0}", String.Join(' ', op.Languages));

                    string? quality = subcommand.Where(x => x.StartsWith("-q")).FirstOrDefault();
                    SetQuality(quality, op);
                    Log.Information("Using quality: {0}", op.Quality.ToString());

                    string? filters = subcommand.Where(x => x.StartsWith("-f")).FirstOrDefault();
                    SetFilters(filters, op);
                    Log.Information("With filters: ", op.IsDenoising || op.IsFilter2D || op.IsScaling || op.IsBilateral || op.IsGaussianWeighted);

                    string? dir = subcommand.Where(x => x.StartsWith("-r")).FirstOrDefault();
                    if (!String.IsNullOrEmpty(dir))
                        op.IsDirectory = true;
                }

                using (var metrics = new MemoryMetricsClient())
                {
                    metrics.OnMeasurementsCompleted += Metrics_OnMeasurementsCompleted;
                    metrics.Start();
                    engines[engineKey](op, openPath);
                    metrics.Stop();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetFilters(string? command, Options options)
        {
            if (String.IsNullOrEmpty(command))
                return;
            string[] f = command.Split('f'); // -fsblg
            options.IsScaling = f[1].Contains('s');
            options.IsBilateral = f[1].Contains('b');
            options.IsDenoising = f[1].Contains('d');
            options.IsFilter2D = f[1].Contains('l'); // f and d alredy exist
            options.IsGaussianWeighted = f[1].Contains('g');
        }

        private void SetQuality(string? command, Options options)
        {
            if (String.IsNullOrEmpty(command))
                return;

            string[] q = command.Split('q');
            QualityEnum quality = QualityEnum.def;
            switch (q[1])
            {
                case "1":
                case "d":
                    quality = QualityEnum.def;
                    break;
                case "2":
                case "f":
                    quality = QualityEnum.fast;
                    break;
                case "3":
                case "b":
                    quality = QualityEnum.best;
                    break;
            }
            options.Quality = quality;
        }

        private void SetLanguage(string? command, Options options)
        {
            if (String.IsNullOrEmpty(command))
                return;

            string[] language = command.Split('l');
            string[] languages = new[] {"rus"};
            switch (language[1])
            {
                case "rus":
                case "ru":
                    languages = new[] { "rus" };
                    break;
                case "eng":
                case "en":
                    languages = new[] { "eng" };
                    break;
                case "deu":
                case "de":
                    languages = new[] { "deu" };
                    break;
                case "frm":
                case "fr":
                    languages = new[] { "frm" };
                    break;
                case "spa":
                case "sp":
                    languages = new[] { "spa" };
                    break;
                case "chi":
                case "ch":
                    languages = new[] { "chi_sim", "chi_tra" };
                    break;
                case "jpn":
                case "jp":
                    languages = new[] { "jpn" };
                    break;
                case "ara":
                case "ar":
                    languages = new[] { "ara" };
                    break;
                case "tur":
                case "tu":
                case "tr":
                    languages = new[] { "tur" };
                    break;
                case "heb":
                case "hb":
                case "he":
                    languages = new[] { "heb" };
                    break;
                default:
                    languages = new[] { "rus", "eng", "deu", "frm", "spa", "chi_sim", "chi_tra", "jpn", "ara", "tur", "heb" };
                    if (options.Quality == QualityEnum.best)
                        languages = new[] { "rus", "eng", "deu", "frm", "chi_sim", "chi_tra", "jpn", "ara", "heb" };
                    break;
            }
            options.Languages = languages;
        }

        private void OcrReaderExec<T>(Options op, string path) where T : IRecognizer
        {
            var perfom = new Stopwatch();
            var typeofT = typeof(T).ToString();
            Console.WriteLine("Performing {0} task... ", typeofT);
            Log.Information("Performing {0} task... ", typeofT);
            perfom.Start();
            try
            {
                using (T? reader = (T?)Activator.CreateInstance(typeof(T), op, true))
                {
                    if (reader == null)
                        throw new ArgumentNullException(nameof(T));
                    if (op.IsDirectory)
                    {
                        DirectoryInfo d1 = new DirectoryInfo(path); //Assuming Test is your Folder

                        FileInfo[] jpgs = d1.GetFiles("*.*")
                            .Where(s => s.Extension == ".png" || s.Extension == ".jpg")
                            .ToArray();
                        foreach (FileInfo jpg in jpgs)
                        {
                            try
                            {
                                reader.Recognize(jpg, savePath);
                                Log.Information(reader.Log, "");
                            }
                            catch (Exception fileEx)
                            {
                                Log.Error(fileEx, jpg.FullName);
                            }
                        }
                        FileInfo[] pdfs = d1.GetFiles("*.pdf");
                        foreach (FileInfo pdf in pdfs)
                        {
                            try
                            {
                                var listPages = PdfConverter.Convert(pdf.FullName, savePath);
                                for (var i = 0; i < listPages.Count; i++)
                                {
                                    string pdfname = pdf.Name.Split('.')[0];
                                    reader.Recognize(listPages[i], $"{pdfname}.Page.{i}", savePath);
                                    Log.Information(reader.Log);
                                }
                            }
                            catch (Exception fileEx)
                            {
                                Log.Error(fileEx, pdf.FullName);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("-------------------------------------------------------------");
                        var file = new FileInfo(path);
                        string result = reader.Recognize(file);
                        Console.WriteLine($"Saved in {savePath}");
                        Task.Run(() => SaveResult(result));
                        Console.WriteLine("-------------------------------------------------------------");
                        Console.WriteLine(reader.Log);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, typeofT);
            }
            finally
            {
                perfom.Stop();
                Console.WriteLine("\tTotal time: {0} seconds.", perfom.Elapsed.TotalSeconds.ToString());
                Log.Information("Total time: {0} seconds.", perfom.Elapsed.TotalSeconds.ToString());
            }
        }

        private async Task SaveResult(string text)
        {
            var fileName = String.Format("{0}\\{1:yyyy-MM-dd hh_mm_ss_fftt}.txt", savePath, DateTime.Now);
            using (StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.Unicode))
            {
                await writer.WriteLineAsync(text);
            }
        }

        private void Metrics_OnMeasurementsCompleted(object? sender, EventArgs e)
        {
            if (sender != null)
                Log.Information(((MemoryMetricsClient)sender).ToString());
        }
    }
}
