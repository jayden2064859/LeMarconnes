using System.ComponentModel.DataAnnotations;


namespace ClassLibrary.DTOs
{
    public class CampingReservationDTO
    {
        [Required]
        public int CustomerId { get; set; } // customerId is nodig om reservation aan customer te linken
        [Required]
        public List<int> AccommodationIds { get; set; } = new List<int>(); // gereserveerde accommodaties moet gekoppeld worden 
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public int AdultsCount { get; set; }
        [Required]
        public int Children0_7Count { get; set; }
        [Required]
        public int Children7_12Count { get; set; }
        [Required]
        public int DogsCount { get; set; }
        [Required]
        public bool HasElectricity { get; set; }

        public int? ElectricityDays { get; set; }
    }
}
