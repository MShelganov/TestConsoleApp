using OpenCvSharp;
using PoiskIT.Andromeda.Settings.Filters;

namespace PoiskIT.Andromeda.Settings.Filters.OpenCvSharp
{
    public class Filter2D : IFilter<Mat>
    {
        public string Name { get; } = "Filter2D";
        public Mat Exec(Mat src)
        {
            float[,] matrix = new float[3, 3]
            {
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 0 }
            };
            var kernel = new Mat(3, 3, MatType.CV_32FC1, matrix);
            var dst = new Mat(src.Size(), src.Type());
            Cv2.Filter2D(src, dst, MatType.CV_32FC1, kernel, new Point(0, 0));
            return dst;
        }
    }
}
