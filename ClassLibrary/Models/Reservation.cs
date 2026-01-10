using ClassLibrary.Models;
using ClassLibrary.Services;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ClassLibrary.Models
{
    // reservation is de base class die properties bevat die relevant zijn voor ALLE type reserveringen,
    // en de subtypes moeten hun eigen logica zelf implementeren.
    public abstract class Reservation // reservation is abstract omdat camping en hotel het moeten inheriten
    {

        // class properties
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime RegistrationDate { get; set; }

        // foreign keys/navigation properties
        public Customer Customer { get; set; }  

        public int CustomerId { get; set; }
        public List<Accommodation> Accommodations { get; set; } = new List<Accommodation>(); 
        public int AccommodationTypeId { get; set; }

        // constructors
        public Reservation() // parameterloze constructor (nodig voor EF)
        {            
        }
        
        // main constructor voor aanmaken van reservering 
        public Reservation(int customerId, DateTime startDate, DateTime endDate)
        {
            if (customerId <= 0) 
            { 
                throw new ArgumentException("Invalid customer"); 
            }

            if (startDate < DateTime.Today)
            {
                throw new ArgumentException("Startdatum moet minimaal vandaag zijn");

            }

            if (endDate <= startDate)
            {
                throw new ArgumentException("Einddatum moet later zijn dan startdatum");
            }
               

            if (endDate < DateTime.Today.AddDays(1))
            {
                throw new ArgumentException("Einddatum moet minimaal morgen zijn");
            }
             
            CustomerId = customerId;
            StartDate = startDate;
            EndDate = endDate;
            RegistrationDate = DateTime.Now;
        
        }

        // methods
        public int GetNumberOfNights() 
        {
            return (EndDate - StartDate).Days;
        }

        public void AddAccommodation(Accommodation accommodation)
        {
            if (accommodation == null)
            {
                throw new ArgumentException("Geen accommodatie gevonden om toe te voegen");
            }
            if (Accommodations.Count > 2)
            {
                throw new ArgumentException("Maximaal 2 accommodaties per reservering");
            }
            Accommodations.Add(accommodation);
        }

        // elke soort reservering heeft een eigen total price calculator nodig met zijn eigen implementatie ervan
        public abstract decimal CalculatePrice(List<Tariff> tariffs);
    }
}