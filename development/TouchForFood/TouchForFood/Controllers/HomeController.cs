using System.Web;
using System.Web.Mvc;
using TouchForFood.Util.User;

namespace TouchForFood.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            //ViewBag.Message = "Welcome to Touch For Food";

            if (HttpContext.Session["role"] == null && Request.IsAuthenticated)
            {
                HttpContext.Session["role"] = UserUtil.getAuthenticatedUser(Request).user_role;
            }

            return View();
        }

    }
}
