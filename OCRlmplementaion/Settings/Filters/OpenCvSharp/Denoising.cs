using OpenCvSharp;
using PoiskIT.Andromeda.Settings.Filters;

namespace PoiskIT.Andromeda.Settings.Filters.OpenCvSharp
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
            var dst = new Mat(src.Size(), src.Type());
            Cv2.FastNlMeansDenoising(src, dst, _h, _templateWindowSize, _searchWindowSize);
            return dst;
        }
    }
}
