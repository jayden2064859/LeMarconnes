using LeMarconnes.Models;

namespace LeMarconnes.Models
{
    public class Tarrif
    {
        public int TarrifId { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }

        // navigation property 
        // een specifiek tarief is gekoppeld aan een accommodatietype
        public AccommodationType AccommodationType { get; set; }

        // waarom? dit zorgt ervoor dat je straks in de database op een specifiek accommodatietype kan zoeken (bijv Camping, Hotel, etc)
        // en van alle accommodaties binnen dat type de individuele tarieven kan bekijken
    }
}