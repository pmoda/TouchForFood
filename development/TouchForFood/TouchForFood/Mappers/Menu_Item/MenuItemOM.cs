using System.Linq;
using TouchForFood.Models;
using TouchForFood.Exceptions;
using TouchForFood.Mappers.Abstract;
using System.Data.Entity.Infrastructure;
using System.Data;
using System;

namespace TouchForFood.Mappers
{
    public class MenuItemOM : GenericOM
    {
        public MenuItemOM():base(){}
        public MenuItemOM(touch_for_foodEntities db):base(db){}

        /// <summary>
        /// Writes a menu_item object to the database
        /// </summary>
        /// <param name="mi">The menu_item object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(menu_item mi)
        {
            db.menu_item.Add(mi);
            return (db.SaveChanges() == 1);
        }

        public Boolean edit(menu_item menu_item)
        {
            menu_item dbVersion = db.menu_item.Find(menu_item.id);
            if (dbVersion.version == menu_item.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(menu_item).State = EntityState.Modified;
                menu_item.version = menu_item.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// When deleteing a menu item set it's is_deleted to true,
        /// only allow this if the menu item is not active. Delete any associated order items to this menu item
        /// </summary>
        /// <param name="menu_item">menu item to delete</param>
        /// <returns>the number of rows affected</returns>
        /// <exception cref="ItemActiveException">Item is currently set to active.</exception>
        public override int delete(int id)
        {
            menu_item menu_item = db.menu_item.Find(id);
            int result = 0;
            if (!menu_item.is_active)
            {
                menu_item.is_deleted = true;
                if (edit(menu_item))
                    result = 1;
            }
            else
            {
                throw new ItemActiveException("This menu item is currently active! Cannot delete it.");
            }
            return result;
        }
    }
}