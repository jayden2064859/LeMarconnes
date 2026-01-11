using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    // LINQ queries (database communicatie via DbContext) worden alleen uitgevoerd in deze services, zodat de API controllers nooit directe communicatie hebben met de database (encapsulation)
    public class PostCampingReservationService
    {
        private readonly LeMarconnesDbContext _context;

        public PostCampingReservationService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        public async Task<string?> ValidateAccommodationAvailability(DateOnly startDate, DateOnly endDate, List<int> accommodationIds)
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
                    string error = $"Accommodatie {accommodation.PlaceNumber} is niet beschikbaar voor de ingevoerde datum";
                    return error;
                }
            }
            return null; // geen conflicten
        }


        public async Task<(Customer?, string?)> GetCustomer(int customerId)
        {
            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                string error = "Klant niet gevonden";
                return (null, error);
            }
            return (customer, null);
        }





    }
}






