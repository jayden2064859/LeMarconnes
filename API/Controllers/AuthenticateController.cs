using API.DbServices;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous] // authenticate endpoint moet publiekelijk bereikbaar zijn
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        
        private readonly AuthenticateDbService _dbService;
        private readonly JwtService _jwtService;

        public AuthenticateController(AuthenticateDbService dbService, JwtService jwtService)
        {
            _dbService = dbService;
            _jwtService = jwtService;
        }

        // POST: api/authenticate
        [HttpPost]
        public async Task<ActionResult<AuthenticateResponseDTO>> Login(AuthenticateDTO dto)
        {

            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Ongeldige invoer");
            }

            // zoek account op basis van gebruikersnaam uit Dto
            var (account, notFoundMessage) = await _dbService.GetAccountByUsernameAsync(dto.Username);

            if (notFoundMessage != null)
            {
                return NotFound("Gebruikersnaam bestaat niet");
            }

            // verify wachtwoord
            // het wachtwoord in de Dto wordt gehashed en vergeleken met de opgeslagen hash van het matchende account in de database
            var (isValid, unauthorizedMessage) = await _dbService.VerifyPasswordAsync(account, dto.Password);
            
            if (!isValid) // in dit geval is het veiliger om op de boolean specifiek te checken voor authentication ipv errormessage 
            {
                return Unauthorized(unauthorizedMessage);
            }

            // na geldige login wordt een jwt token gegenereerd via de service. Met deze token kan een ingelogde user de endpoints gebruiken
            var token = _jwtService.GenerateToken(account);

            var response = new AuthenticateResponseDTO
            {
                Username = account.Username,
                Role = account.AccountRole,
                CustomerId = account.CustomerId,
                FirstName = account.Customer?.FirstName,
                Token = token
            };

            return Ok(response);
        }
    }
}