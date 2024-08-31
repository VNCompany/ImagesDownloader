using System.Windows;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.Common;

internal class ViewWindow<TViewModel>(Window win) : IView<TViewModel> where TViewModel : class, IViewModel
{
    public TViewModel Context { get; } = (win.DataContext as TViewModel) 
        ?? throw new ArgumentException(nameof(win.DataContext));

    public void Close() => win.Close();

    public void Dispose() => Context.Dispose();

    public void Show() => win.Show();

    public bool? ShowDialog() => win.ShowDialog();
}
