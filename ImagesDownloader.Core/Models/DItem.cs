using System.ComponentModel;
using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core.Models;

public class DItem(Uri source, string outputPath) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Uri Source { get; } = source;
    public string OutputPath { get; } = outputPath;

    private bool? _isSuccess;
    public bool? IsSuccess
    {
        get => _isSuccess;
        set
        {
            _isSuccess = value;
            PropertyChanged?.Invoke(this, new(nameof(IsSuccess)));
        }
    }

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
