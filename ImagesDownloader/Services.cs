using ImagesDownloader.Common;
using ImagesDownloader.Interfaces;

namespace ImagesDownloader;

internal static class Services
{
    public static ILogger Logger => DebugLogger.Instance;

    public static readonly History History = History.Load(Logger);
}
