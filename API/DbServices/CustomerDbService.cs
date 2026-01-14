using ClassLibrary.Data;
using ClassLibrary.DTOs;
using ClassLibrary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.DbServices
{
    public class CustomerDbService
    {

        private readonly LeMarconnesDbContext _context;

        public CustomerDbService(LeMarconnesDbContext context)
        {
            _context = context;
        }

        // voor GET /api/customer/{customerId}/reservations
        public async Task<(List<Reservation>? reservations, string? error)> GetCustomerReservationsAsync(int customerId)
        {
            var customerReservations = await _context.Reservations
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Accommodations)
                .ToListAsync();

            if (!customerReservations.Any())
            {
                string error = "Geen reserveringen gevonden";
                return (null, error);
            }

            return (customerReservations, null);
        }

        // voor GET /api/customer (alle)
        public async Task<(List<Customer>? customers, string? error)> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .ToListAsync();

            if (!customers.Any())
            {
                string error = "Geen customers gevonden";
                return (null, error);
            }
            return (customers, null);
        }

        public async Task<(Customer? customer, string? error)> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                string error = $"Customer met id {customerId} niet gevonden";
                return (null, error);
            }

            return (customer, null);
        }

        // voor POST /api/customer
        public async Task<(Customer? customer, string? error)> PostCustomerAsync(CustomerDTO dto)
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
                    string error = "Email is al geregistreerd";
                    return (null, error);
                }

                if (customerExists.Phone == dto.Phone)
                {
                    string error = "Telefoonnummer is al geregistreerd";
                    return (null, error);
                }
            }

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

                return (newCustomer, null);          
        }

        // voor PUT /api/customer
        public async Task<(bool success, string? error)> UpdateCustomerAsync(int id, Customer customer)
        {
            // ingevoerde id moet overeenkomen met het customerId in het nieuwe customer object 
            if (id != customer.CustomerId)
            {
                string error = "Customer ID mismatch";
                return (false, error);
            }

            // customer ophalen uit db
            var customerExists = await _context.Customers.AnyAsync(c => c.CustomerId == id);

            if (!customerExists)
            {
                string error = $"Customer met id {id} niet gevonden";
                return (false, error);
            }

            _context.Entry(customer).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return (true, null);
        }


        // voor DELETE /api/customer
        public async Task<(bool success, string? error)> DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return (false, $"Customer met id {id} niet gevonden");
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return (true, null);
        }
    }
}

