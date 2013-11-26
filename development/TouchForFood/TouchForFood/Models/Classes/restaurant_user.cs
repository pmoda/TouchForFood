using System.ComponentModel.DataAnnotations;

namespace TouchForFood.Models
{
    public class RestaurantUserMetadata
    {
        [Required]
        [Display(Name = "User")]
        public string user_id { get; set; }

        [Required]
        [Display(Name = "Restaurant")]
        public string restaurant_id { get; set; }
    }
}