using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RepPortal.Models
{
    public class Note
    {
        [Key]
        public int NoteId { get; set; }

        [Required]
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Display(Name = "To User")]
        [ForeignKey("ToUserId")]
        public ApplicationUser ToUser { get; set; }

        [Required]
        [Display(Name = "Note")]
        public string Content { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

    }
}
