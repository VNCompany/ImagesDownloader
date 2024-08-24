namespace ImagesDownloader.Common;

public static class TaskPool
{
    public static async Task Run(
        IEnumerable<Func<Task>> tasks, 
        int poolSize, 
        CancellationToken cancellationToken)
    {
        using var semaphore = new SemaphoreSlim(poolSize);
        var taskList = new List<Task>();
        foreach (Func<Task> func in tasks)
        {
            cancellationToken.ThrowIfCancellationRequested();
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
