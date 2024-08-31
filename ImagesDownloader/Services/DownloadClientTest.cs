using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services;

internal class DownloadClientTest(ILogger logger) : IDownloadClient
{
    public async Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken)
    {
        string v = uri.ToString();
        logger.Log(LogType.INFO, $"Download: {v}");
        await Task.Delay(2000, cancellationToken);
        if (v.Contains("vk.com", StringComparison.OrdinalIgnoreCase))
            throw new OperationCanceledException("Timeout exception message", new TimeoutException());
        logger.Log(LogType.INFO, $"Save {v} to {outputPath}");
        await Task.Delay(2000, cancellationToken);
    }
}
