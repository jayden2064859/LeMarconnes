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
        public ReservationStatus CurrentStatus { get; set; } = Reservation.ReservationStatus.Gereserveerd; // initialiseer nieuwe reserveringen op 'gereserveerd'

        public enum ReservationStatus
        {
            Gereserveerd = 0,
            Actief = 1,
            Verlopen = 2
        }
        public DateTime RegistrationDate { get; set; }

        // customer object is nodig als navigation property
        public Customer Customer { get; set; }  

        // customerId is nodig voor DB queries
        public int CustomerId { get; set; }
        public List<Accommodation> Accommodations { get; set; } = new List<Accommodation>(); 

        // constructors
        public Reservation() // parameterloze constructor (nodig voor EF)
        {            
        }
        
        // main constructor voor aanmaken van reservering 
        public Reservation(int customerId, DateTime startDate, DateTime endDate,  int adultsCount, 
            int children0_7Count, int children7_12Count, int dogsCount, bool hasElectricity, int? electricityDays = null)
        {

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

            CurrentStatus = Reservation.ReservationStatus.Gereserveerd;
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
    }
}