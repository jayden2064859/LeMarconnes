
-- voer dit script uit in je eigen DB 
-- (belangrijk voor de integriteit van Status property voor Reserveringen
IF NOT EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_UpdateReservationStatuses')
BEGIN
    EXEC('
    CREATE PROCEDURE sp_UpdateReservationStatuses
    AS
    BEGIN
        SET NOCOUNT ON;
        
        UPDATE Reservations 
        SET Status = 
            CASE 
                WHEN GETDATE() < StartDate THEN ''Gereserveerd''
                WHEN GETDATE() BETWEEN StartDate AND EndDate THEN ''Actief''
                ELSE ''Verlopen''
            END
        WHERE Status != 
            CASE 
                WHEN GETDATE() < StartDate THEN ''Gereserveerd''
                WHEN GETDATE() BETWEEN StartDate AND EndDate THEN ''Actief''
                ELSE ''Verlopen''
            END
        
        PRINT ''Reservation statuses updated: '' + CONVERT(VARCHAR, @@ROWCOUNT) + '' rows affected'';
    END
    ')
    PRINT 'Stored procedure sp_UpdateReservationStatuses created';
END
ELSE
BEGIN
    PRINT 'Stored procedure sp_UpdateReservationStatuses already exists';
END
GO