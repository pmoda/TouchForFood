using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TouchForFood.Models;

namespace TouchForFood.Util.Search
{
    public class SearchService
    {
        // Should be able to search:
        // Item:
        // - name, description, metadata
        // Category:
        // - name
        // Menu:
        // - name
        public static IList<menu_item> GetAll(int restoId)
        {
            using (var db = new touch_for_foodEntities())
            {
                return db.menu_item
                    .Include(x => x.item)
                    .Include(x => x.menu_category.category)
                    .Include(x => x.menu_category.menu)
                    .Where
                    (
                        x => x.is_active == true && 
                        x.is_deleted == false &&
                        x.menu_category.is_active == true &&
                        x.menu_category.is_deleted == false &&
                        x.menu_category.menu.resto_id == restoId && 
                        x.menu_category.menu.is_active == true &&
                        x.menu_category.menu.is_deleted == false
                    ).ToList();
            }
        }
    }
}