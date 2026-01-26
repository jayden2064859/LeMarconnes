using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class UpdateCustomerDTO // voor PUT customer admin endpoint
    {
        public string FirstName { get; set; }
        public string? Infix { get; set; } = null;
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
