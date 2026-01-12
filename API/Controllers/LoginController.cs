using API.DbServices;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LeMarconnesDbContext _context;
        private readonly LoginDbService _dbService;

        public LoginController(LeMarconnesDbContext context, LoginDbService dbService)
        {
            _context = context;
            _dbService = dbService;
        }

        // POST: api/login
        [HttpPost]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginDTO loginDto)
        {

            if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Ongeldige invoer");
            }

            // zoek account op basis van gebruikersnaam uit Dto
            var (account, notFoundMessage) = await _dbService.GetAccountByUsernameAsync(loginDto.Username);

            if (notFoundMessage != null)
            {
                return NotFound("Gebruikersnaam bestaat niet");
            }

            // verify wachtwoord
            // het wachtwoord in de Dto wordt gehashed en vergeleken met de opgeslagen hash van het matchende account in de database
            var (isValid, unauthorizedMessage) = await _dbService.VerifyPasswordAsync(account, loginDto.Password);
            
            if (!isValid) // in dit geval is het veiliger om op de boolean specifiek te checken voor authentication ipv errormessage 
            {
                return Unauthorized(unauthorizedMessage);
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