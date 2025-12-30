using Azure.Identity;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.DTOs;
using ClassLibrary.Services;
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
        public  async Task<IActionResult> CreateAccount(string username, string password, string confirmPassword)
        {
            // service gebruiken om te checken of wachtwoorden overeenkomen
            bool doPasswordsMatch = CreateAccountService.DoPasswordsMatch(password, confirmPassword);

            if (!doPasswordsMatch)
            {
                TempData["Error"] = "Wachtwoorden komen niet overeen";
                return View("CreateAccount");
            }

            bool validPasswordLength = CreateAccountService.ValidPasswordLength(password);

            if (!validPasswordLength)
            {
                TempData["Error"] = "Wachtwoord moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of alle velden zijn ingevuld
            bool validFieldInputs = CreateAccountService.ValidateFields(username, password, confirmPassword);

            if (!validFieldInputs)
            {
                TempData["Error"] = "Vul alle velden in";
                return View("CreateAccount");
            }

            // checken of username input geldig is
            bool validUsernameLength = CreateAccountService.ValidUsernameLength(username);
            
            if (!validUsernameLength)
            {
                TempData["Error"] = "Gebruikersnaam moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of username alleen uit geldige tekens bestaat
            bool validUsernameChars = CreateAccountService.ValidUsernameChars(username);
            if (!validUsernameChars)
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
        public IActionResult CreateCustomer()
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
            bool receivedAccountInfo = CreateCustomerService.AccountInfoReceived(username, password);
            if (!receivedAccountInfo)
            {
                TempData["Error"] = "Sessie verlopen. Probeer opnieuw.";
                return View("CreateAccount");
            }

            // checken of all required parameters geldige invoer hebben (niet null of alleen maar whitespace)
            bool validInputs = CreateCustomerService.RequiredFieldsCheck(firstName, lastName, email, phone);
            if (!validInputs)
            {
                TempData["Error"] = "Vul alle verplichte velden in";
                return View("CreateCustomer");
            }

            // checken of email invoer minimaal 1 '@' teken heeft (en maximaal 1)
            bool validEmail = CreateCustomerService.ValidEmail(email);
            if (!validEmail)
            {
                TempData["Error"] = "Ongeldige email";
                return View("CreateCustomer");
            }

            bool validPhone = CreateCustomerService.ValidatePhone(phone);
            if (!validPhone)
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
        public IActionResult CreateReservation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReservation(List<int> accommodationIds, DateTime startDate, DateTime endDate, int adultsCount,
            int children0_7Count, int children7_12Count, int dogsCount, bool hasElectricity, int? electricityDays = null)
        {
            // customer id uit sessie ophalen
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (!customerId.HasValue)
            {
                return RedirectToAction("Login");
            } 

            // service methods gebruiken voor business rules check

            bool validDates = CreateReservationService.ValidateReservationDates(startDate, endDate);
            if (!validDates)
            {
                TempData["Error"] = "Einddatum moet later dan startdatum zijn";
                return RedirectToAction("CreateReservation");
            }

            bool validAdultCount = CreateReservationService.ValidateAdultCounts(adultsCount);
            if (!validAdultCount)
            {
                TempData["Error"] = "Minimaal 1 volwassene nodig voor een reservering";
                return RedirectToAction("CreateReservation");
            }

            bool validAccommodationCount = CreateReservationService.ValidateAccommodationIds(accommodationIds);
            if (!validAccommodationCount)
            {
                TempData["Error"] = "Minimaal 1 accommodatie nodig voor een reservering";
                return RedirectToAction("CreateReservation");
            }

            if (hasElectricity)
            {
                bool validElectricityDaysInput = CreateReservationService.ValidateElectricity(electricityDays);
                if (!validElectricityDaysInput)
                {
                    TempData["Error"] = "Als je voor elektriciteit heb gekozen moet je minimaal 1 dag kiezen";
                    return RedirectToAction("CreateReservation");
                }
            }

            // dto aanmaken met de user input als data
            var reservationDto = CreateReservationService.CreateNewReservationDTO(customerId.Value, accommodationIds, startDate, endDate, adultsCount, children0_7Count,
                                                                                  children7_12Count, dogsCount, hasElectricity, electricityDays);

            // reservationDTO naar api sturen en de response meteen ophalen
            var reservationResponse = await _httpClient.PostAsJsonAsync("/api/Reservation", reservationDto);

            if (!reservationResponse.IsSuccessStatusCode)
            {
                TempData["Error"] = "Er is iets misgegaan tijdens aanmaken van je reservering";
                return RedirectToAction("CreateReservation");
            }

            TempData["Success"] = "Reservering succesvol aangemaakt!";
            return RedirectToAction("Index");

        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
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

            try
            {
                // loginDto naar api sturen, en de response daarvan in loginResponse var zetten 
                var loginResponse = await _httpClient.PostAsJsonAsync("/api/Login", loginDto);

                if (!loginResponse.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Ongeldige inloggegevens";
                    return View("Login");
                }

                // loginResponse lezen 
                var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDTO>();

                if (loginResult == null)
                {
                    TempData["Error"] = "Er ging iets mis bij het inloggen";
                    return View("Login");
                }

                // sla gebruikersgegevens op in session
                HttpContext.Session.SetInt32("AccountId", loginResult.AccountId);
                HttpContext.Session.SetString("Username", loginResult.Username);
                HttpContext.Session.SetString("AccountRole", loginResult.Role.ToString());

                // haal klantgegevens op alleen als account met een customer gelinked is (dus niet voor medewerker/admin accounts)
                if (loginResult.CustomerId.HasValue)
                {
                    HttpContext.Session.SetInt32("CustomerId", loginResult.CustomerId.Value);
                    HttpContext.Session.SetString("FirstName", loginResult.FirstName);
                    HttpContext.Session.SetString("LastName", loginResult.LastName);
                    HttpContext.Session.SetString("Phone", loginResult.Phone);
                    HttpContext.Session.SetString("Email", loginResult.Email);
                }
                

               // redirect terug naar homepage 
                return RedirectToAction("Index", "Home");
              
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Er is iets fout gegaan tijdens inloggen [check Console voor details]";
                Console.WriteLine($"Error: {ex.Message}");
                return View("Login");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home"); 
        }

        [HttpGet]
        public IActionResult AccountDetails()
        {
           // redirect naar loginpagina als bijv sessie is verlopen (data van die specifieke user is dan niet meer beschikbaar)
            if (HttpContext.Session.GetInt32("AccountId") == null)
            {
                return RedirectToAction("Login");
            }

            return View("AccountDetails");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
