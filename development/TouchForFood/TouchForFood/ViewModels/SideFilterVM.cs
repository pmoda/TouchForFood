using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;

namespace TouchForFood.ViewModels
{
    public class SideFilterVM
    {
        private static touch_for_foodEntities db = new touch_for_foodEntities();

        public menu_category menu_cat { get; set; }
        public IList<side> sides { get; set; }
        public double new_price { get; set; }

        public SideFilterVM(menu_category category, IList<side> sides_list)
        {
            menu_cat = category;
            sides = sides_list;
        }

        public SideFilterVM()
        {
            sides = new List<side>();
            menu_cat = new menu_category();
        }

        public void AddSide(side side)
        {
            sides.Add(side);
        }

        public side FirstOrDefault()
        {
            if (sides.Count > 0)
            {
                return sides.First();
            }
            sides.Add(new side());
            return sides.First();
        }

        public decimal? GetRegularPrice(int item_id)
        {
            decimal? regular = null;
            int maxCount = 0;
            Dictionary<decimal?, int> priceDictionary = new Dictionary<decimal?, int>();
            foreach (menu_item menu_i in db.items.Find(item_id).menu_item)
            {
                if (priceDictionary.ContainsKey(menu_i.price))
                {
                    ++priceDictionary[menu_i.price];
                }
                else
                {
                    priceDictionary.Add(menu_i.price, 1);
                }

                if (priceDictionary[menu_i.price] > maxCount)
                {
                    maxCount = priceDictionary[menu_i.price];
                    regular = menu_i.price;
                }
            }
            if (regular == null)
            {
                regular = (decimal?)00.00;
            }
            return regular;
        }
    }
}