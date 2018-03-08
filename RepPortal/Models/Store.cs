using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int StateId { get; set; }

        public State State { get; set; }

        [Required]
        [StringLength(5, ErrorMessage = "Too many characters.")]
        public string Zipcode { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        [StringLength(11, ErrorMessage = "The phone number is too long.")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Website")]
        [StringLength(100, ErrorMessage = "The website URL is too long.")]
        public string WebsiteURL { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateAdded { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        [Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Please select a status from list.")]
        public int StatusId { get; set; }

        public Status Status { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Date Store Closed")]
        public DateTime? DateClosed { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Last Order Total")]
        public double LasterOrderTotal { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Last Order Date")]
        public DateTime LastOrderDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Last Order Ship Date")]
        public DateTime LastOrderShipDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Last Order Paid Date")]
        public DateTime LastOrderPaidDate { get; set; }

        
        public int Lat { get; set; }
        
        public int Long { get; set; }


        public virtual ICollection<StoreFlag> StoreFlags { get; set; }
        public virtual ICollection<StoreNote> StoreNotes { get; set; }

    }
}