using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class CreateAccountDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PlainPassword { get; set; }
    }
}
