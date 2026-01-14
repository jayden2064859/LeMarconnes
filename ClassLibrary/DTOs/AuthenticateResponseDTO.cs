using ClassLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs
{
    public class AuthenticateResponseDTO
    {
        public string Username { get; set; }
        public Account.Role Role { get; set; }
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }

        [Required] 
        public string Token { get; set; }
    }
}
