using ImagesDownloader.Interfaces;
using ImagesDownloader.Internal;

namespace ImagesDownloader.Infrastructure;

public static class LoggerFactory
{
    public static readonly ILogger Logger = new LoggerDebug();
}
