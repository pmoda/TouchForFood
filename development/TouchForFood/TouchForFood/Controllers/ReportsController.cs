using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.ViewModels;
using TouchForFood.Util.Security;

namespace TouchForFood.Controllers
{
    public class ReportsController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private ReportsIM im;

        public ReportsController()
        {
            im = new ReportsIM(db);
        }

        //
        // GET: /Reports/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Index()
        {
            return View();
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult GenerateMostPopularDishReport()
        {
            ViewBag.restoId = new SelectList(CreateRestoList(), "id", "name");
            return View();
        }

        [HttpPost, ActionName("GenerateMostPopularDishReport")]
        public ActionResult ProcessGenerateMostPopularDishReport(MostPopularDishViewModel mpd)
        {
            IEnumerable<MostPopularDishViewModel> dishes = im.findMostPopular(db.restaurants.Find(mpd.restoId));

            return View("MostPopularDishReport", dishes);
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult GenerateWaiterStatsReport()
        {
            ViewBag.restoId = new SelectList(CreateRestoList(), "id", "name");
            return View();
        }

        [HttpPost, ActionName("GenerateWaiterStatsReport")]
        public ActionResult ProcessGenerateWaiterStatsReport(WaiterStatsViewModel wsvm)
        {
            IEnumerable<WaiterStatsViewModel> waiters = im.findWaiterStats(db.restaurants.Find(wsvm.restoId), wsvm.startDate, wsvm.endDate);

            return View("WaiterStatsReport", waiters);
        }

        /// <summary>
        /// Make a list of tables that could be associated to creating a service request depending on
        /// the user.
        /// </summary>
        private List<restaurant> CreateRestoList()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            List<restaurant> restoList = new List<restaurant>();

            if (authUser.user_role == (int)SiteRoles.Restaurant)
            {
                foreach (restaurant_user ru in authUser.restaurant_user)
                {
                    restoList.Add(ru.restaurant);
                }
            }
            else if (authUser.user_role == (int)SiteRoles.Admin ||
                    authUser.user_role == (int)SiteRoles.Developer)
            {
                restoList = db.restaurants.ToList<restaurant>();
            }
            

            return restoList;
        }

    }
}
