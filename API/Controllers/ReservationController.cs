using LeMarconnes.Data;
using LeMarconnes.Models;
using LeMarconnes.Services;
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
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodation)
                .ToListAsync();
        }

        // GET: api/Reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodation)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // POST: api/Reservation
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(Reservation reservation)
        {
            // specifieke accommodation ophalen
            var accommodation = await _context.Accommodations
                .Include(a => a.AccommodationType)
                .FirstOrDefaultAsync(a => a.AccommodationId == reservation.AccommodationId);

            if (accommodation == null)
                return BadRequest($"Accommodatie met ID {reservation.AccommodationId} niet gevonden");

            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == reservation.CustomerId);

            if (customer == null)
                return BadRequest($"Klant met ID {reservation.CustomerId} niet gevonden");

            // haal tarieven op
            var tariffs = await _context.Tariffs
                .Where(t => t.AccommodationTypeId == accommodation.AccommodationTypeId)
                .ToListAsync();

            
            reservation.Status = "Gereserveerd";
            reservation.RegistrationDate = DateTime.Now;
            reservation.TotalPrice = TariffCalculator.CalculateTotalPrice(reservation, tariffs);

            // validatie
            if (reservation.AdultsCount < 1)
                return BadRequest("Minstens 1 volwassene vereist");

            if (reservation.EndDate <= reservation.StartDate)
                return BadRequest("Einddatum moet na startdatum liggen");


            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReservation", new { id = reservation.ReservationId }, reservation);
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