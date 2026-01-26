using API.Data;
using API.DbServices;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationController : ControllerBase
    {
        private readonly AccommodationDbService _dbService;
        public AccommodationController(AccommodationDbService dbService)
        {
            _dbService = dbService;
        }

        // GET: /api/Accommodation/available-for-dates      
        [AllowAnonymous]
        [HttpGet("available-for-dates")]
        public async Task<ActionResult<List<AvailableForDatesResponseDTO>>> GetAvailableAccommodationsForDates(int accommodationTypeId, DateOnly startDate, DateOnly endDate)
        {
            try
            {
                var availableAccommodations = await _dbService.GetAvailableAccommodationsAsync(accommodationTypeId, startDate, endDate);
                return Ok(availableAccommodations);
            }
            catch (ArgumentException ex) // private method die dbService gebruikt gooit ArgumentException bij business rule violations
            {
                return Conflict(ex.Message);
            }
        }

        // GET: api/accommodation - publieke endpoint
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<GetAllAccommodationsDTO>>> GetAllAccommodations()
        {
            var accommodations = await _dbService.GetAllAccommodationsAsync();
            return Ok(accommodations);
        }


        // GET: api/accommodation/{id} 
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Accommodation>> GetAccommodation(int id)
        {
            var (accommodation, notFoundErrorMsg) = await _dbService.GetAccommodationAsync(id);
           
            if (accommodation == null)
            {
                return NotFound(notFoundErrorMsg);
            }
            return Ok(accommodation);
        }


        //  POST: api/accommodation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Accommodation>> PostAccommodation(PostAccommodationDTO dto)
        {
            var (createdAccommodation, NotFoundErrorMsg) = await _dbService.PostAccommodationAsync(dto);
            
            if (createdAccommodation == null)
            {
                return NotFound(NotFoundErrorMsg);
            }

            return Ok(createdAccommodation);
        }

        // DELETE: api/accommodation
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccommodation(int id)
        { 
            var (succesfullyDeleted, notFoundErrorMsg) = await _dbService.DeleteAccommodationAsync(id);

            if (!succesfullyDeleted)
            {
                return NotFound(notFoundErrorMsg);
            }
            return Ok("Accommodatie succesvol verwijderd");
        }     
    }
}

