using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models;
using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationController : ControllerBase
    {
        private readonly CampingDbContext _context;

        public AccommodationController(CampingDbContext context)
        {
            _context = context;
        }

        // GET: api/accommodation
        [HttpGet]
        public async Task<ActionResult<List<Accommodation>>> GetAccommodations()
        {
            return await _context.Accommodations
            .ToListAsync();
        }

        // GET: api/accommodation/{id} 
        [HttpGet("{id}")]
        public async Task<ActionResult<Accommodation>> GetAccommodation(int id)
        {
            var accommodation = await _context.Accommodations
                .FirstOrDefaultAsync(c => c.AccommodationId == id);

            if (accommodation == null)
            {
                return NotFound();
            }

            return accommodation;
        }

        // GET: /api/Accommodation/availabe-for-dates
        [HttpGet("available-for-dates")]
        public async Task<ActionResult<List<Accommodation>>> GetAvailableAccommodationsForDates(DateTime startDate, DateTime endDate)
        {
            var availableAccommodations = await _context.Accommodations
                .Where(a => a.AccommodationTypeId == 1 &&
                      !_context.Reservations.Any(r => r.CurrentStatus != Reservation.ReservationStatus.Verlopen &&
                      r.Accommodations.Any(ra => ra.AccommodationId == a.AccommodationId) &&
                      !(r.EndDate <= startDate || r.StartDate >= endDate)
                    ))
                .ToListAsync();

            if (!availableAccommodations.Any())
            {
                return NotFound("Geen beschikbare accommodaties gevonden voor gekozen datum");
            }

            return availableAccommodations;
        }


        //  POST: api/accommodation
        [HttpPost]
        public async Task<ActionResult<Accommodation>> PostAccommodation(Accommodation accommodation)
        {
            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            return Ok(accommodation);
        }



        // PUT: api/accommodation
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccommodation(int id, Accommodation accommodation)
        {
            if (id != accommodation.AccommodationId)
            {
                return NotFound();
            }

            if (!AccommodationExists(id))
            {
                return NotFound();
            }

            _context.Entry(accommodation).State = EntityState.Modified;
            await _context.SaveChangesAsync();  

            return NoContent();
        }

        // bool voor PUT method
        private bool AccommodationExists(int id)
        {
            return _context.Accommodations.Any(e => e.AccommodationId == id);
        }

        // DELETE: api/accommodation
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccommodation(int id)
        {
            var accommodation = await _context.Accommodations.FindAsync(id);

            if (accommodation == null)
            {
                return NotFound();
            }

            _context.Accommodations.Remove(accommodation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

      
    }
}

