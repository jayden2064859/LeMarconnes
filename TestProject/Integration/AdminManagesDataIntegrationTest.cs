using API.Data;
using ClassLibrary.Models;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

        // admin verwijderd reservering/klant/account/accommodatie

        // admin wijzigt customer gegevens
    }
}
