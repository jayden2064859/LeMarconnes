using Azure.Core;
using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly LeMarconnesDbContext _context;

        public CustomerController(LeMarconnesDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(CustomerDTO dto)
        {
            // valideren of email en telefoonnummer al bestaan in database
            var customerExists = await _context.Customers
                .Where(c => c.Email == dto.Email || c.Phone == dto.Phone)
                .FirstOrDefaultAsync();

            if (customerExists != null)
            {
                // conflict = voor wanneer de input qua syntax correct is, maar business rules het alsnog niet toelaten
                if (customerExists.Email == dto.Email)
                {
                    return Conflict("Email is al geregistreerd");
                }
               
                if (customerExists.Phone == dto.Phone)
                {
                    return Conflict("Telefoonnummer is al geregistreerd");
                }                
            }
            try
            {


                // constructor in customer class gebruiken om object aan te maken
                var newCustomer = new Customer(
                    dto.FirstName,
                    dto.LastName,
                    dto.Email,
                    dto.Phone,
                    dto.Infix
                );

                _context.Customers.Add(newCustomer);
                await _context.SaveChangesAsync();

                return Ok(newCustomer);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }





        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetCustomers()
        {
            var customers = await _context.Customers
            .ToListAsync();

            return customers;
        }

        // GET: api/customer/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Reservations) // reserveringen meegeven van een specifieke klant
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/customer
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            _context.Entry(customer).State = EntityState.Modified;
 
            await _context.SaveChangesAsync();


            if (!CustomerExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // bool voor PUT method
        private bool CustomerExists(int id)
        {
            var exits = _context.Customers.Any(e => e.CustomerId == id);

            return exits;
        }

        // DELETE: api/customer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok();
        }


    }
}
