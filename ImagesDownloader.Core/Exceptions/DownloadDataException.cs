namespace ImagesDownloader.Core.Exceptions;

public class DownloadDataException(bool isTaskCanceled, string? message, Exception? innerException = null) 
    : Exception(message, innerException)
{
    public bool IsTaskCanceled { get; } = isTaskCanceled;
}
