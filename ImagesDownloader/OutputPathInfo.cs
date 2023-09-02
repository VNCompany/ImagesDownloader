using System;
using System.IO;

namespace ImagesDownloader
{
    class OutputPathInfo
    {
        private OutputPathInfo(DirectoryInfo dirInfo, string fnPattern)
        {
            DirectoryInfo = dirInfo;
            FileNamePattern = fnPattern.Replace("{filename}", "{0}").Replace("{counter}", "{1}");
        }
        
        public DirectoryInfo DirectoryInfo { get; }
        public string FileNamePattern { get; }

        public string GetFilePath(string fileName, int counter)
            => Path.Combine(DirectoryInfo.FullName, string.Format(FileNamePattern, fileName, counter));

        private static readonly char[] __pathSeparators = { '\\', '/' };
        public static OutputPathInfo? Parse(string? path)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    return new OutputPathInfo(new DirectoryInfo(Directory.GetCurrentDirectory()), "{filename}");

                if (path.IndexOfAny(__pathSeparators) == -1)
                {
                    if (path.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                        return null;
                    return new OutputPathInfo(new DirectoryInfo(Directory.GetCurrentDirectory()), path);
                }

                string? dir, file;
                if ((path.Contains("{filename}") || path.Contains("{counter}"))
                    && !string.IsNullOrEmpty(dir = Path.GetDirectoryName(path))
                    && !string.IsNullOrEmpty(file = Path.GetFileName(path)))
                {
                    if (file.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                        throw new ArgumentException("Filename contains invalid chars");
                    return new OutputPathInfo(new DirectoryInfo(dir), file);
                }

                return new OutputPathInfo(new DirectoryInfo(path), "{filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}