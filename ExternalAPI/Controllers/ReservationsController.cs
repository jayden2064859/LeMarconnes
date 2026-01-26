using Microsoft.AspNetCore.Mvc;
using ExternalAPI.Classes;

namespace ExternalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly ExternalApiTestService _service;

        public ReservationsController(ExternalApiTestService service)
        {
            _service = service;
        }

        // GET: /api/Reservations
        [HttpGet]
        public async Task<ActionResult<List<ReservationResponseDTO>>> GetAllReservations()
        {
            try
            {
                var allReservations = await _service.GetAllReservationsAsync();
                return Ok(allReservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Reservations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReservationResponseDTO>> GetReservationbyId(int id)
        {
            try
            {
                var reservation = await _service.GetReservationByIdAsync(id);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /api/Reservations
        [HttpPost]
        public async Task<ActionResult<ReservationResponseDTO>> PostReservation(ReservationDTO reservation)
        {
            try
            {
                var createdReservation = await _service.PostReservationAsync(reservation);
                return Ok(createdReservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Reservations/by-accomodation/{accomodationId}
        [HttpGet]
        public async Task<ActionResult<List<ReservationResponseDTO>>> GetReservationByAccomodationId(int accomodationId)
        {
            try
            {
                var reservation = await _service.GetReservationsByAccomodationIdAsync(accomodationId);
                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




    }
}
