using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly LeMarconnesDbContext _context;
        
        public ReservationController(LeMarconnesDbContext context)
        {
            _context = context;  
        }

        // POST: api/reservation/camping
        [HttpPost("camping")]
        public async Task<ActionResult<CampingReservationResponseDTO>> PostCampingReservation(CampingReservationDTO dto)
        {
            // check gekozen accommodaties voor beschikbaarheid op basis van ingevoerde datum
            foreach (var accommodationId in dto.AccommodationIds)
            {
                // voor elk accommodatie in de dto lijst, check of er al een reservering bestaat die overlapt met de datums
                bool hasOverlap = await _context.Reservations
                    .AnyAsync(r => r.Accommodations.Any(ra => ra.AccommodationId == accommodationId) &&
                                  (r.EndDate > dto.StartDate && r.StartDate < dto.EndDate)); 
                if (hasOverlap)
                {
                    var accommodation = await _context.Accommodations.FindAsync(accommodationId);
                    return Conflict($"Accommodatie {accommodation.PlaceNumber} is niet beschikbaar voor de ingevoerde datum");
                }
            }

            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

            if (customer == null)
            {
                return NotFound("Klant niet gevonden");
            }
                
            // alle geselecteerde accommodaties ophalen
            var accommodations = await _context.Accommodations
                .Where(a => dto.AccommodationIds.Contains(a.AccommodationId))
                .Where(a => a.Type == Accommodation.AccommodationType.Camping)
                .ToListAsync();

            if (!accommodations.Any())
            {
                return NotFound("Geen campingplaatsen gevonden");
            }

            // haal tarieven op voor camping 
            var tariffs = await _context.Tariffs
                .Where(t => t.AccommodationType == Accommodation.AccommodationType.Camping)
                .ToListAsync();

            try
            {
                // maak nieuwe reservering aan met constructor
                var campingReservation = new CampingReservation(
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

                // specifieke customer linken aan reservation
                campingReservation.Customer = customer;

                // reservation koppelen aan customer
                customer.AddReservation(campingReservation);

                // voeg accommodaties toe aan de reservering
                foreach (var accommodation in accommodations)
                {
                    campingReservation.AddAccommodation(accommodation);
                }

                // bereken totale prijs
                // (polymorphism: reservation base class definieert de abstract method, CampingReservation heeft zijn eigen override implementatie ervan)
                campingReservation.TotalPrice = campingReservation.CalculatePrice(tariffs);

                _context.Reservations.Add(campingReservation);
                await _context.SaveChangesAsync();

                var responseDto = new CampingReservationResponseDTO
                {
                    FirstName = campingReservation.Customer.FirstName,
                    Infix = campingReservation.Customer.Infix,
                    LastName = campingReservation.Customer.LastName,
                    StartDate = campingReservation.StartDate,
                    EndDate = campingReservation.EndDate,
                    AdultsCount = campingReservation.AdultsCount,
                    Children0_7Count = campingReservation.Children0_7Count,
                    Children7_12Count = campingReservation.Children7_12Count,
                    DogsCount = campingReservation.DogsCount,
                    HasElectricity = campingReservation.HasElectricity,
                    ElectricityDays = campingReservation.ElectricityDays,
                    TotalPrice = campingReservation.TotalPrice,

                    // linq gebruiken om PlaceNumbers van gekozen accommodaties op te halen
                    AccommodationPlaceNumbers = campingReservation.Accommodations
                        .Select(a => a.PlaceNumber)
                        .ToList()
                };
                return Ok(responseDto);
            }
            catch (ArgumentException ex) // vangt de validations van constructor op
            {
                return Conflict(ex.Message);
            }
        }



        // GET: api/reservation
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetReservations()
        {
            var allReservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodations)
                .ToListAsync();

            return allReservations;
        }


        // GET: api/reservation/{id}
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

        // PUT: api/Reservation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            if (id != reservation.ReservationId)
            {
                return NotFound();
            }

            _context.Entry(reservation).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            if (!ReservationExists(id))
            {
                return NotFound();
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