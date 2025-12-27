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
        public IActionResult CreateAccount(string username, string password, string confirmPassword)
        {

            if (password != confirmPassword)
            {
                TempData["Error"] = "Wachtwoorden komen niet overeen";
                return View();
            }

            // LINQ om alle parameters in 1 keer te valideren
            var requiredFields = new[] {username, password, confirmPassword };
            if (requiredFields.Any(string.IsNullOrWhiteSpace))
            {
                TempData["Error"] = "Vul alle velden in";
                return View("CreateAccount");
            }

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
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Registratiesessie verlopen. Probeer opnieuw.";
                return View("CreateAccount");
            }

            // checken of all required parameters geldig zijn 
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                TempData["Error"] = "Vul alle verplichte velden in";
                return View("CreateCustomer");
            }


            // 1. customer aanmaken

            var customerDto = new CreateCustomerDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Infix = infix
            };


            var customerResponse = await _httpClient.PostAsJsonAsync("/api/Customer", customerDto);

            if (!customerResponse.IsSuccessStatusCode)
            {
                var errorMsg = await customerResponse.Content.ReadAsStringAsync();
                // niet de specifieke error direct tonen aan de gebruiker
                TempData["Error"] = "Er ging iets fout tijdens het opslaan van klantgegevens. Probeer opnieuw";
                // specifieke error wordt wel intern gelogd
                Console.WriteLine($"Fout: {errorMsg}");
                return View("CreateCustomer");
            }

            var customer = await customerResponse.Content.ReadFromJsonAsync<Customer>();

            if (customer == null)
            {
                TempData["Error"] = "Kon klant niet ophalen na aanmaak";
                return View("CreateCustomer");
            }

            // 2. account aanmaken

            var accountDto = new CreateAccountDTO
            {
                CustomerId = customer.CustomerId,
                Username = username,
                PlainPassword = password
            };

            var accountResponse = await _httpClient.PostAsJsonAsync("/api/Account", accountDto);

            if (!accountResponse.IsSuccessStatusCode)
            {
                // rollback customer als account niet aangemaakt wordt
                await _httpClient.DeleteAsync($"/api/Customer/{customer.CustomerId}");

                var errorMsg = await accountResponse.Content.ReadAsStringAsync();
                TempData["Error"] = $"Fout bij aanmaken account: {errorMsg}";
                return View("CreateCustomer");
            }

            HttpContext.Session.Remove("RegisterUsername");
            HttpContext.Session.Remove("RegisterPassword");

            TempData["Success"] = "Registratie voltooid! Je kunt nu inloggen.";
            return View("CreateCustomer");
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Reservation()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
