
namespace ClassLibrary.Models
{
    public class Tariff
    {
        public int TariffId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        // fk 
        public int AccommodationTypeId { get; set; }
        // navigation property - elk tarief hoort bij een type (camping of hotel)
        public AccommodationType AccommodationType { get; set; }

    }
}