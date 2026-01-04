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
    public class LoginController : ControllerBase
    {
        private readonly CampingDbContext _context;

        public LoginController(CampingDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO loginDto)
        {

            // zoek account op basis van gebruikersnaam
            var account = await _context.Accounts
                .Include(a => a.Customer) // <-- deze is belangrijk. hierdoor kunnen we ook alle Customer data gebruiken van een account  (dit kan door de navigation property in het model)
                .FirstOrDefaultAsync(a => a.Username == loginDto.Username);

            if (account == null)
            {
                return Unauthorized("Ongeldige inloggegevens");
            }

            // controleer wachtwoord
            var hasher = new PasswordHasher<Account>();
            var result = hasher.VerifyHashedPassword(null, account.PasswordHash, loginDto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Ongeldige inloggegevens");
            }

            // zet status op active na inloggen
            if (!account.IsActive)
            {
                account.IsActive = true;
                await _context.SaveChangesAsync();
            }

            // retourneer login response 
            var response = new LoginResponseDTO
            {
                AccountId = account.AccountId,
                Username = account.Username,
                Role = account.AccountRole,
                CustomerId = account.CustomerId,
                FirstName = account.Customer?.FirstName,
                LastName = account.Customer?.LastName,
                Email = account.Customer?.Email,
                Phone = account.Customer?.Phone
            };

            return Ok(response);
        }
    }
}