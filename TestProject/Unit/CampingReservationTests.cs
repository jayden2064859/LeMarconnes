using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibrary.Models;
using Xunit;

namespace TestProject.Unit
{
    public class CampingReservationTests
    {
        // UCT-01: valid aantal volwassenen
        [Fact]
        public void CampingReservation_WithValidAdultsCount_ShouldCreateObject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(5);

            // act
            var reservation = new CampingReservation(
            customerId,
            startDate,
            endDate,
            adultsCount: 1, // minimaal 1 volwassene
            children0_7Count: 0, // 0 t/m 2 is valid
            children7_12Count: 0, // 0 t/m 2 is valid
            dogsCount: 0, // 0 t/m 3 is valid
            hasElectricity: false,
            electricityDays: null
            );

            // assert
            Assert.NotNull(reservation);
            Assert.Equal(1, reservation.AdultsCount);
        }

        // UTC-02: invalid aantal volwassenen
        [Fact]
        public void CampingReservation_WithoutValidAdultsCount_ShouldNotCreateobject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(5);

            // act
            try
            {
                var reservation = new CampingReservation(
                customerId,
                startDate,
                endDate,
                adultsCount: 0,
                children0_7Count: 1,
                children7_12Count: 1,
                dogsCount: 0,
                hasElectricity: false,
                electricityDays: null
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }

            // assert
            catch (ArgumentException ex)
            {

                Assert.Equal("Minimaal 1, maximaal 4 volwassenen", ex.Message);
            }

        }

        // UTC-03: valid aantal kinderen per categorie
        [Fact]
        public void CampingReservation_WithInvalidChildrenCount_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(5);

            // act
            try
            {
                var reservation = new CampingReservation(
                customerId,
                startDate,
                endDate,
                adultsCount: 1,
                children0_7Count: -1,
                children7_12Count: 3,
                dogsCount: 0,
                hasElectricity: false,
                electricityDays: null
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }

            // assert
            catch (ArgumentException ex)
            {
                Assert.Equal("Minimaal 0, maximaal 2 kinderen per leeftijdscategorie", ex.Message);
            }
        }

        // UTC-04: invalid aantal honden (max 2)
        [Fact]
        public void CampingReservation_InvalidDogCount_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(5);

            // act
            try
            {
                var reservation = new CampingReservation(
                customerId,
                startDate,
                endDate,
                adultsCount: 1,
                children0_7Count: 0,
                children7_12Count: 0,
                dogsCount: 3,
                hasElectricity: false,
                electricityDays: null
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }

            // assert
            catch (ArgumentException ex)
            {
                Assert.Equal("Minimaal 0, maximaal 2 honden", ex.Message);
            }
        }


        // UTC-05: invalid gekozen dagen elektriciteitsgebruik (hasElectricity = true, dan minstens 1 dag gebruik)
        [Fact]
        public void CampingReservation_HasElectricity_WithNullDays_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(5);

            // act
            try
            {
                var reservation = new CampingReservation(
                customerId,
                startDate,
                endDate,
                adultsCount: 1,
                children0_7Count: 0,
                children7_12Count: 0,
                dogsCount: 0,
                hasElectricity: true,
                electricityDays: null
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }

            // assert
            catch (ArgumentException ex)
            {
                Assert.Equal("Minstens 1 dag elektriciteit vereist", ex.Message);
            }
        }


        // UTC-06: invalid gekozen dagen elektriciteitsgebruik (kan niet hoger zijn dan totaal aantal overnachtingen)
        [Fact]
        public void CampingReservation_ElectricityDays_ExceedsNumberOfNights_ShouldNotCreateObject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1); 
            DateTime endDate = DateTime.Today.AddDays(5); // 4 overnachtingen

            // act
            try
            {
                var reservation = new CampingReservation(
                customerId,
                startDate,
                endDate,
                adultsCount: 1,
                children0_7Count: 0,
                children7_12Count: 0,
                dogsCount: 0,
                hasElectricity: true,
                electricityDays: 5 // 5 dagen elektriciteitsgebruik
                );
                Assert.Fail("Object is ongeldig aangemaakt");
            }

            // assert
            catch (ArgumentException ex)
            {
                Assert.Equal("Aantal dagen elektriciteit kan niet hoger zijn dan aantal overnachtingen", ex.Message);
            }
        }


        // UTC-07: alle values zetten op wat maximaal toegestaan is
        [Fact]
        public void CampingReservation_MaxAllowedInputs_ShouldCreateObject()
        {
            // arrange
            int customerId = 1;
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today.AddDays(11);

            // act
            var reservation = new CampingReservation(
            customerId,
            startDate,
            endDate,
            adultsCount: 4,
            children0_7Count: 2,
            children7_12Count: 2,
            dogsCount: 2,
            hasElectricity: true,
            electricityDays: 10
            );

            // assert
            Assert.NotNull(reservation);

        }
    }
}