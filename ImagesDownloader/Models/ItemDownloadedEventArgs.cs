namespace ImagesDownloader.Models;

internal class ItemDownloadedEventArgs(DownloadItem downloadedItem, bool isSuccess, Exception? exception) : EventArgs
{
    public DownloadItem DownloadedItem { get; } = downloadedItem;
    public bool IsSuccess { get; } = isSuccess;
    public Exception? Exception { get; } = exception;
}
