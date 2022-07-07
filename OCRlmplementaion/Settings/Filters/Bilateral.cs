using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters
{
    public class Bilateral : IFilter
    {
        private Mat temp;
        private int _d;
        private double _sigmaColor;
        private double _sigmaSpace;
        public string Name { get; } = "Bilateral";

        public Bilateral(int d = 5, double sigmaColor = 10, double sigmaSpace = 2)
        {
            temp = new Mat();
            _d = d;
            _sigmaColor = sigmaColor;
            _sigmaSpace = sigmaSpace;
        }
        public void Exec(Mat src, Mat dst)
        {
            CvInvoke.BilateralFilter(src, temp, _d, _sigmaColor, _sigmaSpace); 
        }
    }
}
