using System;

namespace ImagesDownloader
{
    class DownloaderOptions
    {
        public int ThreadsCount { get; set; } = 4;
        
        public Uri? Url { get; set; }
        
        public OutputPathInfo? Output { get; set; }
        
        public string? Selector { get; set; }

        public bool SetThreadsCount(string value)
        {
            if (int.TryParse(value, out int result))
            {
                ThreadsCount = result;
                return true;
            }
            return false;
        }

        public bool SetOutput(string value) => (Output = OutputPathInfo.Parse(value)) != null;

        public bool SetUrl(string value)
        {
            if (string.IsNullOrWhiteSpace(value)
                || Uri.TryCreate(value, UriKind.Absolute, out Uri? result) == false)
                return false;

            Url = result;
            return true;
        }

        public bool SetSelector(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            Selector = value;
            return true;
        }
    }
}