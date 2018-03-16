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
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [ForeignKey("SalesRepId")]
        [Display(Name="Sales Rep")]
        public ApplicationUser SalesRep { get; set; }

        [Required]
        [Display(Name="Store Name")]
        [StringLength(50, ErrorMessage = "Please shorten Store Name.")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Store Street Address")]
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

        [Display(Name = "Phone Number")]
        [StringLength(11, ErrorMessage = "Please use numbers only, no dashes or symbols.")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Website")]
        [StringLength(100, ErrorMessage = "The website URL is too long.")]
        public string WebsiteURL { get; set; }

        [Display(Name = "Store Contact Name")]
        public string ContactName { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateAdded { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }

        //[Range(1, int.MaxValue, ErrorMessage = "Please select a status from list.")]
        public int StatusId { get; set; }

        public Status Status { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Store Closed")]
        public DateTime? DateClosed { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}")]
        [Display(Name = "Last Order Total")]
        public double? LastOrderTotal { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Last Order Date")]
        public DateTime? LastOrderDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Last Order Ship Date")]
        public DateTime? LastOrderShipDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Last Order Paid Date")]
        public DateTime? LastOrderPaidDate { get; set; }

        
        public string Lat { get; set; }
        
        public string Long { get; set; }


        public virtual ICollection<StoreFlag> StoreFlags { get; set; }
        public virtual ICollection<StoreNote> StoreNotes { get; set; }

    }
}