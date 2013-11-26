using System.ComponentModel.DataAnnotations;
using TouchForFood.Util.Review;
using System.Web.Mvc;
using System.Collections.Generic;

namespace TouchForFood.Models
{
    [MetadataType(typeof(ReviewOrderItemMetadata))]
    public partial class review_order_item
    {
    }

    public class ReviewOrderItemMetadata
    {
        [Display(Name = "Written On")]
        [DataType(DataType.Date)]
        public System.DateTime submitted_on { get; set; }

        [Display(Name = "Rate This Item")]
        public int rating { get; set; }
    }
}