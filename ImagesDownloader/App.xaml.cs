using System.Windows;

namespace ImagesDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Views.AddCollectionWindow().Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            AppSettings.Instance.Dispose();
        }
    }

}
