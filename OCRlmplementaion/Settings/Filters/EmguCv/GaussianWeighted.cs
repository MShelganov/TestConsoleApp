using Emgu.CV;
using System.Drawing;

namespace PoiskIT.Andromeda.Settings.Filters.EmguCv
{
    public class GaussianWeighted : IFilter<Mat>
    {
        private Size _size;
        private double _sigma;
        private double _alpha;
        private double _beta;
        private double _gamma;

        public string Name { get; } = "Gaussian";

        public GaussianWeighted(int width = 5, int height = 5, double sigmaX = 0.0,
                                double alpha = 1.5, double beta = -.5, double gamma = 0)
        {
            _size = new Size(width, height);
            _sigma = sigmaX;
            _alpha = alpha;
            _beta = beta;
            _gamma = gamma;
        }
        public Mat Exec(Mat src)
        {
            var dst = new Mat(src.Size, src.Depth, src.NumberOfChannels);
            CvInvoke.GaussianBlur(src, dst, _size, _sigma);
            CvInvoke.AddWeighted(src, _alpha, dst, _beta, _gamma, dst);
            return dst;
        }
    }
}
