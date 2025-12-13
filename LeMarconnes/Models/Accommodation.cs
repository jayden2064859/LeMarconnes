using LeMarconnes.Models;

namespace LeMarconnes.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }
        public string Status { get; set; }
        public AccommodationType AccommodationType { get; set; }


        // methods
        public void IsAvailable() // wordt bool
        {

        }
    }
}