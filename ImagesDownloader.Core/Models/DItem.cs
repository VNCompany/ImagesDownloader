using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core.Models;

public class DItem(Uri source, string outputPath)
{
    public Uri Source { get; } = source;
    public string OutputPath { get; } = outputPath;
    public bool? IsSuccess { get; set; }

    public async Task Download(
        IDownloader downloader, 
        SemaphoreSlim semaphore, 
        CancellationToken cancellationToken,
        Action<DItemDownloadedArgs> callback)
    {
        try
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await downloader.SaveData(Source, OutputPath, cancellationToken);
                IsSuccess = true;
                callback.Invoke(new DItemDownloadedArgs(this, null));
            }
            finally
            {
                semaphore.Release();
            }
        }
        catch (Exception ex)
        {
            if (ex is not OperationCanceledException)
            {
                IsSuccess = false;
                callback.Invoke(new DItemDownloadedArgs(this, ex));
            }
        }
    }

    public override string ToString() => $"{{Source='{Source}', OutputPath='{OutputPath}'}}";
}
