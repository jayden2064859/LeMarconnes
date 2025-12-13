using LeMarconnes.Models;

namespace LeMarconnes.Models
{
    public class AccommodationType
    {
        public int AccommodationTypeId { get; set; }
        public string Name { get; set; }

        // navigation properties
        // een accommodationtype heeft een lijst van meerdere accommodaties
        public List<Accommodation> Accommodations { get; set; } = new List<Accommodation>();
        // een accommodatietype heeft voor elk accommodatie in de lijst een tarief
        public List<Tarrif> Tarrifs { get; set; } = new List<Tarrif>();
    }
}