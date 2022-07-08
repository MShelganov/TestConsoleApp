using System.Text;
using PoiskIT.Andromeda.interfases;
using PoiskIT.Andromeda.Metrics;

namespace PoiskIT.Andromeda.commands
{
    internal class RandomTestCommand : BaseCommand, ICommand
    {
        private enum LanguageEnum
        {
            rus, eng, jpn
        }
        public override string Name => "test";

        public override string Description => "Run random test.";

        private delegate void RandomTest();
        private readonly List<RandomTest> _randomTests;

        public RandomTestCommand()
        {
            _randomTests = new List<RandomTest>();
            _randomTests.Add(Test0);
            _randomTests.Add(Test1);
            _randomTests.Add(Test2);
        }

        public override void Execute(string[]? subcommand = null)
        {
            int test = new Random().Next(_randomTests.Count);
            if (subcommand != null && subcommand.Length > 0)
            {
                if (int.TryParse(subcommand[0], out test))
                    Console.WriteLine($"Passing test number: {test}");
            }
            if (test > _randomTests.Count() - 1)
                test = _randomTests.Count() - 1;
            _randomTests[test]();
        }

        private void Test0()
        {
            var languages = new LanguageEnum[] { LanguageEnum.eng, LanguageEnum.jpn, LanguageEnum.rus };
            string langsStr = String.Join<LanguageEnum>("+", languages);
            Console.WriteLine($"Test enums: {langsStr}"); // Test enums: eng+jpn+rus
        }

        private void Test1()
        {
            Console.WriteLine("Getting all file names from a folder.");
            string tessdatapath = @"C:\tessdata_fast";
            DirectoryInfo d1 = new DirectoryInfo(tessdatapath); //Assuming Test is your Folder

            FileInfo[] Files = d1.GetFiles("*.traineddata"); //Getting Text files
            StringBuilder sb = new StringBuilder();
            Console.WriteLine(tessdatapath);
            foreach (FileInfo file in Files)
            {
                sb.Append($"\"{file.Name.Split('.')[0]}\",");
            }

            
            Console.WriteLine(sb.ToString());
        }

        private void Test2()
        {
            Console.Write("Введите число : ");
            var test = Console.ReadLine();
            if (String.IsNullOrEmpty(test))
                return;
            var metrics = new MemoryMetricsClient();
            metrics.OnMeasurementsCompleted += Metrics_OnMeasurementsCompleted;
            metrics.Start();
            int num = int.Parse(test);
            int factorial = 1;
            for (int i = 1; i <= num; i++)
            {
                factorial *= i;
                if (i == num)
                {
                    Console.Write("{0}", i);
                }
                else
                {
                    Console.Write("{0} * ", i);
                }
            }
            Console.Write(" = {0}", factorial);
            metrics.Stop();
        }

        private void Metrics_OnMeasurementsCompleted(object? sender, EventArgs e)
        {
            if (sender != null)
                Console.WriteLine(((MemoryMetricsClient)sender).ToString());
        }
    }
}
