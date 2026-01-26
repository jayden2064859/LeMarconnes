// Yassir
namespace ClassLibrary.ViewModels
{
    public class HotelReservationConfirmationViewModel
    {
        // Klantgegevens
        public string FirstName { get; set; }
        public string? Infix { get; set; }
        public string LastName { get; set; }

        // Reserveringsdata
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal TotalPrice { get; set; }

        // Specifiek voor hotel
        public int PersonCount { get; set; }

        // Lijst met kamernummers
        public List<string> AccommodationPlaceNumbers { get; set; } = new();
    }
}