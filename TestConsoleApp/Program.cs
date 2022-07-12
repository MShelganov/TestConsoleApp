using PoiskIT.Andromeda.commands;
using PoiskIT.Andromeda.interfases;
using Serilog;
using System.Text;

namespace PoiskIT.Andromeda.Test
{
    class Program
    {
        internal static readonly List<ICommand> commands = new List<ICommand>();
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Loging();
            DI();
            Exec();
            Console.WriteLine("Close.");
            Console.ReadLine();
        }

        internal static void Loging()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

        }

        internal static void DI()
        {
            commands.Add(new HelpCommand(commands));
            commands.Add(new AkkaCommand());
            commands.Add(new OpenCVCommand());
            commands.Add(new OcrCommand());
            commands.Add(new RandomTestCommand());
            commands.Add(new ExitCommand());
        }

        internal static void Exec()
        {
            // Read comman
            string line;
            while ((line = Console.ReadLine()) != null)
            {
                string[] command = line.ToLower().Split(' ');
                if (command.Length > 0 && !String.IsNullOrEmpty(command[0]))
                    foreach (var cmd in commands)
                    {
                        if (cmd.Equals(command[0]))
                        {
                            string[] subcommand = command.Select((v, i) => new { Value = v, Index = i })
                                .Where(x => x.Index > 0)
                                .Select(x => x.Value)
                                .ToArray();
                            cmd.Execute(subcommand);
                            break;
                        }
                    }
            }
        }
    }
}