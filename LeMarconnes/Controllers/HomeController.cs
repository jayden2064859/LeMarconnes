using Azure.Identity;
using ClassLibrary.Data;
using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
                                                string phone, string address, string? infix = null)
        {

            var username = HttpContext.Session.GetString("RegisterUsername");
            var password = HttpContext.Session.GetString("RegisterPassword");

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Registratiesessie verlopen. Probeer opnieuw.";
                return View("CreateAccount");
            }

            Customer customer = null;

            // 1. customer aanmaken
            try
            {
                var customerResponse = await _httpClient.PostAsJsonAsync("/api/Customer", new
                {
                    firstName,
                    lastName,
                    email,
                    phone,
                    address,
                    infix
                });

                var requiredFields = new[] { firstName, lastName, email, phone, address };
                if (requiredFields.Any(string.IsNullOrWhiteSpace))
                {
                    TempData["Error"] = "Vul alle verplichte velden in";
                    return View("CreateCustomer");
                }

                if (!customerResponse.IsSuccessStatusCode)
                {
                    var errorMsg = await customerResponse.Content.ReadAsStringAsync();
                    // niet de specifieke error direct tonen aan de gebruiker
                    TempData["Error"] = "Er ging iets fout tijdens het opslaan van klantgegevens. Probeer opnieuw";
                    // specifieke error wordt wel intern gelogd
                    Console.WriteLine($"Fout: {errorMsg}");
                    return View("CreateCustomer");
                }


                customer = await customerResponse.Content.ReadFromJsonAsync<Customer>();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fout: {ex.Message}";
                return View("CreateCustomer");
            }

            // 2. account aanmaken
            try
            {
                var accountResponse = await _httpClient.PostAsJsonAsync("/api/Account", new
                {
                    customerId = customer.CustomerId,
                    username,
                    plainPassword = password
                });

                if (!accountResponse.IsSuccessStatusCode)
                {
                    // rollback customer als account niet aangemaakt wordt
                     await _httpClient.DeleteAsync($"/api/Customer/{customer.CustomerId}");

                    var errorMsg = await accountResponse.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Fout bij aanmaken account: {errorMsg}";
                    return View("CreateCustomer");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Fout bij aanmaken account: {ex.Message}";
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
