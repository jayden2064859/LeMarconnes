using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.DTOs
{
    public class PostAccommodationDTO
    {
        [Required]
        public string PlaceNumber { get; set; }
        [Required]
        public int Capacity { get; set; }
        public int AccommodationTypeId { get; set; }

    }
}
