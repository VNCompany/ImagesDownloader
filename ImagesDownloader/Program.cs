using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

using HtmlParsing;
using HtmlParsing.XPathSelector;

namespace ImagesDownloader
{
    class Program
    {
        private static DownloaderOptions _options = null!;
        
        private static void Main(string[] args)
        {
            _options = new DownloaderOptions();
            
            try
            {
                ArgsParser.Parse(args, _options);
            }
            catch (Exception e)
            {
                Console.WriteLine("Arguments error: {0}", e.Message);
                return;
            }
            
            if (_options.Url != null && _options.Selector != null)
            {
                try
                {
                    List<Uri> links = LoadLinks(_options.Url, _options.Selector).ToList();
                    Console.WriteLine("Loaded {0} link's", links.Count);
                    
                    if (links.Count > 0)
                        Download(links);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            
                return;
            }
            
            CommandHandler();
        }

        private static void CommandHandler()
        {
            string cmd;
            while ((cmd = Console.ReadLine() ?? string.Empty).ToLower() != "exit")
            {
                string[] cmdParts = cmd.SmartSplit().ToArray();
                switch (cmdParts[0].ToLower())
                {
                    case "threads":
                        if (cmdParts.Length != 2 || !_options.SetThreadsCount(cmdParts[1]))
                            Console.WriteLine("Invalid parameter");
                        break;
                    
                    case "s"
                }
            }
        }

        private static void Download(List<Uri> links)
        {
            
        }

        private static IEnumerable<Uri> LoadLinks(Uri url, string xPathPattern)
        {
            new Uri("frgerg").
            
            using HttpClient http = new HttpClient();
            string html = http.GetStringAsync(url).Result;
            HtmlParser parser = new HtmlParser(html);
            foreach (string? iterLink in parser.SelectStrings(xPathPattern))
            {
                if (string.IsNullOrWhiteSpace(iterLink)) continue;

                string link = iterLink.Trim();
                
                if (link[0] == '/' 
                    && Uri.TryCreate($"{url.Scheme}://{url.Host}{link}", new UriCreationOptions(), out Uri? linkUri))
                    yield return linkUri;
                else if (link.StartsWith("http") && Uri.TryCreate(link, UriKind.Absolute, out linkUri))
                    yield return linkUri;
                else
                {
                    string sep = url.OriginalString.EndsWith('/') ? string.Empty : "/";
                    if (Uri.TryCreate(string.Concat(url.OriginalString, sep, link), new UriCreationOptions(),
                            out linkUri))
                        yield return linkUri;
                }
            }
        }
    }
}