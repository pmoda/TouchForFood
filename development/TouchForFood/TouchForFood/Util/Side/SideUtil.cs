using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Mappers;

namespace TouchForFood.Util.Side
{
    public class SideUtil
    {
        /**
         * Filters the sides and only returns those that are not already assigned to the menu
         * that is passed in.
         * */
        public static IList<side> FilterListBySide(menu_category menu_category)
        {
            touch_for_foodEntities db = new touch_for_foodEntities();
            List<side> filteredList = new List<side>();

            int resto_id = menu_category.menu.resto_id;
            //  We want all sides that do not exist in this menu_category and that belong to this restaurant.
            //  is_deleted and is_active need to be considered too. Show only sides where is_active is false
            //  if side is_deleted is true, then there should be another page to revert that. the side functionality
            //  I'm putting should only set a side to is_active is true or false
            List<side> restaurantSides = db.sides.Where
                (
                    si => 
                        si.menu_category.menu.resto_id == resto_id 
                        && si.is_active == false
                        && si.is_deleted == false
                ).ToList();

            foreach (var side in restaurantSides)
            {
                if (side.menu_category_id.Equals(menu_category.id))
                {
                    filteredList.Add(side);
                }
            }
            db.Dispose();
            return filteredList;
        }
    }
}