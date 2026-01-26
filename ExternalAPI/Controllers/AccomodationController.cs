using Microsoft.AspNetCore.Mvc;
using ExternalAPI.Classes;

namespace ExternalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccomodationController : ControllerBase
    {
        private readonly ExternalApiTestService _service;

        public AccomodationController(ExternalApiTestService service)
        {
            _service = service;
        }

        // GET: /api/Accomodation 
        [HttpGet]
        public async Task<ActionResult<List<AccomodationResponseDTO>>> GetAllAccomodations()
        {
            try
            {
                var allAccomodations = await _service.GetAllAccomodationsAsync();
                return Ok(allAccomodations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /api/Accomodation
        [HttpPost]
        public async Task<ActionResult<AccomodationResponseDTO>> PostAccomodation(AccomodationDTO accomodation)
        {
            try
            {
                var createdAccomodation = await _service.PostAccomodationAsync(accomodation);
                return Ok(createdAccomodation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Accomodation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AccomodationResponseDTO>> GetAccomodationById(int id)
        {
            try
            {
                var accomodations = await _service.GetAccomodationByIdAsync(id);
                return Ok(accomodations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Accomodation/availability
        [HttpGet("availability")]
        public async Task<ActionResult<List<ReservationResponseDTO>>> GetAvailableReservations(int accomodationId, DateTime checkIn, DateTime checkOut)
        {
            try
            {
                var availability = await _service.GetReservationsAvailabilityAsync(accomodationId, checkIn, checkOut);
                return Ok(availability);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
