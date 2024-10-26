namespace ImagesDownloader.ViewModels;

internal class HtmlParserVM : ViewModelBase
{
    #region Properties
    private string _html = string.Empty;
    public string Html
    {
        get => _html;
        set
        {
            SetProperty(ref _html, value);
            OnPropertyChanged(nameof(IsValid));
        }
    }

    public List<string>? XPathList { get; set; }

    private string _xPath = "//article//img/@src";
    public string XPath
    {
        get => _xPath;
        set
        {
            SetProperty(ref _xPath, value);
            OnPropertyChanged(nameof(IsValid));
        }
    }

    public bool IsValid => !string.IsNullOrWhiteSpace(Html) && !string.IsNullOrWhiteSpace(XPath);
    #endregion

    public List<string>? ParseElements()
    {
        try
        {
            var parser = new HtmlParser(Html);
            return parser.Parse(XPath).ToList();
        }
        catch (Exception ex)
        {
            ViewProvider.ShowMessage(ex.GetType().Name, ex.Message);
            return null;
        }
    }
}
