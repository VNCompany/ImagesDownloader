namespace ImagesDownloader.Interfaces;

public interface IDownloader : IDisposable
{
    Task<string> GetHtml(Uri uri, CancellationToken cancellationToken);
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
