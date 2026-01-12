using ClassLibrary.DTOs;
using System.Net.Http.Json;

namespace ClassLibrary.HttpServices
{
    public class LoginHttpService
    {
        private readonly HttpClient _httpClient;

        public LoginHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(LoginResponseDTO?, string?)> LoginAsync(LoginDTO loginDto)
        {
            var postLogin = await _httpClient.PostAsJsonAsync("/api/Login", loginDto);

            if (!postLogin.IsSuccessStatusCode)
            {
                // api error message doorgeven
                var errorContent = await postLogin.Content.ReadAsStringAsync();
                return (null, errorContent);
            }

            var responseDto = await postLogin.Content.ReadFromJsonAsync<LoginResponseDTO>();
            return (responseDto, null);
        }
    }
}