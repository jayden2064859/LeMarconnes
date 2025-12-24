using LeMarconnes.Models;
using LeMarconnes.Services;
using Microsoft.Identity.Client;
using System.Text.Json.Serialization;

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
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }

        // foreign keys voor DB access
        public int CustomerId { get; set; }
        public int AccommodationId { get; set; }

        // navigation properties (relaties in klassendiagram)
        [JsonIgnore]
        public Customer Customer { get; set; }  // elke reservering is gekoppeld aan 1 klant
        [JsonIgnore]
        public Accommodation Accommodation { get; set; }  // elke reservering is gekoppeld aan een specifieke accommodatie


        // constructors
        public Reservation() // lege constructor voor EF om objecten aan te maken
        {
            
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