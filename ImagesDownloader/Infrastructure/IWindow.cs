namespace ImagesDownloader.Infrastructure;

interface IWindow
{
    void Show();
    bool? ShowDialog();
    void Hide();
    void Close();
}
