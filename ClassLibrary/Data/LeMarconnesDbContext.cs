using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;


namespace ClassLibrary.Data
{
    public class LeMarconnesDbContext : DbContext // We gebruiken inheritance om de bestaande methods van Dbcontext te gebruiken 
    {
        // lege constructor voor dependency injection
        public LeMarconnesDbContext(DbContextOptions<LeMarconnesDbContext> options) : base(options) // geeft de geconfigureerde opties door aan de parent class DbContext (zoals connectionstring)
        {        
        }
        public LeMarconnesDbContext() 
        { 
        }

        // initialise alle models die aan de db toegevoegd moeten worden
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=(localdb)\\mssqllocaldb;Database=LeMarconnesDB;Trusted_Connection=True;");
        }


        // OnModelCreating is standaard een override method, omdat we de parent class DbContext gebruiken
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // convert enum naar int voor de db
            modelBuilder.Entity<Accommodation>()
                .Property(a => a.Type)
                .HasConversion<int>();

            modelBuilder.Entity<Tariff>()
                .Property(t => t.AccommodationType)
                .HasConversion<int>();


            // db seeden met camping tarieven 
            modelBuilder.Entity<Tariff>().HasData(
                new Tariff { TariffId = 1, Name = "Campingplaats", Price = 7.50m, AccommodationType = Accommodation.AccommodationType.Camping },
                new Tariff { TariffId = 2, Name = "Volwassene", Price = 6.00m, AccommodationType = Accommodation.AccommodationType.Camping },
                new Tariff { TariffId = 3, Name = "Kind_0_7", Price = 4.00m, AccommodationType = Accommodation.AccommodationType.Camping },
                new Tariff { TariffId = 4, Name = "Kind_7_12", Price = 5.00m, AccommodationType = Accommodation.AccommodationType.Camping },
                new Tariff { TariffId = 5, Name = "Hond", Price = 2.50m, AccommodationType = Accommodation.AccommodationType.Camping },
                new Tariff { TariffId = 6, Name = "Electriciteit", Price = 7.50m, AccommodationType = Accommodation.AccommodationType.Camping },
                new Tariff { TariffId = 7, Name = "Toeristenbelasting", Price = 0.25m, AccommodationType = Accommodation.AccommodationType.Camping }
            );

            // db seeden met hotel tarieven 
            modelBuilder.Entity<Tariff>().HasData(
                new Tariff { TariffId = 8, Name = "Hotelkamer_1Persoon", Price = 42.50m, AccommodationType = Accommodation.AccommodationType.Hotel },
                new Tariff { TariffId = 9, Name = "Hotelkamer_2Personen", Price = 55.00m, AccommodationType = Accommodation.AccommodationType.Hotel },
                new Tariff { TariffId = 10, Name = "Hotelkamer_3Personen", Price = 70.00m, AccommodationType = Accommodation.AccommodationType.Hotel },
                new Tariff { TariffId = 11, Name = "Hotelkamer_4personen", Price = 88.00m, AccommodationType = Accommodation.AccommodationType.Hotel },
                new Tariff { TariffId = 12, Name = "Hotelkamer_5personen", Price = 105.50m, AccommodationType = Accommodation.AccommodationType.Hotel },
                new Tariff { TariffId = 13, Name = "Toeristenbelasting", Price = 0.50m, AccommodationType = Accommodation.AccommodationType.Hotel }
                
            );


            // db seeden met accommodaties
            modelBuilder.Entity<Accommodation>().HasData(
                new Accommodation { AccommodationId = 1, PlaceNumber = "1A", Capacity = 4, Type = Accommodation.AccommodationType.Camping, },
                new Accommodation { AccommodationId = 2, PlaceNumber = "2A", Capacity = 4, Type = Accommodation.AccommodationType.Camping, },
                new Accommodation { AccommodationId = 3, PlaceNumber = "3A", Capacity = 4, Type = Accommodation.AccommodationType.Camping },
                new Accommodation { AccommodationId = 4, PlaceNumber = "4A", Capacity = 4, Type = Accommodation.AccommodationType.Camping },                               
                new Accommodation { AccommodationId = 5, PlaceNumber = "5A", Capacity = 4, Type = Accommodation.AccommodationType.Camping }
            );

            modelBuilder.Entity<Accommodation>().HasData(
                new Accommodation { AccommodationId = 6, PlaceNumber = "101", Capacity = 1, Type = Accommodation.AccommodationType.Hotel }, 
                new Accommodation { AccommodationId = 7, PlaceNumber = "201", Capacity = 2, Type = Accommodation.AccommodationType.Hotel }, 
                new Accommodation { AccommodationId = 8, PlaceNumber = "202", Capacity = 2, Type = Accommodation.AccommodationType.Hotel }, 
                new Accommodation { AccommodationId = 9, PlaceNumber = "301", Capacity = 3, Type = Accommodation.AccommodationType.Hotel }, 
                new Accommodation { AccommodationId = 10, PlaceNumber = "304", Capacity = 4, Type = Accommodation.AccommodationType.Hotel }, 
                new Accommodation { AccommodationId = 11, PlaceNumber = "307", Capacity = 5, Type = Accommodation.AccommodationType.Hotel } 
            );


            // database moet string opslaan ipv int for enum waarden 
            modelBuilder.Entity<Account>()
            .Property(a => a.AccountRole)
            .HasConversion<string>()
            .HasMaxLength(10);


            // relaties en FKs

            // inheritance configuratie 
            modelBuilder.Entity<Reservation>()
                .HasDiscriminator<string>("ReservationType")
                .HasValue<CampingReservation>("Camping")
                .HasValue<HotelReservation>("Hotel");

            // customer 1 - 0..* reservation
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Customer) // een reservation heeft precies 1 customer
                .WithMany(c => c.Reservations) // een customer kan meerdere reservations hebben
                .HasForeignKey(r => r.CustomerId);


            // reservation 0..* - 1..* accommodation
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Accommodations) // een reservation heeft 1 of meer accommodaties 
                .WithMany(a => a.Reservations); // een accommodatie kan meerdere reservations gehad hebben
                
            
            // account 0..1 - 0..1 customer
            // account kan een customer hebben, maar hoeft niet. (Bijv medewerker, admin, eigenaar)
            // Een customer hoeft ook niet altijd een account te hebben (bijv telefonisch en op locatie reserveren)
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Customer)
                .WithOne() // customer gebruikt geen account navigation property (om loops te voorkomen) maar relatie is nog steeds 0..1 - 0..1
                .HasForeignKey<Account>(a => a.CustomerId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade); // Als een customer gedelete wordt, wordt het gelinkte account ook ge-delete (anders zijn er accounts in een invalid state)


            
            modelBuilder.Entity<Reservation>()
                .Property(r => r.TotalPrice)
                .HasPrecision(10, 2);  // max 10 karakters, 2 decimalen

            modelBuilder.Entity<Tariff>()
                .Property(t => t.Price)
                .HasPrecision(10, 2);


            // constraints

           // elke username moet uniek zijn
            modelBuilder.Entity<Account>()
             .HasIndex(a => a.Username)
             .IsUnique();
        }
    }
}
