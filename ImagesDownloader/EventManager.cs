namespace ImagesDownloader;

internal static class EventManager
{
    private static readonly Dictionary<object, List<EventHandler<ViewModelEventArgs>>> _handlers = [];

    public static void AddSource(object source)
    {
        _handlers.Add(source, new List<EventHandler<ViewModelEventArgs>>());
    }

    public static void RemoveSource(object source)
    {
        _handlers.Remove(source);
    }

    public static void AddHandler(object source, EventHandler<ViewModelEventArgs> handler)
    {
        _handlers[source].Add(handler);
    }

    public static void RemoveHandler(object source, EventHandler<ViewModelEventArgs> handler)
    {
        _handlers[source].Remove(handler);
    }

    public static void RaiseEvent(object source, ViewModelEventArgs args)
    {
        foreach (var handler in _handlers[source])
            handler.Invoke(source, args);
    }
}
