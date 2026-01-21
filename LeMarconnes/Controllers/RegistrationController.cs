using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using MVC.HttpServices;

public class RegistrationController : Controller
{
    private readonly RegistrationHttpService _registerService;

    public RegistrationController(RegistrationHttpService registerService)
    {
        _registerService = registerService;
    }

    [HttpGet]
    public IActionResult CreateAccount()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount(string username, string password, string confirmPassword)
    {

        if (password != confirmPassword)
        {
            TempData["Error"] = "Wachtwoorden komen niet overeen";
            return View();
        }

        HttpContext.Session.SetString("RegisterUsername", username);
        HttpContext.Session.SetString("RegisterPassword", password);

        return RedirectToAction("CreateCustomer");
    }

    [HttpGet]
    public IActionResult CreateCustomer()
    {
        string username = HttpContext.Session.GetString("RegisterUsername");
        string password = HttpContext.Session.GetString("RegisterPassword");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return RedirectToAction("CreateAccount");
        }
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer(string firstName, string lastName, string email, string phone, string? infix = null)
    {
        var username = HttpContext.Session.GetString("RegisterUsername");
        var password = HttpContext.Session.GetString("RegisterPassword");

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return RedirectToAction("Login", "Login");
        }

        var registerDto = new RegistrationDTO
        {
            Username = username,
            Password = password,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Phone = phone,
            Infix = infix
        };

        // service doet de api call en geeft eventuele error message terug
        var error = await _registerService.RegisterAsync(registerDto);

        if (error != null)
        {
            TempData["Error"] = error;
            return View();
        }

        // session clearen
        HttpContext.Session.Remove("RegisterUsername");
        HttpContext.Session.Remove("RegisterPassword");

        TempData["LoginSuccess"] = "Registratie voltooid! Je kunt nu inloggen.";
        return View();
    }
}