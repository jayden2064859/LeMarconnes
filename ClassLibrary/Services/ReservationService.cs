using ClassLibrary.DTOs;
using ClassLibrary.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClassLibrary.Services
{
    public class ReservationService
    {
        private readonly HttpClient _httpClient;

        public ReservationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // available-for-dates api call waarbij response meteen teruggegeven wordt
        public async Task<(List<Accommodation>?, string?)> GetAvailableAccommodationsAsync(DateTime startDate, DateTime endDate)
        {
            var available = await _httpClient.GetAsync($"/api/Accommodation/available-for-dates?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            if (!available.IsSuccessStatusCode)
            {
                var errorContent = await available.Content.ReadAsStringAsync();
                return (null, errorContent);
            }

            var response = await available.Content.ReadFromJsonAsync<List<Accommodation>>();
            return (response, null);
        }

        // post reservation api call waarbij response meteen teruggegeven wordt (en eventuele error message via tuple)
        public async Task<(ReservationResponseDTO?, string?)> CreateReservationAsync(CreateReservationDTO reservationDto)
        {
            var postReservation = await _httpClient.PostAsJsonAsync("/api/Reservation", reservationDto);

            if (!postReservation.IsSuccessStatusCode)
            {
                var errorContent = await postReservation.Content.ReadAsStringAsync();
                return(null, errorContent);
            }

            var response = await postReservation.Content.ReadFromJsonAsync<ReservationResponseDTO>();
            return (response, null);
        }
    }
}
