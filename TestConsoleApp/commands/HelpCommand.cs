using PoiskIT.Andromeda.interfases;

namespace PoiskIT.Andromeda.commands
{
    internal class HelpCommand : BaseCommand, ICommand
    {
        private List<ICommand> _commands;

        public HelpCommand(List<ICommand> commands)
        {
            _commands = commands;
        }

        public override string Name => "help";

        public override string Description => "Display the entire list of commands.";

        public override void Execute(string[]? subcommand = null)
        {
            if (subcommand == null || subcommand.Length <= 0)
                foreach (var command in _commands)
                   Console.WriteLine($"{command.Name} : {command.Description}");
            else
            {
                var name_commands = _commands.Select(x => x.Name);
                var coincidences = name_commands.Intersect(subcommand);

                foreach (var command in _commands)
                    foreach(var coincidence in coincidences)
                        if (command.Name == coincidence)
                        {
                            Console.WriteLine($"{command.Name} : {command.Description}");
                            break;
                        }
            }
        }
    }
}
