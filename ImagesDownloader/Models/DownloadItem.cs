using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Models;

internal class DownloadItem(Uri source, string outputPath)
{
    public Uri Source { get; } = source;
    public string OutputPath { get; } = outputPath;
    public bool IsCompleted { get; private set; }
    public bool IsCancelled { get; private set; }

    public async Task Download(IDownloadClient client, CancellationToken cancellationToken)
    {
        try
        {
            await client.SaveData(Source, OutputPath, cancellationToken);
            IsCompleted = true;
        }
        catch (OperationCanceledException ex) when (ex.InnerException is not TimeoutException)
        {
            IsCancelled = true;
            throw;
        }
    }
}
