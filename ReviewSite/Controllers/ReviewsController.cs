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
            return View();
        }
    }
}