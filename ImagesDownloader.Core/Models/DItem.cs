using ImagesDownloader.Core.Exceptions;
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
            await downloader.SaveData(Source, OutputPath, cancellationToken);
            IsSuccess = true;
            callback.Invoke(new DItemDownloadedArgs(this, null));
        }
        catch (Exception ex)
        {
            if (ex is not DownloadDataException ddex || !ddex.IsTaskCanceled)
            {
                IsSuccess = false;
                callback.Invoke(new DItemDownloadedArgs(this, ex));
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public override string ToString() => $"{{Source='{Source}', OutputPath='{OutputPath}'}}";
}
