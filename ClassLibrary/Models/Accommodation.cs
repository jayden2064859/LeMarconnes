using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }
        public AccommodationStatus CurrentStatus { get; set; } = AccommodationStatus.Beschikbaar; // nieuwe accommodations initialiseren op beschikbaar
        public enum AccommodationStatus
        {
            Beschikbaar = 0,
            Bezet = 1
        }
        public int AccommodationTypeId { get; set; }
        public AccommodationType AccommodationType { get; set; }
        public List<Reservation>Reservations { get; set; } = new List<Reservation>();

           
        // methods
        public void AddReservation(Reservation reservation)
        {
            Reservations.Add(reservation);
        }
      
        public void CheckCapacity(int totalPersons)
        {
            // niet meer personen dan capaciteitslimiet van accommodatie
           
        }
    }
}