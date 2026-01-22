using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace API.DbServices
{
    public class RegistrationDbService
    {
        private readonly LeMarconnesDbContext _context;

        public RegistrationDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        // check of username al bestaat
        private async Task<string?> ValidateUsernameAsync(string username)
        {
            var usernameExists = await _context.Accounts.AnyAsync(a => a.Username == username);
            if (usernameExists)
            {
                return "Username is al geregistreerd";
            }
            return null;
        }

        // check of email al bestaat
        private async Task<string?> ValidateEmailAsync(string email)
        {
            var emailExists = await _context.Customers.AnyAsync(c => c.Email == email);
            if (emailExists)
            {
                return "Email is al geregistreerd";
            }
            return null;
        }

        // check of telefoonnummer al bestaat
        private async Task<string?> ValidatePhoneAsync(string phone)
        {
            var phoneExists = await _context.Customers.AnyAsync(c => c.Phone == phone);
            if (phoneExists)
            {
                return "Telefoonnummer is al geregistreerd";
            }
            return null;
        }

        // customer + account object aanmaken
        public async Task<string?> CreateUserAsync(RegistrationDTO dto)
        {
            // private methods van de class gebruiken voor validatie
            var usernameErrorMsg = await ValidateUsernameAsync(dto.Username);

            // username validatie
            if (usernameErrorMsg != null)
            {
                return usernameErrorMsg;
            }
           
            // email validatie
            var emailErrorMsg =  await ValidateEmailAsync(dto.Email);
            
            if (emailErrorMsg != null)
            {
                return emailErrorMsg;
            }
            
            // telefoonnummer validatie
            var phoneErrorMsg = await ValidatePhoneAsync(dto.Phone);
            
            if (phoneErrorMsg != null)
            {
                return phoneErrorMsg;
            }

            // transaction beginnen om ervoor te zorgen dat beide klant EN accountobject succesvol aangemaakt worden. als 1 mislukt, dan wordt niks toegevoegd (rollback)
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var customer = new Customer(
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
                    dto.Infix
                );

                var hasher = new PasswordHasher<Account>();
                var passwordHash = hasher.HashPassword(null, dto.Password);

                var account = new Account(
                    dto.Username,
                    passwordHash,
                    customer // customer wordt aan account gelinked via constructor 
                );

                _context.Customers.Add(customer);
                _context.Accounts.Add(account);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException) // als er iets misgaat tijdens transaction wordt er een rollback van alles binnen de transactie gedaan
            {
                await transaction.RollbackAsync();
                throw;
            }
            return null; // null returnen als alles succesvol is verlopen (geen returntype nodig na registratie)


        }
    }

}