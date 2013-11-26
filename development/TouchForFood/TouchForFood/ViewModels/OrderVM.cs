using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;

namespace TouchForFood.ViewModels
{
    public class OrderVM
    {
        public order order;
        public IList<OrderItemVM> orderItemVMs { get; set; }

        public OrderVM(order order, IList<OrderItemVM> orderItemVMs)
        {
            if (order != null)
            {
                this.order = new order(order);
            }
            this.orderItemVMs = orderItemVMs;
            checkObjects();
        }
        public OrderVM(order order)
        {
            if (order != null)
            {
                this.order = new order(order);
            }
            this.orderItemVMs = new List<OrderItemVM>();
            checkObjects();
        }

        public OrderVM()
        {
            orderItemVMs = new List<OrderItemVM>();
            order = new order();
        }

        public void checkObjects()
        {
            if (this.order == null) return;
            if (this.order.user == null && this.order.user_id != null)
            {
                this.order.user = Util.Order.OrderUtil.filterUser(this.order);
            }
            if (this.order.table == null && this.order.table_id != null)
            {
                this.order.table = Util.Order.OrderUtil.filterTable(this.order);
            }
            if (this.order.waiter == null && this.order.waiter_id!= null)
            {
                this.order.waiter = Util.Order.OrderUtil.filterWaiter(this.order);
            }
            if (this.order.timestamp == DateTime.MinValue)
            {
                this.order.timestamp = DateTime.Now;
            }
        }

        public void addItem(OrderItemVM item)
        {
            orderItemVMs.Add(item);
        }

        public void removeItem(int itemPosition)
        {
            orderItemVMs.RemoveAt(itemPosition);
        }

        public void removeById(int id)
        {
            OrderItemVM oiv = getItemById(id);
            if (oiv != null)
            {
                orderItemVMs.Remove(oiv);
            }
            
        }

        public OrderItemVM getItemById(int id)
        {
            foreach (OrderItemVM oiv in orderItemVMs)
            {
                if (oiv.id == id)
                {
                    return oiv;
                }
            }
            return null;
        }
        
    }
}