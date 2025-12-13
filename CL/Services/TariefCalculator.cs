using CL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CL.Services
{
    public static class TariefCalculator
    {
        // Vaste tarieven
        public const decimal Campingplaats_Per_Nacht = 7.50m;
        public const decimal Volwassene_Per_Nacht = 6.00m;
        public const decimal Kind_0_7_Per_Nacht = 4.00m;
        public const decimal Kind_7_12_Per_Nacht = 5.00m;
        public const decimal Hond_Per_Nacht = 2.50m;
        public const decimal Elektriciteit_Per_Nacht = 7.50m;
        public const decimal Toeristenbelasting_Per_Persoon_Per_Nacht = 0.25m;

        // Totaalprijs berekenen
        public static decimal TotaalPrijs(Reservering reservering)
        {
            int aantalNachten = reservering.AantalNachten;
            decimal totaal = 0;

            // Tarieven van personen worden berekent per persoon, per nacht
            totaal = Campingplaats_Per_Nacht * aantalNachten; 
            totaal = totaal + Volwassene_Per_Nacht * reservering.AantalVolwassenen * aantalNachten;
            totaal = totaal + Kind_0_7_Per_Nacht * reservering.AantalKinderen0_7 * aantalNachten;
            totaal = totaal + Kind_7_12_Per_Nacht * reservering.AantalKinderen7_12 * aantalNachten;

            // Toeristenbelasting meerekenen
            int totaalPersonen = reservering.AantalVolwassenen + 
                                 reservering.AantalKinderen0_7 + 
                                 reservering.AantalKinderen7_12;

            totaal = totaal + Toeristenbelasting_Per_Persoon_Per_Nacht * totaalPersonen * aantalNachten;

            // Hond kost extra per nacht
            if (reservering.Hond)
            {
                totaal = totaal + Hond_Per_Nacht * aantalNachten;
            }

            // Elektriciteit kost extra per nacht
            if (reservering.Elektriciteit && reservering.AantalDagenElektriciteit > 0)
            {
                totaal = totaal + Elektriciteit_Per_Nacht * reservering.AantalDagenElektriciteit;
            }

            return Math.Round(totaal, 2); // afronden op 2 decimalen

        }
    }










}
