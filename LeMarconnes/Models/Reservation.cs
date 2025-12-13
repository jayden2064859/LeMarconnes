using LeMarconnes.Models;

namespace LeMarconnes.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AdultsCount { get; set; }
        public int Children0_7Count { get; set; }
        public int Children7_12Count { get; set; }
        public int DogsCount { get; set; }
        public bool HasElectricity { get; set; }
        public int? ElectricityDays { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Gereserveerd"; 
        public DateTime RegistrationDate { get; set; }

        // foreign keys voor DB access
        public int CustomerId { get; set; }
        public int AccommodationId { get; set; }

        // navigation properties
        // elke reservering is gekoppeld aan 1 klant
        public Customer Customer { get; set; }

        // elke reservering is gekoppeld aan een specifieke accommodatie
        public Accommodation Accommodation { get; set; }


        // methods
        public void GetNumberOfNights() // wordt int
        {

        }

        public void Cancel()
        {

        }

        public void Validate() // wordt bool
        {

        }

    }
}