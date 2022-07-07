using TestConsoleApp.interfases;

namespace TestConsoleApp.commands
{
    internal class ExitCommand : BaseCommand, ICommand
    {
        public override string Name => "exit";

        public override string Description => "Exit the programm.";

        public override void Execute(string[]? subcommand = null)
        {
            GC.Collect();
            Environment.Exit(0);
        }
    }
}
