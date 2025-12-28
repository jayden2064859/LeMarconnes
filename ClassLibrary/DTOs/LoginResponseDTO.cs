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
        public int AccountId { get; set; }
        public string Username { get; set; }

        public Account.Role Role { get; set; }

        // elke user die succesvol een account heeft aangemaakt heeft gegarandeerd ook een valid customer object eraan gelinked, 
        // dus voor een normale user zullen al deze properties altijd data hebben, maar voor eventuele medewerker/admin accounts die
        // vanuit de DB aangemaakt zullen worden en geen persoonsinfo hebben, maken we de customer properties alsnog nullable
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }



    }
}
