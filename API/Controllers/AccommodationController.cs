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
        [AllowAnonymous]
        [HttpGet("available-for-dates")]
        public async Task<ActionResult<List<AvailableAccommodationDTO>>> GetAvailableAccommodationsForDates(
            int accommodationTypeId,
            DateOnly startDate,
            DateOnly endDate)
        {

            if (accommodationTypeId != 1 && accommodationTypeId != 2)
            {
                return Conflict("Ongeldig type (1 = Camping, 2 = Hotel)");
            }
            if (endDate <= startDate)
            {
                return Conflict("Einddatum moet minimaal 1 dag na startdatum zijn");
            }

            // huidige dag bepalen met datetime, en terug converten naar dateonly om te kunnen vergelijken
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (startDate < today || endDate < today.AddDays(1))
            {
                return Conflict("Datums mogen niet in het verleden zijn");
            }

            var availableAccommodations = await _context.Accommodations // geef alle accommodaties terug waarvoor geldt:
                .Where(a => a.AccommodationTypeId == accommodationTypeId &&  // accommodatietype (camping of hotel) komt overeen met meegegeven type                                             
                    !_context.Reservations.Any(r => r.Accommodations.Any(ra => ra.AccommodationId == a.AccommodationId) &&  // implicit junction table wordt hier doorzocht 
                    r.StartDate <= endDate && r.EndDate >= startDate))  // niet gekoppeld aan reserveringen die overlappen met de gevraagde datum periode
                .Select(a => new AvailableAccommodationDTO // voor elke accommodatie die aan bovenstaande regels voldoet, voeg de gevraagde properties toe aan een Dto
                {
                    AccommodationId = a.AccommodationId,
                    PlaceNumber = a.PlaceNumber,
                    Capacity = a.Capacity,
                    AccommodationTypeId = a.AccommodationTypeId

                }).ToListAsync();

            return Ok(availableAccommodations);
        }


        // GET: api/accommodation
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<Accommodation>>> GetAccommodations()
        {
            return await _context.Accommodations
            .ToListAsync();
        }

        // GET: api/accommodation/{id} 
        [AllowAnonymous]
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Accommodation>> PostAccommodation(Accommodation accommodation)
        {
            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            return Ok(accommodation);
        }



        // PUT: api/accommodation
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

