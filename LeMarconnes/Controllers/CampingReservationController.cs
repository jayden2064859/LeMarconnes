using ClassLibrary.DTOs;
using MVC.HttpServices;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MVC.Controllers
{
    public class CampingReservationController : Controller
    {

        private readonly ReservationHttpService _httpService;

        public CampingReservationController(ReservationHttpService httpService)
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
        public async Task<IActionResult> CreateReservation1(DateOnly startDate, DateOnly endDate)
        {
            // datums opslaan in session voor volgende stappen
            HttpContext.Session.SetString("ReservationStartDate", startDate.ToString());
            HttpContext.Session.SetString("ReservationEndDate", endDate.ToString());

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
            DateOnly startDate = DateOnly.Parse(startDateStr);
            DateOnly endDate = DateOnly.Parse(endDateStr);

            var (accommodations, errorMessage) = await _httpService.GetAvailableAccommodationsAsync
                                                                            (startDate, endDate, Accommodation.AccommodationType.Camping);

            if (accommodations == null)
            {
                TempData["Error"] = errorMessage;
                return RedirectToAction("CreateReservation1");
            }

            // viewmodel voor deze view aanmaken en vullen met de ontvangen datums (session strings) en accommodation lijst (api response)
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
            var customerId = HttpContext.Session.GetString("CustomerId");

            // check of sessie verlopen is
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login", "Login"); 
            }

            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("CreateReservation1");
            }

            if (accommodationIds.Count < 1 || accommodationIds.Count > 2 || accommodationIds == null)
            {
                 TempData["Error"] = "Minimaal 1, Maximaal 2 accommodaties toegestaan";

                return RedirectToAction("CreateReservation2");
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


            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                return RedirectToAction("Login", "Login");
            }

            // strings weer terug converten naar date time omdat datum berekening uitgevoerd moet worden
            DateOnly startDate = DateOnly.Parse(startDateStr);
            DateOnly endDate = DateOnly.Parse(endDateStr);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation3(int adultsCount, int children0_7Count, int children7_12Count,
                                                            int dogsCount, bool hasElectricity, int? electricityDays)
        {
            // session data voor reservering ophalen
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");
            var accommodationIdsStr = HttpContext.Session.GetString("ReservationAccommodationIds");
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
                     
            if (string.IsNullOrEmpty(customerIdStr))
            {
                return RedirectToAction("Login", "Login");
            }            

            // opgeslagen datum strings weer terug converten naar DateTime
            DateOnly startDate = DateOnly.Parse(startDateStr);
            DateOnly endDate = DateOnly.Parse(endDateStr);

            // json array van accommodationIds terug converten naar List<int>
            var accommodationIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(accommodationIdsStr);

            // customerId string terug naar int 
            int customerId = int.Parse(customerIdStr);

            // dto aanmaken 
            var dto = new CampingReservationDTO
            {
                CustomerId = customerId,
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

            // token ophalen uit session 
            var token = HttpContext.Session.GetString("JwtToken");

            // service geeft reservation object terug als de endpoint call succesvol is, anders een error message
            var (reservation, errorMessage) = await _httpService.CreateCampingReservationAsync(dto, token);

            if (reservation == null)
            {
                TempData["Error"] = errorMessage;
                return View();
            }

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
