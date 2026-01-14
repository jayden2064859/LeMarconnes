using ClassLibrary.DTOs;

namespace MVC.HttpServices
{
    public class RegistrationHttpService
    {
        private readonly HttpClient _httpClient;

        public RegistrationHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string?> RegisterAsync(RegistrationDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/registration", dto);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return errorContent;
            }
            return null;
        }

        public async Task<string?> CheckUsernameExistsAsync(string username)
        {
            var usernameCheck = await _httpClient.GetAsync($"/api/Account/available/{username}");

            if (!usernameCheck.IsSuccessStatusCode)
            {
                var errorContent = await usernameCheck.Content.ReadAsStringAsync();
                return errorContent;
            }

            return null;
        }
    }
}
