// Yassir - Definitieve Fix
using System.Text.Json;
using ClassLibrary.DTOs;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MVC.HttpServices;

namespace MVC.Controllers
{
    public class HotelReservationController : Controller
    {
        private readonly ReservationHttpService _httpService;
        public HotelReservationController(ReservationHttpService httpService)
        {
            _httpService = httpService;
        }

        [HttpGet]
        public IActionResult CreateReservation1()
        {
            var existingToken = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(existingToken))
            {
                return RedirectToAction("Homepage", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateReservation1(DateOnly startDate, DateOnly endDate)
        {
            // Sla op met simpele namen voor consistentie
            HttpContext.Session.SetString("startDate", startDate.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("endDate", endDate.ToString("yyyy-MM-dd"));

            return RedirectToAction("CreateReservation2");
        }

        [HttpGet]
        public async Task<IActionResult> CreateReservation2()
        {
            var startStr = HttpContext.Session.GetString("startDate");
            var endStr = HttpContext.Session.GetString("endDate");

            if (string.IsNullOrEmpty(startStr) || string.IsNullOrEmpty(endStr))
            {
                return RedirectToAction("CreateReservation1");
            }

            DateOnly startDate = DateOnly.Parse(startStr);
            DateOnly endDate = DateOnly.Parse(endStr);

            var (accommodations, errorMessage) = await _httpService.GetAvailableAccommodationsAsync(startDate, endDate, 2);

            if (accommodations == null)
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction("CreateReservation1");
            }

            return View(new CreateReservation2ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                AvailableAccommodations = accommodations
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation2(List<int> accommodationIds)
        {
            if (accommodationIds == null || accommodationIds.Count == 0)
            {
                TempData["Error"] = "Selecteer minimaal 1 hotelkamer";
                return RedirectToAction("CreateReservation2");
            }

            HttpContext.Session.SetString("ReservationAccommodationIds", JsonSerializer.Serialize(accommodationIds));
            return RedirectToAction("CreateReservation3");
        }

        [HttpGet]
        public IActionResult CreateReservation3()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("startDate")))
            {
                return RedirectToAction("CreateReservation1");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation3(int personCount)
        {
            // Regel 122 FIX: Gebruik de juiste namen die in Stap 1 zijn gezet
            var startStr = HttpContext.Session.GetString("startDate");
            var endStr = HttpContext.Session.GetString("endDate");
            var accIdsJson = HttpContext.Session.GetString("ReservationAccommodationIds");
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            var token = HttpContext.Session.GetString("JwtToken");

            // Veiligheidscheck: als er iets mist, crash niet maar ga terug
            if (startStr == null || endStr == null || accIdsJson == null || customerIdStr == null)
            {
                return RedirectToAction("CreateReservation1");
            }

            var hotelDto = new HotelReservationDTO
            {
                CustomerId = int.Parse(customerIdStr),
                StartDate = DateOnly.Parse(startStr),
                EndDate = DateOnly.Parse(endStr),
                AccommodationIds = JsonSerializer.Deserialize<List<int>>(accIdsJson)!,
                PersonCount = personCount
            };

            var (reservation, errorMessage) = await _httpService.CreateHotelReservationAsync(hotelDto, token);

            if (reservation == null)
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction("CreateReservation3");
            }

            var viewModel = new ReservationConfirmationViewModel
            {
                FirstName = reservation.FirstName,
                Infix = reservation.Infix,
                LastName = reservation.LastName,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                PersonCount = reservation.PersonCount,
                TotalPrice = reservation.TotalPrice,
                AccommodationPlaceNumbers = reservation.AccommodationPlaceNumbers
            };

            // Zorg dat deze view in Views/HotelReservation/ staat
            return View("ReservationConfirmation", viewModel);
        }
    }
}