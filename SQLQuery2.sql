DECLARE @startDate DATE = GETDATE();
DECLARE @endDate DATE = DATEADD(day, 365, @startDate);

WHILE @startDate < @endDate
BEGIN
    -- Insert a flight for Ankara
    INSERT INTO Flights (DepartureAirportId, DestinationAirportId, Quota, DepartureTime, ArrivalTime)
    VALUES (
        (SELECT TOP 1 Id FROM Cities WHERE city = 'Ankara'),
        (SELECT TOP 1 Id FROM Cities WHERE city != 'Ankara' ORDER BY NEWID()),
        ABS(CHECKSUM(NEWID())) % 200 + 1,
        DATEADD(hour, ABS(CHECKSUM(NEWID())) % 24, CAST(@startDate AS DATETIME)),
        DATEADD(hour, ABS(CHECKSUM(NEWID())) % 24 + 1, CAST(@startDate AS DATETIME))
    );

    -- Insert a flight for Istanbul
    INSERT INTO Flights (DepartureAirportId, DestinationAirportId, Quota, DepartureTime, ArrivalTime)
    VALUES (
        (SELECT TOP 1 Id FROM Cities WHERE city = 'İstanbul'),
        (SELECT TOP 1 Id FROM Cities WHERE city != 'İstanbul' ORDER BY NEWID()),
        ABS(CHECKSUM(NEWID())) % 200 + 1,
        DATEADD(hour, ABS(CHECKSUM(NEWID())) % 24, CAST(@startDate AS DATETIME)),
        DATEADD(hour, ABS(CHECKSUM(NEWID())) % 24 + 1, CAST(@startDate AS DATETIME))
    );

    -- Insert a flight for İzmir
    INSERT INTO Flights (DepartureAirportId, DestinationAirportId, Quota, DepartureTime, ArrivalTime)
    VALUES (
        (SELECT TOP 1 Id FROM Cities WHERE city = 'İzmir'),
        (SELECT TOP 1 Id FROM Cities WHERE city != 'İzmir' ORDER BY NEWID()),
        ABS(CHECKSUM(NEWID())) % 200 + 1,
        DATEADD(hour, ABS(CHECKSUM(NEWID())) % 24, CAST(@startDate AS DATETIME)),
        DATEADD(hour, ABS(CHECKSUM(NEWID())) % 24 + 1, CAST(@startDate AS DATETIME))
    );

    SET @startDate = DATEADD(day, 1, @startDate);
END;