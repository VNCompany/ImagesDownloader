using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

using HtmlParsing;
//using HtmlParsing.XPathSelector;

namespace ImagesDownloader
{
    class Program
    {
        private static DownloaderOptions _options;
        private static CommandItem[] _commands;

        static Program()
        {
            _options = new DownloaderOptions();
            _commands = new[]
            {
                new CommandItem(
                    "THREADSCOUNT", "-t",
                    "Set count of downloader threads (max 10)",
                    (val, opt) => val != null && opt.TrySetThreadsCount(val)),
                new CommandItem(
                    "URL", "-u",
                    "Html URL",
                    (val, opt) => val != null && opt.TrySetUrl(val)),
                new CommandItem(
                    "INPUTFILE", "-f",
                    "Path to html file",
                    (val, opt) => val != null && opt.TrySetInputFile(val)),
                new CommandItem(
                    "PATTERN", "-p",
                    "Pattern for parsing html",
                    (val, opt) => val != null && opt.TrySetInputFile(val)),
                new CommandItem(
                    "CONTENTPATH", "-r",
                    "URL path to content directory",
                    (val, opt) => val != null && opt.TrySetContentPath(val)),
                new CommandItem(
                    "OUTPUT", "-o",
                    "", // TODO: Come up with a description
                    (val, opt) => val != null && opt.TrySetPattern(val)),
                new CommandItem(
                    "HELP", "-h",
                    "display this help and exit",
                    Command_Help,
                    requireParameter: false),
            };
        }

        private static bool Main_ParseArgs(string[] args)
        {
            for (int i = 0; i != args.Length; ++i)
            {
                CommandItem? cmd = _commands.FirstOrDefault(c => c.ArgumentName == args[i]);
                if (cmd is null)
                {
                    Console.WriteLine($"Unknown parameter: {args[i]}");
                    return false;
                }
                    
                try
                {
                    if (!cmd.RequireParameter)
                    {
                        if(cmd.Execute(null, _options)) continue;
                    }
                    else if (++i != args.Length)
                    {
                        if (!args[i].StartsWith('-') && cmd.Execute(args[i], _options)) continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Fatal error: {e.Message}");
                    return false;
                }
                Console.WriteLine($"Invalid value for parameter {args[i - 1]}");
                return false;
            }

            return true;
        }
        
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (!Main_ParseArgs(args)) Environment.Exit(1);
            }
        }

        private static bool Command_Help(string? _, DownloaderOptions __)
        {
            Console.WriteLine("USING");
            Console.WriteLine("\tImagesDownloader  [{0}]\n",
                string.Join("] [",
                    _commands.Where(c => c.ArgumentName != null && c.ArgumentName != "-h")
                        .Select(cp => $"{cp.ArgumentName} {cp.Name.ToLower()}")));
            
            Console.WriteLine("COMMANDS");
            foreach (CommandItem cmd in _commands)
            {
                Console.WriteLine(cmd.ArgumentName != null
                    ? $"\t{cmd.Name.ToLower()} ({cmd.ArgumentName})"
                    : $"\t{cmd.Name.ToLower()}");
                Console.WriteLine("\t\t{0}\n", cmd.Description);
            }

            Console.WriteLine();

            return true;
        }
        
        // TODO: сделать расширение для URI для автодополнения относительных ссылок
    }
}