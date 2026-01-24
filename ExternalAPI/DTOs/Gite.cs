namespace ExternalAPI.Classes
{
    public class GiteDTO
    {
        public string name { get; set; }
        public int maxPerson { get; set; }
        public string description { get; set; }
        public bool isAvailable { get; set; }
        public bool hasSleepingSpots { get; set; }
        public Sleepingspot[] sleepingSpots { get; set; }

        public class Sleepingspot
        {
            public string code { get; set; }
            public string type { get; set; }
            public bool isAvailable { get; set; }
        }
    }

    public class GiteResponseDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int maxPerson { get; set; }
        public string description { get; set; }
        public bool isAvailable { get; set; }
        public bool hasSleepingSpots { get; set; }
        public Sleepingspot[] sleepingSpots { get; set; }

        public class Sleepingspot
        {
            public int id { get; set; }
            public string code { get; set; }
            public string type { get; set; }
            public bool isAvailable { get; set; }
        }
    }
}
