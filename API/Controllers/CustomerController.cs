using Azure.Core;
using ClassLibrary.Data;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CampingDbContext _context;

        public CustomerController(CampingDbContext context)
        {
            _context = context;
        }

        // GET: api/customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers
            .ToListAsync();
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

        [HttpPost]


        // [FromBody] Customer customer betekent: Haal de data uit de HTTP request body en zet die om naar een C# object.
        public async Task<ActionResult<Customer>> PostCustomer([FromBody] Customer customer)
        {

            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
                return BadRequest("Email is al geregistreerd");

            if (await _context.Customers.AnyAsync(c => c.Phone == customer.Phone))
                return BadRequest("Telefoonnummer is al geregistreerd");


            // er bestaat dus al een customer object, en die kunnen we direct gebruiken om een nieuwe customer aan te maken met de custom constructor 
            var newCustomer = new Customer(
            customer.FirstName,
            customer.LastName,
            customer.Email,
            customer.Phone,
            customer.Infix
            );

           _context.Customers.Add(newCustomer);
           await _context.SaveChangesAsync();

           return CreatedAtAction("GetCustomer",new { id = newCustomer.CustomerId }, newCustomer);                        
        }

        // PUT: api/customer
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return BadRequest();
            }

            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // bool voor PUT method
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
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

            return NoContent();
        }


    }
}
