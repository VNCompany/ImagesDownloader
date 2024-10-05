using ImagesDownloader.Core.Internal;
using ImagesDownloader.Core.Interfaces;

namespace ImagesDownloader.Core.Factories;

public static class LoggerFactory
{
    public static readonly ILogger Logger = new LoggerConsole();
}
