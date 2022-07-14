using OpenCvSharp;
using PoiskIT.Andromeda.Settings.Filters;

namespace OCRlmplementaion.Settings.Filters.OpenCvSharp
{
    public class Filter2D : IFilter<Mat>
    {
        public string Name { get; } = "Filter2D";
        public void Exec(Mat src, Mat dst)
        {
            float[,] matrix = new float[3, 3]
            {
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 0 }
            };
            var kernel = new Mat(3, 3, MatType.CV_32FC1, matrix);
            Cv2.Filter2D(src, dst, MatType.CV_32FC1, kernel, new Point(0, 0));
        }
    }
}
