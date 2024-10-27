using ImagesDownloader.Core.Services;

namespace ImagesDownloader.ViewModels;

internal class HtmlParserViewModel : ViewModelBase, ICloneable
{
    private readonly AppConfig _appConfig = ServiceAccessor.GetRequiredService<AppConfig>();

    #region Properties
    private string _html = string.Empty;
    public string Html
    {
        get => _html;
        set
        {
            SetProperty(ref _html, value);
            Ok.OnCanExecuteChanged();
        }
    }

    public List<string>? XPathList { get; }

    private string _xPath;
    public string XPath
    {
        get => _xPath;
        set
        {
            SetProperty(ref _xPath, value);
            Ok.OnCanExecuteChanged();
        }
    }
    public List<string>? Result { get; private set; }
    #endregion

    #region Commands
    private RelayCommand? _ok;
    public RelayCommand Ok => _ok ??= new RelayCommand(
        Ok_Execute, 
        canExecute: _ => !string.IsNullOrWhiteSpace(Html) && !string.IsNullOrWhiteSpace(XPath));
    private void Ok_Execute(object? _)
    {
        try
        {
            var parser = new HtmlParser(Html);
            Result = parser.Parse(XPath).ToList();
            UpdateSuggests();
        }
        catch (Exception ex)
        {
            ViewProvider.ShowMessage(ex.GetType().Name, ex.Message);
        }
    }
    #endregion

    public HtmlParserViewModel()
    {
        XPathList = _appConfig.XPathSuggestsList;
        _xPath = _appConfig.XPathLastSuggest;
    }

    public HtmlParserViewModel Clone() => new() { _html = _html, _xPath = _xPath };
    object ICloneable.Clone() => Clone();

    private void UpdateSuggests()
    {
        _appConfig.AddXPathSuggest(XPath);
    }
}
