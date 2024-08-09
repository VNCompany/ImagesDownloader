using System.ComponentModel;

namespace ImagesDownloader.Interfaces;

internal interface IViewModel : INotifyPropertyChanged, IDisposable
{
    IServiceProvider ServiceProvider { get; set; }
}
