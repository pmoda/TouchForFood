using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;

namespace TouchForFood.Mappers
{
    public class MenuCategoryIM : GenericIM
    {
        public MenuCategoryIM() : base() { }
        public MenuCategoryIM(touch_for_foodEntities db) : base(db) { }

        public menu_category find(int id)
        {
            try
            {
                return db.menu_category.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<menu_category> find(bool is_active, bool is_deleted)
        {
            try
            {
                return db.menu_category.Where(mc => mc.is_active == is_active && mc.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<menu_category> find(bool is_deleted, int menu_id)
        {
            try
            {
                return db.menu_category.Where(mc => mc.menu_id == menu_id && mc.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}