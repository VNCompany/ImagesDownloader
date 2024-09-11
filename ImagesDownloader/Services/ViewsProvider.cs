using System.Windows;

using ImagesDownloader.Interfaces;
using ImagesDownloader.Views;
using ImagesDownloader.Models;

namespace ImagesDownloader.Services;

internal class ViewsProvider
{
    private readonly Dictionary<Type, Type> _relations = new()
    {
        [typeof(IViewModel)] = typeof(MainWindow)
    };

    public IView<TViewModel> CreateView<TViewModel>() where TViewModel : class, IViewModel
    {
        var window = (Activator.CreateInstance(_relations[typeof(TViewModel)]) as Window)!;
        return new ViewWindow<TViewModel>(window);
    }
}
