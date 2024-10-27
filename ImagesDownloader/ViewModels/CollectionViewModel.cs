using System.Collections.ObjectModel;

using ImagesDownloader.Core.Extensions;
using ImagesDownloader.Core.Interfaces;
using ImagesDownloader.Core.Services;
using ImagesDownloader.Models;

namespace ImagesDownloader.ViewModels;

internal class CollectionViewModel : ViewModelBase
{
    private readonly ILogger _logger = ServiceAccessor.GetRequiredService<ILogger>();
    private readonly AppConfig _appConfig = ServiceAccessor.GetRequiredService<AppConfig>();

    private HtmlParserViewModel _htmlParserViewModel = new();

    #region Properties
    private Uri? _url;
    public Uri? Url
    {
        get => _url;
        set
        {
            SetProperty(ref _url, value);
            TrySetName(value);
        }
    }

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            SetProperty(ref _name, value);
            Ok.OnCanExecuteChanged();
        }
    }

    public ObservableCollection<Uri> Items { get; } = [];

    public ObservableCollection<string> FailedItems { get; } = [];

    private string _savePath;
    public string SavePath  // TODO: Make UpdatePropertySource=PropertyChanged
    {
        get => _savePath;
        set
        {
            SetProperty(ref _savePath, value);
            Ok.OnCanExecuteChanged();
        }
    }
    #endregion

    #region Commands
    private RelayCommand? _loadHtml;
    public RelayCommand LoadHtml => _loadHtml ??= new RelayCommand(LoadHtml_Execute);
    private async void LoadHtml_Execute(object? parameter)
    {
        bool isControlPressed = (bool)parameter!;

        if (Url == null)
            return;

        await TrySetHtml(Url);
        HtmlParse.Execute(isControlPressed);
    }

    private RelayCommand? _htmlParse;
    public RelayCommand HtmlParse => _htmlParse ??= new RelayCommand(HtmlParse_Execute);
    private void HtmlParse_Execute(object? parameter)
    {
        bool noShowWindow = (parameter as bool?) ?? false;

        if (!noShowWindow)
        {
            var tempViewModel = _htmlParserViewModel.Clone();
            if (ViewProvider.GetWindow(tempViewModel).ShowDialog() != true)
                return;
            _htmlParserViewModel = tempViewModel;
        }
        else
        {
            if (!_htmlParserViewModel.Ok.CanExecute(null))
                return;
            _htmlParserViewModel.Ok.Execute(null);
        }

        Items.Clear();
        FailedItems.Clear();

        if (_htmlParserViewModel.Result == null)
            return;

        foreach (var raw in _htmlParserViewModel.Result)
        {
            var uri = BuildItemUrl(raw);
            if (uri != null)
                Items.Add(uri);
            else
                FailedItems.Add(raw);
        }
    }

    private RelayCommand? _ok;
    public RelayCommand Ok => _ok ??= new RelayCommand(Ok_Execute, Ok_CanExecute);
    private bool Ok_CanExecute(object? _) =>
        !string.IsNullOrWhiteSpace(Name)  // TODO: Check file forbidden chars
        && !string.IsNullOrWhiteSpace(SavePath)  // TODO: Check path exists
        && Items.Count > 0;

    private void Ok_Execute(object? _)
    {
        UpdateSuggests();
    }
    #endregion

    public CollectionViewModel()
    {
        _savePath = _appConfig.SavePathSuggest;

        Items.CollectionChanged += (s, e) => Ok.OnCanExecuteChanged();
    }

    public async Task TrySetHtml(Uri? uri)
    {
        if (uri == null) return;

        try
        {
            var downloader = ServiceAccessor.GetRequiredService<IDownloader>();
            _htmlParserViewModel.Html = await downloader.GetHtml(uri, CancellationToken.None);
        }
        catch (Exception ex)
        {
            ViewProvider.ShowMessage(ex.GetType().Name, ex.Message);
            _logger.Error(ex, "CollectionVM.cs:63 > HtmlParse_Execute");
        }
    }

    private void TrySetName(Uri? uri)
    {
        if (uri == null) return;

        var urlOptions = UrlOptions.Parse(uri);
        Name = urlOptions.FileName ?? urlOptions.Domain;
    }

    private Uri? BuildItemUrl(string input)
    {
        if (!Uri.TryCreate(input, UriKind.RelativeOrAbsolute, out var resultUri))
            return null;

        if (!resultUri.IsAbsoluteUri && Url != null)
            return new Uri(Url, resultUri);
        else if (resultUri.IsAbsoluteUri)
            return resultUri;
        return null;
    }

    private void UpdateSuggests()
    {
        _appConfig.SavePathSuggest = SavePath;
    }
}
