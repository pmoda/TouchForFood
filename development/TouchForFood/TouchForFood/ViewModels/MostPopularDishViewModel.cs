using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TouchForFood.ViewModels
{
    public class MostPopularDishViewModel 
    {
        //
        // GET: /MostPopularDishViewModel/
        public int restoId { get; set; }
        public string restoName { get; set; }
        public string menuItemName { get; set; }
        public int timesOrdered { get; set; }
        public int menuItemId { get; set; }

        public MostPopularDishViewModel(int restoId, String restoName, String menuItemName, int timesOrdered, int menuItemId)
        {
            this.restoId = restoId;
            this.restoName = restoName;
            this.menuItemName = menuItemName;
            this.timesOrdered = timesOrdered;
            this.menuItemId = menuItemId;
        }

        public MostPopularDishViewModel()
        {
        }

    }
}
