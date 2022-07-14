using Emgu.CV;
using PoiskIT.Andromeda.Settings.Filters;

namespace PoiskIT.Andromeda.Settings.Filters.EmguCv
{
    public class Resize : IFilter<Mat>
    {
        private double _size = 3000;
        public string Name { get; } = "Resize";

        public Resize(double minSize = 3000)
        {
            _size = minSize;
        }
        public void Exec(Mat src, Mat dst)
        {
            double dscale = Math.Min(_size / src.Width, _size / src.Height);
            CvInvoke.Resize(src, dst, new System.Drawing.Size(), dscale, dscale);
        }
    }
}
