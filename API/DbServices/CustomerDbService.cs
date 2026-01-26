using API.Data;
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
                return (null, "Geen reserveringen gevonden");
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
                return (null, "Geen customers gevonden");
            }
            return (customers, null);
        }

        public async Task<(Customer? customer, string? error)> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (customer == null)
            {
                return (null, $"Customer met id {customerId} niet gevonden");
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
                    return (null, "Email is al geregistreerd");
                }

                if (customerExists.Phone == dto.Phone)
                {
                    return (null, "Telefoonnummer is al geregistreerd");
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
        public async Task<(Customer? customer, string? error)> UpdateCustomerAsync(int customerId, UpdateCustomerDTO dto)
        {
            // valideren of email al registreerd is         
            var emailConflictMsg = await ValidateEmailAsync(dto.Email, customerId);
            if (emailConflictMsg != null)
            {
                throw new ArgumentException(emailConflictMsg);
            }

            // valideren of telefoonnummer al registreerd is         
            var phoneConflictMsg = await ValidatePhoneAsync(dto.Email, customerId);
            if (phoneConflictMsg != null)
            {
                throw new ArgumentException(phoneConflictMsg);
            }

            // customer ophalen uit db
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            
            if (customer == null)
            {
                return (null, $"Klant met id {customerId} niet gevonden");
            }

            // update customer met de nieuwe input van de dto
            customer.FirstName = dto.FirstName;
            customer.Infix = dto.Infix;
            customer.LastName = dto.LastName;
            customer.Email = dto.Email;
            customer.Phone = dto.Phone;

            // wijzigingen opslaan 
            await _context.SaveChangesAsync();

            // customer ophalen na wijzigingen om terug te laten zien aan de client
            var updatedCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
            // hier zou beter ook een response DTO kunnen worden gebruikt om niet de volledige interne data te tonen,
            // maar omdat deze endpoint alleen gebruikt wordt door de admin, kan het hier wel zonder groot risico

            return (updatedCustomer, null);
        }


        // valideren of email en telefoonnummer al bestaan in database bij andere klanten
        private async Task<string?> ValidateEmailAsync(string email, int customerId)
        {
            var emailExists = await _context.Customers
                .AnyAsync(c => c.Email == email && c.CustomerId != customerId);
            
            if (emailExists)
            {
                return "Email is al geregistreerd";
            }
            return null; 

        }
        private async Task<string?> ValidatePhoneAsync(string phone, int customerId)
        {
            var phoneExists = await _context.Customers
                .AnyAsync(c => c.Phone == phone && c.CustomerId != customerId);

            if (phoneExists)
            {                
                return "Telefoonnummer is al geregistreerd";
            }
            return null; 
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

