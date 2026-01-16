
namespace ClassLibrary.Models
{
    public class Accommodation
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }

        // fk naar AccommodationType
        public int AccommodationTypeId { get; set; }  
    }
}