using ClassLibrary.Models;
using System.Text.Json.Serialization;

namespace ClassLibrary.DTOs
{
    // deze dto is specifiek om het als return type voor de accommodation/available-for-dates endpoint te gebruiken
    // zonder een dto geeft de API alle gelinkte data van een accommodation terug (zoals gekoppelde reserveringen), wat niet nodig is en een security probleem kan zijn
    public class AvailableForDatesResponseDTO
    {
        public int AccommodationId { get; set; }
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }

        [JsonIgnore]
        public int AccommodationTypeId { get; set; }

        public string AccommodationTypeName 
        {
            get
            {
                if (AccommodationTypeId == 1)
                {
                    return "Camping";
                }
                else if (AccommodationTypeId == 2)
                {
                    return "Hotel";
                }
                return "Onbekend";
            }
        }

    }
}
