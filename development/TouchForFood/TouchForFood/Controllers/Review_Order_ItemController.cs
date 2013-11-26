using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Mappers;
using TouchForFood.Attributes;
using TouchForFood.Util.Security;
using TouchForFood.Exceptions;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
    public class Review_Order_ItemController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private Review_Order_ItemIM im;
        private Review_Order_ItemOM om;

        public Review_Order_ItemController() 
        {
            im = new Review_Order_ItemIM(db);
            om = new Review_Order_ItemOM(db);
        }
        //
        // GET: /Review_Order_Item/

        public ViewResult Index()
        {
            return View(im.find());
        }

        //
        // GET: /Review_Order_Item/Details/5

        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Review_Order_Item/Create

        public ActionResult Create()
        {
            ViewBag.order_item_id = new SelectList(db.order_item, "id", "note");
            ViewBag.review_id = new SelectList(db.reviews, "id", "id");
            return View();
        } 

        //
        // POST: /Review_Order_Item/Create

        [HttpPost]
        public ActionResult Create(review_order_item review_order_item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.review_order_item.Add(review_order_item);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.order_item_id = new SelectList(db.order_item, "id", "note", review_order_item.order_item_id);
            ViewBag.review_id = new SelectList(db.reviews, "id", "id", review_order_item.review_id);
            return View(review_order_item);
        }
        
        //
        // GET: /Review_Order_Item/Edit/5
 
        public ActionResult Edit(int id)
        {
            review_order_item review_order_item = db.review_order_item.Find(id);
            ViewBag.order_item_id = new SelectList(db.order_item, "id", "note", review_order_item.order_item_id);
            ViewBag.review_id = new SelectList(db.reviews, "id", "id", review_order_item.review_id);
            return View(review_order_item);
        }

        //
        // POST: /Review_Order_Item/Edit/5

        [HttpPost]
        public ActionResult Edit(review_order_item review_order_item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review_order_item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.order_item_id = new SelectList(db.order_item, "id", "note", review_order_item.order_item_id);
            ViewBag.review_id = new SelectList(db.reviews, "id", "id", review_order_item.review_id);
            return View(review_order_item);
        }

        //
        // GET: /Review_Order_Item/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Review_Order_Item/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            int result = om.delete(id);
            if (result > 1)
            {
                //TODO have a success message
            }
            else
            {
                //TODO have an error message
            }
            return RedirectToAction("Index");
        }

        [HttpPost, Ajax(true)]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer | SiteRoles.Customer)]
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

        private int deleteAction(int id)
        {
            int result = 0;
            try
            {
                //delegate to the output mapper for the deletion
                result = om.delete(id);
                ViewBag.Message = Global.ReviewOrderItemDeleted;
            }
            catch (Exception)
            {
                ViewBag.Error = Global.ServerError;
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}