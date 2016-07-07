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

namespace ReviewSite.Controllers
{
    public class ReviewsController : Controller
    {
        // GET: Reviews
        public ActionResult Index(string id)
        {
            if (Request.QueryString["debug"] != null)
            {
                Handler(Request.QueryString["debug"]);
                return View("Scrape", "_empty");
            }
            else
            {
                Thread t2 = new Thread(() => { getUserInformationString(); });
                t2.Start();
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
            {
                return Convert.ToInt32(vistors.Rows[0][0]);
            }
            return 0;
        }
        public void Handler(string qs)
        {
            switch (qs)
            {
                case "senukegenerate":
                    ViewBag.Content = GetSenukeUrls();
                    break;
                case "spinner":
                    ViewBag.Content = Helpers.ArticleSpinner.SpinText(Request.QueryString["article"]);
                    break;
                case "visitors":
                    ViewBag.Content = GetVisitorCount();
                    break;
                case "wiredscrape":
                    Helpers.Scrape.ScrapeWiredArticles();
                    break;
                case "getgooglelinks":
                    {
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F05%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F06%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F07%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F08%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F09%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F10%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F11%2F+review", 10);
                        LinkScrape.GetGoogleResultUrls("site%3Awww.wired.com%2F2007%2F12%2F+review", 10);
                        break;
                    }
                case "getbinglinks":
                    {
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2016%2F05%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2016%2F06%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2015%2F07%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2015%2F08%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2015%2F09%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2015%2F10%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2015%2F11%+review", 5);
                        LinkScrape.GetBingResultUrls("site%3awired.com%2f2015%2F12%+review", 5);
                        break;
                    }
            }
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
        private int getUserInformationString()
        {
            NameValueCollection pColl = Request.Params;
            Dictionary<string, string> userInfo = new Dictionary<string, string>();
            DateTime dt = new DateTime(1987, 01, 01);
            userInfo.Add("InstanceId", Convert.ToInt32((DateTime.Now - dt).TotalSeconds).ToString());

            for (int i = 0; i <= pColl.Count - 1; i++)
            {
                userInfo.Add(pColl.GetKey(i), Request.Params[pColl.GetKey(i)]);
            }
            return WriteEntry(CreateInsertStatementFromModel(FillUserInfoModel(userInfo)));

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
        private string CreateInsertStatementFromModel(UserInfoModel userInfo)
        {
            string insertStart = "INSERT INTO VistorLogsReviews";
            string columns = " (";
            string values = " VALUES (";
            string delimiter = ",";

            columns += "InstanceId" + delimiter;
            values += GetInstance(DateTime.Now) + delimiter;

            for (int i = 0; i <= userInfo.userProperties.Count - 1; i++)
            {
                if (i == userInfo.userProperties.Count - 1)
                    delimiter = ")";
                if (userInfo.userProperties.ElementAt(i).Key != "item")
                {
                    columns += userInfo.userProperties.ElementAt(i).Key + delimiter;
                    if(userInfo.userProperties.ElementAt(i).Value.Length > 999)
                        values += "'" + userInfo.userProperties.ElementAt(i).Value.Substring(0,995) + "'" + delimiter;
                    else
                        values += "'" + userInfo.userProperties.ElementAt(i).Value + "'" + delimiter;
                }
            }
            values += ";";
            return insertStart + columns + values;
        }
        public static int GetInstance(DateTime msgDt)
        {
            return (int)(((new DateTime(1987, 1, 1)) - msgDt).TotalSeconds) * -1;
        }
    }
}
