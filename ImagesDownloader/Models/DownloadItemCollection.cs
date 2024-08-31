using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

using ImagesDownloader.Common;
using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Models;

internal sealed class DownloadItemCollection : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<ItemDownloadedEventArgs>? ItemDownloaded;

    public DownloadItem[] Items { get; }
    /// <summary>
    /// Item1 is Source URL, 
    /// Item2 is success boolean status
    /// </summary>
    public ObservableCollection<Tuple<string, bool>> Logs { get; } = [];
    public double Percent => (double)Logs.Count / Items.Length * 100.0;

    public DownloadItemCollection(IEnumerable<DownloadItem> items)
    {
        Items = items.ToArray();
        Logs.CollectionChanged += Logs_CollectionChanged;
    }

    public async Task Download(IDownloadClient client, int poolSize, CancellationToken cancellationToken)
    {
        await TaskPool.Run(
            Items.Where(x => !x.IsCompleted).Select(x => (Func<Task>)(async () =>
            {
                Exception? exception = null;
                try
                {
                    await x.Download(client, cancellationToken);
                }
                catch (OperationCanceledException) when (x.IsCancelled)
                {
                    return;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
                OnItemDownloaded(new ItemDownloadedEventArgs(x, exception == null, exception));
            })), poolSize, cancellationToken);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void OnItemDownloaded(ItemDownloadedEventArgs e)
    {
        lock (this) Logs.Add(new(e.DownloadedItem.Source.ToString(), e.IsSuccess));
        ItemDownloaded?.Invoke(this, e);
    }

    private void Logs_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Percent));
    }
}
