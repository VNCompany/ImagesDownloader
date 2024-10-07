using System.ComponentModel;
using System.Collections.ObjectModel;

using ImagesDownloader.Core.Factories;
using ImagesDownloader.Core.Interfaces;
using ImagesDownloader.Core.Extensions;

namespace ImagesDownloader.Core.Models;

public class DItemsCollection(string name, IEnumerable<DItem> items) : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private readonly ILogger _logger = LoggerFactory.Logger;

    public string Name { get; set; } = name;
    public ICollection<DItem> Items { get; set; } = (items as ICollection<DItem>) ?? items.ToArray();
    public ObservableCollection<DItem> DownloadedItems { get; set; } = [];

    public double ProgressPercent => (double)DownloadedItems.Count / Items.Count * 100;

    public async Task Download(SemaphoreSlim semaphore, int poolSize, CancellationToken cancellationToken)
    {
        try
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                using var itemsSemaphore = new SemaphoreSlim(poolSize);
                using var downloader = DownloaderFactory.CreateDownloader();
                var tasks = new List<Task>();
                foreach (var item in Items.Where(x => x.IsSuccess == null))
                    tasks.Add(item.Download(downloader, itemsSemaphore, cancellationToken, callback: OnItemDownloaded));
                await Task.WhenAll(tasks);
            }
            finally
            {
                semaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
            //_logger.Warn("Download collection \"{0}\" has been canceled", Name);
        }
    }

    private void OnItemDownloaded(DItemDownloadedArgs args)
    {
        DownloadedItems.Add(args.Item);
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressPercent)));

        if (args.Item.IsSuccess == false && args.Exception != null)
            _logger.Error(args.Exception, args.Item);
    }
}
