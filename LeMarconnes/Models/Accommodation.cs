using LeMarconnes.Models;
using Microsoft.EntityFrameworkCore;

namespace LeMarconnes.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; } = "Beschikbaar"; // initialiseren op beschikbaar
        public AccommodationType AccommodationType { get; set; }
        public List<Reservation>Reservations { get; set; }

        // foreign key 
        public int AccommodationTypeId { get; set; }
        // methods
        public void IsAvailable() // wordt bool
        {

        }

      
        public void CheckCapacity(int totalPersons)
        {
            // niet meer personen dan capaciteitslimiet van accommodatie
            //(AdultsCount + Children0_7Count + Children7_12Count) <= Accommodation.Capacity");
        }
    }
}