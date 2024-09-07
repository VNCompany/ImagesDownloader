namespace ImagesDownloader.Interfaces;

internal interface IDownloadClient : IDisposable
{
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
