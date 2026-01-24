using API.Data;
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
        public async Task<ActionResult<List<AvailableForDatesResponseDTO>>> GetAvailableAccommodationsForDates(int accommodationTypeId, DateOnly startDate, DateOnly endDate)

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
                .Select(a => new AvailableForDatesResponseDTO // voor elke accommodatie die aan bovenstaande regels voldoet, voeg de gevraagde properties toe aan een Dto
                {
                    AccommodationId = a.AccommodationId,
                    PlaceNumber = a.PlaceNumber,
                    Capacity = a.Capacity,
                    AccommodationTypeId = a.AccommodationTypeId,

                    

                }).ToListAsync();

            return Ok(availableAccommodations);
        }


        // GET: api/accommodation
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<GetAllAccommodationsDTO>>> GetAllAccommodations()
        {
            var accommodations = await _context.Accommodations
                .Select(a => new GetAllAccommodationsDTO
                {
                    PlaceNumber = a.PlaceNumber,
                    Capacity = a.Capacity,
                    AccommodationTypeName = a.AccommodationType.TypeName,
                }
                ).ToListAsync();
            return Ok(accommodations);
        }

        // GET: api/accommodation/{id} 
        [Authorize(Roles = "Admin,Employee")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Accommodation>> GetAccommodation(int id)

        {
            var accommodation = await _context.Accommodations
                .FirstOrDefaultAsync(c => c.AccommodationId == id);

            if (accommodation == null)
            {
                return NotFound($"Accommodatie met id {id} niet gevonden");
            }

            return accommodation;
        }


        //  POST: api/accommodation
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<PostAccommodationResponseDTO>> PostAccommodation(PostAccommodationDTO dto)
        {
            var accommodation = new Accommodation
            {
                PlaceNumber = dto.PlaceNumber,
                Capacity = dto.Capacity,
                AccommodationTypeId = dto.AccommodationTypeId 
                                                             
            };

            _context.Accommodations.Add(accommodation);
            await _context.SaveChangesAsync();

            var createdAccommodation = await _context.Accommodations
                .Include(a => a.AccommodationType)
                .FirstOrDefaultAsync(a => a.AccommodationId == accommodation.AccommodationId);
            if (createdAccommodation == null)
            {
                return NotFound("Accommodatie niet gevonden");
            }

            var responseDto = new PostAccommodationResponseDTO
            {
                AccommodationId = createdAccommodation.AccommodationId,
                PlaceNumber = createdAccommodation.PlaceNumber,
                Capacity = createdAccommodation.Capacity,
                AccommodationTypeId = createdAccommodation.AccommodationTypeId,
                AccommodationTypeName = createdAccommodation.AccommodationType.TypeName
            };

            return Ok(responseDto);
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

