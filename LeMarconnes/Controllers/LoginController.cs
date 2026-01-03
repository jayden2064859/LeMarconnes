using ClassLibrary.DTOs;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;


namespace LeMarconnes.Controllers
{
    public class LoginController : Controller
    {

        private readonly HttpClient _httpClient;

        public LoginController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7290");
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
    }
}
