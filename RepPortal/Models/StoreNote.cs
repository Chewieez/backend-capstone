﻿using System;
using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class StoreNote
    {
        [Key]
        public int StoreNoteId { get; set; }

        [Required]
        public int StoreId { get; set; }

        public Store Store { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        [Required]
        [Display(Name = "Note")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }
    }
}