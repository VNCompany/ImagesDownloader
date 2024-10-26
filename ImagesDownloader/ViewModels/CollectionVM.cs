using System.Collections.ObjectModel;

using ImagesDownloader.Core.Extensions;
using ImagesDownloader.Core.Interfaces;
using ImagesDownloader.Core.Services;
using ImagesDownloader.Models;

namespace ImagesDownloader.ViewModels;

internal class CollectionVM : ViewModelBase
{
    private readonly ILogger _logger = ServiceAccessor.GetRequiredService<ILogger>();
    private readonly AppConfig _appConfig = ServiceAccessor.GetRequiredService<AppConfig>();
    private readonly HtmlParserVM _htmlParserVM = new();

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
            _logger.Info("NAME CHANGED");
        }
    }

    public ObservableCollection<Uri> Items { get; } = [];

    public ObservableCollection<string> FailedItems { get; } = [];

    private string _savePath;
    public string SavePath
    {
        get => _savePath;
        set
        {
            SetProperty(ref _savePath, value);
            Ok.OnCanExecuteChanged();
            _logger.Info("SAVE PATH CHANGED");
        }
    }
    #endregion

    #region Commands

    private RelayCommand? _htmlParse;
    public RelayCommand HtmlParse => _htmlParse ??= new RelayCommand(HtmlParse_Execute);

    private void HtmlParse_Execute(object? parameter)
    {
        bool isFastFilling = parameter != null && (bool)parameter;

        if (!isFastFilling && ViewProvider.GetWindow(_htmlParserVM).ShowDialog() != true || !_htmlParserVM.IsValid)
            return;

        List<string>? rawElements = _htmlParserVM.ParseElements();
        if (rawElements == null)
            return;

        Items.Clear();
        FailedItems.Clear();
        foreach (var raw in rawElements)
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

    public CollectionVM()
    {
        _savePath = _appConfig.SavePathSuggest;
        _htmlParserVM.XPath = _appConfig.XPathLastSuggest;
        _htmlParserVM.XPathList = _appConfig.XPathSuggestsList;

        Items.CollectionChanged += (s, e) => Ok.OnCanExecuteChanged();
    }

    public async Task TrySetHtml(Uri? uri)
    {
        if (uri == null) return;

        try
        {
            var downloader = ServiceAccessor.GetRequiredService<IDownloader>();
            _htmlParserVM.Html = await downloader.GetHtml(uri, CancellationToken.None);
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
        _appConfig.AddXPathSuggest(_htmlParserVM.XPath);
    }
}
