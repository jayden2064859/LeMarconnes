using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccommodationTypeId",
                table: "Reservations");

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

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "AccountId", "AccountRole", "CustomerId", "PasswordHash", "RegistrationDate", "Username" },
                values: new object[] { 3, "Employee", null, "AQAAAAIAAYagAAAAEJkbsW3FiATzLlh0GWtFksdZjlDSF6B4FCQvRoSbI9k2kSYzKDnSHFrYKNkhsTxKqw==", new DateTime(2026, 1, 15, 20, 21, 22, 419, DateTimeKind.Local).AddTicks(1720), "Employee" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 20, 21, 22, 419, DateTimeKind.Local).AddTicks(1744));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "AccommodationTypeId",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 0, 8, 13, 834, DateTimeKind.Local).AddTicks(5961));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 0, 8, 13, 834, DateTimeKind.Local).AddTicks(6014));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 15, 0, 8, 13, 834, DateTimeKind.Local).AddTicks(6030));
        }
    }
}
