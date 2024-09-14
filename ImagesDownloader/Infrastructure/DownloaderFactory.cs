using ImagesDownloader.Interfaces;
using ImagesDownloader.Internal;

namespace ImagesDownloader.Infrastructure;

public static class DownloaderFactory
{
    public static IDownloader CreateDownloader() => new DownloadClientTest();
}
