using ClassLibrary.Models;

namespace ClassLibrary.ViewModels
{
    public class CreateReservation2ViewModel
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public List<Accommodation> AvailableAccommodations { get; set; } = new();
    }
}
