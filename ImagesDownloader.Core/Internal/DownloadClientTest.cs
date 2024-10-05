using ImagesDownloader.Core.Interfaces;
using ImagesDownloader.Core.Factories;
using ImagesDownloader.Core.Exceptions;
using ImagesDownloader.Core.Extensions;

namespace ImagesDownloader.Core.Internal;

internal class DownloadClientTest : IDownloader
{
    private readonly ILogger _logger = LoggerFactory.Logger;

    public async Task<string> GetHtml(Uri uri, CancellationToken cancellationToken)
        => await System.IO.File.ReadAllTextAsync("C:\\test.html", cancellationToken);

    public async Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken)
    {
        try
        {
            string v = uri.ToString();
            _logger.Info("Download: {0}", v);
            await Task.Delay(1000, cancellationToken);
            if (v.Contains("vk.com", StringComparison.OrdinalIgnoreCase))
                throw new OperationCanceledException("Timeout exception message", new TimeoutException());
            _logger.Info("Save {0} to {1}", v, outputPath);
            await Task.Delay(100, cancellationToken);
        }
        catch (Exception ex)
        {
            var type = ex is OperationCanceledException { InnerException: not TimeoutException };
            throw new DownloadDataException(type, ex.Message, ex);
        }
    }

    public void Dispose() => _logger.Info("Download client disposed");
}
