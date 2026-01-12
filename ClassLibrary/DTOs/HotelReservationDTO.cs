using System.ComponentModel.DataAnnotations;


namespace ClassLibrary.DTOs
{
    // MVC controller gebruikt deze dto om de ingevoerde user data naar de API te sturen.
    // (MVC controller ontvangt user data -> MVC gebruikt DTO en stopt user data erin -> MVC gebruikt ReservationService method
    // om het naar de API te sturen -> API Controller endpoint gebruikt deze dto als parameter en ontvangt de user data ->
    // API Controller gebruikt de dto data om er met de class constructor een object van te maken, en voegt het toe aan de DB.
    public class HotelReservationDTO
    {
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public List<int> AccommodationIds { get; set; } = new List<int>();
        [Required]
        public int PersonCount { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
    }
}
