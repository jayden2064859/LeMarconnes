using ClassLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ClassLibrary.Services
{
    public class CreateCustomerService
    {

        // dto wordt gebruikt om de data op te slaan en het van MVC naar API te sturen.
        // In de api controller wordt de dto gebruikt om met de constructor het object aan te maken
        public static CreateCustomerDTO CreateNewCustomerDTO(string firstName, string lastName, string email, string phone, string? infix = null)
        {
            return new CreateCustomerDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Infix = infix
            };
        }

       
        public static bool AccountInfoReceived(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return false;
            }
            return true;
        }

        public static bool RequiredFieldsCheck(string firstName, string lastName, string email, string phone)
        {
            // array maken van alle required fields en linq gebruiken om alles in 1 keer te checken
            var requiredFields = new[] { firstName, lastName, email, phone };
            if (requiredFields.Any(string.IsNullOrWhiteSpace))
            {
                return false;
            }
            return true;
        }

        public static bool ValidEmail(string email)
        {

            // linq gebruiken om te checken of @ gebruikt wordt in email input (minimaal en maximaal 1)
            int atCount = email.Count(c => c == '@');
            if (atCount != 1) 
            {
                return false;
            }
           return true;
        }

        public static bool ValidatePhone(string phone)
        {
            // alleen en + teken op het begin van de string toestaan als de user zijn landcode invoert
            if (phone.StartsWith("+"))
                phone = phone.Substring(1); //substring methode maakt een nieuwe string zonder de eerst ingevoerde karakter, als het een + was

            // checken of phone input alleen uit cijfers bestaat
            if (!phone.All(char.IsDigit)) 
            { 
                return false;
            }

            // nummer kan tussen de 8 en 13 cijfers zijn, afhankelijk van landcode (belgie, nederland, frankrijk etc)
            if (phone.Length < 8 || phone.Length > 13) 
            { 
                return false; 
            }         
            return true;
        }
    }
}


