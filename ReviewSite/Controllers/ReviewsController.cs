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
         //   Helpers.Scrape rd = new Helpers.Scrape();
          //  rd.ParseRdSlides();
            Helpers.LinkScrape.ParseRdSlides("site%3Awired.com+review&oq=site%3Awired.com+review", 10);
            ViewBag.Article = Helpers.ArticleHelper.GetArticle(id);
            return View();
        }
    }
}