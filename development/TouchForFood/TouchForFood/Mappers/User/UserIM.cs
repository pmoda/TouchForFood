using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class UserIM : GenericIM
    {
        public UserIM() : base() { }
        public UserIM(touch_for_foodEntities db) : base(db) { }

        public user find(int id)
        {
            try
            {
                return db.users.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<user> find()
        {
            try
            {
                return db.users.ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}