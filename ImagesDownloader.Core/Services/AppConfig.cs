using Newtonsoft.Json;

namespace ImagesDownloader.Core.Services;

public class AppConfig : IDisposable
{
    const string filePath = "./appconfig.json";

    public static AppConfig CreateInstance()
    {
        if (File.Exists("./appconfig.json"))
            return JsonConvert.DeserializeObject<AppConfig>(File.ReadAllText(filePath)) ?? new();

        return new();
    }

    public void Dispose()
    {
        File.WriteAllText(filePath, JsonConvert.SerializeObject(this));
        GC.SuppressFinalize(this);
    }
}
