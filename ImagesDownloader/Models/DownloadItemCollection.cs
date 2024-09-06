using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ImagesDownloader.Models;

internal sealed class DownloadItemCollection : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public Queue<DownloadItem> ItemsQueue { get; }
    public ObservableCollection<DownloadItem> DownloadedItems { get; } = [];
    public double Percent => (double)DownloadedItems.Count / ItemsQueue.Count * 100.0;

    public DownloadItemCollection(IEnumerable<DownloadItem> items)
    {
        ItemsQueue = new(items);
        DownloadedItems.CollectionChanged += Logs_CollectionChanged;
    }

    private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Percent)));
    }
}
