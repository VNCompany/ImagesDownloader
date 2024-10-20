using ImagesDownloader.Core.Interfaces;
using ImagesDownloader.Core.Extensions;

namespace ImagesDownloader.Core.Services;

internal class DownloadClientTest : IDownloader
{
    private readonly ILogger _logger = ServiceAccessor.GetRequiredService<ILogger>();

    public async Task<string> GetHtml(Uri uri, CancellationToken cancellationToken)
        => await File.ReadAllTextAsync("C:\\test.html", cancellationToken);

    public async Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            string v = uri.ToString();
            _logger.Info("Download: {0}", v);
            await Task.Delay(2000, cancellationToken);
            if (v.Contains("vk.com", StringComparison.OrdinalIgnoreCase))
                throw new OperationCanceledException("Timeout exception message", new TimeoutException());
            _logger.Info("Save {0} to {1}", v, outputPath);
            await Task.Delay(500, cancellationToken);
        }
        catch (OperationCanceledException ex)
        {
            if (ex.InnerException is TimeoutException tex)
                throw tex;
        }
    }

    public void Dispose() => _logger.Info("Download client disposed");
}
