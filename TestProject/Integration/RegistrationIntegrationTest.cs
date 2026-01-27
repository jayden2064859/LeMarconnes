using API.Data;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Net;
using System.Net.Http.Json;

// UC:01 - Registreren als nieuwe klant
public class RegistrationIntegrationTests : IClassFixture<WebApplicationFactory<Program>> // IClassFicture zorgt voor shared context voor alle tests binnen de class
{
    // de integratietests hebben httpclient nodig omdat de volledige applicatieflow van client naar server getest wordt 
    private readonly HttpClient _client;

    public RegistrationIntegrationTests(WebApplicationFactory<Program> factory)
    {
        // sqlite is nodig voor transaction support
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // de options uit de bestaande dbcontext verwijderen (de connnectionstring naar azure db)
                services.RemoveAll<DbContextOptions<LeMarconnesDbContext>>();

                // connectionstring wordt vervangen met sqlite in-memory db
                services.AddDbContext<LeMarconnesDbContext>(options =>
                    options.UseSqlite(connection));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<LeMarconnesDbContext>();

                db.Database.EnsureCreated(); 
            });
        }).CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("http://localhost"), //  dummy address nodig voor httpclient 

        });
    }

    [Fact]
    public async Task Registration_WithValidData_ReturnsOk()
    {
        // ARRANGE
        var dto = new RegistrationDTO
        {
            Username = "newCustomer",
            Password = "1234",
            FirstName = "New",
            Infix = null,
            LastName = "Customer",
            Email = "new.customer@gmail.com",
            Phone = "0639536838"
        };

        // ACT
        var response = await _client.PostAsJsonAsync("/api/registration", dto);

        // DEBUG
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {response.StatusCode}");
        Console.WriteLine($"Content: {responseContent}");

        // ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);


    }
}
