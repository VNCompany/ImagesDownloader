using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace ImagesDownloader
{
    class OutputPath
    {
        public string DirectoryPath { get; }
        
        public string Pattern { get; }
        
        private OutputPath(string dirPath, string pattern)
        {
            DirectoryPath = dirPath;
            Pattern = pattern.Replace("{filename}", "{0}").Replace("{counter}", "{1}");
        }

        public string ToString(string name, int counter) =>
            Path.Combine(DirectoryPath, string.Format(Pattern, name, counter));

        
        public static OutputPath? Create(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            
            string[] pathParts = path.Split(_pathSeparators);

            return !(path.EndsWith('/') || path.EndsWith('\\')) && pathParts[^1].Contains("{filename}")
                ? new OutputPath(Path.GetDirectoryName(path) ?? string.Empty, pathParts[^1])
                : new OutputPath(path, "{filename}");
        }

        private static readonly char[] _pathSeparators = { '\\', '/' };
    }
}