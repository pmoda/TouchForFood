using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;
using System.Data.Entity.Infrastructure;
using System.Data;
using TouchForFood.Exceptions;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Mappers
{
    public class SideOM : GenericOM
    {

        public SideOM() : base() { }
        public SideOM(touch_for_foodEntities db) : base(db) { }

        /// <summary>
        /// Write a side object to the database
        /// </summary>
        /// <param name="side"></param>
        /// <returns>true if successful, false otherwise.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Create(side side)
        {
            db.sides.Add(side);
            return (db.SaveChanges() == 1);
        }

        /// <summary>
        /// Is responsible for handling any modifications to side items
        /// </summary>
        /// <param name="side">Edited side</param>
        /// <returns>Boolean indicating whether the modification took effect</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Boolean Edit(side side)
        {
            side dbVersion = db.sides.Find(side.id);
            if (dbVersion.version == side.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(side).State = EntityState.Modified;
                side.version = side.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// When deleting a side set the is_deleted field to true,
        /// only allow this if the side is not active. Set the sides_id for any associated order items to null
        /// </summary>
        /// <param name="menu_item">menu item to delete</param>
        /// <returns>the number of rows affected</returns>
        /// <exception cref="ItemActiveException">Item is currently set to active.</exception>
        public override int delete(int id)
        {
            side side = db.sides.Find(id);
            int result = 0;
            if (!side.is_active)
            {
                side.is_deleted = true;
                
                side.order_item.ToList().ForEach(oi => db.order_item.Remove(oi));
                result = db.SaveChanges();
                Edit(side);
            }
            else
            {
                throw new ItemActiveException(Global.SideActiveWarning);
            }
            return result;
        }

        public int SetActive(side side, bool isActive)
        {
            side dbVersion = db.sides.Find(side.id);

            int result = 0;
            if (dbVersion.version == side.version && side.is_active == dbVersion.is_active)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(side).State = EntityState.Modified;
                side.is_active = isActive;
                side.version = side.version + 1;
                result = db.SaveChanges();
            }
            
            return result;
        }
    }
}