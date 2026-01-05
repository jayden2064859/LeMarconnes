using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
