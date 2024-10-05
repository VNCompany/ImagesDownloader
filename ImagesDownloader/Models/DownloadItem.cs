namespace ImagesDownloader.Models;

public class DownloadItem(Uri source, string outputPath)
{
    public Uri Source { get; } = source;
    public string OutputPath { get; } = outputPath;
    public bool IsSuccess { get; set; }

    public override string ToString() => $"{{Source='{Source}', OutputPath='{OutputPath}'}}";
}
