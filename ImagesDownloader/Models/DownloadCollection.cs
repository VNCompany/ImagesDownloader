using System.ComponentModel;
using System.Collections.ObjectModel;

namespace ImagesDownloader.Models;

internal class DownloadCollection : INotifyPropertyChanged
{
    private readonly object _locker = new object();
    private readonly int _totalItemsCount;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Uri Url { get; set; }
    public Queue<DownloadItem> Queue { get; set; }
    public ObservableCollection<LogItem> Logs { get; set; }

    public double DownloadPercent => (double)Logs.Count / (double)_totalItemsCount * 100.0;

    public DownloadCollection(Uri url, string outputDir, string fileNameFormat, IEnumerable<Uri> queue)
    {
        Url = url;
        Logs = new ObservableCollection<LogItem>();

        int counter = 1;
        Queue = new Queue<DownloadItem>(from x in queue 
                                        select DownloadItem.Create(counter++, x, outputDir, fileNameFormat));
        _totalItemsCount = Queue.Count;
    }

    public void OnItemDownloaded(DownloadItem item, bool isSuccess)
    {
        lock (_locker)
        {
            Logs.Insert(0, new LogItem(item.Url, isSuccess));
            OnPropertyChanged(nameof(DownloadPercent));
        }
    }

    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
