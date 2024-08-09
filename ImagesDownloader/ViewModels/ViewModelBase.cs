using System.ComponentModel;
using System.Runtime.CompilerServices;

using ImagesDownloader.Interfaces;

namespace ImagesDownloader.ViewModels;

internal abstract class ViewModelBase : IViewModel
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public IServiceProvider ServiceProvider { get; set; } = null!;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected virtual void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        field = newValue;
        OnPropertyChanged(propertyName);
    }

    public virtual void Dispose() { }
}
