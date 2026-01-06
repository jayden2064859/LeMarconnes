using API.Controllers;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TestProject.Integration
{
    public class LoginControllerIntegrationTests : IDisposable
    {
        private readonly CampingDbContext _context;
        private readonly LoginController _controller;

        public LoginControllerIntegrationTests()
        {
            // Setup InMemory database
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options = new DbContextOptionsBuilder<CampingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            _context = new CampingDbContext(options);
            _controller = new LoginController(_context);

            // Seed test data
            CreateCustomer();
        }

        private void CreateCustomer()
        {
            // Customer aanmaken
            var customer = new Customer(
                "John",
                "Doe",
                "john.doe@example.com",
                "0612345678"
            );
            _context.Customers.Add(customer);
            _context.SaveChanges();

            // Account aanmaken met gehashed wachtwoord
            var hasher = new PasswordHasher<Account>();
            var passwordHash = hasher.HashPassword(null, "Test123!");

            var account = new Account(
                "testuser",
                passwordHash,
                customer
            )
            {
                IsActive = false // Start als inactive
            };
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        // ITC-13: Succesvolle login
        [Fact]
        public async Task Login_ValidCredentials_ReturnsLoginResponse()
        {
            // ARRANGE
            var loginDto = new LoginDTO
            {
                Username = "testuser",
                Password = "Test123!"
            };

            // ACT
            var result = await _controller.Login(loginDto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<LoginResponseDTO>(okResult.Value);

            Assert.Equal("testuser", response.Username);
            Assert.Equal("John", response.FirstName);
            Assert.Equal(Account.Role.Customer, response.Role);
            Assert.NotNull(response.CustomerId);
        }

        // ITC-14: Ongeldige username
        [Fact]
        public async Task Login_InvalidUsername_ReturnsUnauthorized()
        {
            // ARRANGE
            var loginDto = new LoginDTO
            {
                Username = "wronguser", // Niet-bestaande username
                Password = "Test123!"
            };

            // ACT
            var result = await _controller.Login(loginDto);

            // ASSERT
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("Ongeldige inloggegevens", unauthorizedResult.Value.ToString());
        }

        // ITC-15: Ongeldig wachtwoord
        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            // ARRANGE
            var loginDto = new LoginDTO
            {
                Username = "testuser",
                Password = "WrongPassword!" // Verkeerd wachtwoord
            };

            // ACT
            var result = await _controller.Login(loginDto);

            // ASSERT
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("Ongeldige inloggegevens", unauthorizedResult.Value.ToString());
        }

        // ITC-16: Lege username
        [Fact]
        public async Task Login_EmptyUsername_ReturnsConflict()
        {
            // ARRANGE
            var loginDto = new LoginDTO
            {
                Username = "", // Lege username
                Password = "Test123!"
            };

            // ACT
            var result = await _controller.Login(loginDto);

            // ASSERT
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("Ongeldige inloggegevens", unauthorizedResult.Value.ToString());
        }

        // ITC-17: Leeg wachtwoord
        [Fact]
        public async Task Login_EmptyPassword_ReturnsConflict()
        {
            // ARRANGE
            var loginDto = new LoginDTO
            {
                Username = "testuser",
                Password = "" // Leeg wachtwoord
            };

            // ACT
            var result = await _controller.Login(loginDto);

            // ASSERT
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            Assert.Contains("Ongeldige inloggegevens", unauthorizedResult.Value.ToString());
        }

        // ITC-18: Account wordt active na succesvolle login
        [Fact]
        public async Task Login_SuccessfulLogin_SetsAccountActive()
        {
            // ARRANGE - Extra inactive account aanmaken
            var customer = new Customer(
                "Jane",
                "Smith",
                "jane@example.com",
                "0698765432"
            );
            _context.Customers.Add(customer);

            var hasher = new PasswordHasher<Account>();
            var passwordHash = hasher.HashPassword(null, "Password123");

            var inactiveAccount = new Account(
                "inactiveuser",
                passwordHash,
                customer
            )
            {
                IsActive = false // Start als inactive
            };
            _context.Accounts.Add(inactiveAccount);
            await _context.SaveChangesAsync();

            var loginDto = new LoginDTO
            {
                Username = "inactiveuser",
                Password = "Password123"
            };

            // ACT
            var result = await _controller.Login(loginDto);

            // ASSERT - Check dat account nu active is
            Assert.IsType<OkObjectResult>(result.Result); // Eerst checken dat login geslaagd is

            var accountInDb = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Username == "inactiveuser");

            Assert.NotNull(accountInDb);
            Assert.True(accountInDb.IsActive); // Moet nu true zijn
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}