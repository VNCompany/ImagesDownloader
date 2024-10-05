namespace ImagesDownloader.Core.Models;

public record class DItemDownloadedArgs(DItem Item, Exception? Exception);
