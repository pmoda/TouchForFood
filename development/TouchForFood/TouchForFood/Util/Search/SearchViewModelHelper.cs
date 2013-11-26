using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.ViewModels;
using System.Data.Entity;

namespace TouchForFood.Util.Search
{
    public class SearchViewModelHelper
    {
        public static SearchViewModel PopulateSearchViewModel(menu_item menuItem)
        {
            return new SearchViewModel
            {
                menuItemId = menuItem.id,
                name = menuItem.item.name,
                description = menuItem.item.description,
                metadata = menuItem.item.metadata,
                catName = menuItem.menu_category.category.name,
                menuName = menuItem.menu_category.menu.name,
                price = menuItem.price
            };
        }

        public static IList<SearchViewModel> PopulateSearchViewModelList(IList<menu_item> searchList)
        {
            return searchList.Select(x => PopulateSearchViewModel(x)).ToList();
        }
    }
}