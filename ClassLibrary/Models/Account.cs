using ClassLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace ClassLibrary.Models
{
    public class Account
    {

        public int AccountId { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Role AccountRole { get; set; }
        public enum Role
        {
            Customer = 0,
            Employee = 1,
            Admin = 2

        }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }

        // customer aan account linken
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // constructor voor klant account aanmaken
        public Account(string username, string passwordHash, Customer customer)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Gebruikersnaam is verplicht", nameof(username));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Wachtwoord hash is verplicht", nameof(passwordHash));
                    
            Username = username;
            PasswordHash = passwordHash;
            AccountRole = Account.Role.Customer;
            CustomerId = customer.CustomerId;
            Customer = customer;
            IsActive = false; //wanneer een user de web app betreedt wordt IsActive op true gezet           
            RegistrationDate = DateTime.Now;
        }

        // constructor voor aanmaken medewerker/admin account
        public Account(string username, string passwordHash, Role role)
        {
            if (role == Role.Customer)
            {
                throw new ArgumentException("Gebruik de customer constructor voor het aanmaken van een klant");
            }

            Username = username;
            PasswordHash = passwordHash;
            AccountRole = role;
            IsActive = false;
            RegistrationDate = DateTime.Now;
        }

        public Account()
        {
        }
    }
}