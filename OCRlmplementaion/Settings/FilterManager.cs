using Emgu.CV;
using PoiskIT.Andromeda.Settings.Filters;

namespace PoiskIT.Andromeda.Settings
{
    public class FilterManager : IDisposable
    {
        private bool _disposed;
        private List<Mat> mats;
        private List<IFilter> filters;

        public FilterManager()
        {
            mats = new List<Mat>();
            filters = new List<IFilter>();
        }

        public void Add(IFilter filter)
        {
            if (filter == null)
                return;

            if (!filters.Exists(x => x.Name == filter.Name))
                filters.Add(filter);
        }

        public Mat Processing(Mat src)
        {
            mats.Add(src);
            foreach (IFilter filter in filters)
            {
                var last = mats.Last();
                var filterMat = new Mat(last.Size, last.Depth, last.NumberOfChannels);
                filter.Exec(last, filterMat);
                mats.Add(filterMat);
            }
            return mats.Last();
        }

        public void SaveFiltered(string? path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            var now = DateTime.Now;
            for (int i = 0; i < mats.Count; i++)
                if (i != 0)
                    mats[i].Save(String.Format("{0}/filtered/{1}.{2:yyyy-MM-dd hh_mm_ss_fftt}{3}", path, filters[i-1].Name, now, ".jpg"));
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
