using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Mappers.Abstract;
using System.Data.Entity.Infrastructure;

namespace TouchForFood.Mappers
{
    public class Order_ItemOM : GenericOM
    {
        public Order_ItemOM() : base() { }
        public Order_ItemOM(touch_for_foodEntities db) : base(db) { }
        

        public Boolean edit(order_item order_item)
        {
            Order_ItemIM im = new Order_ItemIM(db);
            OrderOM orderOM = new OrderOM(db);
            order_item dbVersion = im.find(order_item.id);
            if (dbVersion.version == order_item.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(order_item).State = EntityState.Modified;

                order_item.version = order_item.version + 1;
                db.SaveChanges();
                orderOM.edit(db.orders.Find(order_item.order_id));
                return true;
            }
            return false;

        }

        /// </summary>
        /// <param name="cat">order_item to be deleted</param>
        /// <returns>number of rows affected by the database</returns>
        public override int delete(int id)
        {
            order_item order_item = db.order_item.Find(id);
            int result = 0;

            db.order_item.Remove(order_item);
            result = db.SaveChanges();

            return result;
        }

        
    }
}