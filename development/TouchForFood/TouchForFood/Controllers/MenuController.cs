using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using TouchForFood.App_GlobalResources;
using TouchForFood.Attributes;
using TouchForFood.Exceptions;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using System.Collections.Generic;
using TouchForFood.Util.Search;
namespace TouchForFood.Controllers
{ 
    public class MenuController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private MenuIM im;
        private MenuOM om;
        public MenuController() 
        {
            im = new MenuIM(db);
            om = new MenuOM(db);
        }
        public MenuController(touch_for_foodEntities aDB):this()
        {
            db = aDB;
        }

        
        //
        // GET: /Menu/  SHOULD CUSTOMER HAVE PERMISSION HERE?
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Index()
        {
            //var menus = db.menus.Include(m => m.restaurant).Include(m => m.menu_category);

            //None of these are correct. Idealy we do  im.find(userId);
            // and get ALL menu in all restaurant owned by this person...
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            //db.restaurant_user.Where(r => r.user_id == authUser.id).ToList();
            //return View(im.find(false, false));
            //return View(im.find(true,false));
            return View(im.findByUser(authUser.id));
        }

        //
        // GET: /Menu/UserMenu/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult UserMenu()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

            if (authUser.current_table_id != null)
            {
                var menus = im.findByRestaurant(authUser.table.restaurant.id);
                return View(menus);
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Menu/Order/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Order(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Menu/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            HttpContext.Session["editingMenuId"] = id;
            return View(im.find(id));
        }

        //
        // GET: /Menu/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.resto_id = new SelectList(db.restaurants, "id", "name",db.restaurants.First().id);
            return View();
        } 

        //
        // POST: /Menu/Create
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create(menu menu)
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser.restaurant_user.Count() != 0)
            {
                menu.resto_id = (int)authUser.restaurant_user.First().restaurant_id;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Create(menu))
                    {
                        return RedirectToAction("Details", menu);
                    }
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.resto_id = new SelectList(db.restaurants, "id", "name", menu.resto_id);
            return View(menu);
        }
        
        //
        // GET: /Menu/Edit/5

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)] 
        public ActionResult Edit(int id)
        {
            menu menu = im.find(id);
            ViewBag.resto_id = new SelectList(db.restaurants, "id", "name", menu.resto_id);
            return View(menu);
        }

        //
        // POST: /Menu/Edit/5

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(menu menu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(menu))
                        return RedirectToAction("Index");
                    else
                    {
                        ViewBag.Error =Global.VersioningError;
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                }        
            }
            ViewBag.resto_id = new SelectList(db.restaurants, "id", "name", menu.resto_id);
            return View(menu);
        }

        //
        // GET: /Menu/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            StringBuilder sb = new StringBuilder();
            menu menu =im.find(id);

            //if menu is active, then return to the delete page but with a warning
            if (menu.is_active)
            {
                sb.Append(Global.MenuActiveWarning);
            }            
            //if the menu has associated menu categories, post a message to warn the user
            if (menu.menu_category.Count > 0)
            {
                sb.Append(Global.MenuAssociationWarning);
            }

            ViewBag.Warning = sb.ToString();          
            return View(menu);
        }

        //
        // POST: /Menu/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {            
            try
            {
                //delegate to the output mapper for the deletion
                om.delete(id);
                ViewBag.Message = Global.MenuDeleteSuccessMessage;
            }
            catch (ItemActiveException e)
            {
                ViewBag.Error = e.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        /**
         * Removes the menu category from the the menu.
         * 
         * */
        public int RemoveMenuCategory(int id)
        {
            int result = 0;
            MenuCategoryOM om = new MenuCategoryOM(db);
            try
            {
                result = om.delete(id);
            }
            catch (Exceptions.ItemActiveException e)
            {
                ViewBag.error = e.Message;
            }
            return result;
        }

        /**
         * Associates a category with a menu, thus creating a menu category.
         * Returns a partial view of the newly updated category table view.
         * **/
        [HttpPost,Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public void AddCategory(int menuID, int categoryID)
        {
            menu menu = db.menus.Find(menuID);
            menu_category menu_category = new Models.menu_category();
            menu_category.category_id = categoryID;
            menu_category.menu_id = menuID;
            menu_category.is_active = menu.is_active;
            try
            {
                db.menu_category.Add(menu_category);
                db.SaveChanges();
            }
            catch (System.InvalidOperationException)
            {

            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        //
        // POST: /Menu/toggleActiveMenu/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult toggleActiveMenu(int id)
        {
            menu menu = im.find(id);
            IList<menu> menu_list = menu.restaurant.menus.ToList();
            for (int i = 0; i < menu_list.Count; i++)
            {
                menu resto_menu = menu_list.ElementAt(i);
                if (resto_menu.id != id)
                {
                    resto_menu.is_active = false;
                    if (!om.edit(resto_menu))                    
                    {
                        ViewBag.Error = Global.VersioningError;
                        return RedirectToAction("Index");
                    }
                }
            }
            menu.is_active = !menu.is_active;
            if (!om.edit(menu))
            {
                ViewBag.Error = Global.VersioningError;
            }

            // Load search data into memory
            SearchUtil su = new SearchUtil();
            su.ClearAndFill();

            return  RedirectToAction("Index");
        }
    }
}