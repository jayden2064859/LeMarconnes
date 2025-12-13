using LeMarconnes.Models;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;

namespace LeMarconnes.Models
{
    public class Account
    {

        public int AccountId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }

        // een account kan gekoppeld zijn aan een klant, maar dat hoeft niet altijd het geval te zijn. Customer moet dus nullable zijn
        public Customer? Customer { get; set; }


        // methods
        public void Authenticate(string password) //wordt bool
        {

        }

        public void HasRole(string requiredRole) // wordt bool
        {

        }

    }
}