CREATE OR ALTER PROCEDURE sp_UpdateAllStatuses
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    
    -- update reserveringen naar VERLOPEN (einddatum geweest)
    UPDATE Reservations 
    SET CurrentStatus = 'Verlopen'
    WHERE CurrentStatus IN ('Gereserveerd', 'Actief')
      AND EndDate < @Today;
    
    -- update reserveringen naar ACTIEF (vandaag binnen periode)
    UPDATE Reservations 
    SET CurrentStatus = 'Actief'
    WHERE CurrentStatus = 'Gereserveerd'
      AND @Today BETWEEN StartDate AND EndDate;
    
    -- update accommodaties terug naar BESCHIKBAAR (van verlopen reserveringen)
    UPDATE a
    SET a.CurrentStatus = 'Beschikbaar'
    FROM Accommodations a
    WHERE a.CurrentStatus = 'Bezet'
      AND EXISTS (
          SELECT 1 
          FROM AccommodationReservation ar
          INNER JOIN Reservations r ON ar.ReservationsReservationId = r.ReservationId
          WHERE ar.AccommodationsAccommodationId = a.AccommodationId
            AND r.CurrentStatus = 'Verlopen'
      );
END
GO