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
        private readonly CampingDbContext _context;
        
        public ReservationController(CampingDbContext context)
        {
            _context = context;  
        }

        // POST: api/reservation
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(CreateReservationDTO dto)
        {
            // business rule validaties

            if (!ReservationValidation.ValidDateInput(dto.StartDate, dto.EndDate))
            {
                return Conflict("Start- en einddatum zijn ongeldig");
            }
               
            if (!ReservationValidation.ValidateReservationDates(dto.StartDate, dto.EndDate))
            {
                return Conflict("Einddatum moet later zijn dan startdatum");
            }
               
            if (!ReservationValidation.ValidateAccommodationCount(dto.AccommodationIds))
            {
                return Conflict("Minimaal 1 en maximaal 2 accommodaties toegestaan");
            }

            if (!ReservationValidation.ValidateAdultCounts(dto.AdultsCount))
            {
                return Conflict("Minimaal 1 en maximaal 4 volwassenen");
            }

            if (!ReservationValidation.ValidateChildrenCount(dto.Children0_7Count, dto.Children7_12Count))
            {
                return Conflict("Kinderen: Minimaal 0 en maximaal 2 per categorie");
            }              

            if (!ReservationValidation.ValidateDogsCount(dto.DogsCount))
            {
                return Conflict("Maximaal 2 honden toegestaan");
            }
               
            int numberOfNights = (dto.EndDate - dto.StartDate).Days;
            if (dto.HasElectricity && !ReservationValidation.ValidateElectricityDays(dto.ElectricityDays, numberOfNights))
            {
                return Conflict($"Elektriciteitsdagen moeten tussen 1 en {numberOfNights} liggen");
            }
              
            
            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == dto.CustomerId);

            if (customer == null)
            {
                return NotFound($"Klant met ID {dto.CustomerId} niet gevonden");
            }
                
            // alle geselecteerde accommodaties ophalen
            var accommodations = await _context.Accommodations
                .Include(a => a.AccommodationType)
                .Where(a => dto.AccommodationIds.Contains(a.AccommodationId))
                .ToListAsync();

            if (accommodations.Count == 0)
            {
                return NotFound("Geen geldige accommodaties gevonden");
            }
            

            if (accommodations.Count != dto.AccommodationIds.Count)
            {
                return NotFound("Niet alle accommodaties konden worden gevonden");
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

            // reservation koppelen aan customer
            customer.AddReservation(reservation);

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
                
                // linq gebruiken om PlaceNumbers van gekozen accommodaties op te halen
                AccommodationPlaceNumbers = reservation.Accommodations
                    .Select(a => a.PlaceNumber)
                    .ToList()
            };
            return Ok(responseDto);
        }




        // GET: api/reservation
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetReservations()
        {
            var allReservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodations)
                .ThenInclude(a => a.AccommodationType)
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