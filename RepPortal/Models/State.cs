using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class State
    {
        [Key]
        public int StateId { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Store> Stores { get; set; }
    }
}