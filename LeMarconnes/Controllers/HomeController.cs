using Azure.Identity;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using ClassLibrary.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Numerics;
using System.Threading.Tasks;

namespace LeMarconnes.Controllers
{
    public class HomeController : Controller
    {

        private readonly HttpClient _httpClient;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7290");
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(string username, string password, string confirmPassword)
        {
            // service gebruiken om te checken of wachtwoorden overeenkomen

            if (!CreateAccountService.DoPasswordsMatch(password, confirmPassword))
            {
                TempData["Error"] = "Wachtwoorden komen niet overeen";
                return View("CreateAccount");
            }

            if (!CreateAccountService.ValidPasswordLength(password))
            {
                TempData["Error"] = "Wachtwoord moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of alle velden zijn ingevuld        
            if (!CreateAccountService.ValidateFields(username, password, confirmPassword))
            {
                TempData["Error"] = "Vul alle velden in";
                return View("CreateAccount");
            }

            // checken of username input geldig is                    
            if (!CreateAccountService.ValidUsernameLength(username))
            {
                TempData["Error"] = "Gebruikersnaam moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of username alleen uit geldige tekens bestaat
            if (!CreateAccountService.ValidUsernameChars(username))
            {
                TempData["Error"] = "Gebruikersnaam kan alleen uit letters en cijfers bestaan";
                return View("CreateAccount");
            }

            var accountResponse = await _httpClient.GetAsync($"/api/Account/exists/{username}");

            if (accountResponse.IsSuccessStatusCode)
            {
                // api geeft 200 OK terug als username bestaat
                TempData["Error"] = "Gebruikersnaam is al in gebruik";
                return View("CreateAccount");
            }

            // username en wachtwoord worden opgeslagen in de session als alle service methods true teruggeven (true=valid input)
            try
            {
                HttpContext.Session.SetString("RegisterUsername", username);
                HttpContext.Session.SetString("RegisterPassword", password);
            }
            catch
            {
                throw new ArgumentException("Er is iets misgegaan tijdens het opslaan van accountgegevens");
            }

            return RedirectToAction("CreateCustomer");
        }


        [HttpGet]
        public async Task<IActionResult> CreateCustomer()
        {

            return View("CreateCustomer");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(string firstName, string lastName, string email,
                                                string phone, string? infix = null)
        {

            // opgeslagen username en password van de vorige pagina uit de session ophalen
            var username = HttpContext.Session.GetString("RegisterUsername");
            var password = HttpContext.Session.GetString("RegisterPassword");

            // checken of username en password succesvol uit de session zijn gehaald     
            if (!CreateCustomerService.AccountInfoReceived(username, password))
            {
                TempData["Error"] = "Sessie verlopen. Probeer opnieuw.";
                return View("CreateAccount");
            }

            // checken of all required parameters geldige invoer hebben (niet null of alleen maar whitespace)         
            if (!CreateCustomerService.RequiredFieldsCheck(firstName, lastName, email, phone))
            {
                TempData["Error"] = "Vul alle verplichte velden in";
                return View("CreateCustomer");
            }

            // checken of email invoer minimaal 1 '@' teken heeft (en maximaal 1)
            if (!CreateCustomerService.ValidEmail(email))
            {
                TempData["Error"] = "Ongeldige email";
                return View("CreateCustomer");
            }

            if (!CreateCustomerService.ValidatePhone(phone))
            {
                TempData["Error"] = "Ongeldig telefoonnummer";
                return View("CreateCustomer");
            }

            // 1. customer dto aanmaken
            // customer moet als eerst aangemaakt worden, zodat het customer object teruggegeven kan worden voordat het account object wordt aangemaakt.
            // het (door de database gegenereerde) customerId wordt uit dit customer object gehaald, zodat deze direct gelinked kan worden met het account object in dezelfde method.
            var customerDto = CreateCustomerService.CreateNewCustomerDTO(firstName, lastName, email, phone, infix);

            // dto naar api sturen
            var customerResponse = await _httpClient.PostAsJsonAsync("/api/Customer", customerDto);

            if (!customerResponse.IsSuccessStatusCode)
            {
                var error = await customerResponse.Content.ReadAsStringAsync();
                TempData["Error"] = error;
                return View("CreateCustomer");
            }

            // api response (customer object) lezen als response succesvol is
            var customer = await customerResponse.Content.ReadFromJsonAsync<Customer>();

            if (customer == null)
            {
                TempData["Error"] = "Kon klant niet ophalen na aanmaak";
                return View("CreateCustomer");
            }

            // 2. account dto aanmaken
            var accountDto = CreateAccountService.CreateNewAccountDTO(customer.CustomerId, username, password);

            // dto naar api sturen
            var accountResponse = await _httpClient.PostAsJsonAsync("/api/Account", accountDto);

            if (!accountResponse.IsSuccessStatusCode)
            {

                var error = await accountResponse.Content.ReadAsStringAsync();
                TempData["Error"] = error;

                // rollback customer als account niet aangemaakt wordt
                await _httpClient.DeleteAsync($"/api/Customer/{customer.CustomerId}");

                return View("CreateCustomer");
            }

            // opgeslagen username en wachtwoord verwijderen uit session (niet meer nodig)
            HttpContext.Session.Remove("RegisterUsername");
            HttpContext.Session.Remove("RegisterPassword");

            TempData["LoginSuccess"] = "Registratie voltooid! Je kunt nu inloggen.";
            return RedirectToAction("CreateCustomer");
        }

       [HttpGet]
        public IActionResult ReservationConfirmation()
        {

            return View("ReservationConfirmation");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // checken of beide username en password fields zijn ingevuld
            bool requiredFields = LoginService.RequiredFields(username, password);
            if (!requiredFields)
            {
                TempData["Error"] = "Vul alle velden in";
                return View("Login");
            }

            // dto aanmaken via de login service method
            var loginDto = LoginService.CreateNewLoginDTO(username, password);


            // loginDto naar api sturen, en de response daarvan in loginResponse var zetten 
            var loginResponse = await _httpClient.PostAsJsonAsync("/api/Login", loginDto);

            if (!loginResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Ongeldige inloggegevens";
                return View("Login");
            }

            // login response lezen 
            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDTO>();

            if (loginResult == null)
            {
                TempData["Error"] = "Er ging iets mis bij het inloggen";
                return View("Login");
            }

            // sla gegevens die nodig zijn op in session 
            HttpContext.Session.SetString("Username", loginResult.Username);
            HttpContext.Session.SetString("AccountRole", loginResult.Role.ToString());
            HttpContext.Session.SetString("FirstName", loginResult.FirstName);


            // customer id opslaan in session (wordt gebruikt voor reserveringen en om te checken of session nog valid is)
            if (loginResult.CustomerId.HasValue)
            {
                HttpContext.Session.SetInt32("CustomerId", loginResult.CustomerId.Value);
            }

            // redirect terug naar homepage 
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

   

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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
                return View();
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
                TempData["Error"] = "Minimaal 1 volwassene nodig";
                return View();
            }

            int numberOfNights = (endDate - startDate).Days;

            if (hasElectricity && !CreateReservationService.ValidateElectricity(electricityDays, numberOfNights))
            {
                TempData["Error"] = $"Elektriciteit kan maximaal {numberOfNights} dagen zijn (aantal nachten)";
                return View();
            }

            if (hasElectricity && !CreateReservationService.ValidateElectricity(electricityDays))
            {
                TempData["Error"] = "Kies minimaal 1 dag voor elektriciteit";
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
    }
}
