using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class BillIM : GenericIM
    {

        public BillIM() : base() { }

        public BillIM(touch_for_foodEntities db) : base(db) { }

        public bill find(int id)
        {
            try
            {
                return db.bills.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        
        public ICollection<bill> find()
        {
            try
            {
                return db.bills.ToList();
            }
            catch (System.ArgumentNullException)
            {
                return null;
            }
        }

        internal bill find(int? id)
        {
            try
            {
                return db.bills.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            
        }
    }
}