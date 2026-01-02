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

        // minimaal 1, max 2 accommodations per reservering
        public static bool ValidateAccommodationCount(List<int> accommodationList)
        {
            if (accommodationList.Count > 2 || accommodationList.Count <= 0 || accommodationList == null)
            {
                return false;
            }
            return true;
        }

        // input voor aantal dagan electriciteitsgebruik kan niet hoger zijn dan aantal overnachtingen
        public static bool ValidateElectricity(int? electricityDays, int numberOfNights)
        {
            if (electricityDays == null || electricityDays.Value < 1 || electricityDays.Value > numberOfNights)
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

