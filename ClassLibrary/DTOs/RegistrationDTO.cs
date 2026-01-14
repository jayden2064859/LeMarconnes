using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class RegistrationDTO
    {
        // account
        public string Username { get; set; }
        public string Password { get; set; }

        // customer  
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? Infix { get; set; }
    }
}
