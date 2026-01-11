using Microsoft.AspNetCore.Mvc;
using ClassLibrary.Models;
using ClassLibrary.Data;
using Microsoft.EntityFrameworkCore;
using ClassLibrary.DTOs;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccommodationController : ControllerBase
    {
        private readonly LeMarconnesDbContext _context;

        public AccommodationController(LeMarconnesDbContext context)
        {
            _context = context;
        }

        // GET: /api/Accommodation/available-for-dates      
        // deze endpoint wordt gebruikt in de MVC om beschikbare accommodaties te tonen aan de user op basis van opgegeven datums
        // werkt voor beide Camping en Hotel accommodaties, afhankelijk van het AccommodationType enum dat je meegeeft
        [HttpGet("available-for-dates")]
        public async Task<ActionResult<List<AvailableAccommodationDTO>>> GetAvailableAccommodationsForDates(
            Accommodation.AccommodationType type,
            DateTime startDate,
            DateTime endDate)
        {
            var availableAccommodations = await _context.Accommodations // geef alle accommodaties terug waarvoor geldt:
                .Where(a => a.Type == type &&  // accommodatietype (camping of hotel) komt overeen met meegegeven type
                                               // niet gekoppeld reserveringen die overlappen met de gevraagde datum periode
                    !_context.Reservations.Any(r => r.Accommodations.Any(ra => ra.AccommodationId == a.AccommodationId) &&
                        r.StartDate < endDate && r.EndDate > startDate
                    ))
                .Select(a => new AvailableAccommodationDTO
                {
                    AccommodationId = a.AccommodationId,
                    PlaceNumber = a.PlaceNumber,
                    Capacity = a.Capacity,
                    Type = a.Type

                }).ToListAsync();

             
            return Ok(availableAccommodations);
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

