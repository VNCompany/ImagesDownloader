using System.IO;
using System.Text;

namespace ImagesDownloader.Models;

internal class DownloadItem
{
    public Uri Url { get; set; }
    public string OutputPath { get; set; }

    private DownloadItem(Uri url, string outputDir, string fileName)
    {
        Url = url;
        OutputPath = Path.Combine(outputDir, fileName);
    }

    public static DownloadItem Create(int number, Uri url, string outputDir, string fileNamePattern)
    {
        string urlName = url.Segments.Reverse().SkipWhile(x => x.Trim('/') == string.Empty).FirstOrDefault()
            ?? string.Empty;
        string name;
        string extension;

        int urlExtensionDotPosition = urlName.LastIndexOf('.');
        if (urlExtensionDotPosition == -1)
        {
            name = urlName;
            extension = string.Empty;
        }
        else
        {
            name = urlName.Substring(0, urlExtensionDotPosition);
            extension = urlName.Substring(urlExtensionDotPosition + 1);
        }

        string fileName = new StringBuilder(fileNamePattern)
            .Replace("$number", number.ToString())
            .Replace("$urlname", urlName)
            .Replace("$name", name)
            .Replace("$extension", extension)
            .ToString();

        return new DownloadItem(url, outputDir, fileName);
    }
}
