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
            [Display(Name = "Klant")]
            Customer = 0,

            [Display(Name = "Medewerker")]
            Employee = 1,

            [Display(Name = "Beheerder")]
            Admin = 2

        }
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }

        // customer aan account linken
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }

        // constructors
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

        public Account()
        {
        }

        // methods
        public void Authenticate(string password) //wordt bool
        {

        }

        public void HasRole(string requiredRole) // wordt bool
        {

        }

    }
}