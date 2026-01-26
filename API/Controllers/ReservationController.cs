using API.DbServices;
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
        [Authorize(Roles = "Customer")] // endpoint is alleen voor klanten bedoelt, maar admin mag nog steeds access
        [HttpPost("camping")]
        public async Task<ActionResult<CampingReservationResponseDTO>> PostCampingReservation(CampingReservationDTO dto)
        {
            try
            {   // probeer de nieuwe reservering toe te voegen via de service (geeft nullable tuple terug voor reservering object of errormessage)
                var (reservationResponseDto, errorMessage) = await _dbService.AddCampingReservationAsync(dto);

                if (errorMessage != null)
                {
                    return Conflict(errorMessage);
                }
                return Ok(reservationResponseDto);
            }

            catch (ArgumentException ex) // vangt de validations van constructor op
            {
                return Conflict(ex.Message);
            }
            catch (DbUpdateException) // database errors
            {
                return StatusCode(500, "Er is een fout opgetreden bij het opslaan van de reservering");
            }
        }


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

        // Yassir
        // POST: api/reservation/hotel - nieuwe hotelreservering aanmaken
        [Authorize(Roles = "Admin,Customer")]
        [HttpPost("hotel")]
        public async Task<ActionResult<HotelReservationResponseDTO>> PostHotelReservation(HotelReservationDTO dto)
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

            // tarieven voor hotel ophalen 
            var (tariffs, tariffsNotFoundMessage) = await _dbService.GetHotelTariffsAsync(2); // 2 = AccommodationTypeId 2 (Hotel tarieven)

            if (tariffsNotFoundMessage != null)
            {
                return NotFound(tariffsNotFoundMessage);
            }

            try
            {
                // maak nieuwe reservering aan met constructor
                // var of HotelReservation moet gebruikt worden omdat hotel-specifieke properties beschikbaar moeten zijn voor de responseDto
                var hotelReservation = new HotelReservation(
                    dto.CustomerId,
                    dto.StartDate,
                    dto.EndDate,
                    dto.PersonCount // Specifiek voor hotel
                );

                // class method van abstract Reservation wordt gebruikt om aantal overnachtingen te valideren 
                bool valid = hotelReservation.ValidateNumberOfNights(dto.StartDate, dto.EndDate);

                if (!valid)
                {
                    return Conflict("Maximaal voor 4 weken reserveren toegestaan");
                }

                // specifieke customer linken aan reservation
                hotelReservation.Customer = customer;

                // voeg accommodaties toe aan de reservering
                foreach (var accommodation in accommodations)
                {
                    hotelReservation.AddAccommodation(accommodation); // inheritance (hotelReservation gebruikt parent class Reservation methode)
                }

                // specifieke capaciteit validatie voor hotelreservering
                hotelReservation.ValidateCapacity(); 

                // bereken totale prijs met de override method van HotelReservation class
                hotelReservation.TotalPrice = hotelReservation.CalculatePrice(tariffs);

                var reservationCreated = await _dbService.AddReservationAsync(hotelReservation);

                var responseDto = new HotelReservationResponseDTO
                {
                    FirstName = hotelReservation.Customer.FirstName,
                    Infix = hotelReservation.Customer.Infix,
                    LastName = hotelReservation.Customer.LastName,
                    StartDate = hotelReservation.StartDate,
                    EndDate = hotelReservation.EndDate,
                    PersonCount = hotelReservation.PersonCount, // Specifiek voor hotel
                    TotalPrice = hotelReservation.TotalPrice,

                    // linq gebruiken om PlaceNumbers van gekozen accommodaties op te halen
                    AccommodationPlaceNumbers = hotelReservation.Accommodations
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

        // DELETE: api/Reservation/{id} - specifieke reservering verwijderen
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var (success, errorMessage) = await _dbService.DeleteReservationAsync(id);

            if (!success)
            {
                return NotFound(errorMessage);
            }
            return Ok($"Reservering met id {id} verwijderd.");

        }
    }
}