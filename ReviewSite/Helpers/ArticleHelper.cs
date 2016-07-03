using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReviewSite.Models;
using System.Text.RegularExpressions;

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

        public static string sanitizeArticle(string body)
        {
            body = HttpUtility.HtmlDecode(body);
            body = body.Replace('\n', ' ');
            body = body.Replace('\r', ' ');
            int pos = body.IndexOf("<a class=\"visually");
            if (pos != -1)
            {
                body = body.Substring(0, pos);
            }
            body = body.Replace("wired", "electricienmoinscher");
            body = body.Replace("https", "http");
            var imgs = Regex.Matches(body, "<img\\s+[^>]*src=\"([^\"]*)\"[^>]*>");
            if (imgs != null && imgs.Count > 0)
            {
                foreach (var i in imgs)
                {
                    body = body.Replace(i.ToString(), "");
                }
            }

            body = ReviewSite.Helpers.ArticleSpinner.GetSpunText(body);

            return body;
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