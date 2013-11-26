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
    public class BillOM : GenericOM
    {
        public BillOM() : base() { }
        public BillOM(touch_for_foodEntities db) : base(db) { }

        public Boolean edit(bill bill)
        {
            BillIM im = new BillIM(db);
            bill dbVersion = im.find(bill.id);
            if (dbVersion.version == bill.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(bill).State = EntityState.Modified;

                bill.version = bill.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;

        }

        /// </summary>
        /// <param name="cat">Bill to be deleted</param>
        /// <returns>number of rows affected by the database</returns>
        public override int delete(int id)
        {
            bill bill = db.bills.Find(id);
            int result = 0;

            cleanUpOrderItem(bill);
            db.bills.Remove(bill);
            result = db.SaveChanges();

            return result;
        }

        private void cleanUpOrderItem(bill bill)
        {
            bill theBill = db.bills.Find(bill.id);
            
            foreach (order_item oi in theBill.order_item)
            {
                oi.bill_id = null;
            }

            db.SaveChanges();
        }
    }
}