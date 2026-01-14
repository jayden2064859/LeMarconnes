using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs
{
    public class AuthenticateDTO
    {
        [Required]       
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
