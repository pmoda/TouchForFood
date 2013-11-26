using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers;

namespace TouchForFood.Util.Category
{
    public class CategoryUtil
    {
        /**
         * Filters the categories and only returns those that are not already assigned to the menu
         * that is passed in.
         * */
        public static IList<category> filterListByMenu(menu menu,touch_for_foodEntities db)
        {
            List<category> filteredList = new List<category>();
            MenuCategoryIM im = new MenuCategoryIM(db);

            CategoryIM cim = new CategoryIM(db);
            int resto_id = menu.resto_id;

            bool reject = false;
            foreach (category cat in cim.find().ToList())
            {
                reject = false;
                //First check that the category does belong to the restaurant
                //Find all usages of the category in question in the current restaurant
                List<menu_category> usages = db.menu_category.Where(mc => mc.category_id == cat.id && mc.menu.resto_id == resto_id).ToList();
                //If it was never used by this restaurant, then the restaurant could not have created it
                // because create automatically adds the created item to the menu
                if (usages.Count == 0)
                {
                    reject = true;
                }

                //Check if the category is being used buy the current menu
                foreach (menu_category menu_cat in im.find(false, menu.id))
                {
                    if (cat.id == menu_cat.category_id)
                    {
                        reject = true;
                        break;
                    }
                }
                if (!reject)
                {
                    filteredList.Add(cat);
                }
            }
            return filteredList;
        }
    }
}