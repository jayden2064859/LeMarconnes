using ClassLibrary.Models;
using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace ClassLibrary.HttpServices
{
    public class CustomerHttpService
    {
        private readonly HttpClient _httpClient;

        public CustomerHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // POST customer (gebruikt tuple om beide het object en eventuele error message terug te geven)
        public async Task<(Customer?, string? error)> CreateCustomerAsync(CustomerDTO customerDto)
        {
            var postCustomer = await _httpClient.PostAsJsonAsync("/api/Customer", customerDto);

            if (!postCustomer.IsSuccessStatusCode)
            {
                // error messages van de api controller teruggeven 
                var errorContent = await postCustomer.Content.ReadAsStringAsync();
                return (null, errorContent); // bij errormelding tonen is er geen valid object, dus dan wordt null meegegeven
            }

            var response = await postCustomer.Content.ReadFromJsonAsync<Customer>();
            return (response, null); // bij succesvol aangemaakt object is er geen errormessage, dus dan wordt daarvoor null meegegeven
        }


        // delete customer (rollback voor tijdens registratieproces)
        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            var deleted = await _httpClient.DeleteAsync($"/api/Customer/{customerId}");
            return deleted.IsSuccessStatusCode;
        }
    }
}