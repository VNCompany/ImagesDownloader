using System.IO;
using System.Net.Http;

using ImagesDownloader.Enums;

namespace ImagesDownloader.Models;

internal class DownloadItem(Uri source, string savePath)
{
    public Uri Source { get; } = source;
    public string SavePath { get; } = savePath;
    public DownloadStatus Status { get; private set; } = DownloadStatus.Waiting;
    public bool IsSuccess => Status == DownloadStatus.Done;

    public async Task<DownloadStatus> Download(HttpClient client, CancellationToken cancellationToken)
    {
        try
        {
            byte[] bytes = await client.GetByteArrayAsync(Source, cancellationToken);
            await File.WriteAllBytesAsync(SavePath, bytes, cancellationToken);
            return Status = DownloadStatus.Done;
        }
        catch (TaskCanceledException tce)
        {
            Status = tce.InnerException is TimeoutException
                ? DownloadStatus.Failed : DownloadStatus.Canceled;
            throw;
        }
        catch (Exception)
        {
            Status = DownloadStatus.Failed;
            throw;
        }
    }
}
