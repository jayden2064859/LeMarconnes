using ClassLibrary.DTOs;
using ClassLibrary.Models;
using ClassLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary.Services
{
    public class CreateReservationService
    {
        // method die een DTO object aanmaakt
        public static CreateReservationDTO CreateNewReservationDTO(int customerId, List<int> accommodationIds, DateTime startDate,
            DateTime endDate,int adultsCount, int children0_7Count, int children7_12Count, int dogsCount, bool hasElectricity, int? electricityDays = null)
        {
            return new CreateReservationDTO
            {
                CustomerId = customerId,
                AccommodationIds = accommodationIds,
                StartDate = startDate,
                EndDate = endDate,
                AdultsCount = adultsCount,
                Children0_7Count = children0_7Count,
                Children7_12Count = children7_12Count,
                DogsCount = dogsCount,
                HasElectricity = hasElectricity,
                ElectricityDays = electricityDays

            };
        }


        // max 2 accommodations (campingplekken) in totaal voor een reservering
        public static bool ValidAccommodationCount(List<int> accommodationList)
        {
            if (accommodationList.Count > 2)
            {
                return false;
            }
            return true;
        }
        

        // specifiek checken of start- en einddatum zijn ingevoerd
        public static bool ValidDateInput(DateTime startDate, DateTime endDate)
        {
            if (startDate == default || endDate == default)
            {
                return false;
            }
            return true;
        }

        // alle verplichte velden ingevuld
        public static bool ValidateAllRequiredFields(DateTime startDate, DateTime endDate, List<int>accommodationIds, int adultsCount, 
            int children0_7Count, int children7_12Count, int dogsCount)
        {
            // logica
            return true;

        }

        // einddatum mag niet eerder dan startdatum zijn
        public static bool ValidateReservationDates(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                return false;
            }
            return true;
        }

        // minstens 1 volwassene nodig voor reservering
        public static bool ValidateAdultCounts(int adultsCount)
        {
            if (adultsCount < 1)
            {
                return false;
            }
            return true;
        }

        // minstens 1 accommodation voor een reservering nodig
        public static bool ValidateAccommodationIds(List<int> accommodationIds)
        {
            if (accommodationIds == null || accommodationIds.Count == 0)
            {
                return false;
            }
            return true;
        }

        // als elektriciteit gekozen is voor een accommodatie, moet het voor minstens 1 dag zijn
        public static bool ValidateElectricity(int? electricityDays)
        {    
            if (electricityDays == null || electricityDays.Value < 1)
            {
                return false;
            }
            return true;
        }

    }
}

