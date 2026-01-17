using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject.Unit
{
    public class HotelReservationConstructorTests
    {

        // UTC-08: valid reservering
        [Fact]
        public void HotelReservation_ValidPersonCount_ShouldCreateObject()
        {
            // arrange
            int customerId = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));

            // act
            var reservation = new HotelReservation(
                customerId,
                startDate,
                endDate,
                personCount: 5
            );

            // assert
            Assert.NotNull(reservation);
            Assert.Equal(5, reservation.PersonCount);
        }

        // UTC-09: meer dan 10 personen 
        [Fact]
        public void HotelReservation_InvalidPersonCount_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));

            // act
            try
            {
                var reservation = new HotelReservation(
                customerId,
                startDate,
                endDate,
                personCount: 11
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }

            // assert
            catch (ArgumentException ex)
            {
                Assert.Equal("Minimaal 1, Maximaal 10 personen voor reservering", ex.Message);

            }
        }
        
        // UTC-10: 0 personen
        [Fact]
        public void HotelReservation_ZeroPersonCount_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));

            // act
            try
            {
                var reservation = new HotelReservation(
                    customerId,
                    startDate,
                    endDate,
                    personCount: 0 
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }
            catch (ArgumentException ex)
            {
                // assert
                Assert.Equal("Minimaal 1, Maximaal 10 personen voor reservering", ex.Message);
            }
        }

    }
}
