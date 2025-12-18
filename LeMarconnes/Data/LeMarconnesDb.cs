using LeMarconnes.Models;
using Microsoft.EntityFrameworkCore;


namespace LeMarconnes.Data
{
    public class LeMarconnesDb : DbContext // We gebruiken inheritance om de bestaande methods van Dbcontext te gebruiken 
    {
        // constructor voor dependency injection
        public LeMarconnesDb(DbContextOptions<LeMarconnesDb> options) : base(options) { }  // geeft de geconfigureerde opties door aan de parent class DbContext (zoals connectionstring)

        // initialise alle models die aan de db toegevoegd moeten worden
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<AccommodationType> AccommodationsTypes { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }



        // OnModelCreating is standaard een override method, omdat we van de method van de parent class DbContext lenen.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<AccommodationType>().HasData(
                new AccommodationType { AccommodationTypeId = 1, Name = "Camping" },
                new AccommodationType { AccommodationTypeId = 2, Name = "Hotel" }
            );


            // customer - reservation
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Customer) // een reservation heeft precies 1 customer
                .WithMany(c => c.Reservations) // een customer kan meerdere reservations hebben
                .HasForeignKey(r => r.CustomerId);

            // reservation - accommodation
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Accommodation) // een reservation heeft precies 1 accommodatie eraan gekoppelt
                .WithMany(a => a.Reservations) // een accommodatie kan meerdere reservations in totaal hebben 
                .HasForeignKey(r => r.AccommodationId); 


           // accommodation - accommodationtype
            modelBuilder.Entity<Accommodation>()
                .HasOne(a => a.AccommodationType) // een accommodatie heeft precies 1 accommodatietype (1=camping, 2=gite, 3=hotel)
                .WithMany(at => at.Accommodations) //een accommodatietype kan meerdere accommodaties bevatten (bijv de camping bevat accommodaties 1A, 2B, en 3C)
                .HasForeignKey(a => a.AccommodationTypeId);


            // tariff - accommodatietype
            modelBuilder.Entity<Tariff>() 
                .HasOne(t => t.AccommodationType) // een tarief heeft precies 1 accommodatietype (bijv het tarief voor campingplaats hoort specifiek bij accommodatietype 1 (camping)
                .WithMany(at => at.Tariffs) // een accommodatietype kan meerdere tarieven bevatten
                .HasForeignKey(t => t.AccommodationTypeId);


            // account kan een customer hebben, maar hoeft niet. (Bijv medewerker, admin, eigenaar)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer) // een account kan precies 1 customer hebben
                .WithMany() // deze blijft leeg, want customer verwijst niet terug naar account (klanten moeten ook telefonisch/op locatie kunnen reserveren waarbij ze geen account nodig hebben)
                .HasForeignKey(a => a.CustomerId)
                .IsRequired(false); // dit maakt het nullable

            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(10, 2);  // max 10 karakters, maar dat is meer dan genoeg in dit geval

            modelBuilder.Entity<Tariff>()
                .Property(t => t.Price)
                .HasPrecision(10, 2);  

            // constraints

            // de einddatum van een reservering moet later dan de startdatum zijn
            modelBuilder.Entity<Reservation>()
                .HasCheckConstraint("CHK_EndAfterStart", "EndDate > StartDate");

            // minstens 1 volwassene nodig voor een reservering. aantal kinderen en honden kan 0 zijn
            modelBuilder.Entity<Reservation>()
                .HasCheckConstraint("CHK_ValidCounts",
                "AdultsCount >= 1 AND " +
                "Children0_7Count >= 0 AND " +
                "Children7_12Count >= 0 AND " +
                "DogsCount >= 0");



        }
    }
}
