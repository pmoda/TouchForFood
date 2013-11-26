using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace TouchForFood.ViewModels
{
    public class WaiterStatsViewModel : Controller
    {
        public int restoId { get; set; }
        public string restoName { get; set; }
        public int waiterId { get; set; }
        public string waiterFirstName { get; set; }
        public string waiterLastName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int completedOrders { get; set; }
        //
        // GET: /WaiterStatsViewModel/

        public WaiterStatsViewModel(int restoId, String restoName, int waiterId, String waiterFirstName, String waiterLastName, int completedOrders)
        {
            this.restoId = restoId;
            this.restoName = restoName;
            this.waiterId = waiterId;
            this.waiterFirstName = waiterFirstName;
            this.waiterLastName = waiterLastName;
            this.completedOrders = completedOrders;
        }

        public WaiterStatsViewModel(DateTime startDate, DateTime endDate)
        {
            this.startDate = startDate;
            this.endDate = endDate;
        }

        public WaiterStatsViewModel()
        {
        }

    }
}
