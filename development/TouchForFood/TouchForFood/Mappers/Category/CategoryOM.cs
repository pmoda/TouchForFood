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
    public class CategoryOM : GenericOM
    {
        public CategoryOM() : base() {}
        public CategoryOM(touch_for_foodEntities db) : base(db){}

        /// <summary>
        /// Write a category object to the database
        /// </summary>
        /// <param name="category">The category to write</param>
        /// <returns>true if successful, false otherwise.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Create(category category)
        {
            db.categories.Add(category);
            return (db.SaveChanges() == 1);
        }

        /// <summary>
        /// Is responsible for handling any modifications to category items
        /// </summary>
        /// <param name="category">Edited category</param>
        /// <returns>Boolean indicating whether the modification took effect</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public Boolean edit(category category)
        {
            CategoryIM im = new CategoryIM(db);
            category dbVersion = im.find(category.id);
            if (dbVersion.version == category.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(category).State = EntityState.Modified;

                category.version = category.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;

        }

        /// </summary>
        /// <param name="cat">category to be deleted</param>
        /// <returns>number of rows affected by the database</returns>
        /// <exception cref="AssociationExistsException">Category is associated with either an item or a menu category</exception>
        /// <exception cref="InvalidOperationException">Can be thrown when saving to database.</exception>
        public override int delete(int cat_id)
        {
            category cat = db.categories.Find(cat_id);
            int result = 0;

            //check to see if the category has any items associated with it
            if (cat.items.Count > 0)
            {
                throw new AssociationExistsException("This category has items associated to it. It cannot be deleted");
            }

            //check to see if the category has menu categories associated with it
            if (cat.menu_category.Count > 0)
            {
                throw new AssociationExistsException("This category exists in a menu. It cannot be deleted");
            }

            db.categories.Remove(cat);
            result = db.SaveChanges();
           
            return result;
        }
    }
}