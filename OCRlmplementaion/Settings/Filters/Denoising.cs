using Emgu.CV;

namespace PoiskIT.Andromeda.Settings.Filters
{
    public class Denoising : IFilter
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
        public void Exec(Mat src, Mat dst)
        {
            CvInvoke.FastNlMeansDenoising(src, dst, _h, _templateWindowSize, _searchWindowSize);
        }
    }
}
