using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class PostAccommodationResponseDTO
    {
        public int AccommodationId { get; set; }
        public string? PlaceNumber { get; set; } 
        public int Capacity { get; set; }
        public int AccommodationTypeId { get; set; }
        public string? AccommodationTypeName { get; set; } 
    }
}
