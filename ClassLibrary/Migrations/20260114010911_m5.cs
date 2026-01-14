using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class m5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 14, 2, 9, 11, 373, DateTimeKind.Local).AddTicks(4371));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 13, 22, 1, 52, 379, DateTimeKind.Local).AddTicks(5739));
        }
    }
}
