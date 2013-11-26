using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers;

namespace TouchForFood.Util.Item
{
    public class ItemUtil
    {
        

        /**
            * Filters the categories and only returns those that are not already assigned to the menu
            * that is passed in.
        * */
        public static IList<item> filterListByItem(menu_category menu_category,touch_for_foodEntities db)
        {
            List<item> filteredList = new List<item>();
            MenuItemIM im = new MenuItemIM(db);
            int resto_id = menu_category.menu.resto_id;
            bool reject = false;

            foreach (item i in db.items.ToList())
            {
                reject = false;
                //First check that the category does belong to the restaurant
                //Find all usages of the category in question in the current restaurant
                List<menu_item> usages = db.menu_item.Where(mi => mi.item_id == i.id && mi.menu_category.menu.resto_id == resto_id).ToList();

                //If it was never used by this restaurant, then the restaurant could not have created it
                // because create automatically adds the created item to the menu
                if (usages.Count == 0)
                {
                    reject = true;
                }

                if (menu_category.category.id == i.category_id)
                {
                    foreach (menu_item m_i in im.find(false, menu_category.id))
                    {
                        if (i.id == m_i.item_id)
                        {
                            reject = true;
                            break;
                        }
                    }
                    if (!reject)
                    {
                        filteredList.Add(i);
                    }
                }
            }
            db.Dispose();
            return filteredList;
        }

        //Returns the average rating given to a menu item
        // returns 0 if never rated
        public static double getAverageRating(menu_item mi)
        {
            double totalRating = 0;
            int count = 0;
            List<review_order_item> reviews = mi.order_item.
                SelectMany(oi => oi.review_order_item).
                ToList<review_order_item>();
            foreach (review_order_item roi in reviews)
            {
                count++;
                totalRating += (int)roi.rating;
            }
            if (count == 0) { return 0; }

            return Math.Round((double)(totalRating / count), 2);
        }

    }
    
}