using System;
using System.IO;

using XPathParsing;

namespace ImagesDownloader
{
    enum DownloadMethodType { Undefined, FromFile, FromUrl }

    class DownloaderOptions
    {
        public DownloadMethodType DownloadMethod { get; private set;  } = DownloadMethodType.Undefined;
        
        public int ThreadsCount { get; private set; } = 4;
        
        public Uri? Url { get; private set; }
        
        public string? InputFile { get; private set; }
        
        //public XPathParser? Pattern { get; private set; }
        
        public Uri? ContentPath { get; private set; }
        
        public OutputPath? SavePath { get; private set; }

        public bool TrySetThreadsCount(string value)
        {
            if (int.TryParse(value, out int result) && result > 0 && result <= 10)
            {
                ThreadsCount = result;
                return true;
            }

            return false;
        }

        public bool TrySetUrl(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri? result))
            {
                Url = result;
                DownloadMethod = DownloadMethodType.FromUrl;
                return true;
            }

            return false;
        }

        public bool TrySetInputFile(string value)
        {
            if (File.Exists(value))
            {
                InputFile = value;
                DownloadMethod = DownloadMethodType.FromFile;
                return true;
            }

            return false;
        }

        public bool TrySetPattern(string value)
        {
            try
            {
                //Pattern = new XPathParser(value);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool TrySetContentPath(string value)
        {
            if (Uri.TryCreate(value, UriKind.Absolute, out Uri? result))
            {
                ContentPath = result;
                return true;
            }

            return false;
        }

        public bool TrySetSavePath(string value) => (SavePath = OutputPath.Create(value)) != null;
    }
}