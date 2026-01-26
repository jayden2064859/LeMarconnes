// Yassir
using System.Text.Json;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
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
        // reservering stap 1: datums kiezen
        [HttpGet]
        public IActionResult CreateReservation1()
        {
            var existingToken = HttpContext.Session.GetString("JwtToken"); // bestaat alleen wanneer gebruiker correct geauthenticeerd is
            if (string.IsNullOrEmpty(existingToken)) // alleen een authencated user met een jwt token mag reserveringen plaatsen
            {
                return RedirectToAction("Home", "Homepage");
            }
            return View();
        }

        [HttpPost]
        public IActionResult CreateReservation1(DateOnly startDate, DateOnly endDate)
        {
            // Sla datums op in de sessie
            HttpContext.Session.SetString("ReservationStartDate", startDate.ToString());
            HttpContext.Session.SetString("ReservationEndDate", endDate.ToString());

            return RedirectToAction("CreateReservation2");
        }

        //Hotelamer kiezen
        [HttpGet]
        public async Task<IActionResult> CreateReservation2()
        {
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");

            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("Login", "Login");
            }

            // datums terug converteren van string naar DateOnly
            DateOnly startDate = DateOnly.Parse(startDateStr);
            DateOnly endDate = DateOnly.Parse(endDateStr);

            var (accommodations, errorMessage) = await _httpService.GetAvailableAccommodationsAsync(startDate, endDate, 2);

            if (accommodations == null)
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction("CreateReservation1");
            }

            // invullen view
            var viewModel = new CreateReservation2ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                AvailableAccommodations = accommodations
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation2(List<int> accommodationIds)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");

            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Login");
            }

            // Controleer of er minimaal 1 kamer is gekozen
            if (accommodationIds == null || accommodationIds.Count == 0)
            {
                TempData["Error"] = "Selecteer minimaal 1 hotelkamer";
                return RedirectToAction("CreateReservation2");
            }

            // gekozen IDs opslaan in session als json string
            HttpContext.Session.SetString("ReservationAccommodationIds", JsonSerializer.Serialize(accommodationIds));

            return RedirectToAction("CreateReservation3");
        }

        // Alleen nog persoonsaantal kiezen
        [HttpGet]
        public IActionResult CreateReservation3()
        {
            // datum en accommodaties moeten opgehaald worden om sessie validatie uit te voeren
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");

            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("Login", "Login");
            }

            // strings weer terug converteren naar DateOnly omdat datum berekening uitgevoerd moet worden
            DateOnly startDate = DateOnly.Parse(startDateStr);
            DateOnly endDate = DateOnly.Parse(endDateStr);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation3(int personCount)
        {
            // Haal alles op uit de sessie om de DTO te vullen
            var start = DateOnly.Parse(HttpContext.Session.GetString("ResStart")!);
            var end = DateOnly.Parse(HttpContext.Session.GetString("ResEnd")!);
            var accIdsJson = HttpContext.Session.GetString("ResAccIds");
            var accIds = JsonSerializer.Deserialize<List<int>>(accIdsJson!);
            var customerId = int.Parse(HttpContext.Session.GetString("CustomerId")!);

            // dto aanmaken
            var hotelDto = new HotelReservationDTO
            {
                CustomerId = customerId,
                StartDate = start,
                EndDate = end,
                AccommodationIds = accIds,
                PersonCount = personCount
            };

            var token = HttpContext.Session.GetString("JwtToken");

            var (reservation, errorMessage) = await _httpService.CreateHotelReservationAsync(hotelDto, token);

            var viewModel = new HotelReservationConfirmationViewModel
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
            return View("HotelReservationConfirmation", viewModel);
        }
    }
}
