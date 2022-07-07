using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters
{
    public class Filter2D : IFilter
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
            ConvolutionKernelF kernel = new ConvolutionKernelF(matrix);
            CvInvoke.Filter2D(src, dst, kernel, new System.Drawing.Point(0, 0));
        }
    }
}
