using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace MVC.HttpServices
{
    public class LoginHttpService
    {
        private readonly HttpClient _httpClient;

        public LoginHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(AuthenticateResponseDTO?, string?)> LoginAsync(AuthenticateDTO loginDto)
        {
            var postLogin = await _httpClient.PostAsJsonAsync("/api/authenticate", loginDto);

            if (!postLogin.IsSuccessStatusCode)
            {
                // api error message doorgeven
                var errorContent = await postLogin.Content.ReadAsStringAsync();
                return (null, errorContent);
            }

            var responseDto = await postLogin.Content.ReadFromJsonAsync<AuthenticateResponseDTO>();
            return (responseDto, null);
        }
    }
}