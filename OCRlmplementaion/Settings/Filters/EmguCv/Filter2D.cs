using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters.EmguCv
{
    public class Filter2D : IFilter<Mat>
    {
        public string Name { get; } = "Filter2D";
        public Mat Exec(Mat src)
        {
            var dst = new Mat(src.Size, src.Depth, src.NumberOfChannels);
            float[,] matrix = new float[3, 3]
            {
                { 0, 0, 0 },
                { 0, 1, 0 },
                { 0, 0, 0 }
            };
            ConvolutionKernelF kernel = new ConvolutionKernelF(matrix);
            CvInvoke.Filter2D(src, dst, kernel, new System.Drawing.Point(0, 0));
            return dst;
        }
    }
}
