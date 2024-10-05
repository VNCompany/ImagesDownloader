using ImagesDownloader.Core.Internal;
using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core.Factories;

public static class DownloaderFactory
{
    public static IDownloader CreateDownloader() => new DownloadClientTest();
}
