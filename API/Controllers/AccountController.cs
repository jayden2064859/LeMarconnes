using API.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.DbServices;

namespace API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountDbService _dbService;

        public AccountController(AccountDbService dbService)
        {
            _dbService = dbService;
        }

        // POST: api/account - endpoint voor admins only om handmatig non-customer accounts aan te maken
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(AccountDTO dto)
        {
            try
            {
                var account = await _dbService.PostAccountAsync(dto);

                return Ok(account);
            }
            catch (ArgumentException ex) // returned constructor validaties
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException) // returned database fouten tijdens opslaan
            {
                return StatusCode(500, "Er is een fout opgetreden tijdens het opslaan van het account.");
            }
        }

        // GET: api/account
        [Authorize(Roles = "Admin,Employee")] // medewerkers mogen accountgegevens inzien (niet bewerken)
        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            var accounts = await _dbService.GetAllAccountsAsync();
            return Ok(accounts);
        }


        // GET: api/account/{id}
        [Authorize(Roles = "Admin,Employee")]  
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccount(int id)
        {
            var (account, errorMessage) =  await _dbService.GetAccountByIdAsync(id);
            if (account == null)
            {
                return NotFound(errorMessage);
            }
            return account;
        }

 
        // DELETE: api/account
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var (succesfullyDeleted, notFoundErrormsg) = await _dbService.DeleteAccountAsync(id);

            if (!succesfullyDeleted)
            {
                return NotFound(notFoundErrormsg);
            }
            return Ok("Account succesvol verwijderd");
        }


    }
}
