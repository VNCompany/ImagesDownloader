using System.Windows;

using ImagesDownloader.Views;

namespace ImagesDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new CollectionWindow().Show();
            //new HtmlParserWindow().ShowDialog();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            ServiceAccessor.Close();
        }
    }

}
