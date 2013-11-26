using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class ReviewIM : GenericIM
    {
        public ReviewIM() : base() { }
        public ReviewIM(touch_for_foodEntities db) : base(db) { }

        public review find(int id)
        {
            try
            {
                return db.reviews.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<review> find()
        {
            try
            {
                return db.reviews.ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}