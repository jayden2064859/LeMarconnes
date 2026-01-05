using ClassLibrary.DTOs;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;


namespace LeMarconnes.Controllers
{
    public class LoginController : Controller
    {

        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {

            // checken of beide username en password fields zijn ingevuld
            if (!LoginValidation.RequiredFields(username, password))
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
            var (loginResult, errorMessage) = await _loginService.LoginAsync(loginDto);

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
