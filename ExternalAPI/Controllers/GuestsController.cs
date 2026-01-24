using Microsoft.AspNetCore.Mvc;
using ExternalAPI.Classes;

namespace ExternalAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GuestsController : ControllerBase
    {
        private readonly ExternalApiTestService _service;

        public GuestsController(ExternalApiTestService service)
        {
            _service = service;
        }

        // GET: /api/Guests
        [HttpGet]
        public async Task<ActionResult<List<GuestResponseDTO>>> GetAllGuests()
        {
            try
            {
                var guests = await _service.GetAllGuestsAsync();
                return Ok(guests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET: /api/Guests/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GuestResponseDTO>> GetGuestById(int id)
        {
            try
            {
                var guest = await _service.GetGuestByIdAsync(id);
                return Ok(guest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // POST: /api/Guests
        [HttpPost]
        public async Task<ActionResult<GuestResponseDTO>> CreateGuest(GuestDTO guest)
        {
            try
            {
                var createdGuest = await _service.PostGuestAsync(guest);
                return Ok(createdGuest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
