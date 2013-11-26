using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;

namespace TouchForFood.Util.Session
{
    public class SessionUtil
    {
        /**
         * Filters the categories and only returns those that are not already assigned to the menu
         * that is passed in.
         * */
        public static order getOpenOrder(user usr)
        {
            touch_for_foodEntities db = new touch_for_foodEntities();
            order o;
            var models = db.orders
                .Where(p => p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PLACED || p.order_status == (int)OrderStatusHelper.OrderStatusEnum.EDITING || p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PROCESSING)
                .Where(p=>p.user_id == usr.id)
                .OrderByDescending(p=>p.timestamp)
                .ToList()
                .Select(item =>
                new order
                {
                    id = item.id,
                    timestamp = item.timestamp,
                    total = item.total,
                    order_status = item.order_status,
                    order_item = item.order_item,
                    user = item.user,
                    table = item.table
                }).AsQueryable();

            if (models.Any())
            {
                o = models.ToArray()[0];
                return o;
            }
            return null;
        }
    }
}