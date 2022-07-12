namespace PoiskIT.Andromeda.interfases
{
    internal interface ICommand
    {
        string Name { get; }
        string Description { get; }
        void Execute(string[]? subcommand = null);
    }
}
