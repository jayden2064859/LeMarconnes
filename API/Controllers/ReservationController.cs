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
    public class ReservationController : ControllerBase
    {
        // dependency injection van de camping service
        private readonly ReservationDbService _dbService;

        public ReservationController(ReservationDbService dbService)
        {
            _dbService = dbService;
        }

        // POST: api/reservation/camping - nieuwe reservering aanmaken
        [Authorize(Roles = "Customer")] // endpoint is alleen voor klanten bedoelt, maar admin mag nog steeds access
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
            catch (DbUpdateException) // database errors
            {
                return StatusCode(500, "Er is een fout opgetreden bij het opslaan van de reservering");
            }
        }

        // // Yassir
        [Authorize(Roles = "Customer")]
        [HttpPost("hotel")]
        public async Task<ActionResult<HotelReservationResponseDTO>> PostHotelReservation(HotelReservationDTO dto)
        {
            try
            {
                // service aanroepen om de reservering toe te voegen
                var (responseDto, error) = await _dbService.AddHotelReservationAsync(dto);

                // foutafhandeling
                if (error != null)
                {
                    return Conflict(error);
                }

                
                return Ok(responseDto);
            }
            catch (ArgumentException ex) // vangt o.a. de capaciteits-fout op uit de model
            {
                return Conflict(ex.Message);
            }
            catch (Exception) // vangt onverwachte database-fouten op
            {
                return StatusCode(500, "Er is een fout opgetreden bij het verwerken van de hotelreservering");
            }
        }
        

        // GET: api/reservation - alle reserveringen ophalen
        [Authorize(Roles = "Admin,Employee")] 
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            var allReservations = await _dbService.GetAllReservationsAsync();
            return Ok(allReservations);
        }


        // GET: api/reservation/{id} - specifieke reservering ophalen
        [Authorize(Roles = "Admin,Employee")] 
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