using TestConsoleApp.interfases;
using System.Diagnostics;

namespace TestConsoleApp.commands
{
    internal class OpenCVCommand : BaseCommand, ICommand
    {
        private const string exe = "OpenCVWinForms.exe";
        public override string Name => "opencv";

        public override string Description => "OpenCV test.";

        public override void Execute(string[]? subcommand = null)
        {
            Process.Start(exe);
        }
    }
}
