using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class m1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accommodations",
                columns: table => new
                {
                    AccommodationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accommodations", x => x.AccommodationId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Infix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Tariffs",
                columns: table => new
                {
                    TariffId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    AccommodationType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tariffs", x => x.TariffId);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountRole = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AccommodationTypeId = table.Column<int>(type: "int", nullable: false),
                    ReservationType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    AdultsCount = table.Column<int>(type: "int", nullable: true),
                    Children0_7Count = table.Column<int>(type: "int", nullable: true),
                    Children7_12Count = table.Column<int>(type: "int", nullable: true),
                    DogsCount = table.Column<int>(type: "int", nullable: true),
                    HasElectricity = table.Column<bool>(type: "bit", nullable: true),
                    ElectricityDays = table.Column<int>(type: "int", nullable: true),
                    PersonCount = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationReservation",
                columns: table => new
                {
                    AccommodationsAccommodationId = table.Column<int>(type: "int", nullable: false),
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationReservation", x => new { x.AccommodationsAccommodationId, x.ReservationId });
                    table.ForeignKey(
                        name: "FK_AccommodationReservation_Accommodations_AccommodationsAccommodationId",
                        column: x => x.AccommodationsAccommodationId,
                        principalTable: "Accommodations",
                        principalColumn: "AccommodationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccommodationReservation_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Accommodations",
                columns: new[] { "AccommodationId", "Capacity", "PlaceNumber", "Type" },
                values: new object[,]
                {
                    { 1, 4, "1A", "Camping" },
                    { 2, 4, "2A", "Camping" },
                    { 3, 4, "3A", "Camping" },
                    { 4, 4, "4A", "Camping" },
                    { 5, 4, "5A", "Camping" },
                    { 6, 1, "101", "Hotel" },
                    { 7, 2, "201", "Hotel" },
                    { 8, 2, "202", "Hotel" },
                    { 9, 3, "301", "Hotel" },
                    { 10, 4, "304", "Hotel" },
                    { 11, 5, "307", "Hotel" }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountRole", "CustomerId", "PasswordHash", "RegistrationDate", "Username" },
                values: new object[] { 1, "Admin", null, "AQAAAAIAAYagAAAAED40poWknsiW1HtrueqpONicGpEl+0PpLBHkmcd2Pia8jyo2ZarTY7CqSz8gfUyPLQ==", new DateTime(2026, 1, 15, 0, 8, 13, 834, DateTimeKind.Local).AddTicks(5961), "Admin" });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Email", "FirstName", "Infix", "LastName", "Phone", "RegistrationDate" },
                values: new object[] { 1, "test.customer@gmail.com", "Test", null, "Customer", "0612345678", new DateTime(2026, 1, 15, 0, 8, 13, 834, DateTimeKind.Local).AddTicks(6030) });

            migrationBuilder.InsertData(
                table: "Tariffs",
                columns: new[] { "TariffId", "AccommodationType", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Camping", "Campingplaats", 7.50m },
                    { 2, "Camping", "Volwassene", 6.00m },
                    { 3, "Camping", "Kind_0_7", 4.00m },
                    { 4, "Camping", "Kind_7_12", 5.00m },
                    { 5, "Camping", "Hond", 2.50m },
                    { 6, "Camping", "Electriciteit", 7.50m },
                    { 7, "Camping", "Toeristenbelasting", 0.25m },
                    { 8, "Hotel", "Hotelkamer_1Persoon", 42.50m },
                    { 9, "Hotel", "Hotelkamer_2Personen", 55.00m },
                    { 10, "Hotel", "Hotelkamer_3Personen", 70.00m },
                    { 11, "Hotel", "Hotelkamer_4personen", 88.00m },
                    { 12, "Hotel", "Hotelkamer_5personen", 105.50m },
                    { 13, "Hotel", "Toeristenbelasting", 0.50m }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountRole", "CustomerId", "PasswordHash", "RegistrationDate", "Username" },
                values: new object[] { 2, "Customer", 1, "AQAAAAIAAYagAAAAEJkbsW3FiATzLlh0GWtFksdZjlDSF6B4FCQvRoSbI9k2kSYzKDnSHFrYKNkhsTxKqw==", new DateTime(2026, 1, 15, 0, 8, 13, 834, DateTimeKind.Local).AddTicks(6014), "Customer" });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationReservation_ReservationId",
                table: "AccommodationReservation",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CustomerId",
                table: "Accounts",
                column: "CustomerId",
                unique: true,
                filter: "[CustomerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Username",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationReservation");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Tariffs");

            migrationBuilder.DropTable(
                name: "Accommodations");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
