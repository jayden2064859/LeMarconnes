using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = "Beschikbaar"; // initialiseren op beschikbaar
        public int AccommodationTypeId { get; set; }
        public AccommodationType AccommodationType { get; set; }
        public List<Reservation>Reservations { get; set; } = new List<Reservation>();

           
        // methods
        public void IsAvailable() // wordt bool
        {

        }

      
        public void CheckCapacity(int totalPersons)
        {
            // niet meer personen dan capaciteitslimiet van accommodatie
           
        }
    }
}