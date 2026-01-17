using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
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
        public async Task<string?> ValidateUsernameAsync(string username)
        {
            if (await _context.Accounts.AnyAsync(a => a.Username == username))
            {
                return "Username is al geregistreerd";
            }
            return null;
        }

        // check of email al bestaat
        public async Task<string?> ValidateEmailAsync(string email)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == email))
            {
                return "Email is al geregistreerd";
            }
            return null;
        }

        // check of telefoonnummer al bestaat
        public async Task<string?> ValidatePhoneAsync(string phone)
        {
            if (await _context.Customers.AnyAsync(c => c.Phone == phone))
            {
                return "Telefoonnummer is al geregistreerd";
            }
            return null;
        }

        // customer + account object aanmaken
        public async Task CreateUserAsync(RegistrationDTO dto)
        {
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


        }
    }

}