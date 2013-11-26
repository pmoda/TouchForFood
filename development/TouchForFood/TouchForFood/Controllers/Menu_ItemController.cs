using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Attributes;
using System.Data.Entity.Infrastructure;
using TouchForFood.Mappers;
using TouchForFood.Exceptions;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
    public class Menu_ItemController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private MenuItemIM im;
        private MenuItemOM om;
        //
        // GET: /Menu_Item/
        public Menu_ItemController() 
        {
            im = new MenuItemIM(db);
            om = new MenuItemOM(db);
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index()
        {
            //var menu_item = db.menu_item.Include(m => m.item).Include(m => m.menu_category);
            return View(im.find(true,false));
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public PartialViewResult OnTheMenu(int id)
        {
            //var menu_category = db.menu_category.Include(m => m.category).Include(m => m.menu);
            return PartialView("_OnTheMenu", im.find(false, id));
        }

        //
        // GET: /Menu_Item/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        public PartialViewResult PickSides(int id)
        {            
            return PartialView(im.find(id));
        }

        //
        // GET: /Menu_Item/PartialDetails/5

        public PartialViewResult PartialDetails(int id)
        {
            return PartialView(im.find(id));
        }

        //
        // GET: /Menu_Item/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.item_id = new SelectList(db.items, "id", "name");
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id");
            return View();
        } 

        //
        // POST: /Menu_Item/Create
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create(menu_item menu_item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Create(menu_item))
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception) 
                {
                    ViewBag.error = Global.MenuItemCreateError;
                }
            }
            
            ViewBag.Error = Global.Error_Title;
            ViewBag.item_id = new SelectList(db.items, "id", "name", menu_item.item_id);
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id", menu_item.menu_category_id);
            return View(menu_item);
        }
        
        //
        // GET: /Menu_Item/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            menu_item menu_item =im.find(id);
            ViewBag.item_id = new SelectList(db.items, "id", "name", menu_item.item_id);
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id", menu_item.menu_category_id);
            return View(menu_item);
        }

        //
        // POST: /Menu_Item/Edit/5
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(menu_item menu_item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(menu_item))
                        return RedirectToAction("Index");
                    else
                    {
                        ViewBag.Error = Global.LockError;
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
            if (ViewBag.Error == null)
            {
                ViewBag.Error = Global.Error_Title;
            }
            ViewBag.item_id = new SelectList(db.items, "id", "name", menu_item.item_id);
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id", menu_item.menu_category_id);
            return View(menu_item);
        }

         public PartialViewResult GetAllReviews(menu_item mi)
         {
             List<review_order_item> reviews = im.find(mi.id).order_item.SelectMany(oi => oi.review_order_item)
                                                             .OrderByDescending(r => r.submitted_on)
                                                             .ToList<review_order_item>();

             return PartialView("_PastReviewsPartial", reviews);
         }

        //
        // GET: /Menu_Item/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Menu_Item/Delete/5
        [HttpPost, ActionName("Delete")]
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
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /**
        * The logic for the deleting
        * */
        private int deleteAction(int menu_item)
        {
            
            int result = 0;
            try
            {
                //delegate to the output mapper for the deletion
                result = om.delete(menu_item);
                ViewBag.Message = Global.MenuItemDeleteSuccessMessage;
            }
            catch (ItemActiveException e)
            {
                ViewBag.Error = e.Message;
            }
            return result;
        }
    }
}