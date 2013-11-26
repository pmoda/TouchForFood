using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;

namespace TouchForFood.Util.Order
{
    public class OrderStatusUtil
    {
        private static touch_for_foodEntities db = new touch_for_foodEntities();

        /*
        public static int GetOrderItemStatusId(OrderStatusHelper.OrderItemStatusEnum which)
        {
            var models = db.order_item_status
                .Where(p => p.type == (int)which)
                .ToList()
                .Select(item =>
                new order_item_status
                {
                    id = item.id
                }).AsQueryable();

            if (models.Any())
            {
                return models.ToArray()[0].id;
            }

            return FillOrderItemStatusTableAndGetId(which);
        }

        private static int FillOrderItemStatusTableAndGetId(OrderStatusHelper.OrderItemStatusEnum which)
        {
            const int max = (int)OrderStatusHelper.OrderItemStatusEnum.LAST_STATUS;
            int toret = -1;
            for (int i = 0; i < max; i++)
            {
                order_item_status ois = new order_item_status();
                ois.description = OrderStatusHelper.GetOrderItemStatusDescription((OrderStatusHelper.OrderItemStatusEnum)i);
                ois.name = System.Enum.GetName(typeof(OrderStatusHelper.OrderItemStatusEnum), (OrderStatusHelper.OrderItemStatusEnum)i);
                ois.type = i;

                db.order_item_status.Add(ois);
                db.SaveChanges();
                if (i == (int)which) toret = ois.id;
            }

            return toret;
        }

        public static int GetOrderStatusId(OrderStatusHelper.OrderStatusEnum which)
        {
            var models = db.order_status
                .Where(p => p.type == (int)which)
                .ToList()
                .Select(item =>
                new order_status
                {
                    id = item.id
                }).AsQueryable();

            if (models.Any())
            {
                return models.ToArray()[0].id;
            }

            return FillOrderStatusTableAndGetId(which);
        }

        private static int FillOrderStatusTableAndGetId(OrderStatusHelper.OrderStatusEnum which)
        {
            const int max = (int)OrderStatusHelper.OrderStatusEnum.LAST_STATUS;
            int toret = -1;

            for (int i = 0; i < max; i++)
            {
                order_status os = new order_status();
                os.description = OrderStatusHelper.GetOrderStatusDescription((OrderStatusHelper.OrderStatusEnum)i);
                os.name = System.Enum.GetName(typeof(OrderStatusHelper.OrderStatusEnum), (OrderStatusHelper.OrderStatusEnum)i);
                os.type = i;

                db.order_status.Add(os);
                db.SaveChanges();
                if (i == (int)which) toret = os.id;
            }

            return toret;
        }
        */
    }
}