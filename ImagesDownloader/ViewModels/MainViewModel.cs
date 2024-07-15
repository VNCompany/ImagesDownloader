using System.Collections.ObjectModel;

using ImagesDownloader.Models;
using ImagesDownloader.Common;

namespace ImagesDownloader.ViewModels;

internal class MainViewModel : ViewModelBase
{
    private CancellationTokenSource? _tokenSource;
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
        vm.Dispose();
    }

    private async void Start_Execute(object? _)
    {
        _tokenSource = new CancellationTokenSource();
        using var collectionDownloader = new CollectionsDownloader(Services.Logger, 1, 2);
        await collectionDownloader.Start(Collections, _tokenSource.Token);
        _tokenSource.Dispose();
        _tokenSource = null;
    }

    public override void Dispose()
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        Services.History.Dispose();
        Services.Logger.Dispose();
        base.Dispose();
    }
}
