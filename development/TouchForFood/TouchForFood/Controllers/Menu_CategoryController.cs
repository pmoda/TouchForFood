using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Attributes;
using System.Diagnostics;
using System.Text;
using TouchForFood.Mappers;
using TouchForFood.Exceptions;
using System.Data.Entity.Infrastructure;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;
using TouchForFood.Util.Search;
namespace TouchForFood.Controllers
{ 
    public class Menu_CategoryController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private MenuCategoryIM im;
        private MenuCategoryOM om;

        public Menu_CategoryController() 
        {
            im = new MenuCategoryIM(db);
            om = new MenuCategoryOM(db);
        }
        public Menu_CategoryController(touch_for_foodEntities aDB):this()
        {
            db = aDB;
        }

        //
        // GET: /Menu_Category/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index()
        {
            return View(im.find(true,false));
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public PartialViewResult OnTheMenu(int id)
        {            
            //var menu_category = db.menu_category.Include(m => m.category).Include(m => m.menu);
            return PartialView("_OnTheMenu", im.find(false, id));
        }

        //
        // GET: /Menu_Category/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            HttpContext.Session["editingMenuCatId"] = id;
            return View(im.find(id));
        }

        //
        // GET: /Menu_Category/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.categories, "id", "name");
            ViewBag.menu_id = new SelectList(db.menus, "id", "name");
            return View();
        } 

        //
        // POST: /Menu_Category/Create
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create(menu_category menu_category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Create(menu_category))
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.category_id = new SelectList(db.categories, "id", "name", menu_category.category_id);
            ViewBag.menu_id = new SelectList(db.menus, "id", "name", menu_category.menu_id);
            return View(menu_category);
        }
        
        //
        // GET: /Menu_Category/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            menu_category menu_category = im.find(id);
            ViewBag.category_id = new SelectList(db.categories, "id", "name", menu_category.category_id);
            ViewBag.menu_id = new SelectList(db.menus, "id", "name", menu_category.menu_id);
            return View(menu_category);
        }

        //
        // POST: /Menu_Category/Edit/5
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(menu_category menu_category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(menu_category))
                        return RedirectToAction("Index");
                    else
                    {
                        ViewBag.Error = Global.VersioningError;
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                }  
            }
            ViewBag.category_id = new SelectList(db.categories, "id", "name", menu_category.category_id);
            ViewBag.menu_id = new SelectList(db.menus, "id", "name", menu_category.menu_id);
            return View(menu_category);
        }

        //
        // GET: /Menu_Category/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            StringBuilder sb = new StringBuilder();
            menu_category menu_category = db.menu_category.Find(id);

            //if menu is active, then return to the delete page but with a warning
            if (menu_category.is_active)
            {
                sb.Append(Global.CategoryActiveError);
            }
          
            //if there is a menu item associate to the menu category pass along a warning message
            if (menu_category.menu_item.Count > 0)
            {
                sb.Append(Global.MenuCategoryAssociationWarning);
            }
            ViewBag.Warning = sb.ToString();
            return View(menu_category);
        }

        //
        // POST: /Menu_Category/Delete/5
        [HttpPost, ActionName("Delete"), Ajax(false)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {    
            deleteAction(id);
            return RedirectToAction("Index");
        }

        [HttpPost, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public int AjaxDelete(int id)
        {
            int result = deleteAction(id);
            
            //if it was a successful deletion result should now be at least 1.
            if (result >= 1)
            {
                //result will now contain the id of the row that was just deleted so the user can be visually updated
                result = id;
            }

            //CHECK ELSE RESULT == 0
            // Return message saying that the menu is actice, de activate menu before modyfying
            return result;
        }

     

        [HttpPost, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        /**
         * Removes the menu item from the the menu.
         * 
         * */
        public int RemoveMenuItem(int id)
        {
            int result = 0;
            MenuItemOM om = new MenuItemOM(db);
            
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


        /// <summary>
        ///Associates a menu category category with an item, thus creating a menu item.
        ///Returns a partial view of the newly updated category table view.
        /// </summary>
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult AddItem(int menu_cat_id, int itemID, decimal price)
        {
            menu_category menu_category = db.menu_category.Find(menu_cat_id);
            menu_item menu_item = new Models.menu_item();
            menu_item.item_id = itemID;
            menu_item.menu_category_id = menu_cat_id;
            menu_item.price = price;
            menu_item.is_active = menu_category.is_active;

            try
            {
                db.menu_item.Add(menu_item);
                db.SaveChanges();
            }
            catch (System.InvalidOperationException e)
            {
                ViewBag.error = e.Message;
            }

            // Load search data into memory
            SearchUtil su = new SearchUtil();
            su.ClearAndFill();

            return RedirectToAction("Details", menu_category);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /**
        * The logic for the deleting
        * */
        private int deleteAction(int menu_category)
        {
            int result = 0;
            try
            {
                //delegate to the output mapper for the deletion
                result = om.delete(menu_category);
                ViewBag.Message = Global.MenuDeleteSuccessMessage;
            }
            catch (ItemActiveException e)
            {
                ViewBag.Error = e.Message;
            }
            return result;
        }

        //
        // GET: /Side/PickSides/5

        public PartialViewResult PickSides(int id)
        {
            menu_category menu_category = im.find(id);
            return PartialView(menu_category);
        }
    }
}