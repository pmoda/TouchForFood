using System.Data;
using System.Web.Mvc;
using TouchForFood.App_GlobalResources;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Util.Review;
using TouchForFood.ViewModels;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using TouchForFood.Util;
namespace TouchForFood.Controllers
{
    public class ReviewController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private ReviewIM im;
        private ReviewOM om;        
        public ReviewController()
        {
            im = new ReviewIM(db);
            om = new ReviewOM(db);
        }
        //
        // GET: /Review/

        public ViewResult Index()
        {
            return View(im.find());
        }


        //
        // GET: /Review/Details/5

        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Review/Create

        public ActionResult Create(int orderID)
        {

            ReviewVM reviewVM;
            review review = new review();
            OrderIM oim = new OrderIM(db);

            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

            if (authUser == null)
            {
                return RedirectToAction("Index", "User");
            }
            review.user_id = authUser.id;

            // Look for a valid order before we continue
            order order = oim.find(orderID);
            if (order == null)
            {
                return RedirectToAction("Index", "User");
            }
            review.order_id = order.id;
            if (order.table != null)
            {
                review.restaurant_id = order.table.restaurant_id.Value;
            }
            else
            {
                //TODO: this must be fixed after CAP-480
                review.restaurant_id = db.restaurants.First().id;
            }
            reviewVM = new ReviewVM(review, order);
            Session.Add(Global.ReviewVMSession, reviewVM);
            ViewBag.ratings = new SelectList(Rating.GetRatings(), "Value", "Text");

            return View(reviewVM);
        }

        //
        // POST: /Review/Create

        [HttpPost, ValidateInput(false)]
        public ActionResult Create(ReviewVM review)
        {
            //add the new value to the view VM
            ReviewVM reviewVM = (ReviewVM)Session[Global.ReviewVMSession];
            Review_Order_ItemOM rom = new Review_Order_ItemOM(db);
            if (reviewVM == null)
            {
                ViewBag.error(Global.Review_Create_Error);
                return RedirectToAction("Index", "User");
            }

            //what we have from the user submitted information is the anonymous flag, a rating for each item as well as the review text
            Rating.GetRatingReviewOrderItems(reviewVM.review_order_items, review.review_order_items);
            TextParser.ParseReviewText(reviewVM.review_order_items, review.reviewText);
            TextParser.SetDate(reviewVM.review_order_items);
            
            reviewVM.review.is_anonymous = review.is_anonymous;
            reviewVM.review.rating = Rating.CalculateAverageRating(reviewVM.review_order_items);
            //have to set this to null and only associate the ID. This is a weird behaviour. 
            //If I don't it creates a new order and new order items. This is done in the foreach below
            //No clue why this is happening?
            reviewVM.review.order = null;
            bool result = false;
            bool exceptionThrown = false;
            if (ModelState.IsValid)
            {
                try
                {
                    result = om.Create(reviewVM.review);
                    List<review_order_item> roiList = reviewVM.review_order_items;
                    foreach (review_order_item roi in roiList)
                    {
                        if (!string.IsNullOrEmpty(roi.review_text))
                        {
                            roi.review_id = reviewVM.review.id;
                            roi.order_item = null;
                            rom.Create(roi);
                        }
                    }
                }
                catch (Exception) { exceptionThrown = true; }
            }
            if (result)
            {
                ViewBag.order_id = new SelectList(db.orders, "id", "id", reviewVM.review.order_id);
                ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", reviewVM.review.restaurant_id);
                ViewBag.user_id = new SelectList(db.users, "id", "username", reviewVM.review.user_id);
                ViewBag.ratings = new SelectList(Rating.GetRatings(), "Value", "Text");
                //succesfully created a review, can remove the session one
                Session.Remove(Global.ReviewVMSession);
            }
            else
            {
                ViewBag.error(exceptionThrown ? Global.ServerError : Global.Review_Create_Error);
            }

            return RedirectToAction("Details", "User", new { id = reviewVM.review.user_id });
        }

        private JsonResult GetOrderItemData(int orderID)
        {
            OrderIM oim = new OrderIM(db);
            order o = oim.find(orderID);
            List<order_item> orderItems = new List<order_item>();
            foreach (order_item oi in o.order_item)
            {
                orderItems.Add(oi);
            }
            return Json(orderItems);
        }


        //
        // GET: /Review/Edit/5

        public ActionResult Edit(int id)
        {
            review review = db.reviews.Find(id);
            ViewBag.order_id = new SelectList(db.orders, "id", "id", review.order_id);
            ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", review.restaurant_id);
            ViewBag.user_id = new SelectList(db.users, "id", "username", review.user_id);
            return View(review);
        }

        //
        // POST: /Review/Edit/5

        [HttpPost]
        public ActionResult Edit(review review)
        {
            if (ModelState.IsValid)
            {
                db.Entry(review).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.order_id = new SelectList(db.orders, "id", "id", review.order_id);
            ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", review.restaurant_id);
            ViewBag.user_id = new SelectList(db.users, "id", "username", review.user_id);
            return View(review);
        }

        //
        // GET: /Review/Delete/5
        public ActionResult Delete(int id)
        {
            review review = db.reviews.Find(id);
            return View(review);
        }

        //
        // POST: /Review/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            review review = db.reviews.Find(id);
            foreach (review_order_item roi in db.reviews.Find(review.id).review_order_item)
            {
                db.review_order_item.Remove(roi);
                db.SaveChanges();
            }
            db.reviews.Remove(review);
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