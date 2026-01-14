using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class m4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "RegistrationDate" },
                values: new object[] { "AQAAAAIAAYagAAAAED40poWknsiW1HtrueqpONicGpEl+0PpLBHkmcd2Pia8jyo2ZarTY7CqSz8gfUyPLQ==", new DateTime(2026, 1, 13, 22, 1, 52, 379, DateTimeKind.Local).AddTicks(5739) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                columns: new[] { "PasswordHash", "RegistrationDate" },
                values: new object[] { "$2y$10$Ns/.TfVAGFJEH64fwbIxN.n1jI366DsS4mKl9AmWx/p50wt0WlvyG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });
        }
    }
}
