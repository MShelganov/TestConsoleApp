using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters.EmguCv
{
    public class Bilateral : IFilter<Mat>
    {
        private int _d;
        private double _sigmaColor;
        private double _sigmaSpace;
        public string Name { get; } = "Bilateral";

        public Bilateral(int d = 5, double sigmaColor = 10, double sigmaSpace = 2)
        {
            _d = d;
            _sigmaColor = sigmaColor;
            _sigmaSpace = sigmaSpace;
        }
        public Mat Exec(Mat src)
        {
            var dst = new Mat(src.Size, src.Depth, src.NumberOfChannels);
            CvInvoke.BilateralFilter(src, dst, _d, _sigmaColor, _sigmaSpace);
            return dst;
        }
    }
}
