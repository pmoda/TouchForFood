using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TouchForFood.ViewModels
{
    // Should be able to search:
    // Item:
    // - name, description, metadata
    // Category:
    // - name
    // Menu:
    // - name
    public class SearchViewModel
    {
        // results will be displayed according to menu_item id
        public int menuItemId { get; set; }

        // highest ranked results will be on name and description
        public string name { get; set; }
        public string description { get; set; }

        // lower ranked results will have matches in metadata
        public string metadata { get; set; }

        // lowest ranked results will have matches in category and menu name
        public string catName { get; set; }
        public string menuName { get; set; }

        public decimal price { get; set; }

        public SearchViewModel(int menuItemId, string name, string description, string metadata, string catName, string menuName, decimal price)
        {
            this.menuItemId = menuItemId;
            this.name = name;
            this.description = description;
            this.metadata = metadata;
            this.catName = catName;
            this.menuName = menuName;
            this.price = price;
        }

        public SearchViewModel()
        {
        }
    }
}