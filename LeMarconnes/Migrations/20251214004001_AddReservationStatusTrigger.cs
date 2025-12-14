using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeMarconnes.Migrations
{
    public partial class AddReservationStatusTrigger : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR ALTER PROCEDURE sp_UpdateReserveringStatussen
                AS
                BEGIN
                    SET NOCOUNT ON;
                    
                    UPDATE Reservations 
                    SET Status = 
                        CASE 
                            WHEN GETDATE() < StartDate THEN 'Gereserveerd'
                            WHEN GETDATE() BETWEEN StartDate AND EndDate THEN 'Actief'
                            ELSE 'Verlopen'
                        END
                    WHERE Status != 
                        CASE 
                            WHEN GETDATE() < StartDate THEN 'Gereserveerd'
                            WHEN GETDATE() BETWEEN StartDate AND EndDate THEN 'Actief'
                            ELSE 'Verlopen'
                        END
                    
                    PRINT CONCAT('Statussen bijgewerkt op: ', GETDATE());
                END
                GO
            ");

            migrationBuilder.Sql("EXEC sp_UpdateReserveringStatussen;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_UpdateReserveringStatussen;");
        }
    }
}