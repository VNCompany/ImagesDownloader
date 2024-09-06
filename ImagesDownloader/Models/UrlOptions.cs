namespace ImagesDownloader.Models;

internal class UrlOptions(Uri target, string? fileName)
{
    public Uri Target { get; } = target;
    public string? FileName { get; } = fileName;
    public string Domain => Target.Host;

    public static UrlOptions Parse(Uri input)
    {
        var fileName = input.Segments.Reverse().SkipWhile(x => x.Trim(' ', '/') == string.Empty).FirstOrDefault();
        return new UrlOptions(input, fileName);
    }
}
