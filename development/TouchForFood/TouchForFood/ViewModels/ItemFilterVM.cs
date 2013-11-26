using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
namespace TouchForFood.Models
{
    public class ItemFilterVM
    {
        private static touch_for_foodEntities db = new touch_for_foodEntities();

        public menu_category menu_cat { get; set; }
        public IList<item> items { get; set; }
        public double new_price { get; set; }

        public ItemFilterVM(menu_category category, IList<item> items_list)
        {
            menu_cat = category;
            items = items_list;
        }

        public ItemFilterVM()
        {
            items = new List<item>();
            menu_cat = new menu_category();
        }

        public void addItem(item item)
        {
            items.Add(item);
        }

        public item FirstOrDefault()
        {
            if (items.Count > 0)
            {
                return items.First();
            }
            items.Add(new item());
            return items.First();
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