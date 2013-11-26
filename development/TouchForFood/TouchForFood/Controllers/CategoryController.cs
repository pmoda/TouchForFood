using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Util.Category;
using TouchForFood.Attributes;
using System.Data.Entity.Infrastructure;
using TouchForFood.Mappers;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;
//TODO: Make the save of the partial create also associate the current menu and such
namespace TouchForFood.Controllers
{ 
    public class CategoryController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private CategoryIM im;
        private CategoryOM om;
        public CategoryController()
        {
            im = new CategoryIM(db);
            om = new CategoryOM(db);
        }
        //
        // GET: /Category/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index()
        {
            return View(im.find());
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public PartialViewResult FilterCategories(menu menu)
        {
            CategoryFilterVM catFilter = new CategoryFilterVM(menu, CategoryUtil.filterListByMenu(menu,db));
            return PartialView("_CategoryTable", catFilter);
        }

        //
        // GET: /Category/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Category/Create
        [HttpGet, Ajax(false)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create()
        {
            return View();
        }

        //
        // GET: /Category/Create
        //Ajax call will only return a partial view
        [HttpGet, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult CreatePartial(int menu_id)
        {
            MenuIM im = new MenuIM(db);
            CategoryFilterVM catFilter = new CategoryFilterVM();
            menu m = im.find(menu_id);
            catFilter.m_menu = m;
            catFilter.addCategory(new category());
            return PartialView("_CategoryCreate", catFilter);
        }

        //
        // POST: /Category/Create
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create(category category)
        {
            if (ModelState.IsValid && ValidateCategory(category))
            {
                try
                {
                    if (om.Create(category))
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception)
                {}
            }
            ViewBag.Error = Global.ServerError;
            return View(category);
        }

        public bool ValidateCategory(category cat)
        {
            if (String.IsNullOrEmpty(cat.name))
            {
                ViewBag.Error = "Category name cannot be empty";
                return false;
            }
            return true;
        }
        
        //
        // GET: /Category/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Category/Edit/5
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(category category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(category))
                        return RedirectToAction("Details", "Menu", new { id = HttpContext.Session["editingMenuId"] });
                    else
                    {
                        ViewBag.Error = "This record has already been updated. Please refresh and try again";
                        //TODO: delete this message once viewbag implemented on client side
                        //ModelState.AddModelError("name", "This record has already been updated. Please refresh and try again");
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                }
            }
            else
            {
                ViewBag.Error = Global.ServerError;
            }
            return View(category);
        }

        //
        // GET: /Category/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                om.delete(id);
            }
            catch (Exceptions.AssociationExistsException e)
            {
                ViewBag.error = e.Message;
            }
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public bool addCategoryToMenu(int menu_id, CategoryFilterVM cat_filter)
        {
            bool isSuccess = false;
            category category = cat_filter.m_category.First();

            if (ValidateCategory(category))
            {

                menu menu = db.menus.Find(menu_id);

                menu_category mc = new menu_category();
                mc.category_id = category.id;
                mc.menu_id = menu_id;
                //Inherit parent active status
                mc.is_active = menu.is_active;

                if (category.name != "" && menu_id != 0)
                {
                    db.categories.Add(category);
                    db.menu_category.Add(mc);
                    db.SaveChanges();
                    isSuccess = true;
                }
            }

            if (isSuccess)
            {
                HttpContext.Session["message"] = category.name + " successfuly added.";
            }
            else
            {
                HttpContext.Session["error"] = category.name + " could not be added -  " + ViewBag.Error;
            }

            return isSuccess;
        }
      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}