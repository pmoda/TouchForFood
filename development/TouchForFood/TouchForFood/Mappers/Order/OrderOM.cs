using System.Collections.Generic;
using System.Data;
using System.Linq;
using TouchForFood.Exceptions;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;
using System;
using System.Data.Entity.Infrastructure;

namespace TouchForFood.Mappers
{
    public class OrderOM : GenericOM
    {
        public OrderOM():base(){}

        public OrderOM(touch_for_foodEntities db) : base(db){}

        public Boolean edit(order order)
        {
            OrderIM im = new OrderIM(db);
            order dbVersion = im.find(order.id);
            if (dbVersion.version == order.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(order).State = EntityState.Modified;

                order.version = order.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;
        }
        /// <summary>
        /// Write a order object to the database
        /// </summary>
        /// <param name="order">The order object to write</param>
        /// <param name="setToDefault">If true, sets the order object to default values, if false: leaves object unmodified</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(order order, bool setToDefault)
        {
            if (setToDefault)
            {
                order.order_status = (int)OrderStatusHelper.OrderStatusEnum.PLACED;
                order.timestamp = DateTime.Now;
            }
            
            db.orders.Add(order);
            return (db.SaveChanges() == 1);
        }

        /// <summary>
        /// When deleteing an order we have to make sure that it has no associations, i.e. order items
        /// If it doesn't set the order to DELETED and all of the order items as well, unless they are in processing.
        /// </summary>
        /// <param name="order">the order to delete</param>
        /// <returns>number of records affected</returns>
        /// <exception cref="AssociationExistsException">Order is associated with an order item</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override int delete(int id)
        {
            order order = db.orders.Find(id);
            Dictionary<int, int?> order_item_statuses = new Dictionary<int, int?>();
            int result = 0;
            bool doRevert = false;
            //check to see if the order has any order items associated with it
            IList<order_item> oiList = order.order_item.ToList();


            //itterate over the list and see if any are processing.
            for (int i = 0; i < oiList.Count; i++)
            {
                order_item oi = oiList.ElementAt(i);
                if (oi.order_item_status != (int)OrderStatusHelper.OrderItemStatusEnum.PROCESSING)
                {
                    order_item_statuses.Add(oi.id, oi.order_item_status);
                    oi.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.DELETED;
                    db.Entry(oi).State = EntityState.Modified;
                }
                else
                {
                    //there is an order that is processing we need to revert all of the old statuses back to what they were 
                    //and throw an exception
                    doRevert = true;
                    break;
                }
            }

            if (doRevert)
            {
                revertStatuses(order_item_statuses, oiList);
                throw new Exceptions.ItemActiveException("This order has an item that is being processed. Cannot delete it.");
            }
            else
            {
                order.order_status = (int)OrderStatusHelper.OrderStatusEnum.DELETED;
                db.Entry(order).State = EntityState.Modified;                
            }
            result = db.SaveChanges();
            //Update version
            edit(order);
            

            return result;
        }

        /**
         * Reverts the statuses to what they used to be.
         */
        private void revertStatuses(Dictionary<int, int?> order_item_statuses, IList<order_item> oiList)
        {
            for (int i = 0; i < oiList.Count; i++)
            {
                order_item oi = oiList.ElementAt(i);
                if (order_item_statuses.ContainsKey(oi.id))
                {
                    oi.order_item_status = order_item_statuses[oi.id];
                    db.Entry(oi).State = EntityState.Unchanged;
                }
            }
        }
    }
}