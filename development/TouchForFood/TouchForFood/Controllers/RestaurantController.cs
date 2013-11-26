using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using System.Data.Entity.Infrastructure;
using TouchForFood.Mappers;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
    public class RestaurantController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private RestaurantIM im;
        private RestaurantOM om;

        public RestaurantController() 
        {
            im = new RestaurantIM(db);
            om = new RestaurantOM(db);
        }
        //
        // GET: /Restaurant/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ViewResult Index()
        {
            return View(im.find(false));
        }

        //
        // GET: /Restaurant/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            restaurant restaurant = im.find(id);
            return View(restaurant);
        }

        //
        // GET: /Restaurant/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Restaurant/Create

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ActionResult Create(restaurant restaurant)
        {
            if (ModelState.IsValid)
            {   
                // Try to add the restaurant to the database and save the changes
                // Exception will be thrown in case of errors. Since there is only one exception thrown
                // by the RestaurantOM (of type InvalidOperationException), the same error is thrown for both situations.
                try
                {
                    if (om.Create(restaurant, Util.User.UserUtil.getAuthenticatedUser(Request)))
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (InvalidOperationException e)
                {
                    ViewBag.Error = Global.ServerError;
                    return View(restaurant);
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                    return View(restaurant);
                }

            }
            ViewBag.Error = Global.ServerError;
            return View(restaurant);
        }
        
        //
        // GET: /Restaurant/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Restaurant/Edit/5

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(restaurant restaurant)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(restaurant))
                    {

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = Global.VersioningError;
                        //TODO: delete this message once viewbag implemented on client side
                        //ModelState.AddModelError("name", Global.VersioningError);
                    }
                }
                catch(Exception e)
                {
                    ViewBag.Error = e.Message;
                } 

            }
            return View(restaurant);
        }

        //
        // GET: /Restaurant/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Restaurant/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                om.delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Error = Global.ServerError;
            }
            return View(im.find(id));          
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}