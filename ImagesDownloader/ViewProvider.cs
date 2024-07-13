using System.Windows;
using ImagesDownloader.Interfaces;
using ImagesDownloader.ViewModels;

namespace ImagesDownloader;

internal static class ViewProvider
{
    private class WindowWrapper<TViewModel> : IWindow<TViewModel> where TViewModel : ViewModelBase
    {
        private readonly Window _window;

        public TViewModel? ViewModel { get; }

        public WindowWrapper(Window window)
        {
            _window = window;
            ViewModel = _window.DataContext as TViewModel;
        }

        public void Show() => _window.Show();
        public bool? ShowDialog() => _window.ShowDialog();
        public void Close() => _window.Close();
    }

    public static IWindow<TViewModel> CreateWindow<TViewModel>() where TViewModel : ViewModelBase
    {
        switch (typeof(TViewModel).Name)
        {
            case nameof(MainViewModel):
                return new WindowWrapper<TViewModel>(new Views.MainWindow());

            case nameof(CollectionViewModel):
                return new WindowWrapper<TViewModel>(new Views.CollectionWindow());

            default:
                throw new InvalidOperationException("Unknown ViewModel");
        }
    }
}
