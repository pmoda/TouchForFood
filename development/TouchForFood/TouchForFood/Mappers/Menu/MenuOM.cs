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
    public class MenuOM : GenericOM
    {
        public MenuOM() : base() { }

        public MenuOM(touch_for_foodEntities db) : base(db) { }

        /// <summary>
        /// Writes a menu object to the database
        /// </summary>
        /// <param name="menu">The menu object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(menu menu)
        {
            db.menus.Add(menu);
            return (db.SaveChanges() == 1);
        }

        public Boolean edit(menu menu)
        {
            MenuIM im = new MenuIM(db);
            menu dbVersion = im.find(menu.id);
            if (dbVersion.version == menu.version)
            {
                //Activate / Deactivate the menu means changing the active feild to all menu categories inside it
                //if (dbVersion.is_active != menu.is_active)
                //{
                MenuCategoryOM om = new MenuCategoryOM(db);
                IList<menu_category> menu_category_list = menu.menu_category.ToList();
                for (int i = 0; i < menu_category_list.Count; i++)
                {
                    menu_category mc = menu_category_list.ElementAt(i);
                    mc.is_active = menu.is_active;
                    om.edit(mc);
                }
                // }
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(menu).State = EntityState.Modified;
                menu.version = menu.version + 1;
                db.SaveChanges();
                return true;
            }

            return false;

        }

        /// <summary>
        /// When deleteing a menu set it's is_deleted to true,
        /// as well as cascade to the menu category and menu items.
        /// only allow this is the menu is not active
        /// </summary>
        /// <param name="menu">menu to be deleted</param>
        /// <returns>number of rows affected</returns>
        /// <exception cref="ItemActiveException">Item is currently set to active</exception>
        public override int delete(int id)
        {
            menu menu = db.menus.Find(id);
            MenuCategoryOM mcOM = new MenuCategoryOM(db);

            int result = 0;
            if (!menu.is_active)
            {
                menu.is_deleted = true;
                menu.version++;
                IList<menu_category> mcList = menu.menu_category.ToList();
                for (int i = 0; i < mcList.Count; i++)
                {
                    menu_category mc = mcList.ElementAt(i);
                    mc.is_active = false;
                    mcOM.delete(mc.id);
                }

                db.Entry(menu).State = EntityState.Modified;
                result = db.SaveChanges();

            }
            else
            {
                throw new ItemActiveException("This menu is currently active! Cannot delete it.");
            }
            return result;
        }
    }
}