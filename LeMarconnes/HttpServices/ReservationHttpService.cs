using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MVC.HttpServices
{
    public class ReservationHttpService
    {
        private readonly HttpClient _httpClient;

        public ReservationHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // available-for-dates api call waarbij response meteen teruggegeven wordt
        public async Task<(List<Accommodation>?, string?)> GetAvailableAccommodationsAsync(DateOnly startDate, DateOnly endDate, int accommodationTypeId)
        {
            var available = await _httpClient.GetAsync($"/api/Accommodation/available-for-dates?accommodationTypeId={accommodationTypeId}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            if (!available.IsSuccessStatusCode)
            {
                var errorContent = await available.Content.ReadAsStringAsync();
                return (null, errorContent);
            }


            var response = await available.Content.ReadFromJsonAsync<List<Accommodation>>();
            return (response, null);
        }


        // customer POST endpoint call

        //"(CampingReservationResponseDTO?, string?)" betekent dat deze method een responseDTO terugstuurt,
        // OF een error message string, afhankelijk van het resultaat wat de API teruggeeft.
        public async Task<(CampingReservationResponseDTO?, string?)> CreateCampingReservationAsync(CampingReservationDTO reservationDto, string? token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/reservation/camping");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            request.Content = JsonContent.Create(reservationDto);
            // endpoint call doen met jwt token als header
            var response = await _httpClient.SendAsync(request);

            // return response
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return (null, errorContent);
            }

            var responseDto = await response.Content.ReadFromJsonAsync<CampingReservationResponseDTO>();
            return (responseDto, null);
        }


        // hotel POST endpoint call moet hier

    }
}
