using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

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
            List<string> links = new List<string>();
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append("insert into [DB_9FEBFD_cboseak].[dbo].[ScrapeLinks](url) values ");
            googleQueue.Add("https://www.google.com/search?q=" + searchTerm + "&num=100");
            for (var i = 1; i < pages; i++)
            {
                googleQueue.Add("https://www.google.com/search?q=" + searchTerm + "&num=100&start=" + i + "00");
            }
            foreach (var page in googleQueue)
            {
                using (var client = new WebClient())
                {
                    try
                    {
                        string html = client.DownloadString(page);
                        googleHtml.Add(html);

                    }
                    catch
                    {
                        WebProcessor wp = new WebProcessor();

                        string html = wp.GetGeneratedHTML(page);
                        googleHtml.Add(html);
                    }
                }
            }
            foreach (var src in googleHtml)
            {
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(src);
                var urls = doc.DocumentNode.Descendants("cite");
                foreach (var url in urls)
                {
                    cmdText.Append("('" + url.InnerText + "'),");
                }
            }
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            cn.Open();
            SqlCommand cmd = new SqlCommand(cmdText.ToString(), cn);
            cmd.ExecuteNonQuery();
        }
        public static string AlternateGetHtml(string url){
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.MaximumAutomaticRedirections = 4;
            request.MaximumResponseHeadersLength = 4;
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse ();
            Stream receiveStream = response.GetResponseStream ();
            StreamReader readStream = new StreamReader (receiveStream, Encoding.UTF8);
            string html = readStream.ReadToEnd ();
            response.Close ();
            readStream.Close ();
            return html;
        }
    }
    public class WebProcessor
    {
        private string GeneratedSource { get; set; }
        private string URL { get; set; }

        public string GetGeneratedHTML(string url)
        {
            URL = url;

            Thread t = new Thread(new ThreadStart(WebBrowserThread));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return GeneratedSource;
        }

        private void WebBrowserThread()
        {
            WebBrowser wb = new WebBrowser();
            wb.Navigate(URL);

            wb.DocumentCompleted +=
                new WebBrowserDocumentCompletedEventHandler(
                    wb_DocumentCompleted);

            while (wb.ReadyState != WebBrowserReadyState.Complete)
                Application.DoEvents();

            //Added this line, because the final HTML takes a while to show up
            GeneratedSource = wb.Document.Body.InnerHtml;

            wb.Dispose();
        }

        private void wb_DocumentCompleted(object sender,
            WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            GeneratedSource = wb.Document.Body.InnerHtml;
        }
    }
}