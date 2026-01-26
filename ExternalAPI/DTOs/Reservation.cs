namespace ExternalAPI.Classes
{
    public class ReservationDTO
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public int accomodationId { get; set; }
        public DateTime checkIn { get; set; }
        public DateTime checkOut { get; set; }
        public int numberOfGuests { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
    }

    public class ReservationResponseDTO
    {
        public int id { get; set; }
        public int guestId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public int accomodationId { get; set; }
        public DateTime checkIn { get; set; }
        public DateTime checkOut { get; set; }
        public int numberOfGuests { get; set; }
        public string status { get; set; }
        public string remarks { get; set; }
    }

}


