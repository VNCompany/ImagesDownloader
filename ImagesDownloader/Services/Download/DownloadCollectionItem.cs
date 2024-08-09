using System.IO;
using System.Net.Http;

namespace ImagesDownloader.Services.Download;

public enum DownloadCollectionItemStatus
{
    Waiting = 0,
    Canceled = 1,
    Done = 2,
    Failed = 3
}

internal class DownloadCollectionItem(Uri url, string savePath)
{
    public Uri Url { get; } = url;
    public string SavePath { get; } = savePath;
    public DownloadCollectionItemStatus Status { get; set; }

    public async Task Download(
        HttpClient client, 
        Action<DownloadCollectionItem> downloadedCallback,
        Action<DownloadCollectionItem, Exception> errorCallback,
        CancellationToken cancellationToken)
    {
        try
        {
            byte[] bytes = await client.GetByteArrayAsync(Url, cancellationToken);
            await File.WriteAllBytesAsync(SavePath, bytes, cancellationToken);
            Status = DownloadCollectionItemStatus.Done;
        }
        catch (TaskCanceledException tce)
        {
            if (tce.InnerException is TimeoutException)
            {
                Status = DownloadCollectionItemStatus.Failed;
                errorCallback?.Invoke(this, tce.InnerException);
            }
            else
            {
                Status = DownloadCollectionItemStatus.Canceled;
                return;
            }
        }
        catch (Exception ex)
        {
            Status = DownloadCollectionItemStatus.Failed;
            errorCallback?.Invoke(this, ex);
        }

        downloadedCallback.Invoke(this);
    }
}
