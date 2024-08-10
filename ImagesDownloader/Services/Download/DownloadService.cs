using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services.Download;

internal class DownloadService : IDisposable
{
    private readonly ILogger _logger;

    public DownloadService(ILogger logger)
    {
        _logger = logger;
    }

    public void Dispose()
    {
    }

    private void OnItemDownloadFailed(DownloadCollectionItem item, Exception exception)
    {
        _logger.LogError($"{exception.GetType().Name}: " +
            $"{exception.Message} \n\tUrl: {item.Url} \n\tPath: {item.SavePath}", "Load item");
    }
}
