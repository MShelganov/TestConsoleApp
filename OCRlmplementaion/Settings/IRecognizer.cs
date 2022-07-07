namespace PoiskIT.Andromeda.Settings
{
    public interface IRecognizer : IDisposable
    {
        public string Recognize(string pathFile);
        public void Recognize(string pathFile, string saveFile);
        public string Log { get; }
    }
}
