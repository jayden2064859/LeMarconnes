using ClassLibrary.DTOs;
using MVC.HttpServices;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {

        private readonly LoginHttpService _httpService;

        public LoginController(LoginHttpService httpService)
        {
            _httpService = httpService;
        }

        [HttpGet]
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

            var loginDto = new AuthenticateDTO
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

            HttpContext.Session.SetString("JwtToken", loginResult.Token);
            HttpContext.Session.SetString("CustomerId", loginResult.CustomerId.ToString());
            HttpContext.Session.SetString("Username", loginResult.Username);
            HttpContext.Session.SetString("Role", loginResult.Role.ToString());

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
