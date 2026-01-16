using ClassLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HotelReservation : Reservation
{
    public int PersonCount { get; set; } 

    public HotelReservation() : base() { } // lege constructor (voor EF)
    public HotelReservation(int customerId, DateOnly startDate, DateOnly endDate, int personCount) : base(customerId, startDate, endDate)
    {
        if (personCount < 1 || personCount > 10) 
        {
            throw new ArgumentException("Minimaal 1, Maximaal 10 personen voor reservering");
        }

        PersonCount = personCount;
    }

    // method om te valideren dat het totaal aantal personen voor reservering niet groter is dan totale capaciteit van de gereserveerde hotelkamers 
    public void ValidateCapacity()
        {
            // de capaciteit van alle accommodations (max 2) gekoppeld aan de reservering worden opgetelt om het met personcount te vergelijken
            var totalCapacity = Accommodations.Sum(a => a.Capacity);

            if (PersonCount > totalCapacity)
            {
                throw new Exception($"Aantal personen ({PersonCount}) overschrijdt capaciteit ({totalCapacity})");
            }
        }


    // override van virtual method in base class 
    public override void AddAccommodation(Accommodation hotelAccommodation)
    {
        // eigen logica toevoegen
        if (hotelAccommodation.AccommodationTypeId != 2) // type van elke accommodatie moet Hotel type zijn 
        {
            throw new ArgumentException("Alleen hotel accommodaties mogelijk voor dit type reservering");
        }

        // standaard validatie van de virtual method in de base class wordt hier ook uitgevoerd
        base.AddAccommodation(hotelAccommodation);
    }


    // hotel class erft de method van de base abstract class Reservation over om zijn eigen logica van tarief berekening te implementeren
    public override decimal CalculatePrice(List<Tariff> tariffs)
    {
        int nights = GetNumberOfNights();
        decimal total = 0;

        var hotelTariffs = tariffs
            .Where(t => t.AccommodationTypeId == 2)
            .ToList();

        foreach (var accommodation in Accommodations)
        {
            var roomTariff = hotelTariffs
                .FirstOrDefault(t => t.Name.Contains($"{accommodation.Capacity}Persoon"));

            if (roomTariff != null)
                total += roomTariff.Price * nights;
        }

        var touristTax = hotelTariffs.FirstOrDefault(t => t.Name == "Toeristenbelasting");
        if (touristTax != null)
            total += touristTax.Price * PersonCount * nights;

        return Math.Round(total, 2);
    }

}