using System.Net.Http;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using ImagesDownloader.Enums;

namespace ImagesDownloader.Models;

internal class DownloadItemsCollection(IEnumerable<DownloadItem> items) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public DownloadItem[] Items { get; } = items.ToArray();
    public ObservableCollection<DownloadItem> Log { get; } = [];

    private DownloadStatus _status = DownloadStatus.Waiting;
    public DownloadStatus Status
    {
        get => _status;
        private set => SetProperty(ref _status, value);
    }

    private double _percent;
    public double Percent
    {
        get => _percent;
        private set => SetProperty(ref _percent, value);
    }

    public async Task Download(
        int connectionsPerUnitCount, 
        CancellationToken cancellationToken, 
        Action<DownloadItem> errorCallback)
    {
        using var client = new HttpClient();
        using var sema = new SemaphoreSlim(connectionsPerUnitCount);

        // TODO: Make download queue
        var tasks = new List<Task>();
        foreach (var item in Items.Where(x => x.Status < DownloadStatus.Done))
        {
            var t = Task.Run(async () =>
            {
                await sema.WaitAsync(cancellationToken);

                sema.Release();
            }, cancellationToken);
            tasks.Add(t);
        }

        await Task.WhenAll(tasks);
    }

    private void OnItemDownloaded(DownloadItem item)
    {
        if (item.Status == DownloadStatus.Canceled) return;

        lock (this)
        {
            Log.Add(item);
            Percent = (double)Log.Count / Items.Length * 100.0;
            if (Log.Count == Items.Length) Status = DownloadStatus.Done;
        }
    }

    private void SetProperty<T>(ref T prop, T newValue, [CallerMemberName] string? propertyName = null)
    {
        prop = newValue;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
