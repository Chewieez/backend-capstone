using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class Flag
    {
        [Key]
        public int FlagId { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<StoreFlag> StoreFlags { get; set; }
    }
}