using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;




namespace LeMarconnes.Controllers
{
    public class AccountController : Controller
    {

        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7290");
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

            if (!CreateAccountService.DoPasswordsMatch(password, confirmPassword))
            {
                TempData["Error"] = "Wachtwoorden komen niet overeen";
                return View("CreateAccount");
            }

            if (!CreateAccountService.ValidPasswordLength(password))
            {
                TempData["Error"] = "Wachtwoord moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of alle velden zijn ingevuld        
            if (!CreateAccountService.ValidateFields(username, password, confirmPassword))
            {
                TempData["Error"] = "Vul alle velden in";
                return View("CreateAccount");
            }

            // checken of username input geldig is                    
            if (!CreateAccountService.ValidUsernameLength(username))
            {
                TempData["Error"] = "Gebruikersnaam moet minimaal 4 karakters zijn";
                return View("CreateAccount");
            }

            // checken of username alleen uit geldige tekens bestaat
            if (!CreateAccountService.ValidUsernameChars(username))
            {
                TempData["Error"] = "Gebruikersnaam kan alleen uit letters en cijfers bestaan";
                return View("CreateAccount");
            }

            // valideren of username al bestaat in db voordat account aangemaakt kan worden
            var accountResponse = await _httpClient.GetAsync($"/api/Account/exists/{username}");

            if (accountResponse.IsSuccessStatusCode)
            {
                // api geeft 200 OK terug als username bestaat
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
            if (!CreateCustomerService.AccountInfoReceived(username, password))
            {
                TempData["Error"] = "Sessie verlopen. Probeer opnieuw.";
                return View("CreateAccount");
            }

            // checken of all required parameters geldige invoer hebben (niet null of alleen maar whitespace)         
            if (!CreateCustomerService.RequiredFieldsCheck(firstName, lastName, email, phone))
            {
                TempData["Error"] = "Vul alle verplichte velden in";
                return View("CreateCustomer");
            }

            // checken of email invoer minimaal 1 '@' teken heeft (en maximaal 1)
            if (!CreateCustomerService.ValidEmail(email))
            {
                TempData["Error"] = "Ongeldige email";
                return View("CreateCustomer");
            }

            if (!CreateCustomerService.ValidatePhone(phone))
            {
                TempData["Error"] = "Ongeldig telefoonnummer";
                return View("CreateCustomer");
            }

            // 1. customer dto aanmaken
            // customer moet als eerst aangemaakt worden, zodat het customer object teruggegeven kan worden voordat het account object wordt aangemaakt.
            // het (door de database gegenereerde) customerId wordt uit dit customer object gehaald, zodat deze direct gelinked kan worden met het account object in dezelfde method.
            var customerDto = CreateCustomerService.CreateNewCustomerDTO(firstName, lastName, email, phone, infix);

            // POST customer doen met de dto
            var customerResponse = await _httpClient.PostAsJsonAsync("/api/Customer", customerDto);

            if (!customerResponse.IsSuccessStatusCode)
            {
                var error = await customerResponse.Content.ReadAsStringAsync();
                TempData["Error"] = error;
                return View("CreateCustomer");
            }

            // api response (customer object) lezen als response succesvol is
            var customer = await customerResponse.Content.ReadFromJsonAsync<Customer>();


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
    }
}
