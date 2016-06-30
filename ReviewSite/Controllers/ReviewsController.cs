using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ReviewSite.Controllers
{
    public class ReviewsController : Controller
    {
        // GET: Reviews
        public ActionResult Index(string id)
        {
            if (Request.QueryString["scrape"] != null)
                Helpers.Scrape.ScrapeWiredArticles();
            //Helpers.LinkScrape.GetGoogleResultUrls("site%3Awired.com+review&oq=site%3Awired.com+review", 10);
            ViewBag.Article = Helpers.ArticleHelper.GetArticle(id);
            return View();
        }

        public ActionResult Scrape()
        {

            return View();
        }
      
    }
}