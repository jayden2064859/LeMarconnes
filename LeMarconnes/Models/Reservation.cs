using LeMarconnes.Models;
using Microsoft.Identity.Client;

namespace LeMarconnes.Models
{
    public class Reservation
    {

        // class properties
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
        public Customer Customer { get; set; }  // elke reservering is gekoppeld aan 1 klant
        public Accommodation Accommodation { get; set; }  // elke reservering is gekoppeld aan een specifieke accommodatie


        // constructor
        public Reservation(Customer customer, Accommodation accommodation, DateTime startDate, DateTime endDate, int adultsCount, 
            int children0_7Count, int children7_12Count, int dogsCount, bool hasElectricity, int? electricityDays)
        {

            StartDate = startDate;
            EndDate = endDate;
            AdultsCount = adultsCount;
            Children0_7Count = children0_7Count;
            Children7_12Count = children7_12Count;
            DogsCount = dogsCount;
            HasElectricity = hasElectricity;
            ElectricityDays = hasElectricity ? electricityDays : null; // wordt automatisch null als hasElectricity false is

            Customer = customer;
            CustomerId = customer.CustomerId; //fk
            Accommodation = accommodation;
            AccommodationId = accommodation.AccommodationId; //fk
            

            // default waarden
            Status = "Gereserveerd";
            RegistrationDate = DateTime.Now; 
        }


        // methods
        public int GetNumberOfNights() 
        {
            return (EndDate - StartDate).Days;
        }

        public void Cancel()
        {

        }

        public void Validate() // wordt bool
        {

        }

    }
}