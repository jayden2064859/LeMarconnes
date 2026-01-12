using ClassLibrary.DTOs;
using ClassLibrary.HttpServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MVC.Controllers
{
    public class RegistrationController : Controller
    {

        // dependency injection gebruiken voor de services (api communicatie)
        private readonly AccountHttpService _accountHttpService;
        private readonly CustomerHttpService _customerHttpService;

        public RegistrationController(AccountHttpService accountService, CustomerHttpService customerService)
        {
            _accountHttpService = accountService;
            _customerHttpService = customerService;
        }

        [HttpGet]
        public IActionResult CreateAccount()
        {
            return View("CreateAccount");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount(string username, string password, string confirmPassword)
        {
            // services voor UI/UX validaties

            if (password != confirmPassword)
            {
                TempData["Error"] = "Wachtwoorden komen niet overeen";
                return View("CreateAccount");
            }

            if (username.IsNullOrEmpty() || password.IsNullOrEmpty() || confirmPassword.IsNullOrEmpty())
            {
                TempData["Error"] = "Vul alle velden in";
                return View("CreateAccount");
            }
       
            // valideren of username al bestaat in db voordat account aangemaakt kan worden
            var (exists, error) = await _accountHttpService.CheckUsernameExistsAsync(username); 

            if (exists == null)
            {
                TempData["Error"] = error;
                return View("CreateAccount");
            }

            // username en wachtwoord worden opgeslagen in de session als alle service methods true teruggeven (true=valid input)          
            HttpContext.Session.SetString("RegisterUsername", username);
            HttpContext.Session.SetString("RegisterPassword", password);
            
            return RedirectToAction("CreateCustomer");
        }



       [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View("CreateCustomer");
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer(string firstName, string lastName, string email, string phone, string? infix = null)
        {
            // opgeslagen username en password van de vorige pagina uit de session ophalen
            var username = HttpContext.Session.GetString("RegisterUsername");
            var password = HttpContext.Session.GetString("RegisterPassword");

            // checken of sessie verlopen is (oftewel data van de vorige view is dan niet meer opgeslagen)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login", "Login");
            }

            // 1. customer dto aanmaken
            // customer moet als eerst aangemaakt worden, zodat het customer object teruggegeven kan worden voordat het account object wordt aangemaakt.
            // het (door de database gegenereerde) customerId wordt uit dit customer object gehaald, zodat deze direct gelinked kan worden met het account object in dezelfde method.           
            var customerDto = new CustomerDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Infix = infix
            };

            // POST customer doen met de dto (customer obj wordt uitgelezen als Post succesvol is, anders wordt error uitgelezen)
            var (customer, customerError) = await _customerHttpService.CreateCustomerAsync(customerDto);

            if (customer == null)
            {
                TempData["Error"] = customerError;
                return View("CreateCustomer");
            }

            // 2. account dto aanmaken
            var accountDto = new AccountDTO
            {
                CustomerId = customer.CustomerId,
                Username = username,
                PlainPassword = password
            };

            // dto naar api sturen 
            var (accountCreated, error) = await _accountHttpService.CreateAccountAsync(accountDto);

            if (accountCreated == null)
            {
                // rollback customer als account niet aangemaakt wordt
                await _customerHttpService.DeleteCustomerAsync(customer.CustomerId);

                TempData["Error"] = error;
                return View("CreateCustomer");
            }

            // opgeslagen username en wachtwoord verwijderen uit session (niet meer nodig)
            HttpContext.Session.Remove("RegisterUsername");
            HttpContext.Session.Remove("RegisterPassword");

            TempData["LoginSuccess"] = "Registratie voltooid! Je kunt nu inloggen.";
            return RedirectToAction("CreateCustomer");
        }
    }
}
