using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;

namespace TouchForFood.ViewModels
{
    public class ServiceRequestVM : Controller
    {
        public IEnumerable<service_request> OpenServiceRequests { get; set; }
        public IEnumerable<service_request> AllServiceRequests { get; set; }

        public ServiceRequestVM()
        {
        }
    }
}
