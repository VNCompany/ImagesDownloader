using System.IO;
using System.Net.Http;

namespace ImagesDownloader.Models;

internal class DownloadItem(Uri source, string outputPath)
{
    public Uri Source { get; } = source;
    public string OutputPath { get; } = outputPath;
    public bool IsCompleted { get; private set; }

    public async Task Download(HttpClient client, CancellationToken cancellationToken)
    {
        try
        {
            var data = await client.GetByteArrayAsync(Source, cancellationToken);
            await File.WriteAllBytesAsync(OutputPath, data, cancellationToken);
        }
        catch (Exception ex)
        {
            IsCompleted = ex is not OperationCanceledException || ex.InnerException is not TimeoutException;
            throw;
        }
    }
}
