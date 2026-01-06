using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;


namespace LeMarconnes.Controllers
{
    public class ReservationController : Controller
    {

        private readonly ReservationService _reservationService;

        public ReservationController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        // reservering stap 1: datums kiezen
        [HttpGet]
        public IActionResult CreateReservation1()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation1(DateTime startDate, DateTime endDate)
        {
            // service validaties voor datums
            if (!ReservationValidation.ValidDateInput(startDate, endDate))
            {
                TempData["Error"] = "Voer een geldige start- en einddatum in";

                return View();
            }

            if (!ReservationValidation.ValidateReservationDates(startDate, endDate))
            {
                TempData["Error"] = "Einddatum moet later dan startdatum zijn";
                return View();
            }

            // datums opslaan in session voor volgende stappen
            HttpContext.Session.SetString("ReservationStartDate", startDate.ToString("yyyy-MM-dd"));
            HttpContext.Session.SetString("ReservationEndDate", endDate.ToString("yyyy-MM-dd"));

            // doorsturen naar stap 2
            return RedirectToAction("CreateReservation2");

        }



        // reserveren stap 2: accommodaties kiezen
        [HttpGet]
        public async Task<IActionResult> CreateReservation2()
        {
            // datums ophalen uit session
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");

            // checken of sessie verlopen is
            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("Login", "Login");
            }

            // datums terug converten van string naar DateTime
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            var (accommodations, errorMessage) = await _reservationService.GetAvailableAccommodationsAsync(startDate, endDate);

            if (accommodations == null)
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction("CreateReservation1");
            }

            // viewmodel voor deze view aanmaken en vullen met de ontvangen datums (session strings) en accommodation lijst (api response)
            // deze data wordt gebruikt om bijv de ingevoerde datums weer terug te zetten wanneer de view herlaadt door een invoer error
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
            // datums ophalen uit session voor validatie
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");

            if (!ReservationValidation.ValidateAccommodationCount(accommodationIds))
            {
                TempData["Error"] = "Minimaal 1 en maximaal 2 accommodaties toegestaan";
                return RedirectToAction("CreateReservation2");
            }

            // checken of sessie nog actief is (data van vorige view nog beschikbaar)
            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("Login", "Login");
            }

            // accommodation Ids opslaan in session (als json string)
            HttpContext.Session.SetString("ReservationAccommodationIds",
            System.Text.Json.JsonSerializer.Serialize(accommodationIds));
                        
            return RedirectToAction("CreateReservation3");
        }



        // reserveren stap 3: personen en extra's
        [HttpGet]
        public IActionResult CreateReservation3()
        {
            // datum en accommodaties moeten opgehaald worden om sessie validatie uit te voeren
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");
            var accommodationIdsStr = HttpContext.Session.GetString("ReservationAccommodationIds");

            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("Login", "Login");
            }

            // strings weer terug converten naar date time omdat datum berekening uitgevoerd moet worden
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            int numberOfNights = (endDate - startDate).Days;

            // aantal overnachtingen opslaan in viewbag zodat het in de view gebruikt kan worden
            ViewBag.NumberOfNights = numberOfNights;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation3(int adultsCount, int children0_7Count, int children7_12Count,
                                                            int dogsCount, bool hasElectricity, int? electricityDays = null)
        {
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");
            var accommodationIdsStr = HttpContext.Session.GetString("ReservationAccommodationIds");
            var customerId = HttpContext.Session.GetInt32("CustomerId");

            // checken of sessie nog actief is (customerId in session is nodig)
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login", "Login");
            }

            // opgeslagen datum strings weer terug converten naar DateTime
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // string van accommodationIds terug converten naar List<int>
            var accommodationIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(accommodationIdsStr);

            if (!hasElectricity)
            {
                electricityDays = null;
            }

            // DTO aanmaken
            var reservationDto = new CreateReservationDTO
            {
                CustomerId = customerId.Value,
                AccommodationIds = accommodationIds,
                StartDate = startDate,
                EndDate = endDate,
                AdultsCount = adultsCount,
                Children0_7Count = children0_7Count,
                Children7_12Count = children7_12Count,
                DogsCount = dogsCount,
                HasElectricity = hasElectricity,
                ElectricityDays = electricityDays
            };


            // api call om reservation te posten
            var (reservation, errorMessage) = await _reservationService.CreateReservationAsync(reservationDto);

            if (reservation == null)
            {
                TempData["Error"] = errorMessage;
                return View();
            }

            // session clearen
            HttpContext.Session.Remove("ReservationStartDate");
            HttpContext.Session.Remove("ReservationEndDate");
            HttpContext.Session.Remove("ReservationAccommodationIds");

            // ViewModel voor de volgende pagina aanmaken en vullen met reserveringsdata
            var viewModel = new ReservationConfirmationViewModel
            {
                FirstName = reservation.FirstName,
                Infix = reservation.Infix,
                LastName = reservation.LastName,
                StartDate = reservation.StartDate,
                EndDate = reservation.EndDate,
                AdultsCount = reservation.AdultsCount,
                Children0_7Count = reservation.Children0_7Count,
                Children7_12Count = reservation.Children7_12Count,
                DogsCount = reservation.DogsCount,
                HasElectricity = reservation.HasElectricity,
                ElectricityDays = reservation.ElectricityDays,
                TotalPrice = reservation.TotalPrice,
                AccommodationPlaceNumbers = reservation.AccommodationPlaceNumbers
            };

            return View("ReservationConfirmation", viewModel);
        }

        [HttpGet]
        public IActionResult ReservationConfirmation()
        {

            return View("ReservationConfirmation");
        }

        
    }
}
