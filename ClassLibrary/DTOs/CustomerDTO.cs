using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs
{
    public class CustomerDTO
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public string? Infix { get; set; } 
    }
}
