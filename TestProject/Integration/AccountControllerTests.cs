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
    public class AccountControllerIntegrationTests : IDisposable
    {
        private readonly CampingDbContext _context;
        private readonly AccountController _controller;

        public AccountControllerIntegrationTests()
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
            _controller = new AccountController(_context);

            CreateCustomer();
        }

        private void CreateCustomer()
        {
            var customer = new Customer(
                "Test",
                "Customer",
                "test.customer@example.com",
                "0612345678"
            );
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        // ITC-07: Succesvolle account creatie
        [Fact]
        public async Task PostAccount_ValidData_CreatesAccount()
        {
            // ARRANGE
            var customer = await _context.Customers.FirstAsync();
            var dto = new CreateAccountDTO
            {
                CustomerId = customer.CustomerId,
                Username = "testuser",
                PlainPassword = "Test123!"
            };

            // ACT
            var result = await _controller.PostAccount(dto);

            // ASSERT
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var account = Assert.IsType<Account>(okResult.Value);

            Assert.Equal("testuser", account.Username);
            Assert.NotNull(account.PasswordHash);
            Assert.NotEmpty(account.PasswordHash);
            Assert.Equal(customer.CustomerId, account.CustomerId);
            Assert.Equal(Account.Role.Customer, account.AccountRole);
        }

        // ITC-08: Account aanmaken met niet-bestaande customer
        [Fact]
        public async Task PostAccount_NonExistentCustomer_ReturnsConflict()
        {
            // ARRANGE
            var dto = new CreateAccountDTO
            {
                CustomerId = 999, // Niet-bestaande customer
                Username = "testuser",
                PlainPassword = "Test123!"
            };

            // ACT
            var result = await _controller.PostAccount(dto);

            // ASSERT
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Contains("Customer bestaat niet", notFoundResult.Value.ToString());
        }

        // ITC-09: Username check - bestaande username
        [Fact]
        public async Task CheckUsernameExists_ExistingUsername_ReturnsOk()
        {
            // ARRANGE - Account aanmaken via DB
            var customer = await _context.Customers.FirstAsync();
            var account = new Account(
                "existinguser",
                "hashedpassword123",
                customer
            );
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _controller.CheckUsernameExists("existinguser");

            // ASSERT
            Assert.IsType<OkResult>(result);
        }

        // ITC-10: Username check - niet-bestaande username
        [Fact]
        public async Task CheckUsernameExists_NonExistingUsername_ReturnsNotFound()
        {
            // ACT
            var result = await _controller.CheckUsernameExists("nonexistinguser");

            // ASSERT
            Assert.IsType<NotFoundResult>(result);
        }

        // ITC-11: Account ophalen met ID
        [Fact]
        public async Task GetAccount_ValidId_ReturnsAccount()
        {
            // ARRANGE - Account aanmaken via DB
            var customer = await _context.Customers.FirstAsync();
            var account = new Account(
                "testuser",
                "hashedpassword123",
                customer
            );
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _controller.GetAccount(account.AccountId);

            // ASSERT
            var actionResult = Assert.IsType<ActionResult<Account>>(result);
            var returnedAccount = Assert.IsType<Account>(actionResult.Value);
            Assert.Equal(account.AccountId, returnedAccount.AccountId);
            Assert.Equal("testuser", returnedAccount.Username);
        }

        // ITC-12: Alle accounts ophalen
        [Fact]
        public async Task GetAccounts_ReturnsAllAccounts()
        {
            // ARRANGE - Meerdere accounts aanmaken via DB
            var customer = await _context.Customers.FirstAsync();

            var accounts = new[]
            {
                new Account("user1", "hash1", customer),
                new Account("user2", "hash2", customer),
                new Account("user3", "hash3", customer)
            };

            foreach (var acc in accounts)
            {
                _context.Accounts.Add(acc);
            }
            await _context.SaveChangesAsync();

            // ACT
            var result = await _controller.GetAccounts();

            // ASSERT
            var actionResult = Assert.IsType<ActionResult<List<Account>>>(result);
            var accountList = Assert.IsType<List<Account>>(actionResult.Value);
            Assert.Equal(3, accountList.Count);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}