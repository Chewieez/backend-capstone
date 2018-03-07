using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RepPortal.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Company if Applicable")]
        [StringLength(50, ErrorMessage = "Please shorten Company Name to less than 50 characters.")]
        public string Company { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
        public virtual ICollection<UserState> UserStates { get; set; }
    }
}
