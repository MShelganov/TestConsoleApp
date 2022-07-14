using OpenCvSharp;
using PoiskIT.Andromeda.Settings.Filters;

namespace OCRlmplementaion.Settings.Filters.OpenCvSharp
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
            Cv2.Resize(src, dst, new Size(), dscale, dscale);
        }
    }
}
