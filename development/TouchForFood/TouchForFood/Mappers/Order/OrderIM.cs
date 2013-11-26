using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class OrderIM : GenericIM
    {
        public OrderIM() : base() { }
        public OrderIM(touch_for_foodEntities db) : base(db) { }

        public order find(int id)
        {
            try
            {
                return db.orders.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<order> find()
        {
            try
            {
                return db.orders.ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}