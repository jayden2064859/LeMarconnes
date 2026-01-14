
namespace ClassLibrary.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }

        public AccommodationType Type { get; set; }
        public enum AccommodationType
        {
            Camping = 1,
            Hotel = 2
        }      
    }
}