using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class Review_Order_ItemIM : GenericIM
    {
        public Review_Order_ItemIM() : base() { }

        public Review_Order_ItemIM(touch_for_foodEntities db) : base(db) { }

        public review_order_item find(int id)
        {
            try
            {
                return db.review_order_item.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<review_order_item> find()
        {
            try
            {
                return db.review_order_item.OrderBy(m=>m.submitted_on).ToList();
            }
            catch (System.ArgumentNullException)
            {
                return null;
            }
        }
    }
}