using System.IO;
using System.Net.Http;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Common;

internal class DownloadClient : IDisposable
{
    private readonly ILogger _logger;
    private readonly HttpClient _client;

    public DownloadClient(ILogger logger)
    {
        _logger = logger;
        _client = new HttpClient();
    }

    public async Task<string> DownloadHtml(Uri url)
    {
        try
        {
            return await _client.GetStringAsync(url);
        }
        catch (Exception ex)
        {
            _logger.Error(nameof(DownloadHtml), $"Error while loading html from `{url}` \n\tInfo: {ex.Message}");
            return string.Empty;
        }
    }

    public async Task DownloadFile(Uri url, string outputPath, CancellationToken cancellationToken)
    {
        byte[] buffer;
        try
        {
            buffer = await _client.GetByteArrayAsync(url, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(nameof(DownloadFile),
                $"Error while loading file from `{url}` \n\t" +
                $"Output path: {outputPath} \n\t" +
                $"Info: {ex.Message}");
            throw;
        }

        try
        {
            await File.WriteAllBytesAsync(outputPath, buffer, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.Error(nameof(DownloadFile),
                $"Error while saving to file from `{url}` \n\t" +
                $"Output path: {outputPath} \n\t" +
                $"Info: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _client.Dispose();
    }
}
