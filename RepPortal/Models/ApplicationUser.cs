using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [Display(Name = "Commission Rate")]
        public double CommissionRate { get; set; }

        public DateTime? LastLoggedInDate { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Store> Stores { get; set; }
        [InverseProperty("SalesRep")]
        public virtual ICollection<Store> StoresRep { get; set; }

        [InverseProperty("User")]
        public virtual ICollection<Note> Notes { get; set; }
        [InverseProperty("ToUser")]
        public virtual ICollection<Note> ToNotes { get; set; }

        public virtual ICollection<UserState> UserStates { get; set; }
    }
}
