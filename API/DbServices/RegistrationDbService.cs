// RegisterDbService.cs - VOLLEDIG CONSISTENT
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.DbServices
{
    public class RegistrationDbService
    {
        private readonly LeMarconnesDbContext _context;

        public RegistrationDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        public async Task<string?> ValidateEmailAsync(string email)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == email))
            {
                return "Email is al geregistreerd";
            }
            return null;
        }

        public async Task<string?> ValidUsernameAsync(string username)
        {
            var existingUsername = await _context.Accounts.AnyAsync(a => a.Username == username);

            if (existingUsername)
            {
                string error = "Username is al geregistreerd";
                return error;
            }
            return null;
        }


        public async Task<string?> ValidatePhoneAsync(string phone)
        {
            if (await _context.Customers.AnyAsync(c => c.Phone == phone))
            {
                var error = "Telefoonnummer is al geregistreerd";
                return error;
            }              
            return null;
        }

        // deze methode hoeft zelf niet per se validaties te doen, want die zijn al uitgevoerd voordat deze method gebruikt wordt
        public async Task CreateUserAsync(RegistrationDTO dto)
        {
            var customer = new Customer(
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.Phone,
                dto.Infix
            );

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var hasher = new PasswordHasher<Account>();
            var passwordHash = hasher.HashPassword(null, dto.Password);

            var account = new Account(
                dto.Username,
                passwordHash,
                customer
            );

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return; 
        }
    }
}