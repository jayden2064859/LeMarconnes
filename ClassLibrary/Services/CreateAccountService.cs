using ClassLibrary.DTOs;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Services
{
    public class CreateAccountService
    {
        public static CreateAccountDTO CreateNewAccountDTO(int customerId, string username, string plainPassword)
        {
            return new CreateAccountDTO
            {
                CustomerId = customerId,
                Username = username,
                PlainPassword = plainPassword
            };
        }
        public static bool DoPasswordsMatch(string password, string confirmPassword)
        {
            // wachtwoord inputs moeten matchen
            if (password != confirmPassword)
            {
                return false;
            }
            return true;
        }

        public static bool ValidPasswordLength(string password)
        {
            // minimaal 4 karakters
            if (password.Length <= 3)
            {
                return false;

            }
            return true;
        }
        
        public static bool ValidateFields(string username, string password, string confirmPassword)
        {
            // LINQ om alle parameters in 1 keer te valideren
            var requiredFields = new[] { username, password, confirmPassword };
            if (requiredFields.Any(string.IsNullOrWhiteSpace))
            {
                return false;
            }
            return true;
        }


        // ervoor zorgen dat elke username minimaal 4 karakters is
        public static bool ValidUsernameLength(string username)
        {
            if (username.Length <= 3 || username.Length > 20)
            {
                return false;
            }
            return true;
        }

        // checken of de username invoer alleen maar uit letters en cijfers bestaat (speciale karakters niet toegestaan)
        public static bool ValidUsernameChars(string username)
        {
            if (!username.All(char.IsLetterOrDigit))
            {
                return false;
            }
            return true;
        }






    };

}
