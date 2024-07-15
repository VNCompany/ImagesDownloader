namespace ImagesDownloader.Interfaces;

public interface ILogger : IDisposable
{
    void Error(string source, string message);
}
