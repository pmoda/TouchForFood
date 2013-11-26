using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Attributes;
using TouchForFood.ViewModels;
using System.Data.Entity.Infrastructure;
using TouchForFood.Util.Security;
using TouchForFood.Mappers;
using TouchForFood.Exceptions;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
    public class OrderController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();        
        private OrderIM im;
        private OrderOM om;
        private Order_ItemOM order_itemOM;
        private MenuItemOM menu_itemOM;
        //
        // GET: /Order/

        public OrderController()
        {
            im = new OrderIM(db);
            om = new OrderOM(db);
            order_itemOM = new Order_ItemOM(db);
            menu_itemOM = new MenuItemOM(db);
        }

        public OrderController(touch_for_foodEntities aDb):this()
        {
            db = aDb;
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index(int? id)
        {
            //TODO: why are all these includes here?? the point of entity framework is that you can navigate from
            //relation to relation without these includes
            var models = db.orders
                .Where(p => p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PLACED ||
                            p.order_status == (int)OrderStatusHelper.OrderStatusEnum.EDITING)
                .Where(t => t.id == id)
                .ToList();

            return View("Index",models);
        }

        public PartialViewResult FilterOrder(table table)
        {
            var pendingOrders = table.orders.Where(p => p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PLACED ||
                p.order_status == (int)OrderStatusHelper.OrderStatusEnum.EDITING ||
                p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PROCESSING).
                ToList();
            pendingOrders = pendingOrders.OrderBy(o => o.timestamp).ToList();
            return PartialView("_OrderList", pendingOrders);
        }
        
        //
        // GET: /Order/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /Order/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult ViewFromSession()
        {
            order order = (order)Session["ORDER"];
            if (order == null)
            {
                user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
                if (authUser == null) return View("ViewFromSession", new OrderVM()); // failure
                order = Util.Session.SessionUtil.getOpenOrder(authUser);
            }

            OrderVM orderVM = new OrderVM(order);
            if (order != null)
            {
                foreach (order_item oi in order.order_item)
                {
                    menu_item mi = Util.Order.OrderUtil.filterMenuItem(oi);
                    side si = Util.Order.OrderUtil.filterSide(oi);
                    OrderItemVM orderItemVM = new OrderItemVM(oi, mi, si);
                    orderVM.addItem(orderItemVM);
                }
            }
            
            return View(orderVM);
        }

        //
        // GET: /Order/Create

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.users, "id", "username");
            ViewBag.waiter_id = new SelectList(db.waiters, "id", "first_name");

            var statuses = from TouchForFood.Models.OrderStatusHelper.OrderStatusEnum s in
                               Enum.GetValues(typeof(TouchForFood.Models.OrderStatusHelper.OrderStatusEnum))
                           select new { Value = (int)s, Name = s.ToString() };
            ViewData["order_status"] = new SelectList(statuses, "Value", "Name", OrderStatusHelper.OrderStatusEnum.OPEN);

            return View();
        } 

        //
        // POST: /Order/Create

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Create(order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.Create(order, true))
                    {
                        return RedirectToAction("Index");
                    }
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.ServerError;
            var statuses = from TouchForFood.Models.OrderStatusHelper.OrderStatusEnum s in
                               Enum.GetValues(typeof(TouchForFood.Models.OrderStatusHelper.OrderStatusEnum))
                           select new { Value = (int)s, Name = s.ToString() };
            ViewData["order_status"] = new SelectList(statuses, "Value", "Name", OrderStatusHelper.OrderStatusEnum.OPEN);

            ViewBag.user_id = new SelectList(db.users, "id", "username", order.user_id);
            ViewBag.waiter_id = new SelectList(db.waiters, "id", "first_name", order.waiter_id);
            return View(order);
        }
        
        //
        // GET: /Order/Edit/5

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            order order = im.find(id);
            ViewBag.user_id = new SelectList(db.users, "id", "username", order.user_id);
            ViewBag.waiter_id = new SelectList(db.waiters, "id", "first_name", order.waiter_id);
            //this is causing an issue. Sets the status to 6 instead of 3... Not sure why
            order.order_status = (int)OrderStatusHelper.OrderStatusEnum.EDITING;

            return View(order);
        }

        //
        // POST: /Order/Edit/5

        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Edit(order order)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (om.edit(order))
                        return RedirectToAction("Index");
                    else
                    {
                        ViewBag.Error = Global.OrderLockError;
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

            ViewBag.user_id = new SelectList(db.users, "id", "username", order.user_id);
            ViewBag.waiter_id = new SelectList(db.waiters, "id", "first_name", order.waiter_id);
            return View(order);
        }

        //TO DO
        // Make an overloaded FIinalize(int) to be called from editing
        // Initially we just use the session order to place our order
        // From editing an order object exists in the OrderVM, we just need to get it from the DB

        // POST: /Order/Finalize
        // finalizes the order in session
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Finalize()
        {
            order currentOrder = (order)Session["ORDER"];
            if (currentOrder == null)
            {
                user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
                if (authUser == null) return RedirectToAction("ViewFromSession"); // failure
                currentOrder = Util.Session.SessionUtil.getOpenOrder(authUser);
            }

            Session["ORDER"] = null;
            if (currentOrder == null)
            {
                // no order in session
                return RedirectToAction("ViewFromSession");
            }
            if (currentOrder.order_item.Count == 0)
            {
                if (currentOrder.id > 0)
                {
                    om.delete(currentOrder.id);
                }
                return RedirectToAction("ViewFromSession");
            }
            OrderVM orderVM = new OrderVM(currentOrder);

            if (currentOrder.table_id == null)
            {
                currentOrder.table_id = 1;
            }

            if (currentOrder.order_status == (int)OrderStatusHelper.OrderStatusEnum.OPEN)
            {
                Create(currentOrder);
            }
            else
            {
                currentOrder.order_status = (int)OrderStatusHelper.OrderStatusEnum.PLACED;
                Util.Order.OrderUtil.mergeExistingOrderToDb(currentOrder);
            }

            return  RedirectToAction("ViewFromSession"); //success
        }

        //
        // POST: /Order/AddMenuItem/5 (menu_item id)
        [HttpPost, Ajax(true),Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public int AddMenuItem(int id, int side_id)
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser == null) return -1;
            order currentOrder = (order)Session["ORDER"];

            if (currentOrder == null)
            {
                currentOrder = Util.Session.SessionUtil.getOpenOrder(authUser);
                if (currentOrder == null)
                {
                    currentOrder = new order();
                    currentOrder.order_status = (int)OrderStatusHelper.OrderStatusEnum.OPEN;
                    currentOrder.user_id = authUser.id;
                    currentOrder.table_id = authUser.current_table_id;
                }
                else
                {
                    if (currentOrder.order_status == (int)OrderStatusHelper.OrderStatusEnum.PLACED || currentOrder.order_status == (int)OrderStatusHelper.OrderStatusEnum.OPEN)
                    {
                        currentOrder.order_status = (int)OrderStatusHelper.OrderStatusEnum.EDITING;
                        db.orders.Find(currentOrder.id).order_status = currentOrder.order_status;
                        om.edit(currentOrder);
                    }
                    else
                    {
                        ViewBag.error = "Your order cannot be editied because it is already being processed";
                        return -1;
                    }
                }
                Session["ORDER"] = currentOrder;
            }

            menu_item mi = db.menu_item.Find(id);
            if (mi == null)
            {
                return -1; // failure;
            }

            side si;
            if (side_id == -1)
            {
                si = new side();
                si.id = side_id;
            }
            else
            {
                 si = db.sides.Find(side_id);
                 if (si == null)
                 {
                     return -1;
                 }
            }
            

            const OrderStatusHelper.OrderItemStatusEnum status = OrderStatusHelper.OrderItemStatusEnum.PENDING;
            currentOrder.AddMenuItem(mi, si, status);

            Session["ORDER"] = currentOrder;
            return currentOrder.id;
        }
         
        //
        // GET: /Order/Delete/5

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /Order/Delete/5

        [HttpPost, ActionName("Delete")]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                om.delete(id);
            }
            catch (AssociationExistsException aee)
            {
                ViewBag.error = aee.Message;
            }
            catch (InvalidOperationException ioe)
            {
                ViewBag.error = ioe.Message;
            }
            
            return RedirectToAction("Index");
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Cancel(int id)
        {
            return View(im.find(id));
        }

        [HttpPost, ActionName("Cancel")]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult CancelConfirmed(int id)
        {
            order order = im.find(id);
            bool isInSession = false;
            //order is not in the db yet check the session
            if (order ==null)
            {
                order = (order)Session["ORDER"];
                isInSession = true;
            }
            //Check if any order items are associated to the order. If so remove them. If the order is only in the session there is nothing to remove
            //in the db (including the order items)
            if (order.order_item.Count > 0 && !isInSession)
            {
                List<order_item> itemsToBeRemoved = order.order_item.ToList();

                for (int i = itemsToBeRemoved.Count - 1; i >= 0; i--)
                {
                    db.order_item.Remove(itemsToBeRemoved[i]);
                }

                //Order total should be zero dollars.
                order.total = 0;
            }

            order.order_status = (int)OrderStatusHelper.OrderStatusEnum.CANCELED;
            Session.Remove("ORDER");
            //Increment version number
            //order.version++;
            //TODO: Add code for offline locks, meaning check if saving changes to database is valid.
            if (!isInSession)
            {
                om.edit(order);
            }
            
            return RedirectToAction("ViewFromSession", new OrderVM());
        }


        //
        // GET: /Order/Decline/1?itemID=5
        // Shows a prompt for an item in a specific order to be declined (if the order isn't being edited)
        // TODO: Possibly make the decline AJAX instead of having to load 2 pages to perform the action 
        //  Current: Click decline, load decline confirmation page, click confirm, load orders page
        //  New: Click decline, confirm JS confirmation box, modify the associated menu item with AJAX (or return error message through JS)
        // TODO: Concurrency issues might still be present, even with the use of version numbers
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Decline(int id, int itemID)
        {
            order order =im.find(id);

            // Check the order is not being edited
            if (order.order_status != (int)OrderStatusHelper.OrderStatusEnum.EDITING)
            {
                return View(order);
            }

            return RedirectToAction("ManageIndex", "Table", new { id = order.table_id });
        }

        [HttpPost, ActionName("Decline")]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult DeclineConfirmed(int id, int itemID)
        {
            order order =im.find(id);

            
            // Check if there are order items are associated to the order
            if (order.order_item.Count > 0)
            {
                // If there are, then check the order is not being edited
                if (order.order_status != (int)OrderStatusHelper.OrderStatusEnum.EDITING)
                {
                    order_item orderItemToReject = order.order_item.FirstOrDefault(toReject => toReject.id == itemID);

                    
                    // If it's not, then decline the item by changing the status to rejected
                    orderItemToReject.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.REJECTED;
                    order_itemOM.edit(orderItemToReject);

                    // Get the actual menu item in question and set it as unavailable
                    menu_item itemToSetAsUnavailable = order.order_item.FirstOrDefault(toReject => toReject.id == itemID).menu_item;
                    itemToSetAsUnavailable.is_active = false;
                    menu_itemOM.edit(itemToSetAsUnavailable);

                    Util.Order.OrderUtil.UpdatePrice(ref order);        
                    db.SaveChanges();

                } // End if EDITING

            } // End if order_item.Count > 0

            
            
            return RedirectToAction("ManageIndex", "Table");

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        /*public ViewResult RetrieveOrderItems()
        {
            var orders = db.orders.Include(o => o.order_status).Include(o => o.user).Include(o => o.waiter).OrderBy(o => o.table_id).ThenBy(o => o.timestamp);
            
            return View();
        }  */         
        

        public ActionResult Editing(int id)
        {
            order order = im.find(id);
            Session["ORDER"] = order;

            order.order_status = (int)OrderStatusHelper.OrderStatusEnum.EDITING;
            db.SaveChanges();

            OrderVM orderVM = new OrderVM(order);
            if (order != null)
            {
                foreach (order_item oi in order.order_item)
                {
                    menu_item mi = Util.Order.OrderUtil.filterMenuItem(oi);
                    side si = Util.Order.OrderUtil.filterSide(oi);
                    OrderItemVM orderItemVM = new OrderItemVM(oi, mi, si);
                    orderVM.addItem(orderItemVM);
                }
            }

            return View("ViewFromSession",orderVM);
        }

        [CustomAuthorizationAttribute(Roles = SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult ViewOrderStatus(int id)
        {
            order order = im.find(id);
            Session["ORDER"] = order;

            OrderVM orderVM = new OrderVM(order);
            if (order != null)
            {
                foreach (order_item oi in order.order_item)
                {
                    menu_item mi = Util.Order.OrderUtil.filterMenuItem(oi);
                    side si = Util.Order.OrderUtil.filterSide(oi);
                    OrderItemVM orderItemVM = new OrderItemVM(oi, mi, si);
                    orderVM.addItem(orderItemVM);
                }
            }

            return View("OrderUserDetails", orderVM);
        }

    }
}