namespace ImagesDownloader.Core.Models;

public class DManager(int collectionsPoolSize, int itemsPoolSize) : IDisposable
{
    private readonly Dictionary<DItemsCollection, CancellationTokenSource> _collections = [];
    private readonly List<Task> _tasks = [];
    private readonly SemaphoreSlim _semaphore = new(collectionsPoolSize);

    private CancellationTokenSource? _tokenSource;

    public bool IsRunning { get; private set; }

    public void Start(IEnumerable<DItemsCollection> items)
    {
        if (IsRunning) 
            throw new InvalidOperationException("Failed to start. Downloading already started");

        _tokenSource = new CancellationTokenSource();

        foreach (var collection in items)
        {
            var collectionCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                _tokenSource.Token);
            _collections[collection] = collectionCancellationTokenSource;
            _tasks.Add(collection.Download(_semaphore, itemsPoolSize, collectionCancellationTokenSource.Token));
        }

        IsRunning = true;
    }

    public void Stop()
    {
        if (_tokenSource != null)
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();
            _tokenSource = null;
        }

        if (_tasks.Count > 0)
        {
            Task.WhenAll(_tasks).Wait();
            _tasks.Clear();
        }

        if (_collections.Count > 0)
        {
            foreach (var cancellationTokenSource in _collections.Values)
                cancellationTokenSource.Dispose();
            _collections.Clear();
        }

        IsRunning = false;
    }

    public void Dispose()
    {
        Stop();
        _semaphore.Dispose();
    }
}
