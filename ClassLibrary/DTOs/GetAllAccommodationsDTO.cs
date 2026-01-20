using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    // response DTO voor de public endpoint GET /api/accommodation, zodat interne data niet laten zien wordt
    public class GetAllAccommodationsDTO 
    {
        public string PlaceNumber { get; set; }
        public int Capacity { get; set; }
        public string AccommodationTypeName { get; set; }
    }

}
