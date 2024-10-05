using ImagesDownloader.Tasks;

namespace ImagesDownloader.Models;

public record class ItemSavedEventArgs(
    CollectionItemsTask Source, 
    DownloadItem Item, 
    bool IsSuccess, 
    Exception? Exception);
