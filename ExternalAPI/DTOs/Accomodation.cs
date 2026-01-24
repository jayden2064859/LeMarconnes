namespace ExternalAPI.Classes
{
    public class AccomodationDTO
    {

        public string name { get; set; }
        public string type { get; set; }
        public int maxPerson { get; set; }
        public string description { get; set; }
        public bool isAvailable { get; set; }
    }


    public class AccomodationResponseDTO
    {
        public int accomodationId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int maxPerson { get; set; }
        public string description { get; set; }
        public bool isAvailable { get; set; }
    }

}
