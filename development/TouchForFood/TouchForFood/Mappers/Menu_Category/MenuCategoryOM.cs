using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Exceptions;
using System.Data;
using TouchForFood.Mappers.Abstract;
using System.Data.Entity.Infrastructure;

namespace TouchForFood.Mappers
{
    public class MenuCategoryOM : GenericOM
    {
        public MenuCategoryOM():base(){}
        public MenuCategoryOM(touch_for_foodEntities db):base(db){}

        /// <summary>
        /// Writes  a menu_category object to the database
        /// </summary>
        /// <param name="mc">The menu_category object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(menu_category mc)
        {
            db.menu_category.Add(mc);
            return (db.SaveChanges() == 1);
        }

        public Boolean edit(menu_category menu_category)
        {
            MenuCategoryIM im = new MenuCategoryIM(db);
            menu_category dbVersion = im.find(menu_category.id);
            if (dbVersion.version == menu_category.version)
            {
                //Activate / Deactivate the menu category means changing the active feild to all menu items inside it
                //if (dbVersion.is_active != menu_category.is_active)
                //{
                MenuItemOM om = new MenuItemOM(db);
                IList<menu_item> menu_item_list = menu_category.menu_item.ToList();
                for (int i = 0; i < menu_item_list.Count; i++)
                {
                    menu_item mi = menu_item_list.ElementAt(i);
                    mi.is_active = menu_category.is_active;
                    om.edit(mi);
                }
                //}
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(menu_category).State = EntityState.Modified;
                menu_category.version = menu_category.version + 1;
                db.SaveChanges();
                return true;
            }            
            return false;

        }
       
        /// <summary>
        /// When deleteing a menu category set it's is_deleted to true,as well as cascade to menu items.
        /// only allow this if the menu category is not active
        /// </summary>
        /// <param name="menu_cat">menu category to delete</param>
        /// <returns>number of rows affected</returns>
        /// <exception cref="ItemActiveException">Item is currently set to active</exception>
        /// <exception cref="InvalidOperationException"></exception>
        public override int delete(int id)
        {
            menu_category menu_cat = db.menu_category.Find(id);
            int result = 0;
            MenuItemOM om = new MenuItemOM(db);
            if (!menu_cat.is_active)
            {
                menu_cat.is_deleted = true;
                
                IList<menu_item> menu_item_list = menu_cat.menu_item.ToList();
                for (int i = 0; i < menu_item_list.Count; i++)
                {
                    menu_item mi = menu_item_list.ElementAt(i);
                    mi.is_active = false;
                    om.delete(mi.id);
                }
                db.Entry(menu_cat).State = EntityState.Modified;
                result = db.SaveChanges();

                //Calling edit will update the version
                edit(menu_cat);
                    
            }
            else
            {
                throw new ItemActiveException("This category is currently active! Cannot delete it.");
            }
            return result;
        }
    }
}