
namespace ClassLibrary.Models
{
    // reservation is de base class die properties bevat die relevant zijn voor ALLE type reserveringen,
    // en de subtypes moeten hun eigen logica zelf implementeren.
    public abstract class Reservation // reservation is abstract omdat camping en hotel het moeten inheriten
    {

        // class properties
        public int ReservationId { get; set; }
        public DateOnly StartDate { get; set; } // DateOnly is logischer voor datums, want tarieven zijn per overnachting
        public DateOnly EndDate { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime RegistrationDate { get; set; }

        // foreign keys/navigation properties
        public Customer Customer { get; set; }  

        public int CustomerId { get; set; }
        public List<Accommodation> Accommodations { get; set; } = new List<Accommodation>(); 

        // constructors
        public Reservation() // parameterloze constructor (nodig voor EF)
        {            
        }
        
        // main constructor voor aanmaken van reservering 
        public Reservation(int customerId, DateOnly startDate, DateOnly endDate)
        {
            if (customerId <= 0) 
            { 
                throw new ArgumentException("Invalid customer"); 
            }

            // DateOnly heeft geen .today optie zoals DateTime, 
            // dus DateTime wordt eerst gebruikt om de huidige dag te bepalen, en daarna converted naar DateOnly
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (startDate < today)
            {
                throw new ArgumentException("Startdatum moet minimaal vandaag zijn");

            }

            if (endDate <= startDate)
            {
                throw new ArgumentException("Einddatum moet later zijn dan startdatum");
            }
                          
            CustomerId = customerId;
            StartDate = startDate;
            EndDate = endDate;
            RegistrationDate = DateTime.Now;
        
        }

        // methods
        public int GetNumberOfNights() 
        {
            return EndDate.DayNumber - StartDate.DayNumber;
        }

        // virtual methode: er is een basis implementatie, en subclasses mogen eraan toevoegen of vervangen met hun eigen logica
        public virtual void AddAccommodation(Accommodation accommodation) 
        {
            // logica die voor alle subtypes geldt 
            if (accommodation == null)
            {
                throw new ArgumentException("Geen accommodatie gevonden om toe te voegen");
            }
            if (Accommodations.Count >= 2)
            {
                throw new ArgumentException("Maximaal 2 accommodaties per reservering");
            }
            Accommodations.Add(accommodation);
        }


        // elke soort reservering heeft een eigen total price calculator nodig met zijn eigen implementatie ervan
        public abstract decimal CalculatePrice(List<Tariff> tariffs);
    }
}