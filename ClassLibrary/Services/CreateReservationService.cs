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
            DateTime endDate, int adultsCount, int children0_7Count, int children7_12Count, int dogsCount, bool hasElectricity, int? electricityDays = null)
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

        // minimaal 1, max 2 accommodations per reservering
        public static bool ValidateAccommodationCount(List<int> accommodationList)
        {
            if (accommodationList.Count > 2 || accommodationList.Count <= 0 || accommodationList == null)
            {
                return false;
            }
            return true;
        }

        public static bool ValidateElectricityDays(int? electricityDays, int numberOfNights)
        {
            // als hasElectricity true is, dan kan de input voor electricitydays niet minder dan 1 zijjn
            if (electricityDays < 1)
            {
                return false;
            }

            // input voor aantal dagan electriciteitsgebruik kan niet hoger zijn dan aantal overnachtingen
            if (electricityDays > numberOfNights)
            {
                return false;
            }

            return true;
        }

        // begindatum moet minimaal de huidige datum zijn, en de einddatum minimaal een dag later (er wordt per overnachting gerekent) 
        public static bool ValidDateInput(DateTime startDate, DateTime endDate)
        {
            if (startDate < DateTime.Today || endDate < DateTime.Today.AddDays(1))
            {
                return false;
            }
            return true;
        }

        // alle verplichte velden ingevuld
        public static bool ValidateAllRequiredFields(DateTime startDate, DateTime endDate, List<int> accommodationIds, int adultsCount,
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

        // minstens 1 (max 10) volwassene nodig voor reservering
        public static bool ValidateAdultCounts(int adultsCount)
        {
            if (adultsCount < 1 || adultsCount > 10)
            {
                return false;
            }
            return true;
        }

        // geen negatieve inputs voor beide leeftijdscategorieen 
        public static bool ValidateChildrenCount(int children_07Count, int children7_12Count)
        {
            // geen negatieve inputs voor beide, en beide max 5  
            if (children_07Count < 0 || children7_12Count < 0 || children_07Count > 5 || children7_12Count > 5)
            {
                return false;
            }
            return true;
        }

        // max 3 honden per reservering
        public static bool ValidateDogsCount(int dogsCount)
        {
            if (dogsCount < 0 || dogsCount > 3)
                return false;
            return true;
        }

    }
}

