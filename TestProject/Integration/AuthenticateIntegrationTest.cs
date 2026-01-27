using API.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TestProject.Integration
{

    // UC02: Inloggen - er wordt getest of de JWT token correct wordt teruggegeven na succesvolle authenticatie 
    // vormt de basis voor verdere tests die de beveiligde API endpoints gebruiken
    public class AuthenticateIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly SqliteConnection _connection;

        public AuthenticateIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.RemoveAll<DbContextOptions<LeMarconnesDbContext>>(); // bestaande connectionstring naar AzureDb verwijderen

                    services.AddDbContext<LeMarconnesDbContext>(options =>
                        options.UseSqlite(_connection)); // nieuwe sqlite in memory db voor de tests

                    var sp = services.BuildServiceProvider();
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<LeMarconnesDbContext>();

                    db.Database.EnsureCreated(); // zorgt ervoor dat alle entities aangemaakt worden
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost"),

            });
        }

        // 3 aparte testen om te valideren dat de drie accountrol types de correcte accountrole claim in de JWT token teruggeven

        [Fact]
        public async Task Authenticate_Customer_ReturnsCustomerRole()
        {
            var auth = await AuthHelper.GetAuthResponse(_client, "Customer", "1234");
            Assert.NotNull(auth);
            Assert.False(string.IsNullOrWhiteSpace(auth.Token));
            Assert.Equal(Account.Role.Customer, auth.Role);
        }


        [Fact]
        public async Task Authenticate_Admin_ReturnsAdminRole()
        {
            var auth = await AuthHelper.GetAuthResponse(_client, "Admin", "admin");
            Assert.NotNull(auth);
            Assert.False(string.IsNullOrWhiteSpace(auth.Token));
            Assert.Equal(Account.Role.Admin, auth.Role);
        }


        [Fact]
        public async Task Authenticate_Employee_ReturnsEmployeeRole()
        {
            var auth = await AuthHelper.GetAuthResponse(_client, "Employee", "1234");
            Assert.NotNull(auth);
            Assert.False(string.IsNullOrWhiteSpace(auth.Token));
            Assert.Equal(Account.Role.Employee, auth.Role);
        }

    }
}
