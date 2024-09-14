using ImagesDownloader.Models;

namespace ImagesDownloader.Infrastructure.Downloading;

internal class DoInfo(DownloadItemCollection collection, DownloadItem item)
{
    public DownloadItemCollection Collection { get; } = collection;
    public DownloadItem Item { get; } = item;

    public Exception? Exception { get; set; }
    public bool IsSuccess => Exception == null;
}
