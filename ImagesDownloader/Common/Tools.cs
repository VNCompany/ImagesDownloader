namespace ImagesDownloader.Common;

internal static class Tools
{
    public static string SuggestDirectoryNameByUrl(Uri url)
    {
        string? fromSegments = url.Segments.Select(x => x.Trim('/'))
                                           .Where(x => x != string.Empty)
                                           .LastOrDefault();

        return fromSegments ?? url.Host;
    }
}
