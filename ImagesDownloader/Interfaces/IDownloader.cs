using ImagesDownloader.Exceptions;

namespace ImagesDownloader.Interfaces;

public interface IDownloader : IDisposable
{
    Task<string> GetHtml(Uri uri, CancellationToken cancellationToken);

    /// <summary>
    /// Download content from <paramref name="uri"/> and saves to <paramref name="outputPath"/>
    /// </summary>
    /// <param name="uri">Source uri</param>
    /// <param name="outputPath">Output path</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="SaveDataException"><see cref="SaveDataException.Type"/></exception>
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
