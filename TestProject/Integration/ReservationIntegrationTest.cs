using API.Data;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TestProject.Integration
{
    public class ReservationIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ReservationIntegrationTest(WebApplicationFactory<Program> factory)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<LeMarconnesDbContext>>();

                    services.AddDbContext<LeMarconnesDbContext>(options =>
                        options.UseSqlite(connection));

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LeMarconnesDbContext>();

                    db.Database.EnsureCreated();
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost"),

            });
        }

        // UC-03: Klant reserveert (camping/hotel)
        // camping reservation test
        [Fact]
        public async Task PostCampingReservation_ReturnsCreatedReservation()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Customer", "1234");
            var accommodations = new List<int> { 1, 2 };

            var reservationDto = new CampingReservationDTO
            {
                CustomerId = 1,
                AccommodationIds = accommodations,
                AdultsCount = 1,
                Children0_7Count = 2,
                Children7_12Count = 2,
                DogsCount = 2,
                HasElectricity = true,
                ElectricityDays = 2,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5))
            };

            // ACT
            // expliciete http request is nodig om auth token in de header mee te sturen
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/reservation/camping");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token); // token als header meegeven
            request.Content = JsonContent.Create(reservationDto); // dto als body meegeven
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        // hotel reservation test
        [Fact]
        public async Task PostHotelReservation_ReturnsCreatedReservation()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Customer", "1234");
            var accommodations = new List<int> { 6, 7 };

            var reservationDto = new HotelReservationDTO
            {
                CustomerId = 1,
                AccommodationIds = accommodations,
                PersonCount = 1,
                StartDate = DateOnly.FromDateTime(DateTime.Today),
                EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5))
            };

            // ACT
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/reservation/hotel");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token); 
            request.Content = JsonContent.Create(reservationDto); 
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
