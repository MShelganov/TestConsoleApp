using PoiskIT.Andromeda.interfases;

namespace PoiskIT.Andromeda.commands
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
