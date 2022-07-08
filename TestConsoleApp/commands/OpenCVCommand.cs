using PoiskIT.Andromeda.interfases;
using System.Diagnostics;

namespace PoiskIT.Andromeda.commands
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
