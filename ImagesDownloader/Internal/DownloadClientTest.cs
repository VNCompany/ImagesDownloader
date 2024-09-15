using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Internal;

internal class DownloadClientTest : IDownloader
{
    private readonly ILogger _logger = LoggerFactory.Logger;

    public async Task<string> GetHtml(Uri uri, CancellationToken cancellationToken)
        => await System.IO.File.ReadAllTextAsync("C:\\test.html", cancellationToken);

    public async Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken)
    {
        string v = uri.ToString();
        _logger.LogInformation("Download: {0}", v);
        await Task.Delay(1000, cancellationToken);
        if (v.Contains("vk.com", StringComparison.OrdinalIgnoreCase))
            throw new OperationCanceledException("Timeout exception message", new TimeoutException());
        _logger.LogInformation("Save {0} to {1}", v, outputPath);
        await Task.Delay(1000, cancellationToken);
    }

    public void Dispose() => _logger.LogInformation("Download client disposed");
}
