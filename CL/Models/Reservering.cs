using System;

namespace CL.Models
{
    public class Reservering
    {
        // auto implemented properties
        public int Id { get; set; }
        public int KlantId { get; set; }
        public int AccommodatieId { get; set; }

         
        public DateTime StartDatum { get; set; }
        public DateTime EindDatum { get; set; }

       
        public int AantalVolwassenen { get; set; }
               
        public int AantalKinderen0_7 { get; set; }
        public int AantalKinderen7_12 { get; set; }

        public bool Hond { get; set; } = false;
        public bool Elektriciteit { get; set; } = false;

        // Extra property zodat aantal dagen elektriciteit dynamisch berekend kan worden.
        // bijv. wanneer een klant voor 5 dagen verblijft, maar elektriciteit alleen voor 3 dagen wil gebruiken.
        public int AantalDagenElektriciteit { get; set; } = 0; // standaard op 0 


        public decimal TotaalPrijs { get; set; }
        public string Status { get; set; } 
        public DateTime DatumAangemaakt { get; set; } = DateTime.Now; // standaard naar huidige tijd en datum

        public int AantalNachten
        {
            get { return (EindDatum - StartDatum).Days; }
        
        }

        
    }
}
