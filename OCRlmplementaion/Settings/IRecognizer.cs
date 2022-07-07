namespace PoiskIT.Andromeda.Settings
{
    public interface IRecognizer : IDisposable
    {
        public string Recognize(FileInfo pathFile);
        public void Recognize(FileInfo pathFile, string saveFile);
        public string Log { get; }
    }
}
