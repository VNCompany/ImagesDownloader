namespace ImagesDownloader.Core.Models;

public class DManager : IDisposable
{
    private readonly Dictionary<DItemsCollection, CancellationTokenSource> _collections;

    private DManager(IEnumerable<DItemsCollection> collections)
    {
        _collections = new(collections.)
    }

    public void Stop()
    {

    }

    public void Dispose()
    {

    }

    private void Launch()
    {

    }

    public static DManager Start(
        IEnumerable<DItemsCollection> collections, 
        int collectionsPoolSize, 
        int itemsPoolSize)
}
