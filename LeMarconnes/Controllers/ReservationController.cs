using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;


namespace LeMarconnes.Controllers
{
    public class ReservationController : Controller
    {

        private readonly HttpClient _httpClient;

        public ReservationController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7290");
        }

        // reservering stap 1: datums kiezen
        [HttpGet]
        public IActionResult CreateReservation1()
        {
            var viewModel = new CreateReservation1ViewModel
            {
                // default datum waarden op huidige datum zetten
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation1(DateTime startDate, DateTime endDate)
        {
            // service validaties voor datums
            if (!CreateReservationService.ValidDateInput(startDate, endDate))
            {
                TempData["Error"] = "Voer een geldige start- en einddatum in";

                return View(new CreateReservation1ViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
            }

            if (!CreateReservationService.ValidateReservationDates(startDate, endDate))
            {
                TempData["Error"] = "Einddatum moet later dan startdatum zijn";
                return View(new CreateReservation1ViewModel
                {
                    StartDate = startDate,
                    EndDate = endDate
                });
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
                TempData["Error"] = "Sessie verlopen. Selecteer datums opnieuw";
                return RedirectToAction("CreateReservation1");
            }

            // datums terug converten van string naar DateTime
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // beschikbare accommodaties ophalen met available-for-dates endpoint
            List<Accommodation> availableAccommodations = new List<Accommodation>();
            var response = await _httpClient.GetAsync(
                $"/api/Accommodation/available-for-dates?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            if (response.IsSuccessStatusCode)
            {
                availableAccommodations = await response.Content.ReadFromJsonAsync<List<Accommodation>>();
            }

            // checken of de teruggegeven lijst leeg is of niet
            if (!availableAccommodations.Any())
            {
                TempData["Error"] = "Geen accommodaties beschikbaar voor deze periode";
                return RedirectToAction("CreateReservation1");
            }

            // viewmodel voor deze view aanmaken en vullen met de ontvangen datums (session strings) en accommodation lijst (api response)
            var viewModel = new CreateReservation2ViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                AvailableAccommodations = availableAccommodations
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation2(List<int> accommodationIds)
        {
            // datums ophalen uit session voor validatie
            var startDateStr = HttpContext.Session.GetString("ReservationStartDate");
            var endDateStr = HttpContext.Session.GetString("ReservationEndDate");

            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr))
            {
                TempData["Error"] = "Sessie verlopen. Selecteer opnieuw de datums.";
                return RedirectToAction("CreateReservation1");
            }

            // service validatie voor accommodaties

            if (!CreateReservationService.ValidateAccommodationCount(accommodationIds))
            {
                TempData["Error"] = "Minimaal 1, Max 2 campingplekken per reservering";
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
            var accommodationIdsStr = HttpContext.Session.GetString("ReservationAccommodationIds");

            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr) || string.IsNullOrEmpty(accommodationIdsStr))
            {
                TempData["Error"] = "Sessie verlopen";
                return RedirectToAction("CreateReservation1");
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

            // checken of sessie nog actief is
            if (string.IsNullOrEmpty(startDateStr) || string.IsNullOrEmpty(endDateStr) || string.IsNullOrEmpty(accommodationIdsStr))
            {
                TempData["Error"] = "Sessie verlopen";
                return RedirectToAction("CreateReservation1");
            }

            if (!customerId.HasValue)
                return RedirectToAction("Login");

            // opgeslagen datum strings weer terug converten naar DateTime
            DateTime startDate = DateTime.Parse(startDateStr);
            DateTime endDate = DateTime.Parse(endDateStr);

            // string van accommodationIds terug converten naar List<int>
            var accommodationIds = System.Text.Json.JsonSerializer.Deserialize<List<int>>(accommodationIdsStr);

            // service validaties
            if (!CreateReservationService.ValidateAdultCounts(adultsCount))
            {
                TempData["Error"] = "Minimaal 1 volwassene nodig (max 10)";
                return View();
            }

            if (!CreateReservationService.ValidateChildrenCount(children0_7Count, children7_12Count))
            {
                TempData["Error"] = "Minimaal 0, maximaal 5 kinderen per leeftijdscategorie";
                return View();
            }

            if (!CreateReservationService.ValidateDogsCount(dogsCount))
            {
                TempData["Error"] = "Minimaal 0, maximaal 3 honden";
                return View();
            }

            int numberOfNights = (endDate - startDate).Days;

            if (hasElectricity && !CreateReservationService.ValidateElectricityDays(electricityDays, numberOfNights))
            {
                TempData["Error"] = $"Minimaal 1, maximaal {numberOfNights} dagen voor elektriciteitsgebruik {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
                return View();
            }

            if (!hasElectricity)
            {
                electricityDays = null;
            }

            // DTO aanmaken
            var reservationDto = CreateReservationService.CreateNewReservationDTO(customerId.Value, accommodationIds, startDate, endDate,
                                                                                  adultsCount, children0_7Count, children7_12Count, dogsCount, hasElectricity, electricityDays);

            // api call om reservation te posten
            var reservationResponse = await _httpClient.PostAsJsonAsync("/api/Reservation", reservationDto);

            if (!reservationResponse.IsSuccessStatusCode)
            {
                var errorText = await reservationResponse.Content.ReadAsStringAsync();
                TempData["Error"] = $"Fout: {errorText}";
                return View();
            }

            var reservationResult = await reservationResponse.Content.ReadFromJsonAsync<ReservationResponseDTO>();

            // session clearen
            HttpContext.Session.Remove("ReservationStartDate");
            HttpContext.Session.Remove("ReservationEndDate");
            HttpContext.Session.Remove("ReservationAccommodationIds");

            // ViewModel voor de volgende pagina aanmaken en vullen met reserveringsdata
            var viewModel = new ReservationConfirmationViewModel
            {
                FirstName = reservationResult.FirstName,
                Infix = reservationResult.Infix,
                LastName = reservationResult.LastName,
                StartDate = reservationResult.StartDate,
                EndDate = reservationResult.EndDate,
                AdultsCount = reservationResult.AdultsCount,
                Children0_7Count = reservationResult.Children0_7Count,
                Children7_12Count = reservationResult.Children7_12Count,
                DogsCount = reservationResult.DogsCount,
                HasElectricity = reservationResult.HasElectricity,
                ElectricityDays = reservationResult.ElectricityDays,
                TotalPrice = reservationResult.TotalPrice,
                AccommodationPlaceNumbers = reservationResult.AccommodationPlaceNumbers
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
