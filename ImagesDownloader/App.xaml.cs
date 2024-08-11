using Microsoft.Extensions.DependencyInjection;
using System.Windows;

using ImagesDownloader.Interfaces;
using ImagesDownloader.Services;

namespace ImagesDownloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();

            DISource.Resolver = vmType =>
            {
                var service = _serviceProvider.GetRequiredService(vmType);
                if (service is IViewModel viewModel)
                    viewModel.ServiceProvider = _serviceProvider;
                return service;
            };
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ILogger, DebugLogger>();
            services.AddSingleton<AppSettings>();
            services.AddSingleton<Services.Download.DownloadService>();
            services.AddViewModels();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            new Views.MainWindow().Show();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            _serviceProvider.Dispose();
        }
    }

}
