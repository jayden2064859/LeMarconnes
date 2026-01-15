using API.DbServices;
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
    public class ReservationController : ControllerBase
    {
        // dependency injection van de camping service
        private readonly ReservationDbService _dbService;

        public ReservationController(ReservationDbService dbService)
        {
            _dbService = dbService;
        }

        // POST: api/reservation/camping - nieuwe reservering aanmaken
        [Authorize(Roles = "Admin,Customer")] // endpoint is alleen voor klanten bedoelt, maar admin mag nog steeds access
        [HttpPost("camping")]
        public async Task<ActionResult<CampingReservationResponseDTO>> PostCampingReservation(CampingReservationDTO dto)
        {
            // database communicatie is encapsulated in API services
            // check in de db of er al een reservering bestaat die overlapped in datum
            var accommodationConflictMessage = await _dbService.ValidateAccommodationAvailabilityAsync(
            dto.StartDate,
            dto.EndDate,
            dto.AccommodationIds);

            if (accommodationConflictMessage != null)
            {
                return Conflict(accommodationConflictMessage);
            }

            // specifieke customer ophalen 
            var (customer, customerNotFoundMessage) = await _dbService.GetCustomerAsync(dto.CustomerId);

            if (customerNotFoundMessage != null)
            {
                return NotFound(customerNotFoundMessage);
            }

            // geselecteerde accommodaties ophalen
            var (accommodations, accommodationNotFoundMessage) = await _dbService.GetSelectedAccommodationsAsync(dto.AccommodationIds);

            if (accommodationNotFoundMessage != null)
            {
                return NotFound(accommodationNotFoundMessage);
            }

            // tarieven voor camping ophalen 
            var (tariffs, tariffsNotFoundMessage) = await _dbService.GetCampingTariffsAsync(Accommodation.AccommodationType.Camping);

            if (tariffsNotFoundMessage != null)
            {
                return NotFound(tariffsNotFoundMessage);
            }

            try
            {
                // maak nieuwe reservering aan met constructor
                // var of CampingReservation moet gebruikt worden omdat camping-specifieke properties beschikbaar moeten zijn voor de responseDto
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

                // voeg accommodaties toe aan de reservering
                foreach (var accommodation in accommodations)
                {
                    campingReservation.AddAccommodation(accommodation); // inheritance (campingReservation gebruikt parent class Reservation methode)
                }

                // bereken totale prijs met de override method van CampingReservation class
                campingReservation.TotalPrice = campingReservation.CalculatePrice(tariffs);


                var reservationCreated = await _dbService.AddCampingReservationAsync(campingReservation);

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
            catch (DbUpdateException ex) // vangt database conflicten op
            {

                return Conflict(ex.Message);
            }
        }

        // GET: api/reservation - alle reserveringen ophalen
        [Authorize(Roles = "Admin")] 
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetAllReservations()
        {
            var allReservations = await _dbService.GetAllReservationsAsync();
            return Ok(allReservations);
        }


        // GET: api/reservation/{id} - specifieke reservering ophalen
        [Authorize(Roles = "Admin")] 
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservationById(int id)
        {
            var (reservation, errorMessage) = await _dbService.GetReservationByIdAsync(id);

            if (errorMessage != null)
            {
                return NotFound(errorMessage);
            }
            return Ok(reservation);

        }

        // PUT: api/Reservation/{id} - specifieke reservering bewerken
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            var (succesfullyUpdated, errorMessage) = await _dbService.UpdateReservationAsync(id, reservation);

            if (!succesfullyUpdated)
            {
                return NotFound(errorMessage);
            }

            return Ok();
        }

        // DELETE: api/Reservation/5 - specifieke reservering verwijderen
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var (success, errorMessage) = await _dbService.DeleteReservationAsync(id);

            if (!success)
            {
                return NotFound(errorMessage);
            }
            return Ok();




        }
    }
}