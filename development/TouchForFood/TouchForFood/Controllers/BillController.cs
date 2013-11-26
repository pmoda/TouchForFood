using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using TouchForFood.Attributes;
using TouchForFood.Mappers;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
    public class BillController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        const string lockError = "This bill has already been updated somewhere else. Please refresh and try again";

        private BillIM im;
        private BillOM om;
        private OrderIM orderIM;
        private OrderOM orderOM;
        private Order_ItemIM order_itemIM;
        private Order_ItemOM order_itemOM;


        public BillController()
        {
            im = new BillIM(db);
            om = new BillOM(db);
            orderIM = new OrderIM(db);
            orderOM = new OrderOM(db);
            order_itemIM = new Order_ItemIM(db);
            order_itemOM = new Order_ItemOM(db);
        }

        //
        // GET: /Bill/

        public ViewResult Index()
        {
            var bills = db.bills.Include(b => b.order);
            return View(bills.ToList());
        }

        //
        // GET: /Bill/Details/5

        public ViewResult Details(int id)
        {
            bill bill = im.find(id);
            return View(bill);
        }

        //
        // GET: /Bill/Create

        public ActionResult Create()
        {
            ViewBag.order_id = new SelectList(db.orders, "id", "id");
            return View();
        } 

        //
        // POST: /Bill/Create

        [HttpPost]
        public ActionResult Create(bill bill)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.bills.Add(bill);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            ViewBag.order_id = new SelectList(db.orders, "id", "id", bill.order_id);
            return View(bill);
        }
        
        //
        // POST: /Bill/CreateBillForOrder/5 (menu_item id)
        [HttpPost, Ajax(true), Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer | SiteRoles.Customer)]
        public int CreateBillForOrder(int id)
        {
            
            order theOrder = orderIM.find(id);
            if (!Util.Bill.BillUtil.CheckItemsRemaining(theOrder))
                return 0;
            bill newBill = new bill();
            newBill.order_id = id;
            newBill.paid = false;
            newBill.timestamp = DateTime.Now;
            newBill.total = 0;
            newBill.tps = 0;
            newBill.tvq = 0;
            Create(newBill);

            return (newBill.id > 0) ? 1 : 0;
        }

        //
        // POST: /Bill/AddOrderItemToBill/5&billId=
        [HttpPost, Ajax(true), Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer | SiteRoles.Customer)]
        public int AddOrderItemToBill(int? billId, int? orderItemId)
        {
            bill theBill = im.find(billId);
            order_item oi = order_itemIM.find(orderItemId);
            if (theBill == null || oi == null) return 0;
            oi.bill_id = theBill.id;
            order_itemOM.edit(oi);

            theBill = im.find(theBill.id);
            Util.Bill.BillUtil.Update(ref theBill);
            om.edit(theBill);

            return 1;
        }

        //
        // POST: /Bill/DeleteBillFromOrder/5 (menu_item id)
        [HttpPost, Ajax(true), Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer | SiteRoles.Customer)]
        public int DeleteBillFromOrder(int id)
        {
            om.delete(id);
            return (im.find(id) == null) ? 1 : 0;
        }

        public ActionResult RemoveOrderItem(int id)
        {
            order_item oi = order_itemIM.find(id);
            if (oi == null) return RedirectToAction("Index");

            int bill_id = (int)oi.bill_id;
            oi.bill_id = null;
            order_itemOM.edit(oi);

            bill theBill = im.find(bill_id);
            Util.Bill.BillUtil.Update(ref theBill);
            om.edit(theBill);

            return RedirectToAction("ManageBills", "Bill", new { id = theBill.order_id });
        }

        public ActionResult BillPaid(int id)
        {
            bill bill = db.bills.Find(id);
            bill.paid = true;
            List<order_item> temp = new List<order_item>();
            temp.AddRange(bill.order_item);
            foreach (order_item oi in temp)
            {
                oi.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.PAID;
                order_itemOM.edit(oi);
            }
            
            if(Util.Bill.BillUtil.CheckFullyPaid(bill)){
                order o = bill.order;
                o.order_status = (int)OrderStatusHelper.OrderStatusEnum.COMPLETE;
                orderOM.edit(o);
            }
            om.edit(bill);
            return RedirectToAction("ManageBills", "Bill", new { id = bill.order_id});
        }

        public ActionResult BillNotPaid(int id)
        {
            bill bill = db.bills.Find(id);
            bill.paid = false;
            List<order_item> temp = new List<order_item>();
            temp.AddRange(bill.order_item);
            foreach (order_item oi in temp)
            {
                oi.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.DELIVERED;
                order_itemOM.edit(oi);
            }
            order o = bill.order;
            o.order_status = (int)OrderStatusHelper.OrderStatusEnum.PLACED;
            orderOM.edit(o);
            om.edit(bill);
            return RedirectToAction("ManageBills", "Bill", new { id = bill.order_id });
        }

        //
        // GET: /Bill/Edit/5
 
        public ActionResult Edit(int id)
        {
            bill bill = im.find(id);
            ViewBag.order_id = new SelectList(db.orders, "id", "id", bill.order_id);
            return View(bill);
        }

        //
        // POST: /Bill/Edit/5

        [HttpPost]
        public ActionResult Edit(bill bill)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(bill))
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
            ViewBag.order_id = new SelectList(db.orders, "id", "id", bill.order_id);
            return View(bill);
        }

        //
        // GET: /Bill/ManageBills/5
        public ActionResult ManageBills(int id)
        {
            order order = orderIM.find(id);
            return View("ManageBills", order);
        }

        //
        // GET: /Bill/Delete/5
 
        public ActionResult Delete(int id)
        {
            bill bill = im.find(id);
            return View(bill);
        }

        //
        // POST: /Bill/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            om.delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}