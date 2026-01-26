using ExternalAPI.Classes;

namespace ExternalAPI
{
    public class ExternalApiTestService
    {
        private readonly HttpClient _httpClient;

        public ExternalApiTestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET /api/Accomodations
        public async Task<List<AccomodationResponseDTO>> GetAllAccomodationsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Accomodations");
            var accomodations = await response.Content.ReadFromJsonAsync<List<AccomodationResponseDTO>>();

            if (!accomodations.Any())
            {
                throw new Exception("Geen accomodaties gevonden");
            }
            return accomodations;
        }

        // POST /api/Accomodations
        public async Task<AccomodationResponseDTO> PostAccomodationAsync(AccomodationDTO accomodation)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Accomodation", accomodation);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Fout bij het aanmaken van een accomodatie");
            }
            var createdAccomodation = await response.Content.ReadFromJsonAsync<AccomodationResponseDTO>();
            return createdAccomodation;
        }

        // GET /api/Accomodations/{id}
        public async Task<AccomodationResponseDTO> GetAccomodationByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Accomodations/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Accomodatie niet gevonden");
            }
            var accomodation = await response.Content.ReadFromJsonAsync<AccomodationResponseDTO>();
            return accomodation;
        }

        // POST /api/Gites
        public async Task<GiteResponseDTO> PostGiteAsync(GiteDTO gite)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Gites", gite);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Fout bij het aanmaken van een gite");
            }
            var createdGite = await response.Content.ReadFromJsonAsync<GiteResponseDTO>();
            return createdGite;
        }


        // GET /api/Gites/{id}
        public async Task<List<GiteResponseDTO>> GetGitesByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Gites/{id}");
            var gites = await response.Content.ReadFromJsonAsync<List<GiteResponseDTO>>();

            if (!gites.Any())
            {
                throw new Exception("Geen gites gevonden");
            }
            return gites;
        }

        // GET /api/Reservations
        public async Task<List<ReservationResponseDTO>> GetAllReservationsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Reservations");
            var reservations = await response.Content.ReadFromJsonAsync<List<ReservationResponseDTO>>();
            if (!reservations.Any())
            {
                throw new Exception("Geen reserveringen gevonden");
            }
            return reservations;
        }

        // POST /api/Reservations
        public async Task<ReservationResponseDTO> PostReservationAsync(ReservationDTO reservation)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Reservations", reservation);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Fout bij het aanmaken van een reservering");
            }
            var createdReservation = await response.Content.ReadFromJsonAsync<ReservationResponseDTO>();
            return createdReservation;
        }

        // GET /api/Reservations/by-accomodation/{accomodationId}
        public async Task<List<ReservationResponseDTO>> GetReservationsByAccomodationIdAsync(int accomodationId)
        {
            var response = await _httpClient.GetAsync($"/api/Reservations/by-accomodation/{accomodationId}");
            var reservations = await response.Content.ReadFromJsonAsync<List<ReservationResponseDTO>>();
            if (!reservations.Any())
            {
                throw new Exception("Geen reserveringen gevonden voor deze accomodatie");
            }
            return reservations;
        }


        // GET /api/Reservations/availability
        public async Task<List<ReservationResponseDTO>> GetReservationsAvailabilityAsync(int accomodationId, DateTime checkIn, DateTime checkOut)
        {
            var response = await _httpClient.GetAsync($"/api/Reservations/availability?accomodationId=1&checkIn={checkIn:yyyy-MM-dd}&checkOut={checkOut:yyyy-MM-dd}");
            var reservations = await response.Content.ReadFromJsonAsync<List<ReservationResponseDTO>>();
            if (!reservations.Any())
            {
                throw new Exception("Geen beschikbare reserveringen gevonden");
            }
            return reservations;
        }

        // GET /api/Reservations/{id}
        public async Task<ReservationResponseDTO> GetReservationByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Reservations/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Reservering niet gevonden");
            }
            var reservation = await response.Content.ReadFromJsonAsync<ReservationResponseDTO>();
            return reservation;
        }

        // GET /api/Users 
        public async Task<List<UserResponseDTO>> GetAllUsersAsync()
        {
            var response = await _httpClient.GetAsync("/api/Users");
            var users = await response.Content.ReadFromJsonAsync<List<UserResponseDTO>>();
            if (!users.Any())
            {
                throw new Exception("Geen users gevonden");
            }
            return users;
        }

        // GET /api/Guests
        public async Task<List<GuestResponseDTO>> GetAllGuestsAsync()
        {
            var response = await _httpClient.GetAsync("/api/Guests");
            var guests = await response.Content.ReadFromJsonAsync<List<GuestResponseDTO>>();
            if (!guests.Any())
            {
                throw new Exception("Geen guests gevonden");
            }
            return guests;
        }

        // POST /api/Guests
        public async Task<GuestResponseDTO> PostGuestAsync(GuestDTO guest)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/Guests", guest);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Fout bij het aanmaken van een guest");
            }
            var createdGuest = await response.Content.ReadFromJsonAsync<GuestResponseDTO>();
            return createdGuest;
        }

        // GET /api/Guests/{id}
        public async Task<GuestResponseDTO> GetGuestByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Guests/{id}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Guest niet gevonden");
            }
            var guest = await response.Content.ReadFromJsonAsync<GuestResponseDTO>();
            return guest;

        }

    }
}



