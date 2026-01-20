using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 20, 2, 27, 58, 883, DateTimeKind.Local).AddTicks(4917));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 20, 2, 27, 58, 883, DateTimeKind.Local).AddTicks(4977));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 20, 2, 27, 58, 883, DateTimeKind.Local).AddTicks(4967));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 20, 2, 27, 58, 883, DateTimeKind.Local).AddTicks(4992));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 19, 22, 54, 29, 695, DateTimeKind.Local).AddTicks(4186));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 19, 22, 54, 29, 695, DateTimeKind.Local).AddTicks(4225));

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 3,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 19, 22, 54, 29, 695, DateTimeKind.Local).AddTicks(4215));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 19, 22, 54, 29, 695, DateTimeKind.Local).AddTicks(4240));
        }
    }
}
