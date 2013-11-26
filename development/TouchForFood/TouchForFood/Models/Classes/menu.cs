using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TouchForFood.Models
{
    [MetadataType(typeof(MenuMetadata))]
    public partial class menu
    {
    }

    public class MenuMetadata
    {
        // The display label for the form input used by the view
        [Display(Name = "Restaurant")]
        public int? resto_id { get; set; }

        // Makes sure that the form can't be submitted without this field filled out
        [Required]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} and {1} characters long.", MinimumLength = 1)]
        // The display label for the form input used by the view
        [Display(Name = "Menu Name")]
        public string name { get; set; }

        [Display(Name = "Active")]
        public bool is_active { get; set; }
       
    }
}