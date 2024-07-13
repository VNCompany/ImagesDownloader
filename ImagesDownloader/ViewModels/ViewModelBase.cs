using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ImagesDownloader.ViewModels;

internal abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected virtual void SetProperty<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
    {
        field = newValue;
        OnPropertyChanged(propertyName);
    }

    public virtual void Dispose() { }
}
