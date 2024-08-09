namespace ImagesDownloader.Services.Download;

internal class DownloadLogItem
{
    public Uri Url { get; set; }
    public bool IsSuccess { get; set; }

    public DownloadLogItem(Uri url, bool isSuccess)
    {
        Url = url;
        IsSuccess = isSuccess;
    }
}
