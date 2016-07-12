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
using System.Data;
using System.Text.RegularExpressions;

namespace ReviewSite.Helpers
{
    public class Scrape
    {
        private static IEnumerable<HtmlAgilityPack.HtmlNode> GetElementsByClass(HtmlAgilityPack.HtmlDocument doc,
            string className)
        {
            return doc.DocumentNode.Descendants("div")
                .Where(c => c.Attributes.Contains("class") && c.Attributes["class"].Value.Contains(className));
        }
        public static int NonQueryHelper(string stmt)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            cn.Open();
            SqlCommand cmd = new SqlCommand(stmt, cn);
            return cmd.ExecuteNonQuery();
        }
        public static void PullAllLinks(string url)
        {
            var doc = PullAgilityHtml(url);
            var links = doc.DocumentNode.Descendants("a");
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append("insert into [DB_9FEBFD_cboseak].[dbo].[ScrapedHrefs](url) values ");
            foreach (var link in links)
            {
                cmdText.Append("('" + link.OuterHtml.ToString().Replace("'","\"") + "'),");
            }
                    string stmt = cmdText.ToString().Replace("{", "");
        stmt = stmt.Replace("}", "");
            stmt = stmt.Substring(0, stmt.Length - 1).Replace("'http","\"http");
            
        NonQueryHelper(stmt);
        }
        
        private static HtmlAgilityPack.HtmlDocument PullAgilityHtml(string url)
        {
            try
            {
                string html = "";

                using (var client = new WebClient())
                {
                    html = client.DownloadString(FixUrl(url));
                }
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                return doc;
            }
            catch
            {
                return null;
            }
        }

        private static string FixUrl(string url)
        {
            return !url.StartsWith("http") ? "http://" + url : url;
        }



        public static int ScrapeWiredArticles()
        {
            var articleUrls = new List<string>();
            var articles = new List<Models.Article>();
            var urls = LinkScrape.StoredProcHelper("GetUnscrapedArticleUrls");
            string articleTitle = null;
            for (var i = 0; i < urls.Rows.Count; i++)
                articleUrls.Add(urls.Rows[i][0].ToString());

            foreach (var articleUrl in articleUrls)
            {
                var doc = PullAgilityHtml(articleUrl);
                if (doc == null)
                    continue;
                var articleContent = doc.DocumentNode.Descendants("article").FirstOrDefault();
                //var articleDate = doc.DocumentNode.Descendants("time");
                if (articleContent != null)
                if (!articleUrl.Contains(".pdf"))
                {
                    articleTitle = doc.DocumentNode.Descendants("title").FirstOrDefault().InnerText;
                    articleTitle = articleTitle.Replace("WIRED", "");
                    articleTitle = articleTitle.Replace("|", "");
                    articleTitle = articleTitle.Trim();
                }

                if (articleContent == null) continue;

                var temp = new Models.Article();
                temp.OriginalUrl = articleUrl;
                temp.BodyText = articleContent.InnerHtml;
                temp.TextId = PullTextId(articleUrl);
                if (articleTitle != null) temp.Title = articleTitle;
                //if (articleDate != null) temp.PostedDate = articleDate.InnerText;

                articles.Add(temp);
            }
            return ArticleHelper.WriteArticleCollection(articles);
        }

        private static string PullTextId(string text)
        {
            var pattern = @"\/(.*?)\/";
            var matches = Regex.Matches(text, pattern);
            return matches[matches.Count - 1].Value.Replace("/","");
        }
    }
}

public class LinkScrape
    {
    public static void GetBingResultUrls(string searchTerm, int pages)
    {
        List<string> googleQueue = new List<string>();
        List<string> googleHtml = new List<string>();
        StringBuilder cmdText = new StringBuilder();
        cmdText.Append("insert into [DB_9FEBFD_cboseak].[dbo].[ScrapeLinks](url) values ");
        for (var i = 1; i < pages; i++)
        {
            googleQueue.Add("https://www.google.com/search?q=" + searchTerm + "&first=" + (i * 50));
        }
        foreach (var page in googleQueue)
        {
            using (var client = new WebClient())
            {
                try
                {
                    //try using webclient, first...
                    string html = client.DownloadString(page);
                    googleHtml.Add(html);

                }
                catch
                {
                    //...if that fails, its usually because of cross-domain issues so try requesting html as a browser
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
        string stmt = cmdText.ToString().Replace("{", "");
        stmt = stmt.Replace("}", "");
        stmt = stmt.Substring(0, stmt.Length - 1);
        NonQueryHelper(stmt);
    }
    public static int NonQueryHelper(string stmt)
    {
        SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
        cn.Open();
        SqlCommand cmd = new SqlCommand(stmt, cn);
        return cmd.ExecuteNonQuery();
    }
    public static void GetGoogleResultUrls(string searchTerm, int pages)
    {
        List<string> googleQueue = new List<string>();
        List<string> googleHtml = new List<string>();
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
                    //try using webclient, first...
                    string html = client.DownloadString(page);
                    googleHtml.Add(html);

                }
                catch
                {
                    //...if that fails, its usually because of cross-domain issues so try requesting html as a browser
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
        string stmt = cmdText.ToString().Replace("{", "");
        stmt = stmt.Replace("}", "");
        stmt = stmt.Substring(0, stmt.Length - 1);
        NonQueryHelper(stmt);

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


        public static DataTable StoredProcHelper(string proc, SqlParameterCollection parameters = null)
        {
            SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString);
            cn.Open();
            SqlCommand cmd = new SqlCommand(proc, cn) {CommandType = CommandType.StoredProcedure};
            if (parameters != null) 
                foreach (var param in parameters)
                    cmd.Parameters.Add(param);
            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            return dt;
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
