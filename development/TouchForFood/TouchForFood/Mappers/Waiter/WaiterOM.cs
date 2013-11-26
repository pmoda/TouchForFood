using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class WaiterOM : GenericOM
    {
        public WaiterOM() :base(){}
        public WaiterOM(touch_for_foodEntities db):base(db){}

        /// <summary>
        /// Writes a waiter object to the database
        /// </summary>
        /// <param name="waiter">The waiter object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(waiter waiter)
        {
            db.waiters.Add(waiter);
            return (db.SaveChanges() == 1);
        }

        public override int delete(int id)
        {
            waiter waiter = db.waiters.Find(id);
            int result = 0;

            clearOrder(waiter.orders);

            db.waiters.Remove(waiter);
            result = db.SaveChanges();

            return result;
        }

        private void clearOrder(ICollection<order> orders)
        {
            for (int i = 0; i < orders.Count;i++ )
            {
                orders.ElementAt(i).waiter_id = null;
                orders.ElementAt(i).version = orders.ElementAt(i).version + 1;
                db.Entry(orders.ElementAt(i)).State = EntityState.Modified;
            }
        }
    }
}