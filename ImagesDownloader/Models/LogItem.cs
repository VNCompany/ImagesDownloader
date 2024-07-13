namespace ImagesDownloader.Models;

internal class LogItem
{
    public Uri Url { get; set; }
    public bool IsSuccess { get; set; }

    public LogItem(Uri url, bool isSuccess)
    {
        Url = url;
        IsSuccess = isSuccess;
    }
}
