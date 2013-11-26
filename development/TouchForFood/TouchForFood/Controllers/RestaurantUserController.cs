using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;
using System;

namespace TouchForFood.Controllers
{ 
    public class RestaurantUserController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();

        //
        // GET: /RestaurantUser/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Developer)]
        public ViewResult Index()
        {
            var restaurant_user = db.restaurant_user.Include(r => r.restaurant).Include(r => r.user);
            return View(restaurant_user.ToList());
        }

        //
        // GET: /RestaurantUser/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name");
            ViewBag.user_id = new SelectList(db.users.Where(u => u.user_role == (int)SiteRoles.Restaurant
                || u.user_role == (int)SiteRoles.Admin), "id", "username");
            return View();
        } 

        //
        // POST: /RestaurantUser/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Developer)]
        [HttpPost]
        public ActionResult Create(restaurant_user restaurant_user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (restaurant_user.restaurant_id != null && restaurant_user.user_id != null)
                    {
                        db.restaurant_user.Add(restaurant_user);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    ViewBag.Error = Global.RestaurantUserNotSpecifiedError;
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                }
            }

            ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", restaurant_user.restaurant_id);
            ViewBag.user_id = new SelectList(db.users.Where(u => u.user_role == (int)SiteRoles.Restaurant
                || u.user_role == (int)SiteRoles.Admin), "id", "username");
            return View(restaurant_user);
        }

        //
        // GET: /RestaurantUser/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            restaurant_user restaurant_user = db.restaurant_user.Find(id);
            return View(restaurant_user);
        }

        //
        // POST: /RestaurantUser/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Developer)]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            restaurant_user restaurant_user = db.restaurant_user.Find(id);
            db.restaurant_user.Remove(restaurant_user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}