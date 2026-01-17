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
            _dbService = dbService; // constructor injection
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationDTO dto)
        {
            // username check
            var usernameError = await _dbService.ValidateUsernameAsync(dto.Username);
            if (usernameError != null)
            {
                return Conflict(usernameError);
            }

            // email check
            var emailError = await _dbService.ValidateEmailAsync(dto.Email);
            if (emailError != null)
            {
                return Conflict(emailError);

            }

            // telefoonnummer check
            var phoneError = await _dbService.ValidatePhoneAsync(dto.Phone);
            if (phoneError != null)
            {
                return Conflict(phoneError);
            }
              
            try
            {
                await _dbService.CreateUserAsync(dto);
                return Ok("Registratie voltooid");
            }
            catch (ArgumentException ex) // constructor validaties van account + customer object worden hier opgevangen
            {
                return Conflict(ex.Message);
            }
        }

    }
}