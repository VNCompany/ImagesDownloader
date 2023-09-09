using System;

namespace ImagesDownloader
{
    internal class CommandItem
    {
        private Func<string?, bool> _cb = null!;
        
        public string Name { get; }
        
        public string? ArgumentName { get; }
        
        public string Description { get; }

        private CommandItem(string name, string? argumentName, string description)
        {
            Name = name;
            ArgumentName = argumentName;
            Description = description;
        }

        public CommandItem(string name, string? argumentName, string description, Func<string?, bool> callback) 
            : this(name, argumentName, description)
        {
            _cb = callback;
        }

        public bool Execute(string? parameter) => _cb.Invoke(parameter);
    }
}