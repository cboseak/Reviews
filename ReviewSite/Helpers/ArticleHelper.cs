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
                                 orderby a.Id
                    select a;
                Random rand = new Random();
                int randonRecord = rand.Next(0, allRecords.Count() - 1);

                return allRecords.Count() > 2 ? allRecords.Skip(randonRecord).FirstOrDefault() : allRecords.FirstOrDefault();
            }
        }

        public static int WriteArticleCollection(List<Article> articles)
        {
            using (var context = new DB_9FEBFD_cboseakEntities())
            {
                foreach (var article in articles)
                {
                    context.Articles.Add(article);
                }
                return context.SaveChanges();
            }

        }
    }
}