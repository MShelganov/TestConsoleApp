namespace PoiskIT.Andromeda.Settings.Filters
{
    public interface IFilter<T> where T : IDisposable
    {
        public string Name { get; }

        public T Exec(T src);
    }
}
