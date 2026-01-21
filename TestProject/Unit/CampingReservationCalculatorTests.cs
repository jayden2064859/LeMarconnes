using ClassLibrary.Models;


namespace TestProject.Unit
{
    public class CampingReservationCalculatorTests
    {
        // UTC-18: correcte prijsberekening voor campingreservering
        [Fact]
        public void CampingReservation_CalculatePrice_ShouldReturnCorrectTotal()
        {
            // arrange
            int customerId = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)); // 2 nachten

            var reservation = new CampingReservation(
                customerId,
                startDate,
                endDate,
                adultsCount: 2,
                children0_7Count: 1,
                children7_12Count: 0,
                dogsCount: 1,
                hasElectricity: true,
                electricityDays: 2
            );

            // mock accommodatie (camping)
            var campingAccommodation = new Accommodation
            {
                AccommodationTypeId = 1,
                Capacity = 4
            };
            reservation.AddAccommodation(campingAccommodation);

            // mock tarieven
            var tariffs = new List<Tariff>
            {
                new Tariff { Name = "Campingplaats", Price = 20m, AccommodationTypeId = 1 },
                new Tariff { Name = "Volwassene", Price = 5m, AccommodationTypeId = 1 },
                new Tariff { Name = "Kind_0_7", Price = 2.5m, AccommodationTypeId = 1 },
                new Tariff { Name = "Kind_7_12", Price = 3m, AccommodationTypeId = 1 },
                new Tariff { Name = "Hond", Price = 3m, AccommodationTypeId = 1 },
                new Tariff { Name = "Electriciteit", Price = 4m, AccommodationTypeId = 1 },
                new Tariff { Name = "Toeristenbelasting", Price = 1.5m, AccommodationTypeId = 1 }
            };

            // act
            var totalPrice = reservation.CalculatePrice(tariffs);

            // assert
            Assert.Equal(88m, totalPrice);
        }
    }
}
