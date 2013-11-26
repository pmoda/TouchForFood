using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;

namespace TouchForFood.ViewModels
{
    public class OrderItemVM
    {
        public long id;
        public order_item order_item;
        public menu_item menu_item;
        public int order_item_status;
		public side side;

        public OrderItemVM(order_item oi, menu_item mi, side si)
        {
            id = oi.id;
            this.order_item = oi;
            this.menu_item = mi;
            order_item_status = (int)oi.order_item_status;
			this.side = si;
        }
    }
}