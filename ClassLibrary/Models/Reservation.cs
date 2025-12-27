using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models
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
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }

      
        // navigation properties (relaties in klassendiagram)
        public Customer Customer { get; set; }  // elke reservering is gekoppeld aan 1 klant
        public int CustomerId { get; set; }
        public List<Accommodation> Accommodations { get; set; } = new List<Accommodation>(); // elke reservering is gekoppeld aan specifieke accommodaties (minimaal 1, mogelijk meer)

        // constructors
        public Reservation() // parameterloze constructor (nodig voor EF)
        {            
            Status = "Gereserveerd";
            RegistrationDate = DateTime.Now;
        }
        
        // main constructor voor aanmaken van reservering 
        public Reservation(int customerId, DateTime startDate, DateTime endDate,  int adultsCount, 
            int children0_7Count, int children7_12Count, int dogsCount, bool hasElectricity, int? electricityDays = null)
        {
            if (endDate <= startDate)           
            throw new ArgumentException("Einddatum moet na startdatum zijn");
            

            if (adultsCount < 1)           
            throw new ArgumentException("Er moet minimaal 1 volwassene zijn");
            

            if (customerId <= 0)
            throw new ArgumentException("Invalid customer");

            CustomerId = customerId;
            StartDate = startDate;
            EndDate = endDate;
            AdultsCount = adultsCount;
            Children0_7Count = children0_7Count;
            Children7_12Count = children7_12Count;
            DogsCount = dogsCount;
            HasElectricity = hasElectricity;
            ElectricityDays = electricityDays;

            Status = "Gereserveerd";
            RegistrationDate = DateTime.Now;
        
        }

        // methods
        public int GetNumberOfNights() 
        {
            return (EndDate - StartDate).Days;
        }

        public void AddAccommodation(Accommodation accommodation)
        {
            Accommodations.Add(accommodation);
        }

        public void Cancel()
        {

        }

        public void Validate() // wordt bool
        {

        }

    }
}