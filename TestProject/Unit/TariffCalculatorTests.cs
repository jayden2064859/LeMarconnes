using ClassLibrary.Services;
using ClassLibrary.Models;
using Xunit;

namespace TestProject.Unit
{
    public class TariffCalculatorTests
    {
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

            var tariffs = new List<Tariff>
            {
                new Tariff {
                    TariffId = 1,
                    Type = "Campingplaats",
                    Price = 7.50m,
                    AccommodationTypeId = 1
                },
                new Tariff {
                    TariffId = 2,
                    Type = "Volwassene",
                    Price = 6.00m,
                    AccommodationTypeId = 1
                },
                new Tariff {
                    TariffId = 7,
                    Type = "Toeristenbelasting",
                    Price = 0.25m,
                    AccommodationTypeId = 1
                }
            };

            // ACT
            var result = ClassLibrary.Services.TariffCalculator.CalculateTotalPrice(reservation, tariffs);

            // ASSERT
            // Handmatige berekening:
            // Campingplaats: 7.50 × 2 nachten = 15.00
            // Volwassenen: 6.00 × 2 personen × 2 nachten = 24.00
            // Toeristenbelasting: 0.25 × 2 personen × 2 nachten = 1.00
            // Totaal: 40.00
            decimal expected = 40.00m;

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateTotalPrice_MultipleAccommodations_ReturnsCorrectPrice()
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

            var tariffs = new List<Tariff>
            {
                new Tariff { Type = "Campingplaats", Price = 7.50m, AccommodationTypeId = 1 },
                new Tariff { Type = "Volwassene", Price = 6.00m, AccommodationTypeId = 1 },
                new Tariff { Type = "Kind_0_7", Price = 4.00m, AccommodationTypeId = 1 },
                new Tariff { Type = "Kind_7_12", Price = 5.00m, AccommodationTypeId = 1 },
                new Tariff { Type = "Hond", Price = 2.50m, AccommodationTypeId = 1 },
                new Tariff { Type = "Electriciteit", Price = 7.50m, AccommodationTypeId = 1 },
                new Tariff { Type = "Toeristenbelasting", Price = 0.25m, AccommodationTypeId = 1 }
            };

            // ACT - Test met 2 accommodaties
            int numberOfAccommodations = 2;
            var result = ClassLibrary.Services.TariffCalculator.CalculateTotalPrice(
                reservation,
                tariffs,
                numberOfAccommodations
            );

            // ASSERT
            // Handmatige berekening voor 2 accommodaties, 3 nachten:
            // Campingplaats: 7.50 × 3 × 2 = 45.00
            // Volwassenen: 6.00 × 2 × 3 = 36.00
            // Kind 0-7: 4.00 × 1 × 3 = 12.00
            // Kind 7-12: 5.00 × 1 × 3 = 15.00
            // Hond: 2.50 × 1 × 3 = 7.50
            // Toeristenbelasting: 0.25 × 4 personen × 3 = 3.00
            // Electriciteit: 7.50 × 3 × 2 = 45.00
            // Totaal: 163.50

            decimal expected = 163.50m;

            Assert.Equal(expected, result);
        }
    }
}