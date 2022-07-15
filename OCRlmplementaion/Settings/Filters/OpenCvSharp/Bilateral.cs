using OpenCvSharp;

namespace PoiskIT.Andromeda.Settings.Filters.OpenCvSharp
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
            var dst = new Mat(src.Size(), src.Type());
            Cv2.CvtColor(src, src, ColorConversionCodes.GRAY2BGR);
            Cv2.BilateralFilter(src, dst, _d, _sigmaColor, _sigmaSpace);
            return dst;
        }
    }
}
