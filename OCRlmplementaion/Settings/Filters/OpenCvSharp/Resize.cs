using OpenCvSharp;
using PoiskIT.Andromeda.Settings.Filters;

namespace PoiskIT.Andromeda.Settings.Filters.OpenCvSharp
{
    public class Resize : IFilter<Mat>
    {
        private double _size = 3000;
        public string Name { get; } = "Resize";

        public Resize(double minSize = 3000)
        {
            _size = minSize;
        }
        public Mat Exec(Mat src)
        {
            var dst = new Mat(src.Size(), src.Type());
            double dscale = Math.Min(_size / src.Width, _size / src.Height);
            Cv2.Resize(src, dst, new Size(), dscale, dscale);
            return dst;
        }
    }
}
