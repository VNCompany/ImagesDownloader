using ImagesDownloader.ViewModels;

namespace ImagesDownloader.Interfaces;

internal interface IWindow<TViewModel> where TViewModel : ViewModelBase
{
    TViewModel? ViewModel { get; }
    void Show();
    bool? ShowDialog();
    void Close();
}
