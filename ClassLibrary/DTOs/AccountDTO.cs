using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs
{
    public class AccountDTO // voor admin only endpoint om employee/admin accounts aan te maken
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string PlainPassword { get; set; }

        [Required]
        public Account.Role Role { get; set; }      
    }
}
