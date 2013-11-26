using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class Order_ItemIM : GenericIM
    {

        public Order_ItemIM() : base() { }

        public Order_ItemIM(touch_for_foodEntities db) : base(db) { }

        public order_item find(int id)
        {
            try
            {
                return db.order_item.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<order_item> find()
        {
            try
            {
                return db.order_item.ToList();
            }
            catch (System.ArgumentNullException)
            {
                return null;
            }
        }

        internal order_item find(int? id)
        {
            try
            {
                return db.order_item.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }           
        }
    }
}