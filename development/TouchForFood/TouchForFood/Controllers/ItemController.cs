using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Util.Item;
using TouchForFood.Attributes;
using System.Data.Entity.Infrastructure;
using TouchForFood.Mappers;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;
using TouchForFood.Util.Search;

namespace TouchForFood.Controllers
{ 
    public class ItemController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private ItemIM im;
        private ItemOM om;

        public ItemController()
        {
            im = new ItemIM(db);
            om = new ItemOM(db);
        }

        public ItemController(touch_for_foodEntities aDB): this()
        {
            db = aDB;
        }

        //
        // GET: /Item/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index()
        {
            return View(im.find());
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public PartialViewResult FilterItems(menu_category menu_cat)
        {
            ItemFilterVM iFilter = new ItemFilterVM(menu_cat, ItemUtil.filterListByItem(menu_cat,db));
            return PartialView("_ItemTable", iFilter);
        }

        //
        // GET: /Item/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Item/View/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult View(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Item/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.category_id = new SelectList(db.categories, "id", "name");
            return View();
        }

        [HttpGet, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult CreatePartial(int cat_id)
        {
            ItemFilterVM iFilter = new ItemFilterVM();
            menu_category c = db.menu_category.Find(cat_id);
            iFilter.menu_cat = c;
            iFilter.addItem(new item());
            return PartialView("_ItemCreate", iFilter);
        }

        //
        // POST: /Item/Create
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create(item item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Create(item))
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.category_id = new SelectList(db.categories, "id", "name", item.category_id);
            return View(item);
        }
        
        //
        // GET: /Item/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            item item = im.find(id);
            ViewBag.category_id = new SelectList(db.categories, "id", "name", item.category_id);
            return View(item);
        }

        //
        // POST: /Item/Edit/5
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(item item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(item))
                        return RedirectToAction("Details", "Menu_Category", new { id = HttpContext.Session["editingMenuCatId"] });
                    else
                    {
                        ViewBag.Error = "This record has already been updated. Please refresh and try again";
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                }
            }
            ViewBag.category_id = new SelectList(db.categories, "id", "name", item.category_id);
            return View(item);
        }

        //
        // GET: /Item/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            item item = im.find(id);
            return View(item);
        }

        //
        // POST: /Item/Delete/5
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public bool addItemToMenu(int menu_cat_id, ItemFilterVM i_filter, decimal price)
        {
            bool isSuccess = false;
            item item = i_filter.items.First();
            menu_category mc = db.menu_category.Find(menu_cat_id);
            menu_item mi = new menu_item();

            //Initialize values
            mi.item_id = item.id;
            mi.menu_category_id = menu_cat_id;
            item.category_id = db.menu_category.Find(menu_cat_id).category_id;
            mi.price = price;
            //Inherit parent active status
            mi.is_active = mc.is_active;

            if (item.name != "" && menu_cat_id != 0)
            {
                db.items.Add(item);
                db.menu_item.Add(mi);
                db.SaveChanges();
                isSuccess = true;
            }
            
            if (isSuccess)
            {
                HttpContext.Session["message"] = item.name + " successfuly added.";
                // Load search data into memory
                SearchUtil su = new SearchUtil();
                su.ClearAndFill();
            }
            else
            {
                HttpContext.Session["error"] = item.name + " could not be added.";
            }

            return isSuccess;
        }


        //
        // GET: /Item/PartialDetails/5

        public PartialViewResult PartialDetails(int id)
        {
            return PartialView(im.find(id));
        }

    }
}