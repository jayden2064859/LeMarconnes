using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LeMarconnes.Migrations
{
    /// <inheritdoc />
    public partial class AddStaticData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AccommodationsTypes",
                columns: new[] { "AccommodationTypeId", "Name" },
                values: new object[,]
                {
                    { 1, "Camping" },
                    { 2, "Hotel" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AccommodationsTypes",
                keyColumn: "AccommodationTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AccommodationsTypes",
                keyColumn: "AccommodationTypeId",
                keyValue: 2);
        }
    }
}
