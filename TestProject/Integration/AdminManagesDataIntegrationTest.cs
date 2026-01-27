using API.Data;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TestProject.Integration
{
    public class AdminManagesDataIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly SqliteConnection _connection;

        public AdminManagesDataIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<LeMarconnesDbContext>>();

                    services.AddDbContext<LeMarconnesDbContext>(options =>
                        options.UseSqlite(_connection));

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

        // admin maakt reservering/klant/account/accommodatie aan
        [Fact]
        public async Task Admin_CreatesData_Succeeds()
        {
            // admin registreren & inloggen
            var admin = new { Username = "Admin", Password = "admin", Email = "admin@test.com", LastName = "Admin", FirstName = "Admin", Phone = "0612345678" };
            await _client.PostAsJsonAsync("/api/registration", admin);
            var auth = await AuthHelper.GetAuthResponse(_client, "Admin", "admin");

            // maakt klant aan
            var dto = new
            {
                FirstName = "Test",
                LastName = "Klant",
                Email = "unique.create@test.com",
                Phone = "0611223344",
                Password = "Password123!"
            };

            // post request met admin token
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/customer");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);
            request.Content = JsonContent.Create(dto);

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // admin verwijderd reservering/klant/account/accommodatie
        [Fact]
        public async Task Admin_DeletesData_Succeeds()
        {
            // admin registreren & inloggen
            var admin = new { Username = "Admin", Password = "admin", Email = "admin@test.com", LastName = "Admin", FirstName = "Admin", Phone = "0612345678" };
            await _client.PostAsJsonAsync("/api/registration", admin);
            var auth = await AuthHelper.GetAuthResponse(_client, "Admin", "admin");

            // maakt klant aan
            var customer = new { FirstName = "Weg", LastName = "Klant", Email = "weg@test.com", Phone = "0600000000", Password = "Password123!" };
            var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/customer");
            createReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);
            createReq.Content = JsonContent.Create(customer);
            var createRes = await _client.SendAsync(createReq);

            // id ophalen uit de response
            var responseString = await createRes.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(responseString);
            int customerId = doc.RootElement.GetProperty("customerId").GetInt32();

            // delete met id
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/customer/{customerId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        // admin update reservering/klant/account/accommodatie
        [Fact]
        public async Task Admin_UpdatesCustomerData_Succeeds()
        {
            // ad,omin registreren & inloggen
            var admin = new { Username = "Admin", Password = "admin", Email = "admin@test.com", LastName = "Admin", FirstName = "Admin", Phone = "0612345678" };
            await _client.PostAsJsonAsync("/api/registration", admin);
            var auth = await AuthHelper.GetAuthResponse(_client, "Admin", "admin");

            // klant aanmaken
            var customer = new { FirstName = "Oud", LastName = "Klant", Email = "update@test.com", Phone = "0600000000", Password = "Password123!" };
            var createReq = new HttpRequestMessage(HttpMethod.Post, "/api/customer");
            createReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);
            createReq.Content = JsonContent.Create(customer);
            var createRes = await _client.SendAsync(createReq);

            // id ophalen uit de response
            var responseString = await createRes.Content.ReadAsStringAsync();
            using var doc = System.Text.Json.JsonDocument.Parse(responseString);
            int customerId = doc.RootElement.GetProperty("customerId").GetInt32();

            // update met id
            var updateDto = new { FirstName = "Nieuw", LastName = "Admin", Email = "update@test.com", Phone = "0611111111", Password = "Password123!" };
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/customer/{customerId}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.Token);
            request.Content = JsonContent.Create(updateDto);

            var response = await _client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        // admin maakt nieuwe accommodatie aan
        [Fact]
        public async Task Admin_Creates_NewAccommodation_ReturnsCreated()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Admin", "admin");

            var newAccommodation = new PostAccommodationDTO
            {
                PlaceNumber = "5C",
                Capacity = 4,
                AccommodationTypeId = 1
            };


            // ACT
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/accommodation");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token);
            request.Content = JsonContent.Create(newAccommodation);
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

    }
}
  





