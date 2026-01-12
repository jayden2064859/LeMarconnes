using ClassLibrary.Models;

namespace ClassLibrary.DTOs
{
    public class LoginResponseDTO
    {
        public string Username { get; set; }
        public Account.Role Role { get; set; }
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
    }
}
