namespace ImagesDownloader.Exceptions;

public enum SaveDataExceptionType
{
    TaskCanceled,
    Other
}

public class SaveDataException(SaveDataExceptionType type, string? message, Exception? innerException = null) 
    : Exception(message, innerException)
{
    public SaveDataExceptionType Type { get; } = type;
}
