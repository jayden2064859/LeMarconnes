using API.DbServices;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CustomerDbService _dbService;

        public CustomerController(CustomerDbService dbService)
        {
            _dbService = dbService;
        }

        // POST: api/customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO dto)
        {
            var (customer, error) = await _dbService.PostCustomerAsync(dto);

            if (error != null)
            {
                return Conflict(error);
            }

            return Ok(customer);
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            var (customers, error) = await _dbService.GetAllCustomersAsync();

            if (error != null)
            {
                return Conflict(error);
            }

            return Ok(customers); 
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var (customer, error) = await _dbService.GetCustomerByIdAsync(id);

            if (error != null)
            {
                return NotFound(error);
            }

            return Ok(customer);
        }

        // GET: api/customer/{id}/reservations
        [HttpGet("{id}/reservations")]
        public async Task<ActionResult<List<Reservation>>> GetCustomerReservations(int id)
        {
            var (reservations, error) = await _dbService.GetCustomerReservationsAsync(id);

            if (error != null)
            {
                return NotFound(error);
            }

            return Ok(reservations);
        }

        // PUT: api/customer/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            var (success, error) = await _dbService.UpdateCustomerAsync(id, customer);

            if (!success)
            {
                return NotFound(error);
            }
            return Ok("Gegevens succesvol gewijzigd");
        }

        // DELETE: api/customer/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var (success, error) = await _dbService.DeleteCustomerAsync(id);

            if (!success)
            {
                return NotFound(error);
            }

            return Ok("Customer verwijderd");
        }
    }
}
