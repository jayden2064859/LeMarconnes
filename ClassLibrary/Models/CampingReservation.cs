using ClassLibrary.Models;
using ClassLibrary.Services;

public class CampingReservation : Reservation // CampingReservation erft alle eigenschappen van de base class (abstract) Reservation over
{
    public int AdultsCount { get; set; }
    public int Children0_7Count { get; set; }
    public int Children7_12Count { get; set; }
    public int DogsCount { get; set; }
    public bool HasElectricity { get; set; }
    public int? ElectricityDays { get; set; }

    public CampingReservation() : base()  { } // lege constructor (voor EF)


    // Constructor roept base constructor aan
    public CampingReservation(int customerId, DateTime startDate, DateTime endDate, int adultsCount, int children0_7Count, int children7_12Count,
        int dogsCount, bool hasElectricity, int? electricityDays = null) : base(customerId, startDate, endDate)
    {

        // minimaal 1, maximaal 4 volwassenen per reservering
        if (adultsCount < 1 || adultsCount > 4)
        {
            throw new ArgumentException("Minimaal 1, maximaal 4 volwassenen");
        }

        // minimaal 0, maximaal 4 kinderen per reservering
        int totalChildren = children0_7Count + children7_12Count;

        if ( totalChildren < 0 || totalChildren > 4)
        {
            throw new ArgumentException("Minimaal 0, maximaal 2 kinderen per leeftijdscategorie");
        }

        // een klant kan max 2 campingplekken reserveren. Elke campingplek staat 2 volwassenen, 2 kinderen toe. 
        // dus 2x het max aantal per campingplek = max 8 personen per reservering
        int totalCount = adultsCount + children0_7Count + children7_12Count;
        if (totalCount > 8)
        {
            throw new ArgumentException("Maximaal 8 personen per reservering toegestaan");
        }

        // niet meer dan 2 honden
        if (dogsCount < 0 || dogsCount > 2)
        {
            throw new ArgumentException("Minimaal 0, maximaal 2 honden");
        }

        // als klant electriciteit wil gebruiken, kan het aantal niet minder dan 1 dag zijn
        if (hasElectricity && electricityDays < 1)
        {
            throw new ArgumentException("Ongeldige invoer voor elektriciteitsgebruik");
        }

        // als klant electriciteit wil gebruiken, kan het aantal niet meer zijn dan het aantal overnachtingen
        if (hasElectricity && electricityDays > GetNumberOfNights())
        {
            throw new ArgumentException("Aantal dagen elektriciteit kan niet hoger zijn dan aantal overnachtingen");
        }
       

        AccommodationTypeId = 1; // camping typeid is altijd 1
        AdultsCount = adultsCount;
        Children0_7Count = children0_7Count;
        Children7_12Count = children7_12Count;
        DogsCount = dogsCount;
        HasElectricity = hasElectricity;
        ElectricityDays = electricityDays;
    }

    // camping class erft de method van de base abstract class Reservation over om zijn eigen logica van tarief berekening te implementeren
    public override decimal CalculatePrice(List<Tariff> tariffs)
    {
        int nights = GetNumberOfNights();
        int numAccommodations = Accommodations.Count;
        decimal total = 0;

        var campingTariffs = tariffs
            .Where(t => t.AccommodationType == Accommodation.AccommodationType.Camping)
            .ToList();

        total += GetTariff(campingTariffs, "Campingplaats") * nights * numAccommodations;
        total += GetTariff(campingTariffs, "Volwassene") * AdultsCount * nights;
        total += GetTariff(campingTariffs, "Kind_0_7") * Children0_7Count * nights;
        total += GetTariff(campingTariffs, "Kind_7_12") * Children7_12Count * nights;

        if (DogsCount > 0)
            total += GetTariff(campingTariffs, "Hond") * DogsCount * nights;

        if (HasElectricity && ElectricityDays.HasValue)
            total += GetTariff(campingTariffs, "Electriciteit") * ElectricityDays.Value * numAccommodations;

        int totalPersons = AdultsCount + Children0_7Count + Children7_12Count;
        total += GetTariff(campingTariffs, "Toeristenbelasting") * totalPersons * nights;

        return Math.Round(total, 2);
    }

    private decimal GetTariff(List<Tariff> tariffs, string name)
    {
        return tariffs.FirstOrDefault(t => t.Name == name).Price;
    }

}