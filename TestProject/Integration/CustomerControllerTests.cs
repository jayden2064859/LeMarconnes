using API.Controllers;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace TestProject.Integration
{
    public class CustomerControllerIntegrationTests : IDisposable
    {
        private readonly CampingDbContext _context;
        private readonly CustomerController _controller;

        public CustomerControllerIntegrationTests()
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
            _controller = new CustomerController(_context);
        }

        // ITC-01: Succesvolle customer creatie
        [Fact]
        public async Task PostCustomer_ValidData_ReturnsCreatedCustomer()
        {
            // ARRANGE
            var dto = new CreateCustomerDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "0612345678",
                Infix = "van"
            };

            // ACT
            var result = await _controller.PostCustomer(dto);

            // ASSERT - Check het type van de response
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var customer = Assert.IsType<ClassLibrary.Models.Customer>(okResult.Value);

            Assert.Equal("John", customer.FirstName);
            Assert.Equal("Doe", customer.LastName);
            Assert.Equal("john.doe@example.com", customer.Email);
            Assert.Equal("0612345678", customer.Phone);
            Assert.Equal("van", customer.Infix);
        }

        // ITC-02: Duplicate email moet BadRequest geven
        [Fact]
        public async Task PostCustomer_DuplicateEmail_ReturnsConflict()
        {
            // ARRANGE - Eerste customer aanmaken
            var firstDto = new CreateCustomerDTO
            {
                FirstName = "First",
                LastName = "Customer",
                Email = "duplicate@example.com",
                Phone = "0611111111"
            };
            await _controller.PostCustomer(firstDto);

            var secondDto = new CreateCustomerDTO
            {
                FirstName = "Second",
                LastName = "Customer",
                Email = "duplicate@example.com", 
                Phone = "0622222222"
            };

            // ACT
            var result = await _controller.PostCustomer(secondDto);

            // ASSERT
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Contains("Email is al geregistreerd", conflictResult.Value.ToString());
        }

        // ITC-03: Duplicate telefoonnummer moet conflict geven
        [Fact]
        public async Task PostCustomer_DuplicatePhone_ReturnsConflict()
        {
            // ARRANGE - Eerste customer aanmaken
            var firstDto = new CreateCustomerDTO
            {
                FirstName = "First",
                LastName = "Customer",
                Email = "first@example.com",
                Phone = "0612345678"
            };
            await _controller.PostCustomer(firstDto);

            // Tweede customer metzelfde telefoonnummer
            var secondDto = new CreateCustomerDTO
            {
                FirstName = "Second",
                LastName = "Customer",
                Email = "second@example.com",
                Phone = "0612345678" // Zelfde telefoonnummer!
            };

            // ACT 
            var result = await _controller.PostCustomer(secondDto);

            // ASSERT
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
            Assert.Contains("Telefoonnummer is al geregistreerd", conflictResult.Value.ToString());
        }

        // ITC-04: Customer ophalen met ID
        [Fact]
        public async Task GetCustomer_ValidId_ReturnsCustomer()
        {
            // ARRANGE - Customer aanmaken via directe DB voor meer controle
            var customer = new ClassLibrary.Models.Customer(
                "Test",
                "User",
                "test@example.com",
                "0612345678"
            );
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // ACT
            var result = await _controller.GetCustomer(customer.CustomerId);

            // ASSERT
            var okResult = Assert.IsType<ActionResult<ClassLibrary.Models.Customer>>(result);
            var returnedCustomer = Assert.IsType<ClassLibrary.Models.Customer>(okResult.Value);
            Assert.Equal(customer.CustomerId, returnedCustomer.CustomerId);
            Assert.Equal("Test", returnedCustomer.FirstName);
            Assert.Equal("User", returnedCustomer.LastName);
        }

        // ITC-05: Niet-bestaande customer ophalen moet NotFound geven
        [Fact]
        public async Task GetCustomer_InvalidId_ReturnsNotFound()
        {
            // ACT
            var result = await _controller.GetCustomer(999); // Niet-bestaand ID

            // ASSERT
            Assert.IsType<NotFoundResult>(result.Result);
        }

        // ITC-06: Alle customers ophalen
        [Fact]
        public async Task GetCustomers_ReturnsAllCustomers()
        {
            // ARRANGE - Meerdere customers aanmaken via directe DB
            var customers = new[]
            {
                new ClassLibrary.Models.Customer("John", "Doe", "john@example.com", "0611111111"),
                new ClassLibrary.Models.Customer("Jane", "Smith", "jane@example.com", "0622222222"),
                new ClassLibrary.Models.Customer("Bob", "Johnson", "bob@example.com", "0633333333")
            };

            foreach (var customer in customers)
            {
                _context.Customers.Add(customer);
            }
            await _context.SaveChangesAsync();

            // ACT
            var result = await _controller.GetCustomers();

            // ASSERT
            var actionResult = Assert.IsType<ActionResult<List<ClassLibrary.Models.Customer>>>(result);
            var customerList = Assert.IsType<List<ClassLibrary.Models.Customer>>(actionResult.Value);
            Assert.Equal(3, customerList.Count);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}