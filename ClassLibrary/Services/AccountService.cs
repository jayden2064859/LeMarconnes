using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;


namespace ClassLibrary.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;

        public AccountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> CheckUsernameExistsAsync(string username)
        {
            var exists = await _httpClient.GetAsync($"/api/Account/exists/{username}");
            return exists.IsSuccessStatusCode; 
        }

        // account object is niet nodig na succesvol aanmaken, dus alleen een true/false is nodig hiervoor
        public async Task<(bool?, string?)> CreateAccountAsync(CreateAccountDTO accountDto)
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
