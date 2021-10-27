using AngleSharp;
using AngleSharp.Html.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TextQuest.Classes
{
    public static class Utils
    {
        public static IBrowsingContext Context = BrowsingContext.New(Configuration.Default.WithDefaultLoader().WithDefaultCookies());
        public static CookieContainer Cookie = new CookieContainer();
        public static IHtmlDocument Doc;
        public static String Html => Doc.DocumentElement.InnerHtml;


        public static HttpClient Client;




        public static String OpenLink(String Link,TimeSpan timeout=default)
        {
            if (timeout == default) timeout = new TimeSpan(0,0,60);
            SetTimeout(timeout);
            Doc = (IHtmlDocument)Context.OpenAsync(Link).Result;
            return Html;
        }


        public static void SetTimeout(TimeSpan timeout=default)
        {
            if (timeout==default)
            {
                timeout = new TimeSpan(0, 0, 60);
            }
            (Context.OriginalServices.First(x => x is AngleSharp.Io.DefaultHttpRequester) as AngleSharp.Io.DefaultHttpRequester).Timeout = timeout;
        }


        public static String GetResponse(String Link, Method Method, HttpContent Content=null)
        {

            return "";
        }







        public enum Method
        {
            Get,
            Post,
        }





    }
}
