using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class m3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccommodationType",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Accommodations");

            migrationBuilder.AddColumn<int>(
                name: "AccommodationTypeId",
                table: "Tariffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AccommodationTypeId",
                table: "Accommodations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AccommodationType",
                columns: table => new
                {
                    AccommodationTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationType", x => x.AccommodationTypeId);
                });

            migrationBuilder.InsertData(
                table: "AccommodationType",
                columns: new[] { "AccommodationTypeId", "TypeName" },
                values: new object[,]
                {
                    { 1, "Camping" },
                    { 2, "Hotel" }
                });

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 1,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 2,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 3,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 4,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 5,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 6,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 7,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 8,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 9,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 10,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 11,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 16, 12, 49, 52, 574, DateTimeKind.Local).AddTicks(9422));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 16, 12, 49, 52, 574, DateTimeKind.Local).AddTicks(9476));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 16, 12, 49, 52, 574, DateTimeKind.Local).AddTicks(9469));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 16, 12, 49, 52, 574, DateTimeKind.Local).AddTicks(9487));

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 1,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 2,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 3,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 4,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 5,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 6,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 7,
                column: "AccommodationTypeId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 8,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 9,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 10,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 11,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 12,
                column: "AccommodationTypeId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 13,
                column: "AccommodationTypeId",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationType");

            migrationBuilder.DropColumn(
                name: "AccommodationTypeId",
                table: "Tariffs");

            migrationBuilder.DropColumn(
                name: "AccommodationTypeId",
                table: "Accommodations");

            migrationBuilder.AddColumn<string>(
                name: "AccommodationType",
                table: "Tariffs",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Accommodations",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 1,
                column: "Type",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 2,
                column: "Type",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 3,
                column: "Type",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 4,
                column: "Type",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 5,
                column: "Type",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 6,
                column: "Type",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 7,
                column: "Type",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 8,
                column: "Type",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 9,
                column: "Type",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 10,
                column: "Type",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Accommodations",
                keyColumn: "AccommodationId",
                keyValue: 11,
                column: "Type",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 20, 21, 22, 419, DateTimeKind.Local).AddTicks(1673));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 20, 21, 22, 419, DateTimeKind.Local).AddTicks(1728));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 20, 21, 22, 419, DateTimeKind.Local).AddTicks(1720));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 20, 21, 22, 419, DateTimeKind.Local).AddTicks(1744));

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 1,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 2,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 3,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 4,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 5,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 6,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 7,
                column: "AccommodationType",
                value: "Camping");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 8,
                column: "AccommodationType",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 9,
                column: "AccommodationType",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 10,
                column: "AccommodationType",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 11,
                column: "AccommodationType",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 12,
                column: "AccommodationType",
                value: "Hotel");

            migrationBuilder.UpdateData(
                table: "Tariffs",
                keyColumn: "TariffId",
                keyValue: 13,
                column: "AccommodationType",
                value: "Hotel");
        }
    }
}
