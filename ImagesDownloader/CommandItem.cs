using System;

namespace ImagesDownloader
{
    internal class CommandItem
    {
        private readonly Func<string?, DownloaderOptions, bool> _cb;
        
        public string Name { get; }
        
        public string? ArgumentName { get; }
        
        public string Description { get; }
        
        public bool RequireParameter { get; }

        public CommandItem(string name, string? argumentName, string description, 
            Func<string?, DownloaderOptions, bool> callback,
            bool requireParameter = true)
        {
            _cb = callback;
            
            Name = name;
            ArgumentName = argumentName;
            Description = description;
            RequireParameter = requireParameter;
        }

        public bool Execute(string? parameter, DownloaderOptions options) => _cb.Invoke(parameter, options);
    }
}