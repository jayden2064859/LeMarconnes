using API.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
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
    public class EmployeeRetreivesDataIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly SqliteConnection _connection;

        public EmployeeRetreivesDataIntegrationTest(WebApplicationFactory<Program> factory)
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

        // UC-04: Medewerker ziet gegevens in (kan niks toevoegen of deleten)
        [Fact]
        public async Task Employee_Gets_AllReservations_ReturnsOK()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Employee", "1234");

            // ACT
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/Reservation");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token); // token als header meegeven
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Employee_Gets_AccountById_ReturnsOK()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Employee", "1234");

            // ACT
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/account/1");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token); // token als header meegeven
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        
        [Fact]
        public async Task Employee_DeletesAccount_ReturnsForbidden()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Employee", "1234");

            // ACT
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/account/4");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token); // token als header meegeven
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }


        [Fact]
        public async Task Employee_PostsAccount_ReturnsForbidden()
        {
            // ARRANGE
            var authentication = await AuthHelper.GetAuthResponse(_client, "Employee", "1234");
           
            var newAccount = new AccountDTO
            {
                Username = "newemployee",
                PlainPassword = "password123",
                Role = Account.Role.Employee
            };
            // ACT
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/account");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authentication.Token); // token als header meegeven
            request.Content = JsonContent.Create(newAccount);
            var response = await _client.SendAsync(request);

            // ASSERT
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }
    }
}
