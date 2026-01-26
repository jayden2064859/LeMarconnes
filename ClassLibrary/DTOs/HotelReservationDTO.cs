using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Yassor
namespace ClassLibrary.DTOs
{
    public class HotelReservationDTO
    {
        public int CustomerId { get; set; }
        public List<int> AccommodationIds { get; set; } = new();
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PersonCount { get; set; }
    }
}
