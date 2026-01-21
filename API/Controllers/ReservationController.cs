using API.DbServices;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        // dependency injection van de camping service
        private readonly ReservationDbService _dbService;

        public ReservationController(ReservationDbService dbService)
        {
            _dbService = dbService;
        }

        // POST: api/reservation/camping - nieuwe reservering aanmaken
        [Authorize(Roles = "Admin,Customer")] // endpoint is alleen voor klanten bedoelt, maar admin mag nog steeds access
        [HttpPost("camping")]
        public async Task<ActionResult<CampingReservationResponseDTO>> PostCampingReservation(CampingReservationDTO dto)
        {
            try
            {   // probeer de nieuwe reservering toe te voegen via de service (geeft nullable tuple terug voor reservering object of errormessage)
                var (reservationResponseDto, errorMessage) = await _dbService.AddCampingReservationAsync(dto);

                if (errorMessage != null)
                {
                    return Conflict(errorMessage);
                }
                return Ok(reservationResponseDto);
            }

            catch (ArgumentException ex) // vangt de validations van constructor op
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, "Er is een fout opgetreden bij het opslaan van de reservering");
            }
        }


        // GET: api/reservation - alle reserveringen ophalen
        [Authorize(Roles = "Admin")] 
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            var allReservations = await _dbService.GetAllReservationsAsync();
            return Ok(allReservations);
        }


        // GET: api/reservation/{id} - specifieke reservering ophalen
        [Authorize(Roles = "Admin")] 
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            var (reservation, errorMessage) = await _dbService.GetReservationByIdAsync(id);

            if (errorMessage != null)
            {
                return NotFound(errorMessage);
            }
            return Ok(reservation);

        }

        // PUT: api/Reservation/{id} - specifieke reservering bewerken
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            var (succesfullyUpdated, errorMessage) = await _dbService.UpdateReservationAsync(id, reservation);

            if (!succesfullyUpdated)
            {
                return NotFound(errorMessage);
            }

            return Ok($"Reservering {id} is succesvol gewijzigd.");
        }

        // DELETE: api/Reservation/{id} - specifieke reservering verwijderen
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var (success, errorMessage) = await _dbService.DeleteReservationAsync(id);

            if (!success)
            {
                return NotFound(errorMessage);
            }
            return Ok($"Reservering met id {id} verwijderd.");





        }
    }
}