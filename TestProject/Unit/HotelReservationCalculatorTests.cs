using ClassLibrary.Models;

namespace TestProject.Unit
{
    public class HotelReservationCalculatorTests
    {
        // UTC-19: correcte prijsberekening voor hotelreservering
        [Fact]
        public void HotelReservation_CalculatePrice_ShouldReturnCorrectTotal()
        {
            // arrange
            int customerId = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)); // 2 nachten

            var reservation = new HotelReservation(
                customerId,
                startDate,
                endDate,
                personCount: 1
            );

            // mock hotelkamer
            var hotelRoom = new Accommodation
            {
                AccommodationTypeId = 2,
                Capacity = 2
            };
            reservation.AddAccommodation(hotelRoom);

            // mock tarieven
            var tariffs = new List<Tariff>
            {
                new Tariff { Name = "2PersoonKamer", Price = 80m, AccommodationTypeId = 2 },
                new Tariff { Name = "Toeristenbelasting", Price = 2m, AccommodationTypeId = 2 }
            };

            // act
            var totalPrice = reservation.CalculatePrice(tariffs);

            // assert
            Assert.Equal(164m, totalPrice);
        }
    }
}
