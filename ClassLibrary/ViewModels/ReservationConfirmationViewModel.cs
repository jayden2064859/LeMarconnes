using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.ViewModels
{
    public class ReservationConfirmationViewModel
    {
        public string FirstName { get; set; }
        public string? Infix { get; set; }
        public string LastName { get; set; }
 
        public string FullName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Infix))
                {
                    return FirstName + " " + LastName;
                }
                else
                {
                    return FirstName + " " + Infix + " " + LastName;
                }
            }
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AdultsCount { get; set; }
        public int Children0_7Count { get; set; }
        public int Children7_12Count { get; set; }
        public int DogsCount { get; set; }
        public bool HasElectricity { get; set; }
        public int? ElectricityDays { get; set; }
        public decimal TotalPrice { get; set; }
        public List<string> AccommodationPlaceNumbers { get; set; } = new();
    }
}
