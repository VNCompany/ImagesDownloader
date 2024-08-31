namespace ImagesDownloader.Common;

public sealed class TaskPool<T>(Dictionary<T, (Task, CancellationTokenSource)> items) : IDisposable
{
    public async Task Cancel(T item)
    {
        var (task, tokenSource) = items[item];
        tokenSource.Cancel();
        await task;
    }

    public async Task CancelAll()
    {
        foreach (var item in items.Values) item.Item2.Cancel();
        await WaitAll();
    }

    public async Task WaitAll() => await Task.WhenAll(items.Values.Select(t => t.Item1));

    public void Dispose()
    {
        CancelAll().Wait();
        foreach (var item in items.Values)
            item.Item2.Dispose();
    }
}

public sealed class TaskPool
{
    public static TaskPool<T> Run<T>(
        IEnumerable<T> collection, 
        Func<T, Task> executor, 
        int poolSize,
        CancellationToken cancellationToken) where T : class
    {
        using var semaphore = new SemaphoreSlim(poolSize);
        var items = new Dictionary<T, (Task, CancellationTokenSource)>();
        foreach (var obj in collection)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            
        }
    }

    public static async Task RunOld(
    IEnumerable<Func<Task>> tasks,
    int poolSize,
    CancellationToken cancellationToken)
    {
        using var semaphore = new SemaphoreSlim(poolSize);
        var taskList = new List<Task>();
        foreach (Func<Task> func in tasks)
        {
            if (cancellationToken.IsCancellationRequested)
                break;
            taskList.Add(Task.Run(async () =>
            {
                await semaphore.WaitAsync(cancellationToken);
                await func.Invoke();
                semaphore.Release();
            }, cancellationToken));
        }

        await Task.WhenAll(taskList);
    }
}