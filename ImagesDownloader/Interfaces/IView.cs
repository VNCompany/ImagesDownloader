namespace ImagesDownloader.Interfaces;

internal interface IView<TViewModel> : IDisposable where TViewModel : IViewModel
{
    TViewModel Context { get; }

    void Show();
    bool? ShowDialog();
    void Close();
}
