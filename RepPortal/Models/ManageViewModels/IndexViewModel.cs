using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public string Username { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Company if Applicable")]
        [StringLength(50, ErrorMessage = "Please shorten Company Name to less than 50 characters.")]
        public string Company { get; set; }

        [Display(Name = "Commission Rate")]
        public double CommissionRate { get; set; }

        public string StatusMessage { get; set; }


    }
}
