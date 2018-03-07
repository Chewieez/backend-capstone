using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Color { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
    }
}