using API.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DbServices
{
    public class AccommodationDbService
    {
        private readonly LeMarconnesDbContext _context;
        public AccommodationDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        private Accommodation CreateAccommodationObject(PostAccommodationDTO dto)
        {
            var accommodation = new Accommodation
            {
                PlaceNumber = dto.PlaceNumber,
                Capacity = dto.Capacity,
                AccommodationTypeId = dto.AccommodationTypeId
            };
            return accommodation;
        }

        // voor POST /api/accommodation
        public async Task<(Accommodation? accommodation, string? error)> PostAccommodationAsync(PostAccommodationDTO dto)
        {
            // accommodatie object aanmaken
            var newAccommodation = CreateAccommodationObject(dto);

            // accommodatie toevoegen aan database
            _context.Accommodations.Add(newAccommodation);
            await _context.SaveChangesAsync();

            // aangemaakte accommodatie ophalen 
            var createdAccommodation = await _context.Accommodations
                .Include(a => a.AccommodationType)
                .FirstOrDefaultAsync(a => a.AccommodationId == newAccommodation.AccommodationId);

            if (createdAccommodation == null)
            {
                return (null, "Accommodatie niet gevonden na aanmaak");
            }
            return (createdAccommodation, null);

        }

        // voor GET /api/accommodation (alle)
        public async Task<List<GetAllAccommodationsDTO>> GetAllAccommodationsAsync()
        {
            var allAccommodations = await _context.Accommodations
                .Select(a => new GetAllAccommodationsDTO
                {
                    PlaceNumber = a.PlaceNumber,
                    Capacity = a.Capacity,
                    AccommodationTypeName = a.AccommodationType.TypeName,
                }).ToListAsync();
            return allAccommodations;
        }

        // voor GET /api/accommodation/{accommodationId}
        public async Task<(Accommodation? accommodation, string? error)> GetAccommodationAsync(int accommodationId)
        {
            var accommodation = await _context.Accommodations
                .FirstOrDefaultAsync(c => c.AccommodationId == accommodationId);

            if (accommodation == null)
            {
                return (null, $"Accommodatie met id {accommodationId} niet gevonden");
            }
            return (accommodation, null);
        }


        // voor DELETE /api/accommodation/{accommodationId}
        public async Task<(bool success, string? notFoundError)> DeleteAccommodationAsync(int accommodationId)
        {
            // accommodatie ophalen
            var accommodation = await _context.Accommodations.FindAsync(accommodationId);

            if (accommodation == null)
            {
                return (false, $"Accommodatie met id {accommodationId} niet gevonden");
            }

            // accommodatie verwijderen
            _context.Accommodations.Remove(accommodation);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        // voor GET /api/accommodation/available-for-dates
        public async Task<List<AvailableForDatesResponseDTO>> GetAvailableAccommodationsAsync(int accommodationTypeId, DateOnly startDate, DateOnly endDate)
        {
            var conflictError = ValidateConflicts(accommodationTypeId, startDate, endDate);

            if (conflictError != null)
            {
                throw new ArgumentException(conflictError); // argument exception wordt hier gebruikt omdat het met business rules te maken heeft, en de constructors gebruiken daar ook argument exceptions voor
            }

            var availableAccommodations = await _context.Accommodations // geef alle accommodaties terug waarvoor geldt:
                .Where(a => a.AccommodationTypeId == accommodationTypeId &&  // accommodatietype (camping of hotel) komt overeen met meegegeven type  
                !_context.Reservations.Any(r => r.Accommodations.Any(ra => ra.AccommodationId == a.AccommodationId) && // implicit junction table wordt hier doorzocht 
                r.StartDate <= endDate && r.EndDate >= startDate)) // niet gekoppeld aan reserveringen die overlappen met de gevraagde datum periode
                .Select(a => new AvailableForDatesResponseDTO // voor elke accommodatie die aan bovenstaande regels voldoet, voeg de gevraagde properties toe aan een Dto
                {
                    AccommodationId = a.AccommodationId,
                    PlaceNumber = a.PlaceNumber,
                    Capacity = a.Capacity,
                    AccommodationTypeId = a.AccommodationTypeId,
                }).ToListAsync();
            return availableAccommodations; // return lijst van dtos (beschikbare accommodaties)
        }

        private string? ValidateConflicts(int accommodationTypeId, DateOnly startDate, DateOnly endDate)
        {
            // check of accommodationTypeId geldig is (1 of 2)
            if (accommodationTypeId != 1 && accommodationTypeId != 2)
            {
                return "Ongeldig type (1 = Camping, 2 = Hotel)";
            }

            // check of einddatum minimaal 1 dag na startdatum is
            if (endDate <= startDate)
            {
                return "Einddatum moet minimaal 1 dag na startdatum zijn";
            }

            // huidige dag bepalen met datetime, en terug converten naar dateonly om te kunnen vergelijken
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);
            if (startDate < today || endDate < today.AddDays(1))
            {
                return "Datums mogen niet in het verleden zijn";
            }
            return null; // geen conflicten
        }
    }
}
