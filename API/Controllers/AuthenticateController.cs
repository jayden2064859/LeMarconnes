using API.DbServices;
using API.Services;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [AllowAnonymous] // authenticate endpoint moet publiekelijk bereikbaar zijn
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        
        private readonly AuthenticateDbService _dbService;

        public AuthenticateController(AuthenticateDbService dbService)
        {
            _dbService = dbService;
        }

        // POST: api/authenticate
        [HttpPost]
        public async Task<ActionResult<AuthenticateResponseDTO>> Login(AuthenticateDTO dto)
        {

            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Ongeldige invoer");
            }

            var (authenticateResponseDTO, errorMessage) = await _dbService.AuthenticateAsync(dto);

            if (authenticateResponseDTO == null)
            {
                return Unauthorized(errorMessage);
            }
            return Ok(authenticateResponseDTO);
        }
    }
}