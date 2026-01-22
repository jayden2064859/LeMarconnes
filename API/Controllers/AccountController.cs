using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly LeMarconnesDbContext _context;

        public AccountController(LeMarconnesDbContext context)
        {
            _context = context;
        }

        // POST: api/account - endpoint voor admins only om handmatig non-customer accounts aan te maken
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(AccountDTO dto)
        {
            if (dto.Role == Account.Role.Customer)
            {
                return Conflict("Gebruik registratie endpoint om klantaccounts aan te maken");
            }

            // wachtwoord hashen 
            var hasher = new PasswordHasher<Account>();
            var passwordHash = hasher.HashPassword(null, dto.PlainPassword);

            try
            {
                // 2e account constructor gebruiken voor non customers 
                var account = new Account(
                    dto.Username,
                    passwordHash,
                    dto.Role);

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                return Ok(account);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // GET: api/account
        [Authorize(Roles = "Admin,Employee")] // medewerkers mogen accountgegevens inzien (niet bewerken)
        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            var allAccounts = await _context.Accounts
           .ToListAsync();

            return allAccounts;
        }


        // GET: api/account/{id}
        [Authorize(Roles = "Admin,Employee")]  
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(c => c.AccountId == id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

 
        // DELETE: api/account
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}
