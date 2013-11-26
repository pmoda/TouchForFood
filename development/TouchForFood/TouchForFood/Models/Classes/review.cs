using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using System.Web.Mvc;

namespace TouchForFood.Models
{
    [MetadataType(typeof(ReviewMetadata))]
    public partial class review
    {
        public string written_by { get; set; }
    }

    public class ReviewMetadata
    {
        [Display(Name = "Rating")]
        [Required]
        public decimal rating { get; set; }

        [Display(Name = "Written By")]
        public string written_by { get; set; }

        [Display(Name = "Make Review Anonymous")]
        public bool is_anonymous { get; set; }

    }
}