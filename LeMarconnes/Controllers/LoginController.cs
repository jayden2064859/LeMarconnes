using ClassLibrary.DTOs;
using ClassLibrary.HttpServices;
using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {

        private readonly LoginHttpService _httpService;

        public LoginController(LoginHttpService httpService)
        {
            _httpService = httpService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {

            // checken of beide username en password fields zijn ingevuld
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Vul alle velden in";
                return View("Login");
            }

            var loginDto = new LoginDTO
            {
                Username = username,
                Password = password
            };

            // loginDto naar api sturen, en de response daarvan in loginResponse var zetten 
            var (loginResult, errorMessage) = await _httpService.LoginAsync(loginDto);

            if (loginResult == null)
            {
                TempData["Error"] = errorMessage;
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
            return RedirectToAction("Homepage", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Homepage", "Home");
        }
    }
}
