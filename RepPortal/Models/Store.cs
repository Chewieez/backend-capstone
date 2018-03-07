using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class Store
    {
        [Key]
        public int StoreId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name="Store Name")]
        [StringLength(50, ErrorMessage = "Please shorten Store Name.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Street Address")]
        [StringLength(60, ErrorMessage = "Please shorten Street Address to less than 60 characters.")]
        public string StreetAddress { get; set; }

        [Required]
        [Display(Name = "City")]
        [StringLength(40, ErrorMessage = "Please shorten City name to less than 40 characters.")]
        public string City { get; set; }

        [Required]
        [Display(Name = "State")]
        [StringLength(60, ErrorMessage = "Please shorten Street Address to less than 60 characters.")]
        public int StateId { get; set; }

    }
}