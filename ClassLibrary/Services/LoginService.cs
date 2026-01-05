using ClassLibrary.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Services
{
    public class LoginService
    {
        private readonly HttpClient _httpClient;

        public LoginService(HttpClient httpClient)
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