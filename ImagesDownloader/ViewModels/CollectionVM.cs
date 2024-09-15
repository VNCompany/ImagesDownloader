using System.Windows;
using System.Collections.ObjectModel;

using ImagesDownloader.Models;

namespace ImagesDownloader.ViewModels;

internal class CollectionVM : ViewModelBase
{
    #region Properties

    public bool? DialogResult { get; set; }

    private Uri? _source;
    public Uri? Source
    {
        get => _source;
        set
        {
            _source = value;
            TrySetCollectionNameFromSource();
        }
    }

    private string _collectionName = string.Empty;
    public string CollectionName
    {
        get => _collectionName;
        set => SetProperty(ref _collectionName, value);
    }

    private string _savePath = string.Empty;
    public string SavePath
    {
        get => _savePath;
        set => SetProperty(ref _savePath, value);
    }

    private List<Uri> _items = [];
    public List<Uri> Items
    {
        get => _items;
        set => SetProperty(ref _items, value);
    }

    private ObservableCollection<string> _failedItems = [];
    public ObservableCollection<string> FailedItems
    {
        get => _failedItems;
        set => SetProperty(ref _failedItems, value);
    }

    private int _poolSize = 1;
    public int PoolSize
    {
        get => _poolSize;
        set => SetProperty(ref _poolSize, value);
    }

    private int _sleepTime = 0;
    public int SleepTime
    {
        get => _sleepTime;
        set => SetProperty(ref _sleepTime, value);
    }

    private string _nameFormat = "[N]";
    public string NameFormat
    {
        get => _nameFormat;
        set => SetProperty(ref _nameFormat, value);
    }

    #endregion

    #region Commands

    private RelayCommand? _copyToClipboardFailedItems;
    public RelayCommand CopyToClipBoardFailedItems => _copyToClipboardFailedItems
        ??= new RelayCommand(CopyToClipBoardFailedItems_Execute);

    #endregion

    #region Handlers

    private void CopyToClipBoardFailedItems_Execute(object? _)
    {
        if (FailedItems.Count == 0) return;
        Clipboard.SetText(string.Join(Environment.NewLine, FailedItems));
    }

    #endregion

    public void ParseLinks(IEnumerable<string> links)
    {
        foreach (var link in links)
        {
            if (Uri.TryCreate(link, UriKind.RelativeOrAbsolute, out var url))
            {
                if (url.IsAbsoluteUri)
                {
                    Items.Add(url);
                    continue;
                }
                else if (Source != null)
                {
                    Items.Add(new Uri(Source, url));
                    continue;
                }
            }
            FailedItems.Add(link);
        }
    }

    private void TrySetCollectionNameFromSource()
    {
        if (_source == null) return;
        var urlOpts = UrlOptions.Parse(_source);
        CollectionName = urlOpts.FileName ?? urlOpts.Domain;
    }
}
