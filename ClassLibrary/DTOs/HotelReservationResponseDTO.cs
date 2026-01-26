using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Yassir
namespace ClassLibrary.DTOs
{
        public class HotelReservationResponseDTO
        {
            // klantgegevens voor de bevestiging
            public string FirstName { get; set; }
            public string? Infix { get; set; }
            public string LastName { get; set; }

            // reserveringsgegevens
            public DateOnly StartDate { get; set; }
            public DateOnly EndDate { get; set; }
            public int PersonCount { get; set; }
            public decimal TotalPrice { get; set; }

            // lijst met kamernummers
            public List<string> AccommodationPlaceNumbers { get; set; } = new();
        }
}
