using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models;

namespace TestProject.Unit
{
    public class ReservationConstructorTests
    {
        // UTC-11: invalid customerId (0 of negatief getal)
        [Fact]
        public void BaseReservation_InvalidCustomerId_ShouldNotCreateObject()
        {
            // arrange
            int customerId = -1;
            int personCount = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(2));

            // act
            try
            {
                Reservation reservation = new HotelReservation( // een van de subtypes is nog steeds nodig omdat de base class abstract is
                    customerId, 
                    startDate,
                    endDate,
                    personCount
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }
            catch (ArgumentException ex)
            {
                // assert
                Assert.Equal("Invalid customer", ex.Message);
            }

        }

        // UTC-12: startdatum is eerder dan vandaag
        [Fact]
        public void BaseReservation_InvalidStartDate_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            int personCount = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); // startdatum wordt gezet op gisteren
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));

            // act
            try
            {
                Reservation reservation = new HotelReservation(
                    customerId,
                    startDate,
                    endDate,
                    personCount
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }
            catch (ArgumentException ex)
            {
                // assert
                Assert.Equal("Startdatum moet minimaal vandaag zijn", ex.Message);
            }

        }

        // UTC-13: einddatum eerder dan startdatum
        [Fact]
        public void BaseReservation_EndDateBeforeStartDate_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            int personCount = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)); // einddatum is een dag eerder dan startdatum

            // act
            try
            {
                Reservation reservation = new HotelReservation(
                    customerId,
                    startDate,
                    endDate,
                    personCount
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }
            catch (ArgumentException ex)
            {
                // assert
                Assert.Equal("Einddatum moet later zijn dan startdatum", ex.Message);
            }

        }

        // UTC-14: einddatum op dezelfde dag als startdatum
        [Fact]
        public void BaseReservation_InvalidEndDate_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            int personCount = 1;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)); // einddatum op dezelfde dag

            // act
            try
            {
                Reservation reservation = new HotelReservation(
                    customerId,
                    startDate,
                    endDate,
                    personCount
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }
            catch (ArgumentException ex)
            {
                // assert
                Assert.Equal("Einddatum moet later zijn dan startdatum", ex.Message);
            }

        }

        // UCT-15: GetNumberOfNights() method
        [Fact]
        public void GetNumberOfNightsMethodTest()
        {
            // arrange
            int personCount = 2;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));

            var reservation = new HotelReservation(
                customerId: 1,
                startDate,
                endDate,
                personCount
            );

            // act
            int numberOfNights = reservation.GetNumberOfNights();

            // assert
            Assert.Equal(4, numberOfNights); // 4 nachten
        }

        // UTC-16: AddAccommodation() method - valid accommodation toevoegen
        [Fact]
        public void AddAccommodation_ValidAccommodation_ShouldAddToList()
        {
            // arrange
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));

            var reservation = new HotelReservation(1, startDate, endDate, 2);
            var accommodation = new Accommodation
            {
                AccommodationId = 1,
                PlaceNumber = "A1",
                Capacity = 2,
                AccommodationTypeId = 2 // 2 = hotel
            };

            // act
            reservation.AddAccommodation(accommodation);

            // assert
            Assert.Single(reservation.Accommodations);
            Assert.Equal("A1", reservation.Accommodations[0].PlaceNumber);
        }

        // UTC-17: AddAccommodation() method - meer dan 2 accommodaties toevoegen
        [Fact]
        public void AddAccommodation_MoreThanTwoAccommodations_ShouldThrowException()
        {
            // arrange
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1));
            DateOnly endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(5));
            var reservation = new HotelReservation(1, startDate, endDate, 2);

            var acc1 = new Accommodation { AccommodationId = 1, AccommodationTypeId = 2, PlaceNumber = "A1", Capacity = 2 };
            var acc2 = new Accommodation { AccommodationId = 2, AccommodationTypeId = 2, PlaceNumber = "A2", Capacity = 2 };
            var acc3 = new Accommodation { AccommodationId = 3, AccommodationTypeId = 2, PlaceNumber = "A3", Capacity = 2 };

            reservation.AddAccommodation(acc1);
            reservation.AddAccommodation(acc2);

            // act
            try
            {
                reservation.AddAccommodation(acc3);
                Assert.Fail("Object is ongeldig aangemaakt");
            }
            catch (ArgumentException ex)
            {
                // assert
                Assert.Equal("Maximaal 2 accommodaties per reservering", ex.Message);
            }
        }
    }
}