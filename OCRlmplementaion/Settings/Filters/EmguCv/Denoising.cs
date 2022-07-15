using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters.EmguCv
{
    public class Denoising : IFilter<Mat>
    {
        private float _h;
        private int _templateWindowSize;
        private int _searchWindowSize;
        public string Name { get; } = "Denois";

        public Denoising(float h = 3f, int templateWindowSize = 7, int searchWindowSize = 21)
        {
            _h = h;
            _templateWindowSize = templateWindowSize;
            _searchWindowSize = searchWindowSize;
        }
        public Mat Exec(Mat src)
        {
            var dst = new Mat(src.Size, src.Depth, src.NumberOfChannels);
            CvInvoke.FastNlMeansDenoising(src, dst, _h, _templateWindowSize, _searchWindowSize);
            return dst;
        }
    }
}
