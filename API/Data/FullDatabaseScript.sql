IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AccommodationTypes] (
    [AccommodationTypeId] int NOT NULL IDENTITY,
    [TypeName] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_AccommodationTypes] PRIMARY KEY ([AccommodationTypeId])
);
GO

CREATE TABLE [Customers] (
    [CustomerId] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [Infix] nvarchar(max) NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [RegistrationDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
);
GO

CREATE TABLE [Accommodations] (
    [AccommodationId] int NOT NULL IDENTITY,
    [PlaceNumber] nvarchar(max) NOT NULL,
    [Capacity] int NOT NULL,
    [AccommodationTypeId] int NOT NULL,
    CONSTRAINT [PK_Accommodations] PRIMARY KEY ([AccommodationId]),
    CONSTRAINT [FK_Accommodations_AccommodationTypes_AccommodationTypeId] FOREIGN KEY ([AccommodationTypeId]) REFERENCES [AccommodationTypes] ([AccommodationTypeId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Tariffs] (
    [TariffId] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Price] decimal(10,2) NOT NULL,
    [AccommodationTypeId] int NOT NULL,
    CONSTRAINT [PK_Tariffs] PRIMARY KEY ([TariffId]),
    CONSTRAINT [FK_Tariffs_AccommodationTypes_AccommodationTypeId] FOREIGN KEY ([AccommodationTypeId]) REFERENCES [AccommodationTypes] ([AccommodationTypeId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Accounts] (
    [AccountId] int NOT NULL IDENTITY,
    [Username] nvarchar(450) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    [AccountRole] nvarchar(10) NOT NULL,
    [RegistrationDate] datetime2 NOT NULL,
    [CustomerId] int NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountId]),
    CONSTRAINT [FK_Accounts_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Reservations] (
    [ReservationId] int NOT NULL IDENTITY,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [TotalPrice] decimal(10,2) NOT NULL,
    [RegistrationDate] datetime2 NOT NULL,
    [CustomerId] int NOT NULL,
    [ReservationType] nvarchar(13) NOT NULL,
    [AdultsCount] int NULL,
    [Children0_7Count] int NULL,
    [Children7_12Count] int NULL,
    [DogsCount] int NULL,
    [HasElectricity] bit NULL,
    [ElectricityDays] int NULL,
    [PersonCount] int NULL,
    CONSTRAINT [PK_Reservations] PRIMARY KEY ([ReservationId]),
    CONSTRAINT [FK_Reservations_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([CustomerId]) ON DELETE CASCADE
);
GO

CREATE TABLE [AccommodationReservation] (
    [AccommodationsAccommodationId] int NOT NULL,
    [ReservationId] int NOT NULL,
    CONSTRAINT [PK_AccommodationReservation] PRIMARY KEY ([AccommodationsAccommodationId], [ReservationId]),
    CONSTRAINT [FK_AccommodationReservation_Accommodations_AccommodationsAccommodationId] FOREIGN KEY ([AccommodationsAccommodationId]) REFERENCES [Accommodations] ([AccommodationId]) ON DELETE CASCADE,
    CONSTRAINT [FK_AccommodationReservation_Reservations_ReservationId] FOREIGN KEY ([ReservationId]) REFERENCES [Reservations] ([ReservationId]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccommodationTypeId', N'TypeName') AND [object_id] = OBJECT_ID(N'[AccommodationTypes]'))
    SET IDENTITY_INSERT [AccommodationTypes] ON;
INSERT INTO [AccommodationTypes] ([AccommodationTypeId], [TypeName])
VALUES (1, N'Camping'),
(2, N'Hotel');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccommodationTypeId', N'TypeName') AND [object_id] = OBJECT_ID(N'[AccommodationTypes]'))
    SET IDENTITY_INSERT [AccommodationTypes] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccountId', N'AccountRole', N'CustomerId', N'PasswordHash', N'RegistrationDate', N'Username') AND [object_id] = OBJECT_ID(N'[Accounts]'))
    SET IDENTITY_INSERT [Accounts] ON;
INSERT INTO [Accounts] ([AccountId], [AccountRole], [CustomerId], [PasswordHash], [RegistrationDate], [Username])
VALUES (1, N'Admin', NULL, N'AQAAAAIAAYagAAAAED40poWknsiW1HtrueqpONicGpEl+0PpLBHkmcd2Pia8jyo2ZarTY7CqSz8gfUyPLQ==', '2026-01-19T22:54:29.6954186+01:00', N'Admin'),
(3, N'Employee', NULL, N'AQAAAAIAAYagAAAAEJkbsW3FiATzLlh0GWtFksdZjlDSF6B4FCQvRoSbI9k2kSYzKDnSHFrYKNkhsTxKqw==', '2026-01-19T22:54:29.6954215+01:00', N'Employee');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccountId', N'AccountRole', N'CustomerId', N'PasswordHash', N'RegistrationDate', N'Username') AND [object_id] = OBJECT_ID(N'[Accounts]'))
    SET IDENTITY_INSERT [Accounts] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CustomerId', N'Email', N'FirstName', N'Infix', N'LastName', N'Phone', N'RegistrationDate') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] ON;
INSERT INTO [Customers] ([CustomerId], [Email], [FirstName], [Infix], [LastName], [Phone], [RegistrationDate])
VALUES (1, N'test.customer@gmail.com', N'Test', NULL, N'Customer', N'0612345678', '2026-01-19T22:54:29.6954240+01:00');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CustomerId', N'Email', N'FirstName', N'Infix', N'LastName', N'Phone', N'RegistrationDate') AND [object_id] = OBJECT_ID(N'[Customers]'))
    SET IDENTITY_INSERT [Customers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccommodationId', N'AccommodationTypeId', N'Capacity', N'PlaceNumber') AND [object_id] = OBJECT_ID(N'[Accommodations]'))
    SET IDENTITY_INSERT [Accommodations] ON;
INSERT INTO [Accommodations] ([AccommodationId], [AccommodationTypeId], [Capacity], [PlaceNumber])
VALUES (1, 1, 4, N'1A'),
(2, 1, 4, N'2A'),
(3, 1, 4, N'3A'),
(4, 1, 4, N'4A'),
(5, 1, 4, N'5A'),
(6, 2, 1, N'101'),
(7, 2, 2, N'201'),
(8, 2, 2, N'202'),
(9, 2, 3, N'301'),
(10, 2, 4, N'304'),
(11, 2, 5, N'307');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccommodationId', N'AccommodationTypeId', N'Capacity', N'PlaceNumber') AND [object_id] = OBJECT_ID(N'[Accommodations]'))
    SET IDENTITY_INSERT [Accommodations] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccountId', N'AccountRole', N'CustomerId', N'PasswordHash', N'RegistrationDate', N'Username') AND [object_id] = OBJECT_ID(N'[Accounts]'))
    SET IDENTITY_INSERT [Accounts] ON;
INSERT INTO [Accounts] ([AccountId], [AccountRole], [CustomerId], [PasswordHash], [RegistrationDate], [Username])
VALUES (2, N'Customer', 1, N'AQAAAAIAAYagAAAAEJkbsW3FiATzLlh0GWtFksdZjlDSF6B4FCQvRoSbI9k2kSYzKDnSHFrYKNkhsTxKqw==', '2026-01-19T22:54:29.6954225+01:00', N'Customer');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'AccountId', N'AccountRole', N'CustomerId', N'PasswordHash', N'RegistrationDate', N'Username') AND [object_id] = OBJECT_ID(N'[Accounts]'))
    SET IDENTITY_INSERT [Accounts] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'TariffId', N'AccommodationTypeId', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Tariffs]'))
    SET IDENTITY_INSERT [Tariffs] ON;
INSERT INTO [Tariffs] ([TariffId], [AccommodationTypeId], [Name], [Price])
VALUES (1, 1, N'Campingplaats', 7.5),
(2, 1, N'Volwassene', 6.0),
(3, 1, N'Kind_0_7', 4.0),
(4, 1, N'Kind_7_12', 5.0),
(5, 1, N'Hond', 2.5),
(6, 1, N'Electriciteit', 7.5),
(7, 1, N'Toeristenbelasting', 0.25),
(8, 2, N'Hotelkamer_1Persoon', 42.5),
(9, 2, N'Hotelkamer_2Personen', 55.0),
(10, 2, N'Hotelkamer_3Personen', 70.0),
(11, 2, N'Hotelkamer_4personen', 88.0),
(12, 2, N'Hotelkamer_5personen', 105.5),
(13, 2, N'Toeristenbelasting', 0.5);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'TariffId', N'AccommodationTypeId', N'Name', N'Price') AND [object_id] = OBJECT_ID(N'[Tariffs]'))
    SET IDENTITY_INSERT [Tariffs] OFF;
GO

CREATE INDEX [IX_AccommodationReservation_ReservationId] ON [AccommodationReservation] ([ReservationId]);
GO

CREATE INDEX [IX_Accommodations_AccommodationTypeId] ON [Accommodations] ([AccommodationTypeId]);
GO

CREATE UNIQUE INDEX [IX_Accounts_CustomerId] ON [Accounts] ([CustomerId]) WHERE [CustomerId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Accounts_Username] ON [Accounts] ([Username]);
GO

CREATE INDEX [IX_Reservations_CustomerId] ON [Reservations] ([CustomerId]);
GO

CREATE INDEX [IX_Tariffs_AccommodationTypeId] ON [Tariffs] ([AccommodationTypeId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260119215429_InitialCreate', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

UPDATE [Accounts] SET [RegistrationDate] = '2026-01-20T02:27:58.8834917+01:00'
WHERE [AccountId] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Accounts] SET [RegistrationDate] = '2026-01-20T02:27:58.8834977+01:00'
WHERE [AccountId] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Accounts] SET [RegistrationDate] = '2026-01-20T02:27:58.8834967+01:00'
WHERE [AccountId] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Customers] SET [RegistrationDate] = '2026-01-20T02:27:58.8834992+01:00'
WHERE [CustomerId] = 1;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260120012759_m2', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Phone');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Customers] ALTER COLUMN [Phone] nvarchar(450) NOT NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Email');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Customers] ALTER COLUMN [Email] nvarchar(450) NOT NULL;
GO

UPDATE [Accounts] SET [RegistrationDate] = '2026-01-20T15:40:40.1208949+01:00'
WHERE [AccountId] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Accounts] SET [RegistrationDate] = '2026-01-20T15:40:40.1209009+01:00'
WHERE [AccountId] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Accounts] SET [RegistrationDate] = '2026-01-20T15:40:40.1208999+01:00'
WHERE [AccountId] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Customers] SET [RegistrationDate] = '2026-01-20T15:40:40.1209025+01:00'
WHERE [CustomerId] = 1;
SELECT @@ROWCOUNT;

GO

CREATE UNIQUE INDEX [IX_Customers_Email] ON [Customers] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Customers_Phone] ON [Customers] ([Phone]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260120144040_m3', N'8.0.10');
GO

COMMIT;
GO

