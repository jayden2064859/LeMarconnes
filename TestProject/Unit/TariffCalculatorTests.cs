using ClassLibrary.Services;
using ClassLibrary.Models;
using Xunit;

namespace TestProject.Unit
{
    public class TariffCalculatorTests
    {
        private readonly List<Tariff> _allTariffs = new List<Tariff>
        {
            new Tariff { Type = "Campingplaats", Price = 7.50m, AccommodationTypeId = 1 },
            new Tariff { Type = "Volwassene", Price = 6.00m, AccommodationTypeId = 1 },
            new Tariff { Type = "Kind_0_7", Price = 4.00m, AccommodationTypeId = 1 },
            new Tariff { Type = "Kind_7_12", Price = 5.00m, AccommodationTypeId = 1 },
            new Tariff { Type = "Hond", Price = 2.50m, AccommodationTypeId = 1 },
            new Tariff { Type = "Electriciteit", Price = 7.50m, AccommodationTypeId = 1 },
            new Tariff { Type = "Toeristenbelasting", Price = 0.25m, AccommodationTypeId = 1 }
        };

        // UTC-01: Eenvoudige reservering (geen extra's)
        [Fact]
        public void CalculateTotalPrice_SimpleReservation_ReturnsCorrectPrice()
        {
            // ARRANGE
            var reservation = new Reservation(
                customerId: 1,
                startDate: new DateTime(2024, 6, 1),
                endDate: new DateTime(2024, 6, 3), // 2 nachten
                adultsCount: 2,
                children0_7Count: 0,
                children7_12Count: 0,
                dogsCount: 0,
                hasElectricity: false,
                electricityDays: null
            );

            // ACT - 1 accommodatie (default)
            var result = TariffCalculator.CalculateTotalPrice(reservation, _allTariffs);

            // ASSERT
            decimal expected = 40.00m; // €40,00
            Assert.Equal(expected, result);
        }

        // UTC-02: Reservering met alle extra's (1 accommodatie)
        [Fact]
        public void CalculateTotalPrice_AllExtrasOneAccommodation_ReturnsCorrectPrice()
        {
            // ARRANGE
            var reservation = new Reservation(
                customerId: 1,
                startDate: new DateTime(2024, 6, 1),
                endDate: new DateTime(2024, 6, 4), // 3 nachten
                adultsCount: 2,
                children0_7Count: 1,
                children7_12Count: 1,
                dogsCount: 1,
                hasElectricity: true,
                electricityDays: 3
            );

            // ACT - 1 accommodatie (default)
            var result = TariffCalculator.CalculateTotalPrice(reservation, _allTariffs);

            // ASSERT
            decimal expected = 118.50m; // €118,50
            Assert.Equal(expected, result);
        }

        // UTC-03: Reservering met gedeeltelijke elektriciteit (1 accommodatie)
        [Fact]
        public void CalculateTotalPrice_PartialElectricityOneAccommodation_ReturnsCorrectPrice()
        {
            // ARRANGE
            var reservation = new Reservation(
                customerId: 1,
                startDate: new DateTime(2024, 6, 1),
                endDate: new DateTime(2024, 6, 4), // 3 nachten
                adultsCount: 2,
                children0_7Count: 1,
                children7_12Count: 1,
                dogsCount: 1,
                hasElectricity: true,
                electricityDays: 2 // Alleen 2 van de 3 dagen elektriciteit
            );

            // ACT - 1 accommodatie (default)
            var result = TariffCalculator.CalculateTotalPrice(reservation, _allTariffs);

            // ASSERT
            decimal expected = 111.00m; // €111,00
            Assert.Equal(expected, result);
        }

        // UTC-04: Reservering met alle extra's (meerdere accommodaties)
        [Fact]
        public void CalculateTotalPrice_AllExtrasMultipleAccommodations_ReturnsCorrectPrice()
        {
            // ARRANGE
            var reservation = new Reservation(
                customerId: 1,
                startDate: new DateTime(2024, 6, 1),
                endDate: new DateTime(2024, 6, 4), // 3 nachten
                adultsCount: 2,
                children0_7Count: 1,
                children7_12Count: 1,
                dogsCount: 1,
                hasElectricity: true,
                electricityDays: 3
            );

            // ACT - 2 accommodaties
            int numberOfAccommodations = 2;
            var result = TariffCalculator.CalculateTotalPrice(
                reservation,
                _allTariffs,
                numberOfAccommodations
            );

            // ASSERT
            decimal expected = 163.50m; // €163,50
            Assert.Equal(expected, result);
        }

        // UTC-05: Reservering met gedeeltelijke elektriciteit (meerdere accommodaties)
        [Fact]
        public void CalculateTotalPrice_PartialElectricityMultipleAccommodations_ReturnsCorrectPrice()
        {
            // ARRANGE
            var reservation = new Reservation(
                customerId: 1,
                startDate: new DateTime(2024, 6, 1),
                endDate: new DateTime(2024, 6, 4), // 3 nachten
                adultsCount: 2,
                children0_7Count: 1,
                children7_12Count: 1,
                dogsCount: 1,
                hasElectricity: true,
                electricityDays: 2 // Alleen 2 van de 3 dagen elektriciteit
            );

            // ACT - 2 accommodaties
            int numberOfAccommodations = 2;
            var result = TariffCalculator.CalculateTotalPrice(
                reservation,
                _allTariffs,
                numberOfAccommodations
            );

            // ASSERT
            decimal expected = 148.50m; // €148,50
            Assert.Equal(expected, result);
        }
    }
}