using System.Windows;

using ImagesDownloader.Views;
using ImagesDownloader.ViewModels;

namespace ImagesDownloader.Infrastructure;

static class ViewProvider
{
    private class WindowWrapper(Window window) : IWindow
    {
        public void Close() => window.Close();

        public void Hide() => window.Hide();

        public void Show() => window.Show();

        public bool? ShowDialog() => window.ShowDialog();
    }

    // MAPPING FROM VIEWMODEL TYPE TO WINDOW TYPE
    private static Dictionary<Type, Type> _mappings = new()
    {
        [typeof(HtmlParserViewModel)] = typeof(HtmlParserWindow)
    };

    public static void ShowMessage(string caption, string message)
        => MessageBox.Show(message, caption);

    public static bool ShowMessageDialog(string caption, string message)
        => MessageBox.Show(message, caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK;

    public static IWindow GetWindow<T>() where T : ViewModelBase
        => new WindowWrapper(GetWindowByViewModelType(typeof(T)));

    public static IWindow GetWindow(ViewModelBase viewModel)
    {
        Window window = GetWindowByViewModelType(viewModel.GetType());
        window.DataContext = viewModel;
        return new WindowWrapper(window);
    }

    private static Window GetWindowByViewModelType(Type vmType)
        => (Activator.CreateInstance(_mappings[vmType]) as Window) 
               ?? throw new InvalidOperationException("Windows instance creation failed");
}
