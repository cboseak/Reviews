using NHunspell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ReviewSite.Helpers
{
    public class ArticleSpinner
    {
        public static string SpinText(string text)
        {
            var words = text.Split(' ');
            MyThes thes = new MyThes(HttpRuntime.AppDomainAppPath + "th_en_US_new.dat");
            Hunspell hunspell = new Hunspell(HttpRuntime.AppDomainAppPath + "en_US.aff", HttpRuntime.AppDomainAppPath + "en_US.dic");
            for (var i = 0; i < words.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                ThesResult tr = thes.Lookup(words[i], hunspell);
                if (tr != null)
                {
                    foreach (ThesMeaning meaning in tr.Meanings)
                    {
                        foreach (string synonym in meaning.Synonyms)
                        {
                            sb.Append(synonym + "|");
                        }
                    }
                }
                if (sb.ToString().Length > 2)
                    words[i] = "{" + sb.ToString().Substring(0, sb.ToString().Length - 1) + "}";
            }
            text = string.Join(" ", words);
            return text;
        }

        public static string GetSpunText(string text)
        {
            var words = text.Split(' ');

            MyThes thes = new MyThes(HttpRuntime.AppDomainAppPath + "th_en_US_new.dat");
            Hunspell hunspell = new Hunspell(HttpRuntime.AppDomainAppPath + "en_US.aff", HttpRuntime.AppDomainAppPath + "en_US.dic");
            for (var i = 0; i < words.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                ThesResult tr = thes.Lookup(words[i], hunspell);
                if (tr != null)
                {
                    foreach (ThesMeaning meaning in tr.Meanings)
                    {
                        Random random = new Random();
                        int randNum = random.Next(0, meaning.Synonyms.Count());
                        if (randNum % 5 == 0)
                        {
                            words[i] = meaning.Synonyms[randNum];
                        }
                    }
                }
            }
            text = string.Join(" ", words);
            return text;
        }

        public static string GetLightSpunText(string text)
        {
            var words = text.Split(' ');


            MyThes thes = new MyThes(HttpRuntime.AppDomainAppPath + "th_en_US_new.dat");
            Hunspell hunspell = new Hunspell(HttpRuntime.AppDomainAppPath + "en_US.aff", HttpRuntime.AppDomainAppPath + "en_US.dic");
            for (var i = 0; i < words.Length; i++)
            {
                StringBuilder sb = new StringBuilder();
                ThesResult tr = thes.Lookup(words[i], hunspell);
                if (tr != null)
                {
                    foreach (ThesMeaning meaning in tr.Meanings)
                    {
                        Random random = new Random();

                        words[i] = meaning.Synonyms[0];
                    }
                }
            }
            text = string.Join(" ", words);
            return text;
        }
    }
}