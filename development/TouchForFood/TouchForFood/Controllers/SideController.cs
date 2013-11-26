using System;
using System.Linq;
using System.Web.Mvc;
using TouchForFood.Attributes;
using TouchForFood.Exceptions;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using TouchForFood.ViewModels;
using TouchForFood.App_GlobalResources;
using TouchForFood.Util.Side;

namespace TouchForFood.Controllers
{ 
    public class SideController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();

        private SideIM im;
        private SideOM om;
        public SideController()
        {
            im = new SideIM(db);
            om = new SideOM(db);
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public PartialViewResult FilterSides(menu_category menu_cat)
        {
            SideFilterVM iFilter = new SideFilterVM(menu_cat, SideUtil.FilterListBySide(menu_cat)); //(menu_cat, SideUtil.FilterListBySide(menu_cat));
            return PartialView("_SideTable", iFilter);
        }

        //
        // GET: /Side/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index()
        {            
            return View(im.find(true, false));
        }

        //
        // GET: /Side/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Side/Create
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id");
            return View();
        } 

        //
        // POST: /Side/Create

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Create(side side)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Create(side))
                    {
                        return RedirectToAction("Index");
                    }                    
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id", side.menu_category_id);
            return View(side);
        }

        [HttpGet, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult CreatePartial(int cat_id)
        {
            SideFilterVM sideFilter = new SideFilterVM();
            menu_category c = db.menu_category.Find(cat_id);
            sideFilter.menu_cat = c;
            sideFilter.AddSide(new side());
            return PartialView("_SideCreate", sideFilter);
        }
        
        //
        // GET: /Side/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            side side = im.find(id);
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id", side.menu_category_id);
            return View(side);
        }

        //
        // POST: /Side/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        [HttpPost]
        public ActionResult Edit(side side)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Edit(side))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = Global.VersioningError;
                    }
                }
                catch (InvalidOperationException)
                {
                    ViewBag.Error = Global.VersioningError;
                }                
            }
            ViewBag.menu_category_id = new SelectList(db.menu_category, "id", "id", side.menu_category_id);
            return View(side);
        }

        //
        // GET: /Side/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            side side = im.find(id);
            return View(side);
        }

        //
        // POST: /Side/Delete/5
        [HttpPost, ActionName("Delete")]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {
            DeleteAction(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public bool AddSideToCategory(int menuCategoryId, bool isDeleted, bool isActive, SideFilterVM sideVM)
        {
            bool isSuccess = false;
            side side = sideVM.FirstOrDefault();

            side.menu_category = db.menu_category.Find(menuCategoryId);
            side.is_deleted = isDeleted;
            side.is_active = isActive;

            if (side.name != "" && menuCategoryId != 0)
            {
                db.sides.Add(side);
                db.SaveChanges();
                isSuccess = true;
            }

            if (isSuccess)
            {
                HttpContext.Session["message"] = side.name + " successfuly added.";
            }
            else
            {
                HttpContext.Session["error"] = side.name + " could not be added.";
            }

            return false;
        }

        [HttpPost, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public int AjaxDelete(int id)
        {
            int result = DeleteAction(id);

            //if it was a successful deletion result should now be at least 1.
            if (result >= 1)
            {
                //result will now contain the id of the row that was just deleted so the user can be visually updated
                result = id;
            }
            return result;
        }

        [HttpPost, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public int AjaxActive(int id, bool isActive)
        {
            int result = ActiveAction(id, isActive);

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

        #region Private
        
        private int DeleteAction(int sideId)
        {
            int result = 0;
            try
            {
                //delegate to the output mapper for the deletion
                result = om.delete(sideId);
                ViewBag.Message = "Side is now deleted";
            }
            catch (ItemActiveException e)
            {
                ViewBag.Error = e.Message;
            }
            return result;
        }

        private int ActiveAction(int sideId, bool isActive)
        {
            int result = 0;
            
            //delegate to the output mapper for the deletion
            side oldSide = im.find(sideId);
            result = om.SetActive(oldSide, isActive);
            if (isActive)
            {
                ViewBag.Message = "Side is now active"; // I'm putting these in the string table later, I'm paranoid for my merge atm.
            }
            else
            {
                ViewBag.Message = "Side is now deactivated";
            }
            
            return result;
        }

        #endregion
    }
}