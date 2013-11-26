using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using System.Web.Mvc;

namespace TouchForFood.Models
{
    [MetadataType(typeof(UserMetadata))]
    public partial class user 
    {
        // Create the confirm password field since it doesn't exist in the DB
        public string ConfirmPassword { get; set; }
    }

    public class UserMetadata
    {
        // Makes sure that the form can't be submitted without this field filled out
        [Required]
        // Enforce a maximum length of 50 characters, with an error message if the minimum length is less than 4
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 4)]
        // The display label for the form input used by the view
        [Display(Name = "User Name")]
        public string username { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        // Specify the data type the field is so that the form element is properly chosen (hidden characters, in this case)
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        // Compare with the password string (NOT "Password") to make sure they match
        [Compare("password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string first_name { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string last_name { get; set; }

        [Required]
        // Add the EmailAddress datatype
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string email { get; set; }

        [Display(Name = "User Image")]
        public string image_url { get; set; }

        [Display(Name = "User Role (1 = admin, 2 = cust, 4 = resto, 8 = dev)")]
        public int user_role { get; set; }
    }
}