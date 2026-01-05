using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CampingDbContext _context;

        public AccountController(CampingDbContext context)
        {
            _context = context;
        }

        // GET: api/account
        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
             var allAccounts = await _context.Accounts
            .ToListAsync();

            return allAccounts;
        }


        // GET: api/account/{id}
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


        // GET: api/account/exists/{username}
        [HttpGet("exists/{username}")]
        public async Task<IActionResult> CheckUsernameExists(string username)
        {
            var exists = await _context.Accounts.AnyAsync(a => a.Username == username);

            if (exists)
            {
                return Ok();
            }
            else
            {
                return NotFound("Gebruikersnaam is al in gebruik");
            }
        }

        // POST: api/account
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(CreateAccountDTO dto)
        {
            var customer = await _context.Customers.FindAsync(dto.CustomerId);
            if (customer == null)
                return NotFound("Customer bestaat niet");

            // wachtwoord hashen 
            var hasher = new PasswordHasher<Account>();
            var passwordHash = hasher.HashPassword(null, dto.PlainPassword);


            // constructor gebruiken om account object aan te maken
            var account = new Account(
                dto.Username,
                passwordHash,
                customer);

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return Ok(account);
        }


        // PUT: api/account
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            _context.Entry(account).State = EntityState.Modified;
 
            await _context.SaveChangesAsync();

            if (!AccountExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // bool voor PUT method
        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }


        // DELETE: api/account
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
