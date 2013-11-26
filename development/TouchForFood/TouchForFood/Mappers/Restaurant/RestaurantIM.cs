using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class RestaurantIM : GenericIM
    {
        public RestaurantIM() : base() { }
        public RestaurantIM(touch_for_foodEntities db) : base(db) { }

        public restaurant find(int id)
        {
            try
            {
                return db.restaurants.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<restaurant> find(bool is_deleted)
        {
            try
            {
                return db.restaurants.Where(r => r.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}