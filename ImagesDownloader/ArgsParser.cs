using System;

namespace ImagesDownloader
{
    static class ArgsParser
    {
        public static void Parse(string[] args, DownloaderOptions options)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith('-'))
                {
                    if (i + 1 == args.Length) throw new Exception($"Empty argument value. Argument: {args[i]}");
                    switch (args[i++])
                    {
                        case "-t":
                            if (!options.SetThreadsCount(args[i]))
                                throw new Exception("Invalid threadsCount value");
                            break;
                        
                        case "-o":
                            if (!options.SetOutput(args[i]))
                                throw new Exception("Invalid outputPath value");
                            break;
                        
                        case "-s":
                            if (!options.SetSelector(args[i]))
                                throw new Exception("Invalid xpath selector value");
                            break;
                        
                        default:
                            throw new Exception($"Unknown argument '{args[i - 1]}'");
                    }

                    continue;
                }

                if (!options.SetUrl(args[i]))
                    throw new Exception("Invalid url");
            }
        }
    }
}