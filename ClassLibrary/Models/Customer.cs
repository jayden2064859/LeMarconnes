using ClassLibrary.Models;

namespace ClassLibrary.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string FirstName { get; set; }
        public string? Infix { get; set; } = string.Empty;
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime RegistrationDate { get; set; }

        // navigation properties
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();


        // constructors
        public Customer(string firstName, string lastName, string email, string phone, string? infix = null)
        {

            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Voornaam is verplicht", nameof(firstName));

            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Achternaam is verplicht", nameof(lastName));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is verplicht", nameof(email));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Telefoonnummer is verplicht", nameof(phone));


            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Phone = phone;
            Infix = infix;
            RegistrationDate = DateTime.Now;
        }
        public Customer()
        {
        }

        // methods
        public void HasOverlappingReservations() // wordt bool
        {

        }

        public List<Reservation> GetAllReservations()
        {
            return Reservations.ToList();
        }
        public List<Reservation> GetActiveReservations() 
        {
            return Reservations.Where(r => r.Status == "Gereserveerd").ToList();
        }

        public List<Reservation> GetPastReservations()
        {
            return Reservations.Where(r => r.Status == "Verlopen").ToList();
        }

    }
}