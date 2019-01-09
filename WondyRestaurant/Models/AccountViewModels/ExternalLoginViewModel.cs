using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WondyRestaurant.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "last Name")]
        public string LastName { get; set; }

       
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}
