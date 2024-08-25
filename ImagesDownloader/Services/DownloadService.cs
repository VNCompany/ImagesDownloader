using System.Collections.ObjectModel;

using ImagesDownloader.Models;
using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Services;

internal class DownloadService : IDisposable
{
    private readonly ILogger _logger;
    private readonly AppSettings _appSettings;

    public DownloadService(ILogger logger, AppSettings appSettings)
    {
        _logger = logger;
        _appSettings = appSettings;
    }

    public void Dispose() { }
}
