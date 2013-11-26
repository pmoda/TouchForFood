using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.ViewModels;
using System.Data.Entity.Infrastructure;
using TouchForFood.Util.Security;
using TouchForFood.Mappers;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{
    public class Order_ItemController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        const string lockError = "This order item has already been updated somewhere else. Please refresh and try again";

        private Order_ItemOM om;
        private Order_ItemIM im;
        private OrderOM orderOM;

        public Order_ItemController()
        {
            om = new Order_ItemOM(db);
            im = new Order_ItemIM(db);
            orderOM = new OrderOM(db);
        }
        //
        // GET: /Order_Item/

        public ViewResult Index()
        {
            var order_item = db.order_item.Include(o => o.menu_item).Include(o => o.order).Include(o => o.order_item_status);
            return View(order_item.ToList());
        }

        //
        // GET: /Order_Item/Details/5

        public ViewResult Details(int id)
        {
            order_item order_item = im.find(id);
            return View(order_item);
        }

        //
        // GET: /Order_Item/Create

        public ActionResult Create()
        {
            ViewBag.menu_item_id = new SelectList(db.menu_item, "id", "id");
            ViewBag.order_id = new SelectList(db.orders, "id", "id");
            return View();
        }

        //
        // POST: /Order_Item/Create

        [HttpPost]
        public ActionResult Create(order_item order_item)
        {
            if (ModelState.IsValid)
            {
                db.order_item.Add(order_item);
                db.SaveChanges();
                orderOM.edit(db.orders.Find(order_item.order_id));
                return RedirectToAction("Index");
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.menu_item_id = new SelectList(db.menu_item, "id", "id", order_item.menu_item_id);
            ViewBag.order_id = new SelectList(db.orders, "id", "id", order_item.order_id);
            return View(order_item);
        }

        //
        // GET: /Order_Item/Edit/5

        public ActionResult Edit(int id)
        {
            order_item order_item = im.find(id);
            ViewBag.menu_item_id = new SelectList(db.menu_item, "id", "id", order_item.menu_item_id);
            ViewBag.order_id = new SelectList(db.orders, "id", "id", order_item.order_id);
            return View(order_item);
        }

        //
        // POST: /Order_Item/Edit/5

        [HttpPost]
        public ActionResult Edit(order_item order_item)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(order_item))
                        return RedirectToAction("Index");
                    else
                    {
                        ViewBag.Error = lockError;
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

            ViewBag.menu_item_id = new SelectList(db.menu_item, "id", "id", order_item.menu_item_id);
            ViewBag.order_id = new SelectList(db.orders, "id", "id", order_item.order_id);
            return View(order_item);
        }

        //
        // GET: /Order_Item/Accept/5

        public ActionResult Accept(int id)
        {
            int? tableID = -1;
            try
            {
                order_item order_item = db.order_item.Find(id);
                order_item.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.PROCESSING;
                om.edit(order_item);
                tableID = order_item.order.table_id;
                if (Util.Bill.BillUtil.CheckProcessing(order_item.order))
                {
                    order_item.order.order_status = (int)OrderStatusHelper.OrderStatusEnum.PROCESSING;
                }
                orderOM.edit(order_item.order);
            }
            catch (Exception)
            {
                ViewBag.error = Global.Error_Message;
            }
            return RedirectToAction("ManageIndex", "Table", new { id = tableID });
        }
        //
        // GET: /Order_Item/Delete/5

        public ActionResult Delete(int id)
        {
            order_item order_item = im.find(id);
            return View(order_item);
        }

        //
        // POST: /Order_Item/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                order_item order_item = db.order_item.Find(id);
                order o = order_item.order;
                db.order_item.Remove(order_item);
                Util.Order.OrderUtil.UpdatePrice(ref o);
                orderOM.edit(o);
            }
            catch (Exception)
            {
                ViewBag.error = Global.Error_Message;
            }
            return RedirectToAction("Index", "Order");
        }

        //
        // GET: /Order_Item/Delivered/5

        public ActionResult Delivered(int id)
        {
            int? tableID = -1;
            try
            {
                order_item order_item = db.order_item.Find(id);
                order_item.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.DELIVERED;
                om.edit(order_item);
                tableID = order_item.order.table_id;
            }
            catch (Exception)
            {
                ViewBag.error = Global.Error_Message;
            }
            return RedirectToAction("ManageIndex", "Table", new { id = tableID });
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult RemoveOrderItem(int id)
        {
            order order = (order)Session["ORDER"];
            order_item toRemove = null;
            if (order != null)
            {
                foreach (order_item oi in order.order_item)
                {
                    if (oi.id == id)
                    {
                        toRemove = oi;
                    }
                }
            }

            if (toRemove != null)
            {
                order.order_item.Remove(toRemove);
            }
            Util.Order.OrderUtil.UpdatePrice(ref order);
            return RedirectToAction("ViewFromSession", "Order");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}