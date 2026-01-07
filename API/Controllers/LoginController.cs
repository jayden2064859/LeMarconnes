using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
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

            if (!LoginValidation.RequiredFields(loginDto.Username, loginDto.Password))
            {
                return BadRequest("Ongeldige invoer");
            }

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

            var response = new LoginResponseDTO
            {
                Username = account.Username,
                Role = account.AccountRole,
                CustomerId = account.CustomerId,
                FirstName = account.Customer?.FirstName
            };

            return Ok(response);
        }
    }
}