
namespace ClassLibrary.DTOs
{
    // response DTO die de API controller endpoint aanmaakt en terugstuurt naar de Service die de endpoint gecalled heeft
    // Als de POST succesvol is, stuurt de service de response DTO terug naar de MVC (anders error message), en kan de MVC controller
    // de responseDTO gebruiken voor de bevestigingspagina.
    public class HotelReservationResponseDTO
    {
        public string FirstName { get; set; }
        public string? Infix { get; set; }
        public string LastName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PersonCount { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string> AccommodationPlaceNumbers { get; set; } = new List<string>();
    }
}
