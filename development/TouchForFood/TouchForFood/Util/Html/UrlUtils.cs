using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchForFood.Util.Html
{
    public class UrlUtils
    {
        /**
         * Utility method to generate an absolute url given a relative url
         * Based off of code found here:
         * http://stackoverflow.com/questions/126242/how-do-i-turn-a-relative-url-into-a-full-url
         * 
         */
        public static string ConvertRelativeUrlToAbsoluteUrl(string relativeUrl) {
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;

                return string.Format("{0}://{1}{2}{3}",
                    (request.IsSecureConnection) ? "https" : "http",
                    request.Url.Host, (request.Url.Port == 80) ? "" : ":" + request.Url.Port.ToString(),
                    VirtualPathUtility.ToAbsolute(relativeUrl));
            } else 
            {
                return null;
            }

        }
    }
}