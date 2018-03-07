using System.ComponentModel.DataAnnotations;

namespace RepPortal.Models
{
    public class UserState
    {
        [Key]
        public int UserStateId { get; set; }

        [Required]
        public int StateId { get; set; }

        public State State { get; set; }

        [Required]
        public ApplicationUser User { get; set; }

    }
}