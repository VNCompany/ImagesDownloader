namespace ImagesDownloader.Interfaces;

internal interface ILogger
{
    void Info(string message, string? place = null);
    void Warn(string message, string? place = null);
    void Error(string message, string? place = null);
    void Exception(Exception ex, string? place = null);
}
