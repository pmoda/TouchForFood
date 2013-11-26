using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;

namespace TouchForFood.Mappers
{
    public class ItemIM : GenericIM
    {
        public ItemIM() : base() { }
        public ItemIM(touch_for_foodEntities db):base(db){}

        public item find(int id) 
        {
            try
            {
                return db.items.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<item> find()
        {
            try
            {
                return db.items.ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}