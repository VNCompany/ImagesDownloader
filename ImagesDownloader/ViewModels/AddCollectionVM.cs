namespace ImagesDownloader.ViewModels;

internal class AddCollectionVM : ViewModelBase
{
    #region Properties

    private Uri? _url;
    public Uri? Url
    {
        get => _url;
        set => SetProperty(ref _url, value);
    }

    private string _html = string.Empty;
    public string Html
    {
        get => _html;
        set
        {
            SetProperty(ref _html, value);
            Parse.OnCanExecuteChanged();
        }
    }

    private string _xPath = string.Empty;
    public string XPath
    {
        get => _xPath;
        set
        {
            SetProperty(ref _xPath, value);
            Parse.OnCanExecuteChanged();
        }
    }

    private ICollection<string> _xPathHistory = [];
    public ICollection<string> XPathHistory => _xPathHistory;

    #endregion

    #region Commands

    private RelayCommand? _parse;
    public RelayCommand Parse => _parse ??= new RelayCommand(
        (obj) => { },
        CanExecuteParse);

    #endregion

    private bool CanExecuteParse(object? _)
        => !string.IsNullOrWhiteSpace(_html)
        && !string.IsNullOrWhiteSpace(_xPath);
}
