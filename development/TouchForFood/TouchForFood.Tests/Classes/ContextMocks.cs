using System.Web;
using System.Web.Routing;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace TouchForFood.Tests.Classes
{
    /*
     * Class that handles the HTTP contexts, responses and other variables that MVC has as sealed classes.
     * This was obtained directly from the book "Pro ASP.NET MVC Framework" and used with permission by the author.
     * http://www.amazon.com/dp/1430210079/
     */

    public class ContextMocks
    {
        public Moq.Mock<HttpContextBase> HttpContext { get; private set; }
        public Moq.Mock<HttpRequestBase> Request { get; private set; }
        public Moq.Mock<HttpResponseBase> Response { get; private set; }
        public RouteData RouteData { get; private set; }
        public RequestContext rc { get; private set; }
        public ControllerContext cc { get; private set; }
        public ContextMocks(Controller onController)
        {
            // Define all the common context objects, plus relationships between them
            HttpContext = new Moq.Mock<HttpContextBase>();
            Request = new Moq.Mock<HttpRequestBase>();
            Response = new Moq.Mock<HttpResponseBase>();
            HttpContext.Setup(x => x.Request).Returns(Request.Object);
            HttpContext.Setup(x => x.Response).Returns(Response.Object);
            HttpContext.Setup(x => x.Session).Returns(new FakeSessionState());
            Request.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            Response.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            Request.Setup(x => x.QueryString).Returns(new NameValueCollection());
            Request.Setup(x => x.Form).Returns(new NameValueCollection());
            // Apply the mock context to the supplied controller instance
            rc = new RequestContext(HttpContext.Object, new RouteData());
            cc = new ControllerContext(rc, onController);
            onController.ControllerContext = cc;
        }
        public void addController(Controller toAdd)
        {
            toAdd.ControllerContext = cc;
        }
        // Use a fake HttpSessionStateBase, because it's hard to mock it with Moq
        private class FakeSessionState : HttpSessionStateBase
        {
            Dictionary<string, object> items = new Dictionary<string, object>();
            public override object this[string name]
            {
                get { return items.ContainsKey(name) ? items[name] : null; }
                set { items[name] = value; }
            }
        }
    }
}
