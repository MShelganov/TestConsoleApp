using TestConsoleApp.interfases;

namespace TestConsoleApp.commands
{
    internal class BaseCommand : ICommand, IEquatable<ICommand>
    {
        public virtual string Name => throw new NotImplementedException();

        public virtual string Description => String.Empty;

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            ICommand? objAsBase = obj as ICommand;
            if (objAsBase != null) return Equals(objAsBase);
            string? strAsBase = obj as string;
            if (strAsBase != null) return Equals(strAsBase);
            return false;
        }

        public bool Equals(ICommand? other)
        {
            if (other == null) return false;
            return (this.Name.Equals(other.Name));
        }

        public bool Equals(string other)
        {
            if (String.IsNullOrEmpty(other)) return false;
            return this.Name.Equals(other);
        }

        public virtual void Execute(string[]? subcommand = null)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
