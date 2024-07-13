namespace ImagesDownloader;

internal class ViewModelEventArgs : EventArgs
{
    public int EventId { get; set; }
    public object? Parameter { get; set; }

    public ViewModelEventArgs(int eventId, object? parameter)
    {
        EventId = eventId;
        Parameter = parameter;
    }
}
