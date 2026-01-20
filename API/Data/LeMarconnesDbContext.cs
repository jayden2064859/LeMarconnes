using ClassLibrary.Models;
using Microsoft.EntityFrameworkCore;


namespace ClassLibrary.Data
{
    public class LeMarconnesDbContext : DbContext
    {
        public LeMarconnesDbContext(DbContextOptions<LeMarconnesDbContext> options) : base(options) 
        {        
        }

        // initialise alle models die aan de db toegevoegd moeten worden
        public DbSet<Accommodation> Accommodations { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Tariff> Tariffs { get; set; }
        public DbSet<AccommodationType> AccommodationTypes { get; set; }


        // OnModelCreating is standaard een override method, omdat we de parent class DbContext gebruiken
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // db seeden met camping tarieven 
            modelBuilder.Entity<Tariff>().HasData(
                new Tariff { TariffId = 1, Name = "Campingplaats", Price = 7.50m, AccommodationTypeId = 1 },
                new Tariff { TariffId = 2, Name = "Volwassene", Price = 6.00m, AccommodationTypeId = 1 },
                new Tariff { TariffId = 3, Name = "Kind_0_7", Price = 4.00m, AccommodationTypeId = 1 },
                new Tariff { TariffId = 4, Name = "Kind_7_12", Price = 5.00m, AccommodationTypeId = 1 },
                new Tariff { TariffId = 5, Name = "Hond", Price = 2.50m, AccommodationTypeId = 1 },
                new Tariff { TariffId = 6, Name = "Electriciteit", Price = 7.50m, AccommodationTypeId = 1 },
                new Tariff { TariffId = 7, Name = "Toeristenbelasting", Price = 0.25m, AccommodationTypeId = 1 }
            );

            // db seeden met hotel tarieven 
            modelBuilder.Entity<Tariff>().HasData(
                new Tariff { TariffId = 8, Name = "Hotelkamer_1Persoon", Price = 42.50m, AccommodationTypeId = 2 },
                new Tariff { TariffId = 9, Name = "Hotelkamer_2Personen", Price = 55.00m, AccommodationTypeId = 2 },
                new Tariff { TariffId = 10, Name = "Hotelkamer_3Personen", Price = 70.00m, AccommodationTypeId = 2 },
                new Tariff { TariffId = 11, Name = "Hotelkamer_4personen", Price = 88.00m, AccommodationTypeId = 2 },
                new Tariff { TariffId = 12, Name = "Hotelkamer_5personen", Price = 105.50m, AccommodationTypeId = 2 },
                new Tariff { TariffId = 13, Name = "Toeristenbelasting", Price = 0.50m, AccommodationTypeId = 2 }

            );

            // db seeden met accommodatie types (hotel, camping)
            modelBuilder.Entity<AccommodationType>().HasData(
                new AccommodationType { AccommodationTypeId = 1, TypeName = "Camping" },
                new AccommodationType { AccommodationTypeId = 2, TypeName = "Hotel" }

            );


            // db seeden met accommodaties
            modelBuilder.Entity<Accommodation>().HasData(
                new Accommodation { AccommodationId = 1, PlaceNumber = "1A", Capacity = 4, AccommodationTypeId = 1 },
                new Accommodation { AccommodationId = 2, PlaceNumber = "2A", Capacity = 4, AccommodationTypeId = 1 },
                new Accommodation { AccommodationId = 3, PlaceNumber = "3A", Capacity = 4, AccommodationTypeId = 1 },
                new Accommodation { AccommodationId = 4, PlaceNumber = "4A", Capacity = 4, AccommodationTypeId = 1 },
                new Accommodation { AccommodationId = 5, PlaceNumber = "5A", Capacity = 4, AccommodationTypeId = 1 }
            );

            modelBuilder.Entity<Accommodation>().HasData(
                new Accommodation { AccommodationId = 6, PlaceNumber = "101", Capacity = 1, AccommodationTypeId = 2 },
                new Accommodation { AccommodationId = 7, PlaceNumber = "201", Capacity = 2, AccommodationTypeId = 2 },
                new Accommodation { AccommodationId = 8, PlaceNumber = "202", Capacity = 2, AccommodationTypeId = 2 },
                new Accommodation { AccommodationId = 9, PlaceNumber = "301", Capacity = 3, AccommodationTypeId = 2 },
                new Accommodation { AccommodationId = 10, PlaceNumber = "304", Capacity = 4, AccommodationTypeId = 2 },
                new Accommodation { AccommodationId = 11, PlaceNumber = "307", Capacity = 5, AccommodationTypeId = 2 }
            );

            // account table seeden met admin, employee en customer accounts voor api endpoint tests
            
            // admin account - toegang tot alle endpoints
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    AccountId = 1,
                    Username = "Admin",
                    PasswordHash = "AQAAAAIAAYagAAAAED40poWknsiW1HtrueqpONicGpEl+0PpLBHkmcd2Pia8jyo2ZarTY7CqSz8gfUyPLQ==", // wachtwoord is 'admin'
                    AccountRole = Account.Role.Admin,
                    RegistrationDate = DateTime.Now,
                    CustomerId = null
                }
            );

            // employee account - heeft toegang tot alle publieke endpoints + GET voor Account + Customer
            modelBuilder.Entity<Account>().HasData(
            new Account
            {
                AccountId = 3,
                Username = "Employee",
                PasswordHash = "AQAAAAIAAYagAAAAEJkbsW3FiATzLlh0GWtFksdZjlDSF6B4FCQvRoSbI9k2kSYzKDnSHFrYKNkhsTxKqw==", // wachtwoord is '1234"
                AccountRole = Account.Role.Employee,
                RegistrationDate = DateTime.Now,
                CustomerId = null
            });

            // customer account - toegang tot alle publieke endpoints + POST reservering endpoints
            modelBuilder.Entity<Account>().HasData(
                new Account
                {
                    AccountId = 2,
                    Username = "Customer",
                    PasswordHash = "AQAAAAIAAYagAAAAEJkbsW3FiATzLlh0GWtFksdZjlDSF6B4FCQvRoSbI9k2kSYzKDnSHFrYKNkhsTxKqw==", // wachtwoord is '1234'
                    AccountRole = Account.Role.Customer,
                    RegistrationDate = DateTime.Now,
                    CustomerId = 1
                });

            modelBuilder.Entity<Customer>().HasData(
              new Customer
              {
                  CustomerId = 1,
                  FirstName = "Test",
                  Infix = null,
                  LastName = "Customer",
                  Email = "test.customer@gmail.com",
                  Phone = "0612345678",
                  RegistrationDate = DateTime.Now
              });

            // database moet string opslaan ipv int for enum waarden voor accountrol (duidelijker)
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

            // Accommodation 0..* - 1 AccommodationType 
            modelBuilder.Entity<Accommodation>()
                .HasOne(t => t.AccommodationType)
                .WithMany();

            // Tariff 0..* - 1 AccommodationType
            modelBuilder.Entity<Tariff>()
                .HasOne(t => t.AccommodationType)
                .WithMany(); 

            // customer 1 - 0..* reservation
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Customer) // een reservation heeft precies 1 customer
                .WithMany() // een customer kan meerdere reservations hebben (geen navigation property vanuit Customer om navigation loops te voorkomen)
                .HasForeignKey(r => r.CustomerId);


            // reservation 0..* - 1..* accommodation
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Accommodations) // een reservation heeft 1 of meer accommodaties 
                .WithMany(); // een accommodatie kan meerdere reservations gehad hebben  (geen navigation property vanuit Accommodation om navigation loops te voorkomen)


            // account 0..1 - 0..1 customer
            // Een customer hoeft niet altijd een account te hebben (bijv telefonisch en op locatie reserveren)
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

            // elk emailadres moet uniek zijn
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // elk telefoonnummer moet uniek zijn
            modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Phone)
            .IsUnique();

        }
    }
}
