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
        }

        private static IEnumerable<Uri> LoadLinks(Uri url, string xPathPattern)
        {
            using HttpClient http = new HttpClient();
            string html = http.GetStringAsync(url).Result;
            HtmlParser parser = new HtmlParser(html);
            foreach (string? iterLink in parser.SelectStrings(xPathPattern))
            {
                if (string.IsNullOrWhiteSpace(iterLink)) continue;

                string link = iterLink.Trim();
                
                if (link[0] == '/' 
                    && Uri.TryCreate($"{url.Scheme}://{url.Host}{link}", UriKind.Absolute, out Uri? linkUri))
                    yield return linkUri;
                else if (link.StartsWith("http") && Uri.TryCreate(link, UriKind.Absolute, out linkUri))
                    yield return linkUri;
                else
                {
                    string sep = url.OriginalString.EndsWith('/') ? string.Empty : "/";
                    if (Uri.TryCreate(
                            string.Concat(url.OriginalString, sep, link),
                            UriKind.Absolute, 
                            out linkUri))
                        yield return linkUri;
                }
            }
        }
        
        // TODO: укоротить метод смарт сплита
        // TODO: сделать расширение для URI для автодополнения относительных ссылок
        // TODO: сделать класс, который будет определять и хранить путь до файла или url
        // TODO: оптимизировать (по возможности) OutputPathInfo
    }
}