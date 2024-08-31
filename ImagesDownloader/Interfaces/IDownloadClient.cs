namespace ImagesDownloader.Interfaces;

internal interface IDownloadClient
{
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
