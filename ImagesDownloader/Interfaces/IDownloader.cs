namespace ImagesDownloader.Interfaces;

public interface IDownloader : IDisposable
{
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
