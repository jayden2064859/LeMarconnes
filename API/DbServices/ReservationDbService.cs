using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace API.DbServices
{
    // LINQ queries (database communicatie via DbContext) worden alleen uitgevoerd in deze services, zodat de API controllers nooit directe communicatie hebben met de database (encapsulation)
    public class ReservationDbService
    {
        private readonly LeMarconnesDbContext _context;

        public ReservationDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        // voor POST api/reservation/camping
        public async Task<string?> ValidateAccommodationAvailabilityAsync(DateOnly startDate, DateOnly endDate, List<int> accommodationIds)
        {
            // check gekozen accommodaties voor beschikbaarheid op basis van ingevoerde datum
            foreach (var accommodationId in accommodationIds)
            {
                // voor elk accommodatie in de dto lijst, check of er al een reservering bestaat die overlapt met de datums
                bool hasOverlap = await _context.Reservations
                    .AnyAsync(r => r.Accommodations.Any(ra => ra.AccommodationId == accommodationId) &&
                                  (r.EndDate > startDate && r.StartDate < endDate));
                if (hasOverlap)
                {
                    var accommodation = await _context.Accommodations.FindAsync(accommodationId);
                    string errorMessage = $"Accommodatie {accommodation.PlaceNumber} is niet beschikbaar voor de ingevoerde datum";
                    return errorMessage;
                }
            }
            return null; // geen conflicten = geen response nodig
        }

        // return type namen worden voor tuples toegevoegd om duidelijk te maken wat de functie van beide types zijn (functioneel niet nodig)
        public async Task<(Customer? customer, string? errorMessage)> GetCustomerAsync(int customerId)
        {
            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                string errorMessage = "Klant niet gevonden";
                return (null, errorMessage);
            }
            return (customer, null);
        }

        public async Task<(List<Accommodation>? accommodations, string? errorMessage)> GetSelectedAccommodationsAsync(List<int> accommodationIds)
        {
            // alle geselecteerde accommodaties ophalen
            var accommodations = await _context.Accommodations
                .Where(a => accommodationIds.Contains(a.AccommodationId))
                .Where(a => a.AccommodationTypeId == 1)
                .ToListAsync();

            if (!accommodations.Any())
            {
                string errorMessage = "Geen accommodaties gevonden";
                return (null, errorMessage);
            }
            return (accommodations, null);
        }


        public async Task<(List<Tariff>? tariffs, string? errorMessage)> GetCampingTariffsAsync(int accommodationTypeId)
        {
            // haal tarieven op voor camping 
            var tariffs = await _context.Tariffs
                .Where(t => t.AccommodationTypeId == 1)
                .ToListAsync();

            if (!tariffs.Any())
            {
                string errorMessage = "Geen tarieven gevonden";
                return (null, errorMessage);
            }
            return (tariffs, null);
        }

        public async Task<Reservation> AddReservationAsync(Reservation reservation)  // kan voor alle subtypes gebruikt worden 
        {
            // resevering toevoegen aan db
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return reservation;
        }


        // voor GET /api/reservation (alle)
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            var allReservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodations)
                .ToListAsync();

            return allReservations;

        }

        // voor GET /api/reservation/{id}
        public async Task<(Reservation? reservation, string? errorMessage)> GetReservationByIdAsync(int id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Accommodations)
                .FirstOrDefaultAsync(r => r.ReservationId == id);

            if (reservation == null)
            {
                string errorMessage = "reservering niet gevonden";
                return (null, errorMessage);
            }
            return (reservation, null);
        }

        // voor PUT /api/reservation/{id}

        public async Task<(bool success, string? errorMessage)> UpdateReservationAsync(int id, Reservation updatedReservation)
        {
            // check of de ids overeenkomen
            if (id != updatedReservation.ReservationId)
            {
                string errorMessage = "Reservation ID mismatch";
                return (false, errorMessage);
            }

            // check of reservering bestaat
            var existingReservation = await _context.Reservations.FindAsync(id);
            if (existingReservation == null)
            {
                string errorMessage = "Reservering niet gevonden";
                return (false, errorMessage);
            }

            // query om bestaande reservering te updaten met nieuwe data
            _context.Entry(existingReservation).CurrentValues.SetValues(updatedReservation);

            await _context.SaveChangesAsync();
            return (true, null); 
        }

        // voor DELETE /api/reservation/{id}
        public async Task<(bool success, string? errorMessage)> DeleteReservationAsync(int id)
        {
            // specifieke reservering ophalen uit db
            var reservation = await _context.Reservations.FindAsync(id);

            // check of reservering is gevonden
            if (reservation == null)
            {
                string errorMessage = "ReserveringsId niet gevonden";
                return (false, errorMessage);
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return (true, null); // succesvol deleted = true, errorMessage = null
        }

    }
}






