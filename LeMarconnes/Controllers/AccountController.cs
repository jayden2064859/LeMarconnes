using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;




namespace LeMarconnes.Controllers
{
    public class AccountController : Controller
    {

        // dependency injection gebruiken voor de services (api communicatie)
        private readonly AccountService _accountService;
        private readonly CustomerService _customerService;

        public AccountController(AccountService accountService, CustomerService customerService)
        {
            _accountService = accountService;
            _customerService = customerService;
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

            if (!AccountValidation.DoPasswordsMatch(password, confirmPassword))
            {
                TempData["Error"] = "Wachtwoorden komen niet overeen";
                return View("CreateAccount");
            }

            if (!AccountValidation.ValidPasswordLength(password))
            {
                TempData["Error"] = "Wachtwoord moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of alle velden zijn ingevuld        
            if (!AccountValidation.ValidateFields(username, password, confirmPassword))
            {
                TempData["Error"] = "Vul alle velden in";
                return View("CreateAccount");
            }

            // checken of username input geldig is                    
            if (!AccountValidation.ValidUsernameLength(username))
            {
                TempData["Error"] = "Gebruikersnaam moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of username alleen uit geldige tekens bestaat
            if (!AccountValidation.ValidUsernameChars(username))
            {
                TempData["Error"] = "Gebruikersnaam kan alleen uit letters en cijfers bestaan";
                return View("CreateAccount");
            }

            // valideren of username al bestaat in db voordat account aangemaakt kan worden
            var exists = await _accountService.CheckUsernameExistsAsync(username);

            if (exists)
            {
                TempData["Error"] = "Gebruikersnaam is al in gebruik";
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
        public async Task<IActionResult> CreateCustomer(string firstName, string lastName, string email,
                                                string phone, string? infix = null)
        {

            // opgeslagen username en password van de vorige pagina uit de session ophalen
            var username = HttpContext.Session.GetString("RegisterUsername");
            var password = HttpContext.Session.GetString("RegisterPassword");

            // checken of username en password succesvol uit de session zijn gehaald     
            if (!CustomerValidation.AccountInfoReceived(username, password))
            {
                TempData["Error"] = "Sessie verlopen. Probeer opnieuw.";
                return View("CreateAccount");
            }

            // checken of all required parameters geldige invoer hebben (niet null of alleen maar whitespace)         
            if (!CustomerValidation.RequiredFieldsCheck(firstName, lastName, email, phone))
            {
                TempData["Error"] = "Vul alle verplichte velden in";
                return View("CreateCustomer");
            }

            // checken of email invoer minimaal 1 '@' teken heeft (en maximaal 1)
            if (!CustomerValidation.ValidEmail(email))
            {
                TempData["Error"] = "Ongeldige email";
                return View("CreateCustomer");
            }

            if (!CustomerValidation.ValidatePhone(phone))
            {
                TempData["Error"] = "Ongeldig telefoonnummer";
                return View("CreateCustomer");
            }

            // 1. customer dto aanmaken
            // customer moet als eerst aangemaakt worden, zodat het customer object teruggegeven kan worden voordat het account object wordt aangemaakt.
            // het (door de database gegenereerde) customerId wordt uit dit customer object gehaald, zodat deze direct gelinked kan worden met het account object in dezelfde method.           
            var customerDto = new CreateCustomerDTO
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Phone = phone,
                Infix = infix
            };


            // POST customer doen met de dto (customer obj wordt uitgelezen als Post succesvol is, anders wordt error  uitgelezen)
            var (customer, customerError) = await _customerService.CreateCustomerAsync(customerDto);

            if (customer == null)
            {
                TempData["Error"] = customerError;
                return View("CreateCustomer");
            }

            // 2. account dto aanmaken
            var accountDto = new CreateAccountDTO
            {
                CustomerId = customer.CustomerId,
                Username = username,
                PlainPassword = password
            };


            // dto naar api sturen 
            var (accountCreated, accountError) = await _accountService.CreateAccountAsync(accountDto);

            if (accountCreated == null)
            {
                // rollback customer als account niet aangemaakt wordt
                await _customerService.DeleteCustomerAsync(customer.CustomerId);

                TempData["Error"] = accountError;
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
