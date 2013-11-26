using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class WaiterIM : GenericIM
    {
        public WaiterIM() : base() { }
        public WaiterIM(touch_for_foodEntities db) : base(db) { }

        public waiter find(int id)
        {
            try
            {
                return db.waiters.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<waiter> find()
        {
            try
            {
                return db.waiters.ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}