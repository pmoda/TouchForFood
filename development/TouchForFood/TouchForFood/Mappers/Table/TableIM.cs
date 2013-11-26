using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class TableIM : GenericIM
    {
        public TableIM() : base() { }
        public TableIM(touch_for_foodEntities db) : base(db) { }

        public table find(int id)
        {
            try
            {
                return db.tables.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<table> find()
        {
            try
            {
                return db.tables.ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}