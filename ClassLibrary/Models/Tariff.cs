using ClassLibrary.Models;
using static ClassLibrary.Models.Accommodation;

namespace ClassLibrary.Models
{
    public class Tariff
    {
        public int TariffId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        // navigation property 
        public Accommodation.AccommodationType AccommodationType { get; set; }

    }
}