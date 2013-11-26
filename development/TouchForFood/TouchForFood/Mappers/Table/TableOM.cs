using System.Collections.Generic;
using System.Data;
using System.Linq;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;
using System;
namespace TouchForFood.Mappers
{
    public class TableOM : GenericOM
    {
        public TableOM():base(){}
 
        public TableOM(touch_for_foodEntities db):base(db){}

        /// <summary>
        /// Writes a table object to the database
        /// </summary>
        /// <param name="table">The table object to write</param>
        /// <param name="assignDefaultName">If true, assigns a default name to the table using restaurant_id and name, otherwise leaves the name unchanged</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(table table)
        {
            db.tables.Add(table);
            return (db.SaveChanges() == 1);
        }

        /// <summary>
        /// When trying to delete a table, remove the id from any associated order.
        /// If the orders are currently being processed throw an exception.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int delete(int id)
        {
            table table = db.tables.Find(id);
            
            int result = 0;

            //clear the orders
            clearOrders(table.orders);

            //clear the service requests
            clearServiceRequests(table.service_request);

            //clear the user
            clearUser(table.users);

            db.tables.Remove(table);
            result = db.SaveChanges();
            
            return result;
        }

        private void clearOrders(ICollection<order> orders)
        {
            for (int i =0;i < orders.Count; i++)
            {
                order o = orders.ElementAt(i);
                o.table_id = null;
                db.Entry(o).State = EntityState.Modified;
            }
        }

        private void clearUser(ICollection<user> users)
        {
            for(int i =0; i < users.Count;i++)
            {
                user u = users.ElementAt(i);
                u.current_table_id = null;
                db.Entry(u).State = EntityState.Modified;
            }
        }

        private void clearServiceRequests(ICollection<service_request> serviceRequests)
        {
            for (int i =0; i < serviceRequests.Count; i++)
            {
                service_request sr = serviceRequests.ElementAt(i);
                sr.table_id = null;
                db.Entry(sr).State = EntityState.Modified;
            }
        }
    }
}