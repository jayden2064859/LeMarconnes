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
        public async Task<(List<Accommodation>?, string?)> GetAvailableAccommodationsAsync(DateOnly startDate, DateOnly endDate, Accommodation.AccommodationType type)
        {
            var available = await _httpClient.GetAsync($"/api/Accommodation/available-for-dates?type={type}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
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
        public async Task<(CampingReservationResponseDTO?, string?)> CreateCampingReservationAsync(CampingReservationDTO reservationDto)
        {
            var postReservation = await _httpClient.PostAsJsonAsync("/api/reservation/camping", reservationDto);

            if (!postReservation.IsSuccessStatusCode)
            {
                var errorContent = await postReservation.Content.ReadAsStringAsync();
                return(null, errorContent);
            }

            var response = await postReservation.Content.ReadFromJsonAsync<CampingReservationResponseDTO>();
            return (response, null);
        }


        // hotel POST endpoint call moet hier

    }
}
