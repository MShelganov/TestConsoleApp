namespace PoiskIT.Andromeda.Settings
{
    public interface IRecognizer : IDisposable
    {
        public string Recognize(FileInfo imageInfo);
        public void Recognize(FileInfo imageInfo, string saveFile);
        public string Recognize(byte[] image);
        public void Recognize(byte[] image, string nameFile, string pathFile);
        public string Log { get; }
    }
}
