using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.DbServices
{
    public class LoginDbService
    {
        private readonly LeMarconnesDbContext _context;

        public LoginDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        public async Task<(Account? account, string? errorMessage)> GetAccountByUsernameAsync(string username)
        {
            // zoek account op basis van gebruikersnaam
            var account = await _context.Accounts
                .Include(a => a.Customer) // "include" is mogelijk door navigation property in model
                .FirstOrDefaultAsync(a => a.Username == username);

            if (account == null)
            {
                string errorMessage = "Gebruikersnaam bestaat niet";
                return (null, errorMessage); // return errormessage, geen account object
            }
            return (account, null); // return account object, geen errormessage
        }

        // het meegegeven wachtwoord wordt gehashed en vergeleken met de opgeslagen hash van het matchende account in de database
        public async Task<(bool isValid, string? errorMessage)> VerifyPasswordAsync(Account account, string password)
        {
            var hasher = new PasswordHasher<Account>();
            var result = hasher.VerifyHashedPassword(null, account.PasswordHash, password);

            if (result != PasswordVerificationResult.Success)
            {
                string errorMessage = "Ongeldige inloggegevens";
                return (false, errorMessage);
            }
            return (true, null);
        }

    }
}
