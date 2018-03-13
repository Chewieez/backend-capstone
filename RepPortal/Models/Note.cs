using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        public Guid ApplicationUserId { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

        public Guid ToUserId { get; set; }
        [Display(Name = "To User")]
        public ApplicationUser ToUser { get; set; }

        [Required]
        [Display(Name = "Note")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

    }
}
