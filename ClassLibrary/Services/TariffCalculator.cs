using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibrary.Services
{
    public static class TariffCalculator
    {
        public static decimal CalculateTotalPrice(Reservation reservation, List<Tariff> tariffs, int numberOfAccommodations = 1)
        {
            int numberOfNights = reservation.GetNumberOfNights();
            decimal total = 0;

            // filter tarieven voor camping (AccommodationTypeId = 1)
            var campingTariffs = tariffs.Where(t => t.AccommodationTypeId == 1).ToList();

            // zoek specifieke tarieven
            var campingPlace = campingTariffs.FirstOrDefault(t => t.Type == "Campingplaats")?.Price ?? 0;
            var adult = campingTariffs.FirstOrDefault(t => t.Type == "Volwassene")?.Price ?? 0;
            var child0_7 = campingTariffs.FirstOrDefault(t => t.Type == "Kind_0_7")?.Price ?? 0;
            var child7_12 = campingTariffs.FirstOrDefault(t => t.Type == "Kind_7_12")?.Price ?? 0;
            var dog = campingTariffs.FirstOrDefault(t => t.Type == "Hond")?.Price ?? 0;
            var electricity = campingTariffs.FirstOrDefault(t => t.Type == "Electriciteit")?.Price ?? 0;
            var touristTax = campingTariffs.FirstOrDefault(t => t.Type == "Toeristenbelasting")?.Price ?? 0;


            // campingplaats per nacht × aantal accommodaties
            total = campingPlace * numberOfNights * numberOfAccommodations;


            // personen per nacht
            total += adult * reservation.AdultsCount * numberOfNights;
            total += child0_7 * reservation.Children0_7Count * numberOfNights;
            total += child7_12 * reservation.Children7_12Count * numberOfNights;

            // toeristenbelasting per persoon per nacht
            int totalPersons = reservation.AdultsCount +
                               reservation.Children0_7Count +
                               reservation.Children7_12Count;
            total += touristTax * totalPersons * numberOfNights;

            // honden per nacht
            if (reservation.DogsCount > 0)
            {
                total += dog * reservation.DogsCount * numberOfNights;
            }

            // electriciteit per accommodatie
            if (reservation.HasElectricity && reservation.ElectricityDays.HasValue)
            {
                total += electricity * reservation.ElectricityDays.Value * numberOfAccommodations;
            }

            return Math.Round(total, 2);
        }
    }
}