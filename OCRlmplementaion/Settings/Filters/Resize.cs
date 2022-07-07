using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters
{
    public class Resize : IFilter
    {
        private double _size = 3000;
        private double _h;
        private int _templateWindowSize;
        private int _searchWindowSize;
        public string Name { get; } = "Resize";

        public Resize(double h = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
        {
            _h = h;
            _templateWindowSize = templateWindowSize;
            _searchWindowSize = searchWindowSize;
        }
        public void Exec(Mat src, Mat dst)
        {
            double dscale = Math.Min(_size / (double)src.Width, _size / (double)src.Height);
            CvInvoke.Resize(src, dst, new System.Drawing.Size(), dscale, dscale);
        }
    }
}
