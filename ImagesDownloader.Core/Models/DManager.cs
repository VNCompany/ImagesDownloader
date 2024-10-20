namespace ImagesDownloader.Core.Models;

public class DManager(int collectionsPoolSize, int itemsPoolSize)
{
    private readonly Dictionary<DItemsCollection, CancellationTokenSource> _collections = [];

    private Task? _currentTask;
    private CancellationTokenSource? _tokenSource;

    public bool IsRunning { get; private set; }

    public void Start(IEnumerable<DItemsCollection> items)
    {
        if (IsRunning) 
            throw new InvalidOperationException("Failed to start. Downloading already started");

        _currentTask = StartTasks(items);
    }

    public void Wait() => _currentTask?.Wait();

    public void Stop() => _tokenSource?.Cancel();

    private async Task StartTasks(IEnumerable<DItemsCollection> collections)
    {
        var tasks = new List<Task>();

        _tokenSource = new CancellationTokenSource();
        using var semaphore = new SemaphoreSlim(collectionsPoolSize);
        foreach (var collection in collections)
        {
            var cts = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
            _collections[collection] = cts;
            tasks.Add(collection.Download(semaphore, itemsPoolSize, cts.Token));
        }

        IsRunning = true;
        Console.WriteLine("IsRunning is true");
        await Task.WhenAll(tasks);
        Console.WriteLine("Task is completed");

        Clear();
        Console.WriteLine("Clear()");
        IsRunning = false;
        Console.WriteLine("IsRunning is false");
    }

    private void Clear()
    {
        foreach (var cancellationTokenSource in _collections.Values)
            cancellationTokenSource.Dispose();
        _collections.Clear();

        _tokenSource?.Dispose();
        _tokenSource = null;
    }
}
