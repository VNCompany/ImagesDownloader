namespace ImagesDownloader.Models;

internal class UrlOptions
{
    public Uri Target { get; }
    public string? FileName { get; }
    public string Domain => Target.Host;

    public UrlOptions(Uri target, string? fileName)
    {
        Target = target;
        FileName = fileName;
    }

    public static UrlOptions Parse(Uri input)
    {
        var fileName = input.Segments.Reverse().SkipWhile(x => x.Trim(' ', '/') == string.Empty).FirstOrDefault();
        return new UrlOptions(input, fileName);
    }
}
