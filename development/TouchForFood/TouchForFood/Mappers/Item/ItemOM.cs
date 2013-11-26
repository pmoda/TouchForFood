using TouchForFood.Models;
using TouchForFood.Exceptions;
using TouchForFood.Mappers.Abstract;
using System.Data.Entity.Infrastructure;
using System.Data;
using System;

namespace TouchForFood.Mappers
{
    public class ItemOM : GenericOM
    {
        public ItemOM(): base(){}

        public ItemOM(touch_for_foodEntities db) : base(db){}

        /// <summary>
        /// Write an item object to the database
        /// </summary>
        /// <param name="item">The item to be written</param>
        /// <returns>true if successful, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Create(item item)
        {
            db.items.Add(item);
            return (db.SaveChanges() == 1);
        }

        public Boolean edit(item item)
        {
            ItemIM im = new ItemIM(db);
            item dbVersion = im.find(item.id);
            if (dbVersion.version == item.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(item).State = EntityState.Modified;

                item.version = item.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// When deleteing an item we have to make sure that it has no associations, i.e. menu_items
        /// </summary>
        /// <param name="item">the item to be deleted</param>
        /// <returns>the number of rows affected by the db change</returns>
        /// <exception cref="AssociationExistsException">Item is associated with a menu item</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override int delete(int id)
        {
            item item = db.items.Find(id);
            int result = 0;
            //check to see if the category has any items associated with it
            if (item.menu_item.Count > 0)
            {
                throw new AssociationExistsException("This item exists in a menu. It cannot be deleted");
            }

            db.items.Remove(item);
            result = db.SaveChanges();

            return result;
        }
    }
}