using System.Collections.Generic;
using System.Data;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;
using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
namespace TouchForFood.Mappers
{

    public class RestaurantOM : GenericOM
    {
        public RestaurantOM() : base() { }
        public RestaurantOM(touch_for_foodEntities db) : base(db) { }

        /// <summary>
        /// Writes a restaurant obejct to the database
        /// </summary>
        /// <param name="restaurant">The restaurant object to write</param>
        /// <param name="user">The user to associate as owner to the restaurant</param>
        /// <returns>True if successful, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Create(restaurant restaurant, user user)
        {
            if (user == null) return false;
            db.restaurants.Add(restaurant);
            if (db.SaveChanges() == 0) return false;

            // assign the user to this restaurant.
            restaurant_user ru = new restaurant_user();
            ru.user_id = user.id;
            ru.restaurant_id = restaurant.id;
            db.restaurant_user.Add(ru);

            if (db.SaveChanges() == 0)
            {
                // Remove the restaurant because the owner could not be assigned to it.
                delete(restaurant.id);
                return false;
            }
            return true;
        }

        /// <summary>
        /// when updating a record we would like to increment the value of version to
        /// </summary>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Boolean edit(restaurant restaurant)
        {
            RestaurantIM im = new RestaurantIM(db);
            restaurant dbVersion = im.find(restaurant.id);
            if (dbVersion.version == restaurant.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(restaurant).State = EntityState.Modified;
                restaurant.version = restaurant.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// When deleting a restaurant set it's menus to inactive and is deleted to false.
        /// Have to cascade down to waiter, reviews, restaurant user and table
        /// Then set the restaurant's is_deleted field to true.
        /// </summary>
        /// <param name="menu">Restaurant to delete</param>
        /// <returns>Number of rows affected by the change</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int delete(int id)
        {
            restaurant restaurant = db.restaurants.Find(id);
            int result = 0;

            clearMenus(restaurant.menus);
            clearReviews(restaurant.reviews);
            clearWaiters(restaurant.waiters);
            clearResaurantUser(restaurant.restaurant_user);
            clearTables(restaurant.tables);

            restaurant.is_deleted = true;
            db.Entry(restaurant).State = EntityState.Modified;

            result = db.SaveChanges();
            edit(restaurant);

            return result;
        }

        private void clearResaurantUser(ICollection<restaurant_user> restaurantUsers)
        {
            for (int i = 0; i < restaurantUsers.Count(); i++)
            {
                restaurantUsers.ElementAt(i).restaurant_id = null;
                db.Entry(restaurantUsers.ElementAt(i)).State = EntityState.Modified;
            }
        }

        private void clearWaiters(ICollection<waiter> waiters)
        {
            foreach (waiter w in waiters)
            {
                w.resto_id = null;
                db.Entry(w).State = EntityState.Modified;
            }
        }

        private void clearMenus(ICollection<menu> menus)
        {
            MenuOM om = new MenuOM(db);
            foreach (menu m in menus)
            {
                m.is_active = false;
                om.delete(m.id);
            }
        }

        private void clearTables(ICollection<table> tables)
        {
            TableOM om = new TableOM(db);
            for (int i = 0; i < tables.Count; i++)
            {
                om.delete(tables.ElementAt(i).id);
            }
        }

        private void clearReviews(ICollection<review> reviews)
        {
            ReviewOM om = new ReviewOM(db);
            for (int i = 0; i < reviews.Count; i++)
            {
                om.delete(reviews.ElementAt(i).id);
            }
        }
    }
}