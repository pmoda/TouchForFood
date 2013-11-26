using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TouchForFood.Models
{
    [MetadataType(typeof(OrderMetadata))]
    public partial class order
    {
        public string written_by { get; set; }
        public int uniqueId = 0;
        public order(order order){
            id = order.id;
            order_item = order.order_item;
            order_status = order.order_status;
            table_id = order.table_id;
            table = order.table;
            timestamp = order.timestamp;
            total = order.total;
            user = order.user;
            user_id = order.user_id;
            version = order.version;
            waiter = order.waiter;
            waiter_id = order.waiter_id;
            written_by = order.written_by;
        }
        public void AddMenuItem(menu_item mi, side si, OrderStatusHelper.OrderItemStatusEnum orderItemStatus)
        {
            order_item oi = new order_item();
            oi.menu_item_id = mi.id;
            if (si.id == -1)
            {
                oi.sides_id = null;
            }
            else
            {
                oi.sides_id = si.id;
            }
            oi.id = uniqueId--;
            oi.order_id = this.id;
            oi.order_item_status = (int)orderItemStatus;

            this.order_item.Add(oi);
            
            if (this.total == null) this.total = (decimal)0.0;
            this.total += mi.price;
        }
    }

    public class OrderMetadata
    {
        [DisplayFormat(DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime timestamp { get; set; }
    }
}