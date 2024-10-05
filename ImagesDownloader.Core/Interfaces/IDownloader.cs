namespace ImagesDownloader.Core.Interfaces;

public interface IDownloader : IDisposable
{
    /// <summary>
    /// Загрузить контент как html
    /// </summary>
    /// <param name="uri">URL</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <returns>HTML строка</returns>
    Task<string> GetHtml(Uri uri, CancellationToken cancellationToken);

    /// <summary>
    /// Загружает контент из <paramref name="uri"/> и сохраняет его в <paramref name="outputPath"/>
    /// </summary>
    /// <param name="uri">URL</param>
    /// <param name="outputPath">Путь сохранения</param>
    /// <param name="cancellationToken">Токен отмены</param>
    /// <exception cref="SaveDataException"><see cref="SaveDataException.Type"/></exception>
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
