using API.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<(CampingReservationResponseDTO? dto, string? error)> AddCampingReservationAsync(CampingReservationDTO dto)  // kan voor alle subtypes gebruikt worden 
        {
            // checken of accommodaties al gekoppeld zijn aan de reservering die overlappen met de ingevoerde datums
            var accommodationConflictMessage = await ValidateAccommodationAvailabilityAsync(
                dto.StartDate,
                dto.EndDate,
                dto.AccommodationIds);

            if (accommodationConflictMessage != null)
            {
                return (null, accommodationConflictMessage);
            }

            // specifieke customer ophalen
            var (customer, notFoundError) = await GetCustomerAsync(dto.CustomerId);

            if (notFoundError != null)
            {
                return (null, notFoundError);
            }

            // geselecteerde accommodaties ophalen
            var (accommodations, accommodationsError) = await GetSelectedAccommodationsAsync(dto.AccommodationIds);

            if (accommodationsError != null)
            {
                return (null, accommodationsError);
            }

            // tarieven voor specifieke reserveringstype ophalen
            var (tariffs, tariffsError) = await GetCampingTariffsAsync(1); // accommodatieTypeId 1 = camping

            if (tariffsError != null)
            {
                return (null, tariffsError);
            }

            // campingreservation object aanmaken 
            var campingReservation = CreateCampingReservationObject(dto);

            // Reservation base class method gebruiken om max aantal overnachtingen te valideren
            var validAmountOfNights = campingReservation.ValidateNumberOfNights(dto.StartDate, dto.EndDate);

            if (!validAmountOfNights)
            {
                return (null, "Ongeldig aantal nachten voor reservering");
            }

            // customer koppelen aan de reservering
            campingReservation.Customer = customer;

            // voeg accommodaties toe aan de reservering
            foreach (var accommodation in accommodations)
            {
                campingReservation.AddAccommodation(accommodation); // inheritance (campingReservation gebruikt parent class Reservation methode)
            }

            // bereken totale prijs met de override method van CampingReservation class
            campingReservation.TotalPrice = campingReservation.CalculatePrice(tariffs);


            // resevering toevoegen aan db
            _context.Reservations.Add(campingReservation);
            await _context.SaveChangesAsync();

            // response dto aanmaken en terugsturen naar de controller 
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

            return (responseDto, null);
        }

        // // Yassir - methode voor het verwerken van een hotelreservering
        public async Task<(HotelReservationResponseDTO? dto, string? error)> AddHotelReservationAsync(HotelReservationDTO dto)
        {
            // 1. controleren of accommodaties beschikbaar zijn voor de ingevoerde datums
            var conflict = await ValidateAccommodationAvailabilityAsync(dto.StartDate, dto.EndDate, dto.AccommodationIds);
            if (conflict != null) return (null, conflict);

            // 2. benodigde data ophalen: klant, accommodaties, tarieven
            var (customer, customerErr) = await GetCustomerAsync(dto.CustomerId);
            if (customerErr != null) return (null, customerErr);

            var (accommodations, accErr) = await GetSelectedHotelAccommodationsAsync(dto.AccommodationIds);
            if (accErr != null) return (null, accErr);

            var (tariffs, tariffErr) = await GetHotelTariffsAsync(2); // Type 2 = Hotel
            if (tariffErr != null) return (null, tariffErr);

            // 3. maak het hotelreservering object aan
            var hotelReservation = CreateHotelReservationObject(dto);

            // 4. validaties uitvoeren
            if (!hotelReservation.ValidateNumberOfNights(dto.StartDate, dto.EndDate))
                return (null, "Maximaal voor 4 weken reserveren toegestaan");

            hotelReservation.Customer = customer;

            foreach (var acc in accommodations)
            {
                hotelReservation.AddAccommodation(acc);
            }

            // 5. capaciteit valideren en prijs berekenen
            hotelReservation.ValidateCapacity();
            hotelReservation.TotalPrice = hotelReservation.CalculatePrice(tariffs);

            // 6. resrevering opslaan in de database
            _context.Reservations.Add(hotelReservation);
            await _context.SaveChangesAsync();

            // 7. data mappen naar response DTO en terugsturen
            return (MapToHotelResponseDto(hotelReservation), null);
        }

        // // Yassir - methode voor het aanmaken van het hotel object
        private HotelReservation CreateHotelReservationObject(HotelReservationDTO dto)
        {
            return new HotelReservation(
                dto.CustomerId,
                dto.StartDate,
                dto.EndDate,
                dto.PersonCount
            );
        }

        // // Yassir - methode voor het ophalen van alleen hotel-accommodaties
        private async Task<(List<Accommodation>? accommodations, string? errorMessage)> GetSelectedHotelAccommodationsAsync(List<int> accommodationIds)
        {
            var accommodations = await _context.Accommodations
                .Where(a => accommodationIds.Contains(a.AccommodationId))
                .Where(a => a.AccommodationTypeId == 2) // Type 2 is voor hotelkamers
                .ToListAsync();

            if (!accommodations.Any()) return (null, "Geen hotelkamers gevonden");
            return (accommodations, null);
        }

        // // Yassir - methode voor het mappen naar de response DTO
        private HotelReservationResponseDTO MapToHotelResponseDto(HotelReservation res)
        {
            return new HotelReservationResponseDTO
            {
                FirstName = res.Customer.FirstName,
                Infix = res.Customer.Infix,
                LastName = res.Customer.LastName,
                StartDate = res.StartDate,
                EndDate = res.EndDate,
                PersonCount = res.PersonCount,
                TotalPrice = res.TotalPrice,
                AccommodationPlaceNumbers = res.Accommodations.Select(a => a.PlaceNumber).ToList()
            };
        }

        private CampingReservation CreateCampingReservationObject(CampingReservationDTO dto)
        {
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
            return campingReservation;
        }

        private async Task<string?> ValidateAccommodationAvailabilityAsync(DateOnly startDate, DateOnly endDate, List<int> accommodationIds)
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
                    return $"Accommodatie {accommodation.PlaceNumber} is niet beschikbaar voor de ingevoerde datum";
                }
            }
            return null; // geen conflicten = geen response nodig
        }

        // return type namen worden voor tuples toegevoegd om duidelijk te maken wat de functie van beide types zijn (functioneel niet nodig)
        private async Task<(Customer? customer, string? errorMessage)> GetCustomerAsync(int customerId)
        {
            // specifieke customer ophalen 
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return (null, "Klant niet gevonden");
            }
            return (customer, null);
        }

        private async Task<(List<Accommodation>? accommodations, string? errorMessage)> GetSelectedAccommodationsAsync(List<int> accommodationIds)
        {
            // alle geselecteerde accommodaties ophalen
            var accommodations = await _context.Accommodations
                .Where(a => accommodationIds.Contains(a.AccommodationId))
                .Where(a => a.AccommodationTypeId == 1)
                .ToListAsync();

            if (!accommodations.Any())
            {
                return (null, "Geen accommodaties gevonden");
            }
            return (accommodations, null);
        }

        private async Task<(List<Tariff>? tariffs, string? errorMessage)> GetCampingTariffsAsync(int accommodationTypeId)
        {
            // haal tarieven op voor camping 
            var tariffs = await _context.Tariffs
                .Where(t => t.AccommodationTypeId == 1)
                .ToListAsync();

            if (!tariffs.Any())
            {
                return (null, "Geen tarieven gevonden");
            }
            return (tariffs, null);
        }

        // // Yassir - Hulpmethode voor het ophalen van de hotel-tarieven
        public async Task<(List<Tariff> tariffs, string errorMessage)> GetHotelTariffsAsync(int accomodationTypeId)
        {
            // Haal alle tarieven op die horen bij het hotel (Type 2)
            var tariffs = await _context.Tariffs
                .Where(t => t.AccommodationTypeId == 2)
                .ToListAsync();

            if (!tariffs.Any())
            {
                return (null, "Geen tarieven gevonden");
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
                return (null, "reservering niet gevonden");
            }
            return (reservation, null);
        }

        // voor DELETE /api/reservation/{id}
        public async Task<(bool success, string? errorMessage)> DeleteReservationAsync(int id)
        {
            // specifieke reservering ophalen uit db
            var reservation = await _context.Reservations.FindAsync(id);

            // check of reservering is gevonden
            if (reservation == null)
            {

                return (false, "ReserveringsId niet gevonden");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return (true, null); // succesvol deleted = true, errorMessage = null
        }
    }
}