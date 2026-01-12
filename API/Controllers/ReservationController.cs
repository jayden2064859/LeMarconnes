using API.Services;
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
        // dependency injection van de camping service
        private readonly CampingReservationService _service;

        public ReservationController(LeMarconnesDbContext context, CampingReservationService service)
        {
            _service = service;
        }

        // POST: api/reservation/camping
        [HttpPost("camping")]
        public async Task<ActionResult<CampingReservationResponseDTO>> PostCampingReservation(CampingReservationDTO dto)
        {
            // database communicatie is encapsulated in API services
            // check in de db of er al een reservering bestaat die overlapped in datum
            var accommodationConflictError = await _service.ValidateAccommodationAvailability(dto.StartDate, dto.EndDate, dto.AccommodationIds);

            if (accommodationConflictError != null)
            {
                return Conflict(accommodationConflictError);
            }

            // specifieke customer ophalen 
            var (customer, customerNotFoundError) = await _service.GetCustomer(dto.CustomerId);

            if (customerNotFoundError != null)
            {
                return NotFound(customerNotFoundError);
            }

            // geselecteerde accommodaties ophalen
            var (accommodations, accommodationNotFoundError) = await _service.GetSelectedAccommodations(dto.AccommodationIds);

            if (accommodationNotFoundError != null)
            {
                return NotFound(accommodationNotFoundError);
            }

            // tarieven voor camping ophalen 
            var (tariffs, tariffsNotFoundError) = await _service.GetCampingTariffs(Accommodation.AccommodationType.Camping);

            if (tariffsNotFoundError != null)
            {
                return NotFound(tariffsNotFoundError);
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

                // reservation koppelen aan customer
                customer.AddReservation(campingReservation); // inheritance (campingReservation gebruikt parent class Reservation methode)

                // voeg accommodaties toe aan de reservering
                foreach (var accommodation in accommodations)
                {
                    campingReservation.AddAccommodation(accommodation); // inheritance (campingReservation gebruikt parent class Reservation methode)
                }

                // bereken totale prijs met de override method van CampingReservation class
                campingReservation.TotalPrice = campingReservation.CalculatePrice(tariffs);


                var reservationCreated = await _service.AddCampingReservation(campingReservation);

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


        // GET: api/reservation
        [HttpGet]
        public async Task<ActionResult<List<Reservation>>> GetReservations()
        {
            var allReservations = await _service.GetAllReservations();
            return Ok(allReservations);
        }


        // GET: api/reservation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var (reservation, errorMessage) = await _service.GetReservationById(id);

            if (errorMessage != null)
            {
                return NotFound(errorMessage);
            }
            return Ok(reservation);

        }

        // PUT: api/Reservation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, Reservation reservation)
        {
            var (succesfullyUpdated, errorMessage) = await _service.UpdateReservation(id, reservation);

            if (!succesfullyUpdated)
            {
                return NotFound(errorMessage);
            }

            return Ok();
        }

        // DELETE: api/Reservation/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var (success, errorMessage) = await _service.DeleteReservation(id);

            if (!success)
            {
                return NotFound(errorMessage);
            }
            return Ok();




        }
    }
}