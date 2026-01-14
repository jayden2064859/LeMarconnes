// RegisterController.cs - VOLLEDIG CONSISTENT
using API.DbServices;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
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
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegistrationDTO dto)
        {

            var usernameConflict = await _dbService.ValidUsernameAsync(dto.Username);
            if (usernameConflict != null)
            {
                return Conflict(usernameConflict);
            }

            var emailConflict = await _dbService.ValidateEmailAsync(dto.Email);
            if (emailConflict != null)
            {
                return Conflict(emailConflict);
            }

            var phoneConflict = await _dbService.ValidatePhoneAsync(dto.Phone);
            if (phoneConflict != null)
            {
                return Conflict(phoneConflict);
            }

            try
            {
                await _dbService.CreateUserAsync(dto); 

                return Ok("Registratie voltooid");
            }

            catch (ArgumentException ex) // constructor validaties worden hier opgevangen
            {
                return Conflict(ex.Message); 
            }
            catch (DbUpdateException ex) // database errors worden hier opgevangen
            {
                return Conflict(ex.Message);
            }
        }
    }
}