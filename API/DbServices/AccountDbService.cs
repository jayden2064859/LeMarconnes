using API.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace API.DbServices
{
    public class AccountDbService
    {
        private readonly LeMarconnesDbContext _context;

        public AccountDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        private bool ValidateAccountRole(AccountDTO dto)
        {
            if (dto.Role == Account.Role.Customer)
            {
                return false;
            }
            return true;
        }

        private string HashPassword(string plainPassword)
        {
            var hasher = new PasswordHasher<Account>();
            var passwordHash = hasher.HashPassword(null, plainPassword);
            return passwordHash;
        }

        private Account CreateAccountObject(string passwordHash, AccountDTO dto)
        {
            var account = new Account(
                dto.Username,
                passwordHash,
                dto.Role);

            return account;
        }

        // voor POST /api/accounts (nieuw account)
        public async Task<(Account? account, string? error)> PostAccountAsync(AccountDTO dto)
        {
            // valideren dat deze methode alleen admin/employee accounts kan aanmaken
            var validRole = ValidateAccountRole(dto);
            if (!validRole)
            {
                return (null, "Gebruik registratie endpoint om klantaccounts aan te maken");
            }

            // wachtwoord hashen
            var hashedPassword = HashPassword(dto.PlainPassword);

            // account object aanmaken (via constructor0
            var account = CreateAccountObject(hashedPassword, dto);

            // account toevoegen en opslaan in database
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return (account, null);
        }


        // voor GET /api/accounts (alle)
        public async Task<List<Account>> GetAllAccountsAsync()
        {
            var allAccounts = await _context.Accounts
                .ToListAsync();

            return allAccounts;

        }

        // voor GET /api/accounts/{accountId}
        public async Task<(Account? account, string? error)> GetAccountByIdAsync(int accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(c => c.AccountId == accountId);

            if (account == null)
            {
                return (null, $"Account met id {accountId} niet gevonden");
            }

            return (account, null);
        }


        // voor DELETE /api/accounts/{accountId}
        public async Task<(bool succes, string? error)> DeleteAccountAsync(int accountId)
        {
            var account = await _context.Accounts.FindAsync(accountId);

            if (account == null)
            {
                return (false, $"Account met id {accountId} niet gevonden");
            }
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            
            return (true, null);    
        }

    }
}
