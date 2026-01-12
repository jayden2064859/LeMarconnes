using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.DTOs
{
    public class AccountDTO
    {
        [Required]
        public int? CustomerId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PlainPassword { get; set; }

        public Account.Role? Role { get; set; }      
    }
}
