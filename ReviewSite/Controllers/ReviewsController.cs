using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
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
            Thread t2 = new Thread(() => { getUserInformationString(); });
            t2.Start();
            if (Request.QueryString["scrape"] != null)
                Helpers.Scrape.ScrapeWiredArticles();
            var r = Request;
            ViewBag.Article = Helpers.ArticleHelper.GetArticle(id);
            if (Request.Url != null)
            {
                if (Regex.Match(Request.Url.DnsSafeHost, @"\.(.*)").Success)
                {
                    string afterDot = Regex.Match(Request.Url.DnsSafeHost, @"\.(.*)").Value;
                    ViewBag.Brand = Request.Url.DnsSafeHost.Replace(afterDot, "");
                }
                else
                {
                    ViewBag.Brand = Request.Url.DnsSafeHost;
                }
            }
            return View();
        }

        public ActionResult Scrape()
        {

            return View();
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