namespace ImagesDownloader.Services.Download;

internal sealed class DownloadService : IDisposable
{
    private readonly object _thLocker = new object();

    public event EventHandler<(object, object)>? ItemDownloaded;

    public async Task Start() { }

    public async Task Stop() { }

    public void Dispose()
    {
        Stop();
    }

    private void OnItemDownloaded(DownloadCollection collection, DownloadCollectionItem item)
    {
        lock (_thLocker)
            ItemDownloaded?.Invoke(this, (collection.Source, item.Source));
    }
}
