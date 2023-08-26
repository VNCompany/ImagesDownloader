using HtmlParsing;
using HtmlParsing.XPathSelector;

using System.Net;

namespace Tests;

[TestFixture]
public class XPathSelectorTests
{
    private const string HTML_PATH = "../../../test2.html";
    
    [Test]
    public void Test1()
    {
        HtmlParser parser = new HtmlParser(File.ReadAllText(HTML_PATH));
        
         Assert.That(parser.SelectStrings("body//main").First()?.Trim().StartsWith("Lorem ipsum"), Is.True);
         Assert.That(parser.SelectStrings("body/main").First()?.Trim().EndsWith("id est laborum."), Is.True);
        
         Assert.That(parser.SelectStrings("body//a[1]/@href").First(), Is.EqualTo("/catalog"));
        
         Assert.That(parser.SelectStrings("body//*[@class='header']/a/@href").ToArray(), Is.EquivalentTo(new[]
         {
             "/index",
             "/catalog",
             "/about",
             "/contacts",
         }));
    }
}