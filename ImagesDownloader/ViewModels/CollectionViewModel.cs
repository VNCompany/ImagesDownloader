﻿using System.IO;
using System.Windows;

using ImagesDownloader.Common;

namespace ImagesDownloader.ViewModels;

internal class CollectionViewModel : ViewModelBase
{
    private readonly DownloadClient _downloadClient = new(DebugLogger.Instance);

    private Uri _url = new Uri("http://localhost");

    public Uri Url => _url;

    public Action? CloseActionCallback { get; set; }

    private string _urlString = string.Empty;
    public string UrlString
    {
        get => _urlString;
        set => SetProperty(ref _urlString, value);
    }

    private string _html = string.Empty;
    public string Html
    {
        get => _html;
        set
        {
            SetProperty(ref _html, value);
            ParseHtml.OnCanExecuteChanged();
        }
    }

    private string _xPath = string.Empty;
    public string XPath
    {
        get => _xPath;
        set
        {
            SetProperty(ref _xPath, value);
            ParseHtml.OnCanExecuteChanged();
        }
    }

    private List<Uri> _items = [];
    public List<Uri> Items
    {
        get => _items;
        set
        {
            SetProperty(ref _items, value);
            Add.OnCanExecuteChanged();
        }
    }

    private string _savePath = string.Empty;
    public string SavePath
    {
        get => _savePath;
        set
        {
            SetProperty(ref _savePath, value);
            Add.OnCanExecuteChanged();
        }
    }

    private string _namePattern = string.Empty;
    public string NamePattern
    {
        get => _namePattern;
        set
        {
            SetProperty(ref _namePattern, value);
            Add.OnCanExecuteChanged();
        }
    }


    private RelayCommand? _analyze;
    public RelayCommand Analyze => _analyze ??= new RelayCommand(Analyze_Execute);

    private RelayCommand? _parseHtml;
    public RelayCommand ParseHtml => _parseHtml ??= new RelayCommand(ParseHtml_Execute, 
        canExecute: _ => !string.IsNullOrWhiteSpace(Html) && !string.IsNullOrWhiteSpace(XPath));

    private RelayCommand? _selectDirectory;
    public RelayCommand SelectDirectory => _selectDirectory ??= new RelayCommand(SelectDirectory_Execute);

    private RelayCommand? _add;
    public RelayCommand Add => _add ??= new RelayCommand(Add_Execute,
        canExecute: _ => Items.Count > 0 
            && !string.IsNullOrWhiteSpace(SavePath) 
            && !string.IsNullOrWhiteSpace(NamePattern));

    public CollectionViewModel()
    {
        EventManager.AddSource(this);
    }

    private void Analyze_Execute(object? param)
    {
        bool isAutoPaste = (bool)param!;
        if (!isAutoPaste)
        {
            if (ValidateUrlString(UrlString, showMessage: true))
                GetHtml();
            return;
        }
    }

    private void ParseHtml_Execute(object? _)
    {
        var parser = HtmlParser.TryCreate(Html);
        if (parser == null)
        {
            MessageBox.Show("Invalid HTML");
            return;
        }

        var links = new List<Uri>();
        var failedLinks = new List<string>();
        foreach (var item in parser.Parse(XPath))
        {
            Uri? link = TryCreateItemUri(item);
            if (link != null)
                links.Add(link);
            else
                failedLinks.Add(item);
        }

        Items = links;
        if (failedLinks.Count > 0)
        {
            Clipboard.SetText(string.Join(Environment.NewLine, failedLinks));
            MessageBox.Show($"Failed parsed links ({failedLinks.Count}) copied to clipboard");
        }
    }

    private void SelectDirectory_Execute(object? _)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog();
        bool? dialogResult;
        try
        {
            if (!string.IsNullOrWhiteSpace(SavePath))
                dialog.InitialDirectory = SavePath;
            dialogResult = dialog.ShowDialog();
        }
        catch (Exception)
        {
            dialog.InitialDirectory = string.Empty;
            dialogResult = dialog.ShowDialog();
        }

        if (dialogResult == true)
            SavePath = dialog.FolderName;
    }

    private void Add_Execute(object? _)
    {
        try
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);
        }
        catch (Exception)
        {
            MessageBox.Show("Invalid save path");
            return;
        }

        if (Path.GetInvalidFileNameChars().Any(NamePattern.Contains))
        {
            MessageBox.Show("Invalid name pattern");
            return;
        }

        CloseActionCallback?.Invoke();
    }

    private Uri? TryCreateItemUri(string input)
    {
        Uri? result;
        if (input.StartsWith("http") 
            && Uri.TryCreate(input, UriKind.Absolute, out result)
            || Uri.TryCreate(_url, input, out result))
            return result;
        return null;
    }

    private bool ValidateUrlString(string newValue, bool showMessage)
    {
        if (!string.IsNullOrWhiteSpace(newValue)
            && Uri.TryCreate(newValue, UriKind.Absolute, out Uri? uriResult))
        {
            _url = uriResult;
            SavePath = Path.Combine(
                !string.IsNullOrWhiteSpace(SavePath) ? SavePath : Environment.CurrentDirectory,
                Tools.SuggestDirectoryNameByUrl(_url));
            return true;
        }
        else if (showMessage)
            MessageBox.Show($"Invalid URL: {newValue}");

        return false;
    }

    private async void GetHtml()
    {
        try
        {
            Html = await _downloadClient.DownloadHtml(_url);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        EventManager.RemoveSource(this);
        _downloadClient.Dispose();
    }
}
