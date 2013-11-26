using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class MenuItemIM : GenericIM
    {
        public MenuItemIM() : base() { }
        public MenuItemIM(touch_for_foodEntities db) : base(db) { }

        public menu_item find(int id)
        {
            try
            {
                return db.menu_item.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<menu_item> find(bool is_active, bool is_deleted)
        {
            try
            {
                return db.menu_item.Where(mi => mi.is_active == is_active && mi.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
        public ICollection<menu_item> find(bool is_deleted, int menu_category_id)
        {
            try
            {
                return db.menu_item.Where(mi => mi.menu_category_id == menu_category_id && mi.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}