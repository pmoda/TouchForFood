using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using System.Data;

namespace TouchForFood.Util.Order
{
    public class OrderUtil
    {
        private static touch_for_foodEntities db = new touch_for_foodEntities();

        /**
         * Filters the categories and only returns those that are not already assigned to the menu
         * that is passed in.
         * */
        public static side filterSide(order_item oi)
        {
            side si = (side)db.sides.Find(oi.sides_id);
            return si;
        }

        public static menu_item filterMenuItem(order_item oi)
        {
            menu_item mi = (menu_item)db.menu_item.Find(oi.menu_item_id);
            mi.item = filterItem(mi);

            return mi;
        }

        public static item filterItem(menu_item mi)
        {
            item i = (item)db.items.Find(mi.item_id);
            return i;
        }
        public static table filterTable(order order)
        {
            table t = (table)db.tables.Find(order.table_id);
            return t;
        }
        public static waiter filterWaiter(order order)
        {
            waiter w = (waiter)db.waiters.Find(order.waiter_id);
            return w;
        }
        public static user filterUser(order order)
        {
            user u = (user)db.users.Find(order.user_id);
            return u;
        }

        public static void UpdatePrice(ref order theOrder)
        {
            theOrder.total = 0;
            decimal tempPrice = 0;
            foreach (order_item oi in theOrder.order_item)
            {
                if (oi.order_item_status == (int)OrderStatusHelper.OrderItemStatusEnum.REJECTED) continue;
                menu_item tempMenuItem = db.menu_item.Find(oi.menu_item_id);
                tempPrice = tempMenuItem.price;
                theOrder.total += tempPrice;
            }
        }

        public static void mergeExistingOrderToDb(order order)
        {
            order initialOrder = db.orders.Find(order.id);
            db.Entry(initialOrder).State = EntityState.Modified;

            initialOrder.order_status = order.order_status;
            initialOrder.total = order.total;
            initialOrder.waiter_id = order.waiter_id;
            initialOrder.table_id = order.table_id;
            initialOrder.version++;
            initialOrder.written_by = order.written_by;

            foreach (order_item oi in order.order_item)
            {
                if (oi.id <= 0)
                {
                    db.order_item.Add(oi);
                }
            }

            List<order_item> toRemove = new List<order_item>();
            for (int i = 0; i < initialOrder.order_item.Count; i++)
            {
                bool found = false;
                for (int k = 0; k < order.order_item.Count; k++)
                {
                    if (order.order_item.ElementAt(k).id == initialOrder.order_item.ElementAt(i).id)
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    toRemove.Add(initialOrder.order_item.ElementAt(i));
                }
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                db.order_item.Remove(toRemove[i]);
            }

            db.SaveChanges();
        }
    }
}