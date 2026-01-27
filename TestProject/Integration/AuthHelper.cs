using System.Net.Http.Json;
using ClassLibrary.DTOs;

namespace TestProject.Integration
{
    public static class AuthHelper // helper method voor de tests die een jwt token nodig hebben voor toegang tot de endpoints (UC04 en UC05)
    {
        public static async Task<AuthenticateResponseDTO> GetAuthResponse(HttpClient client, string username, string password)
        {
            var dto = new AuthenticateDTO
            {
                Username = username,
                Password = password
            };

            var response = await client.PostAsJsonAsync("/api/authenticate", dto);

            var result = await response.Content.ReadFromJsonAsync<AuthenticateResponseDTO>();
            return result;
        }
    }
}