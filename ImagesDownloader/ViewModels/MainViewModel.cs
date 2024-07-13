using System.Collections.ObjectModel;

using ImagesDownloader.Models;
using ImagesDownloader.Common;

namespace ImagesDownloader.ViewModels;

internal class MainViewModel : ViewModelBase
{
    public ObservableCollection<DownloadCollection> Collections { get; } = [];

    private DownloadCollection? selectedCollectionItem;
    public DownloadCollection? SelectedCollectionItem
    {
        get => selectedCollectionItem;
        set => SetProperty(ref selectedCollectionItem, value);
    }

    private RelayCommand? _add;
    public RelayCommand Add => _add ??= new RelayCommand(Add_Execute);

    private RelayCommand? _start;
    public RelayCommand Start => _start ??= new RelayCommand(
        Start_Execute,
        _ => Collections.Select(x => x.Queue.Count).Sum() > 0);

    private void Add_Execute(object? _)
    {
        var window = ViewProvider.CreateWindow<CollectionViewModel>();
        var vm = window.ViewModel!;
        vm.CloseActionCallback = () =>
        {
            Collections.Add(new DownloadCollection(vm.Url, vm.SavePath, vm.NamePattern, vm.Items));
            window.Close();
            Start.OnCanExecuteChanged();
        };

        window.ShowDialog();
    }

    private async void Start_Execute(object? _)
    {
        using (var collectionDownloader = new CollectionsDownloader(DebugLogger.Instance, 1, 2))
            await collectionDownloader.Start(Collections, default);
    }
}
