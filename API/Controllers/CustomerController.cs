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

        // POST: api/customer - customer aanmaken
        [Authorize(Roles = "Admin")] // alleen admin mag individuele customers aanmaken. normale gebruikers doen dit via de registration endpoint
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

        // GET: api/customer - alle customers ophalen
        [Authorize(Roles = "Admin,Employee")] // medewerkers mogen customer info inzien
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

        // GET: api/customer/{id} - specifieke customer ophalen
        [Authorize(Roles = "Admin,Employee")]
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

        // GET: api/customer/{id}/reservations - reserveringen van specifieke customer ophalen
        [Authorize(Roles = "Admin,Employee")]
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

        // PUT: api/customer/{id} - gegevens van specifieke customer bewerken
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<Customer>> PutCustomer(int id, UpdateCustomerDTO dto)
        {
            var (updatedCustomer, notFoundMsg) = await _dbService.UpdateCustomerAsync(id, dto);

            if (updatedCustomer == null)
            {
                return NotFound(notFoundMsg);
            }
            return Ok(updatedCustomer);
        }

        [HttpPatch("{customerId}")]
        public async Task<IActionResult> PatchCustomer(int customerId, PatchCustomerDTO dto)
        {
            var (customer, error) = await _dbService.PatchCustomerAsync(customerId, dto);

            if (error != null)
                return BadRequest(error);

            return Ok(customer);
        }

        // DELETE: api/customer/{id} - specifieke customer verwijderen
        [Authorize(Roles = "Admin")]
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
