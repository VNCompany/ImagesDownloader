namespace ImagesDownloader.Core.Interfaces;

public interface IDownloader : IDisposable
{
    /// <summary>
    /// ��������� ������� ��� html
    /// </summary>
    /// <param name="uri">URL</param>
    /// <param name="cancellationToken">����� ������</param>
    /// <returns>HTML ������</returns>
    Task<string> GetHtml(Uri uri, CancellationToken cancellationToken);

    /// <summary>
    /// ��������� ������� �� <paramref name="uri"/> � ��������� ��� � <paramref name="outputPath"/>
    /// </summary>
    /// <param name="uri">URL</param>
    /// <param name="outputPath">���� ����������</param>
    /// <param name="cancellationToken">����� ������</param>
    /// <exception cref="SaveDataException"><see cref="SaveDataException.Type"/></exception>
    Task SaveData(Uri uri, string outputPath, CancellationToken cancellationToken);
}
