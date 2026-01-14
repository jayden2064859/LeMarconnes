using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClassLibrary.Migrations
{
    /// <inheritdoc />
    public partial class m6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccommodationReservation_Reservations_ReservationsReservationId",
                table: "AccommodationReservation");

            migrationBuilder.RenameColumn(
                name: "ReservationsReservationId",
                table: "AccommodationReservation",
                newName: "ReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_AccommodationReservation_ReservationsReservationId",
                table: "AccommodationReservation",
                newName: "IX_AccommodationReservation_ReservationId");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 14, 2, 15, 6, 815, DateTimeKind.Local).AddTicks(4127));

            migrationBuilder.AddForeignKey(
                name: "FK_AccommodationReservation_Reservations_ReservationId",
                table: "AccommodationReservation",
                column: "ReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AccommodationReservation_Reservations_ReservationId",
                table: "AccommodationReservation");

            migrationBuilder.RenameColumn(
                name: "ReservationId",
                table: "AccommodationReservation",
                newName: "ReservationsReservationId");

            migrationBuilder.RenameIndex(
                name: "IX_AccommodationReservation_ReservationId",
                table: "AccommodationReservation",
                newName: "IX_AccommodationReservation_ReservationsReservationId");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "AccountId",
                keyValue: 2,
                column: "RegistrationDate",
                value: new DateTime(2026, 1, 14, 2, 9, 11, 373, DateTimeKind.Local).AddTicks(4371));

            migrationBuilder.AddForeignKey(
                name: "FK_AccommodationReservation_Reservations_ReservationsReservationId",
                table: "AccommodationReservation",
                column: "ReservationsReservationId",
                principalTable: "Reservations",
                principalColumn: "ReservationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
