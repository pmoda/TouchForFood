using System;
using System.Collections.Generic;
using System.Linq;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;
using System.Data.Entity;


namespace TouchForFood.Mappers
{
    public class MenuIM : GenericIM
    {
        public MenuIM() : base() { }
        public MenuIM(touch_for_foodEntities db) : base(db) { }

        public menu find(int id) 
        {
            try
            {
                return db.menus.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<menu> find(bool is_active, bool is_deleted)
        {
            try
            {
                return db.menus.Where(m => m.is_active == is_active && m.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<menu> findByRestaurant(int restoId)
        {
            try
            {
                return db.menus.Include(m => m.restaurant).Include(m => m.menu_category).Where(m => m.restaurant.id == restoId && m.is_active == true && m.is_deleted == false).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<menu> findByUser(int userId)
        {
            try
            {
                //Get all the user's associations to restaurant and add each of those 
                //restaurant's menus to the list of menus to be returned
                user user = db.users.Find(userId);
                List<menu> menus = new List<menu>();
                foreach (restaurant_user ru in user.restaurant_user)
                {
                    menus.AddRange(ru.restaurant.menus);
                }
                return menus;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}