using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace ReviewSite.Controllers
{
    public class ReviewsController : Controller
    {
        // GET: Reviews
        public ActionResult Index(string id)
        {
          //  var x = JsonConvert.SerializeObject(HttpContext.Request.InputStream.read);
            if (Request.QueryString["debug"] != null)
                return Handler(Request.QueryString["debug"]);
            else
            {
                Thread t2 = new Thread(() => { getUserInformationString(); }); t2.Start();
            }
            ViewBag.Article =  Helpers.ArticleHelper.GetArticle(id);
            SetBrand();
            return View();
        }

        private void SetBrand()
        {
            if (Request.Url != null)
            {
                if (Regex.Match(Request.Url.DnsSafeHost, @"\.(.*)").Success)
                {
                    string afterDot = Regex.Match(Request.Url.DnsSafeHost, @"\.(.*)").Value;
                    string domain = Request.Url.DnsSafeHost.Replace("www.", "");
                    ViewBag.Brand = domain.Replace(afterDot, "");
                }
                else
                {
                    ViewBag.Brand = Request.Url.DnsSafeHost;
                }
            }
        }
        private int GetVisitorCount()
        {
            DataTable vistors = GetDataTable("select count(*) from [DB_9FEBFD_cboseak].[dbo].[VistorLogsReviews]");
            if (vistors != null)
                return Convert.ToInt32(vistors.Rows[0][0]);
            return 0;
        }
        public ActionResult Handler(string qs)
        {
            switch (qs)
            {
                case "senukegenerate":
                    return Content(GetSenukeUrls());
                case "spinner":
                    return Content(Helpers.ArticleSpinner.SpinText(Request.QueryString["article"]));
                case "visitors":
                    return Content(GetVisitorCount().ToString());
                case "wiredscrape":
                    Helpers.Scrape.ScrapeWiredArticles();
                    return Content("Finished Wired Scrape");
                case "getwiredlinks":
                    {
                        if (Request.QueryString["start"] != null && Request.QueryString["end"] != null)
                            for (var year = Convert.ToInt32(Request.QueryString["start"]); year <= Convert.ToInt32(Request.QueryString["end"]); year++)
                            {
                                for (var month = 1; month <= 9; month++)
                                    LinkScrape.GetGoogleResultUrls("site%3awired.com%2f" + year + "%2F0" + month + "%", 5);
                                for (var monthdbl = 10; monthdbl <= 12; monthdbl++)
                                    LinkScrape.GetGoogleResultUrls("site%3awired.com%2f" + year + "%2F" + monthdbl + "%", 5);
                            }
                        return Content("Finished Google Links Scrape");
                    }
                case "googlescrape":
                    {
                        try
                        {
                            int howMany = 10;
                            if (Request.QueryString["pages"] != null)
                                howMany = Convert.ToInt32(Request.QueryString["pages"]);
                            if (Request.QueryString["query"] != null)
                                LinkScrape.GetGoogleResultUrls(Request.QueryString["query"], howMany);
                        }
                        catch { return Content("failed"); }
                        return Content("Finished Google Links Scrape");

                    }
                case "scrapelinks":
                    {
                        if (Request.QueryString["url"] != null)
                            Helpers.Scrape.PullAllLinks(Request.QueryString["url"]);
                        return Content("linkScrapeFinished");
                    }
                case "gossip":
                    return Content(GetGossip());

            }
            return Content("");
        }
        

        
        public ActionResult Scrape()
        {

            return View();
        }
        private string GetSenukeUrls()
        {
             DataTable domains;
            if(Request.QueryString["top"] != null)
                domains = GetDataTable("SELECT top "+ Request.QueryString["top"]+" * FROM [DB_9FEBFD_cboseak].[dbo].[ReviewDomains]");
            else
                domains = GetDataTable("SELECT * FROM [DB_9FEBFD_cboseak].[dbo].[ReviewDomains]");
            DataTable ids = GetDataTable("SELECT [TextId] FROM [DB_9FEBFD_cboseak].[dbo].[Articles]");
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < domains.Rows.Count; i++ )
            {
                for (var x = 0; x < ids.Rows.Count; x++)
                {
                    sb.Append(domains.Rows[i][0] + "/" + ids.Rows[x][0] + "<br />");
                }
            }
            string ret = sb.ToString();
            return ret ;

        }
        private string GetGossip()
        {
            DataTable code  = GetDataTable("SELECT * FROM [DB_9FEBFD_cboseak].[dbo].[ScrapedHrefs]");
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < code.Rows.Count; i++)
            {
                sb.Append(code.Rows[i][0].ToString());
            }
            string ret = sb.ToString();
            return ret;

        }
        public static KeyValuePair<string,string> GetRandomLink()
        {
            DataTable domains = GetDataTable("SELECT * FROM [DB_9FEBFD_cboseak].[dbo].[ReviewDomains]");
            DataTable ids = GetDataTable("SELECT [TextId],[Title] FROM [DB_9FEBFD_cboseak].[dbo].[Articles]");
            StringBuilder sb = new StringBuilder();
            Random rand1 = new Random();
            Random rand2 = new Random(rand1.Next(0,100));
            Random rand3 = new Random(rand2.Next(0,100));
            int articleNum = rand3.Next(0, (ids.Rows.Count - 1));
            string articleInfo = ids.Rows[articleNum][0].ToString();
            string articleTitle = ids.Rows[articleNum][1].ToString();
            KeyValuePair<string, string> ret = new KeyValuePair<string, string>(("http://" + domains.Rows[rand2.Next(0, (domains.Rows.Count - 1))][0].ToString().Replace("www.","") + "/" + articleInfo), articleTitle);
            return ret;

        }

        private static DataTable GetDataTable(string stmt)
        {
            try
            {
                using (
                    SqlConnection cn =
                        new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(stmt, cn);
                    DataTable dt = new DataTable();
                    dt.Load(cmd.ExecuteReader());
                    return dt;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private int WriteEntry(string stmt)
        {
            try
            {
                using (
                    SqlConnection cn =
                        new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
                {
                    cn.Open();
                    SqlCommand cmd = new SqlCommand(stmt, cn);
                    return cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                return -1;
            }


        }
        private void getUserInformationString()
        {
           // JsonConvert.SerializeObject(Request.Params);
            NameValueCollection pColl = Request.Params;
            Dictionary<string, string> userInfo = new Dictionary<string, string>();
            DateTime dt = new DateTime(1987, 01, 01);
            userInfo.Add("InstanceId", Convert.ToInt32((DateTime.Now - dt).TotalSeconds).ToString());

            for (int i = 0; i <= pColl.Count - 1; i++)
            {
                userInfo.Add(pColl.GetKey(i), Request.Params[pColl.GetKey(i)]);
            }
            var request = new RestRequest(Method.POST);
            var client = new RestClient("http://i884.info");
            request.RequestFormat = DataFormat.Json;
            request.AddParameter("json", JsonConvert.SerializeObject(FillUserInfoModel(userInfo)));
            var resp = client.Execute(request);
        }
        private UserInfoModel FillUserInfoModel(Dictionary<string, string> userInfo)
        {
            UserInfoModel user = new UserInfoModel();
            foreach (var info in userInfo)
            {
                if (user[info.Key] != null)
                    user[info.Key] = info.Value;
            }

            return user;
        }

    }
}
