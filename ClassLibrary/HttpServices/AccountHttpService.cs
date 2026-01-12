using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace ClassLibrary.HttpServices
{
    public class AccountHttpService
    {
        private readonly HttpClient _httpClient;

        public AccountHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(bool?, string?)> CheckUsernameExistsAsync(string username)
        {
            var usernameExists = await _httpClient.GetAsync($"/api/Account/exists/{username}");
            
            if (!usernameExists.IsSuccessStatusCode)
            {
                var errorContent = await usernameExists.Content.ReadAsStringAsync();
                return (null, errorContent);
            }
            
            return (usernameExists.IsSuccessStatusCode, null); 
        }

        // account object is niet nodig na succesvol aanmaken, dus alleen een true/false is nodig hiervoor
        public async Task<(bool?, string?)> CreateAccountAsync(AccountDTO accountDto)
        {
            var account = await _httpClient.PostAsJsonAsync("/api/Account", accountDto);

            if (!account.IsSuccessStatusCode)
            {
                // api error message doorgeven
                var errorContent = await account.Content.ReadAsStringAsync();
                return (null, errorContent);
            }

            return (account.IsSuccessStatusCode, null);
        }


    }
}
