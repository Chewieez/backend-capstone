using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class State
    {
        [Key]
        public int StateId { get; set; }

        [Required]
        public string Name { get; set; }
    }
}