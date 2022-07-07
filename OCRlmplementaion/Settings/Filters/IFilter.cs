using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters
{
    public interface IFilter
    {
        public string Name { get; }

        public void Exec(Mat src, Mat dst);
    }
}
