using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ReviewSite.Helpers
{
    public class Scrape
    {

        public static void ParseRdSlides(string url)
        {
            string html = "";

            using (var client = new WebClient())
            {
                html = client.DownloadString(url);
            }
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            //var fullSlides = GetElementsByClass(doc, "rd-slide-full");
            //var titles = GetElementsByClass(doc, "rd-slide-title");
            //var images = GetElementsByClass(doc, "rd-slide-img");
            //var bodies = GetElementsByClass(doc, "rd-slide-caption");
        }

        public static IEnumerable<HtmlAgilityPack.HtmlNode> GetElementsByClass(HtmlAgilityPack.HtmlDocument doc,
            string className)
        {
            return doc.DocumentNode.Descendants("div")
                .Where(c => c.Attributes.Contains("class") && c.Attributes["class"].Value.Contains(className));
        }

        public  static IEnumerable<HtmlAgilityPack.HtmlNode> GetElementsByTag(HtmlAgilityPack.HtmlDocument doc, string tagName)
        {
            return doc.DocumentNode.Descendants("body")
                .Where(c => c.Attributes.Contains(tagName));
        }

    }

    public class LinkScrape
    {
        public static void ParseRdSlides(string searchTerm, int pages)
        {
            List<string> googleQueue = new List<string>();
            List<string> googleHtml = new List<string>();
            List<string> scrapeList = new List<string>();
            googleQueue.Add("https://www.google.com/search?q=" + searchTerm + "&num=100");
            for (var i = 1; i < pages; i++)
            {
                googleQueue.Add("https://www.google.com/search?q=" + searchTerm + "&num=100&start=" + i + "00");
            }
            foreach (var page in googleQueue)
            {
                using (var client = new WebClient())
                {
                    googleHtml.Add(client.DownloadString(page));
                }
            }
            foreach (var src in googleHtml)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(src);
                var forms = doc.DocumentNode.Descendants("cite");
               
            }
            //var fullSlides = GetElementsByClass(doc, "rd-slide-full");
            //var titles = GetElementsByClass(doc, "rd-slide-title");
            //var images = GetElementsByClass(doc, "rd-slide-img");
            //var bodies = GetElementsByClass(doc, "rd-slide-caption");
        }
    }
}