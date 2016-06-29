using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewSite.Models;

namespace ReviewSite.Helpers
{
    public class ArticleHelper
    {
        public static Article GetArticle(string id)
        {
            using (var context = new DB_9FEBFD_cboseakEntities())
            {

                var articles = from a in context.Articles
                    where a.TextId == id
                    select a;

                if (articles.Any())
                    return articles.FirstOrDefault();

                var allRecords = from a in context.Articles
                    select a;
                //var count = allRecords.Count() - 1;
                //Random rand = new Random();
                //int randonRecord = rand.Next(0, count);
                
                return allRecords.FirstOrDefault();

            }
        }
    }
}