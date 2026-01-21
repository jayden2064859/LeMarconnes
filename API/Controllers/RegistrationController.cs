using API.DbServices;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [AllowAnonymous] // public endpoint
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly RegistrationDbService _dbService;

        public RegistrationController(RegistrationDbService dbService)
        {
            _dbService = dbService; 
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationDTO dto)
        {            
            try
            {
                var registrationError = await _dbService.CreateUserAsync(dto);
                if (registrationError != null)
                {
                    return Conflict(registrationError);
                }
                return Ok("Registratie voltooid");
            }
            catch (ArgumentException ex) // constructor validaties van account + customer object worden hier opgevangen
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Er is een fout opgetreden tijdens het opslaan van de registratie.");
            }
        }

    }
}