using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using System.Web.Mvc;

namespace TouchForFood.Models
{
    [MetadataType(typeof(RestaurantMetadata))]
    public partial class restaurant
    {
    }

    public class RestaurantMetadata
    {
        [Display(Name = "Restaurant Name")]
        public string name { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Postal Code")]
        public string postal_code { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }
    }
}