using ClassLibrary.Models;

namespace ClassLibrary.DTOs
{
    // deze dto is specifiek om het als return type voor de accommodation/available-for-dates endpoint te gebruiken
    // zonder een dto geeft de API alle gelinkte data van een accommodation terug (zoals gekoppelde reserveringen), wat niet nodig is en een security probleem kan zijn
    public class AvailableAccommodationDTO
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }
        public Accommodation.AccommodationType Type { get; set; }
    }
}
