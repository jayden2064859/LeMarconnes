namespace ClassLibrary.DTOs
{
    public class CampingReservationResponseDTO
    {
        public string FirstName { get; set; }
        public string? Infix { get; set; }
        public string LastName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int AdultsCount { get; set; }
        public int Children0_7Count { get; set; }
        public int Children7_12Count { get; set; }
        public int DogsCount { get; set; }
        public bool HasElectricity { get; set; }
        public int? ElectricityDays { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string> AccommodationPlaceNumbers { get; set; } = new List<string>();
    }
}