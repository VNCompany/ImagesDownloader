using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Models;

internal class DownloadItem(Uri source, string outputPath)
{
    public Uri Source { get; } = source;
    public string OutputPath { get; } = outputPath;
    public bool IsSuccess { get; set; }
}
