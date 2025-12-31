using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly CampingDbContext _context;
        
        public ReservationController(CampingDbContext context)
        {
            _context = context;  
        }

        // GET: api/reservation
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodations)
                .ThenInclude(a => a.AccommodationType)
                .ToListAsync();
        }

        // GET: api/Reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodations)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // POST: api/reservation
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation([FromBody] CreateReservationDTO dto)
        {
            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

            if (customer == null)
            {
                return BadRequest($"Klant met ID {dto.CustomerId} niet gevonden");
            }
                
            // alle accommodaties ophalen
            var accommodations = await _context.Accommodations
                .Include(a => a.AccommodationType)
                .Where(a => dto.AccommodationIds.Contains(a.AccommodationId))
                .ToListAsync();

            if (accommodations.Count == 0)
            {
                return BadRequest("Geen geldige accommodaties gevonden");
            }
            

            if (accommodations.Count != dto.AccommodationIds.Count)
            {
                return BadRequest("Niet alle accommodaties konden worden gevonden");
            }

            // check of accommodaties beschikbaar zijn voor de gekozen periode
            var overlappingReservations = await _context.Reservations
                .Include(r => r.Accommodations)
                .Where(r => (r.CurrentStatus == Reservation.ReservationStatus.Gereserveerd|| r.CurrentStatus == Reservation.ReservationStatus.Actief) && // zowel gereserveerde als actieve reserveringen
                           r.Accommodations.Any(a => dto.AccommodationIds.Contains(a.AccommodationId)) &&
                           !(r.EndDate <= dto.StartDate || r.StartDate >= dto.EndDate))
                .ToListAsync();

            if (overlappingReservations.Any())
            {
                var conflicterendeAccommodaties = overlappingReservations
                    .SelectMany(r => r.Accommodations)
                    .Where(a => dto.AccommodationIds.Contains(a.AccommodationId))
                    .Select(a => a.AccommodationId)
                    .Distinct()
                    .ToList();

                return BadRequest($"Deze accommodaties zijn momenteel niet beschikbaar op de gekozen datums: {conflicterendeAccommodaties}");
            }

            // haal tarieven op voor camping 
            var tariffs = await _context.Tariffs
                .Where(t => t.AccommodationTypeId == 1)
                .ToListAsync();

            // maak nieuwe reservering aan met constructor
            var reservation = new Reservation(
                dto.CustomerId,
                dto.StartDate,
                dto.EndDate,
                dto.AdultsCount,
                dto.Children0_7Count,
                dto.Children7_12Count,
                dto.DogsCount,
                dto.HasElectricity,
                dto.ElectricityDays
            );

            // specifiek customer object linken aan reservation
            reservation.Customer = customer;

            // voeg accommodaties toe aan de reservering
            foreach (var accommodation in accommodations)
            {
                reservation.AddAccommodation(accommodation);
                accommodation.CurrentStatus = Accommodation.AccommodationStatus.Bezet; // status updaten van elke accommodation die gekoppeld is aan een (niet verlopen) reservering
            }

            // bereken totale prijs
            reservation.TotalPrice = TariffCalculator.CalculateTotalPrice(
                reservation,
                tariffs,
                accommodations.Count
            );

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            var responseDto = new ReservationResponseDTO
            {
                FirstName = reservation.Customer.FirstName,
                Infix = reservation.Customer.Infix,
                LastName = reservation.Customer.LastName,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                AdultsCount = reservation.AdultsCount,
                Children0_7Count = reservation.Children0_7Count,
                Children7_12Count = reservation.Children7_12Count,
                DogsCount = reservation.DogsCount,
                HasElectricity = reservation.HasElectricity,
                ElectricityDays = reservation.ElectricityDays,
                TotalPrice = reservation.TotalPrice,
                AccommodationPlaceNumbers = reservation.Accommodations
                    .Select(a => a.PlaceNumber)
                    .ToList()
            };
            return Ok(responseDto);
        }

        // PUT: api/Reservation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return BadRequest();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Reservation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }


    }
}