using LeMarconnes.Models;

namespace LeMarconnes.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string? Infix { get; set; } = string.Empty;
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime RegistrationDate { get; set; }

        // navigation property
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        // methods
        public void HasOverlappingReservations() // wordt bool
        {

        }

        public void GetActiveReservations() // wordt List<Reservation>
        {

        }
    }
}