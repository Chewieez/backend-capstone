using System;
using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class StoreFlag
    {
        [Key]
        public int StoreFlagId { get; set; }

        [Required]
        public int FlagId { get; set; }

        public Flag Flag { get; set; }

        [Required]
        public int StoreId { get; set; }

        public Store Store { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }


    }
}