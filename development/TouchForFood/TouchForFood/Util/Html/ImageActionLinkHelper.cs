using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Web.Mvc.Html;
using System.Web.Mvc.Ajax;

namespace TouchForFood.Util.Html
{
    public static class ImageActionLinkHelper
    {

        public static MvcHtmlString ImageActionLink(this AjaxHelper helper, string imageUrl, string altText, string height, string width, string actionName, object routeValues, AjaxOptions ajaxOptions)
        {
            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", imageUrl);
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("height", height);
            builder.MergeAttribute("width", width);
            var link = helper.ActionLink("[replaceme]", actionName, routeValues, ajaxOptions);
            return new MvcHtmlString(link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing))); 
        }
    }
}