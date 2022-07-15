using PoiskIT.Andromeda.Settings.Filters;

namespace PoiskIT.Andromeda.Settings
{
    public class FilterManager<T> : IDisposable where T : class, IDisposable
    {
        private bool _disposed;
        private List<T> mats;
        private List<IFilter<T>> filters;

        public FilterManager()
        {
            mats = new List<T>();
            filters = new List<IFilter<T>>();
        }

        public void Add(IFilter<T> filter)
        {
            if (filter == null)
                return;

            if (!filters.Exists(x => x.Name == filter.Name))
                filters.Add(filter);
        }

        public T Processing(T src)
        {
            mats.Add(src);
            foreach (IFilter<T> filter in filters)
            {
                var last = mats.Last();

                mats.Add(filter.Exec(last));
            }
            return mats.Last();
        }

        public void SaveFiltered(string? path, Func<T, string, bool> Save)
        {
            if (string.IsNullOrEmpty(path))
                return;

            var now = DateTime.Now;
            for (int i = 0; i < mats.Count; i++)
                if (i != 0)
                    if (!Save(mats[i], String.Format("{0}/filtered/{1}.{2:yyyy-MM-dd hh_mm_ss_fftt}{3}", path, filters[i - 1].Name, now, ".jpg")))
                        throw new Exception(String.Format("{0} not saved", mats[i]));
                
        }

        public void Dispose()
        {
            if (_disposed)
                return;
            for (int i = mats.Count - 1; i >= 0; i--)
                mats[i].Dispose();
            mats.Clear();
            filters.Clear();
            _disposed = true;
        }
    }
}
