using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autopark.Infrastructure.Database.Migrations
{
    public partial class ExpandGuidsIntAndBigintSeeded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Expand phase: add Guid columns seeded from existing int/bigint IDs and create Guid FKs
            migrationBuilder.Sql(@"
/* =========================
   0. Общие комментарии
   - Для каждой таблицы добавляем IdGuid + UNIQUE + DEFAULT NEWSEQUENTIALID
   - Для дочерних таблиц добавляем Guid-FK колонки и заполняем их детерминированно
   - Где FK был NOT NULL — делаем Guid-FK NOT NULL; где был NULLABLE — оставляем NULLABLE
   ========================= */

/* =========================
   1) Users
   ========================= */
ALTER TABLE [Users] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Users CHAR(28) = 'A12F3C9E-0000-5000-8000-0005';
UPDATE [Users]
SET [IdGuid] = CAST(@prefix_Users + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Users] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Users] ADD CONSTRAINT [UQ_Users_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Users] ADD CONSTRAINT [DF_Users_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   2) Managers
   ========================= */
ALTER TABLE [Managers] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Managers CHAR(28) = 'A12F3C9E-0000-5000-8000-0009';
UPDATE [Managers]
SET [IdGuid] = CAST(@prefix_Managers + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Managers] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Managers] ADD CONSTRAINT [UQ_Managers_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Managers] ADD CONSTRAINT [DF_Managers_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   3) Enterprises
   ========================= */
ALTER TABLE [Enterprises] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Enterprises CHAR(28) = 'A12F3C9E-0000-5000-8000-0001';
UPDATE [Enterprises]
SET [IdGuid] = CAST(@prefix_Enterprises + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Enterprises] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Enterprises] ADD CONSTRAINT [UQ_Enterprises_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Enterprises] ADD CONSTRAINT [DF_Enterprises_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   4) BrandModels
   ========================= */
ALTER TABLE [BrandModels] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_BrandModels CHAR(28) = 'A12F3C9E-0000-5000-8000-0003';
UPDATE [BrandModels]
SET [IdGuid] = CAST(@prefix_BrandModels + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [BrandModels] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [BrandModels] ADD CONSTRAINT [UQ_BrandModels_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [BrandModels] ADD CONSTRAINT [DF_BrandModels_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   5) Vehicles
   ========================= */
ALTER TABLE [Vehicles] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Vehicles CHAR(28) = 'A12F3C9E-0000-5000-8000-0002';
UPDATE [Vehicles]
SET [IdGuid] = CAST(@prefix_Vehicles + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Vehicles] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Vehicles] ADD CONSTRAINT [UQ_Vehicles_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Vehicles] ADD CONSTRAINT [DF_Vehicles_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   6) Drivers
   ========================= */
ALTER TABLE [Drivers] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Drivers CHAR(28) = 'A12F3C9E-0000-5000-8000-0004';
UPDATE [Drivers]
SET [IdGuid] = CAST(@prefix_Drivers + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Drivers] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Drivers] ADD CONSTRAINT [UQ_Drivers_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Drivers] ADD CONSTRAINT [DF_Drivers_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   7) ManagerEnterprises
   ========================= */
ALTER TABLE [ManagerEnterprises] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_ManagerEnterprises CHAR(28) = 'A12F3C9E-0000-5000-8000-000A';
UPDATE [ManagerEnterprises]
SET [IdGuid] = CAST(@prefix_ManagerEnterprises + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [ManagerEnterprises] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [ManagerEnterprises] ADD CONSTRAINT [UQ_ManagerEnterprises_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [ManagerEnterprises] ADD CONSTRAINT [DF_ManagerEnterprises_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   8) Credentials
   ========================= */
ALTER TABLE [Credentials] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Credentials CHAR(28) = 'A12F3C9E-0000-5000-8000-0006';
UPDATE [Credentials]
SET [IdGuid] = CAST(@prefix_Credentials + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Credentials] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Credentials] ADD CONSTRAINT [UQ_Credentials_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Credentials] ADD CONSTRAINT [DF_Credentials_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   9) UserRoles
   ========================= */
ALTER TABLE [UserRoles] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_UserRoles CHAR(28) = 'A12F3C9E-0000-5000-8000-0007';
UPDATE [UserRoles]
SET [IdGuid] = CAST(@prefix_UserRoles + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [UserRoles] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [UserRoles] ADD CONSTRAINT [UQ_UserRoles_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [UserRoles] ADD CONSTRAINT [DF_UserRoles_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   10) Devices
   ========================= */
ALTER TABLE [Devices] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Devices CHAR(28) = 'A12F3C9E-0000-5000-8000-0008';
UPDATE [Devices]
SET [IdGuid] = CAST(@prefix_Devices + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Devices] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Devices] ADD CONSTRAINT [UQ_Devices_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Devices] ADD CONSTRAINT [DF_Devices_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   11) Trips
   ========================= */
ALTER TABLE [Trips] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_Trips CHAR(28) = 'A12F3C9E-0000-5000-8000-000B';
UPDATE [Trips]
SET [IdGuid] = CAST(@prefix_Trips + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [Trips] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Trips] ADD CONSTRAINT [UQ_Trips_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [Trips] ADD CONSTRAINT [DF_Trips_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   12) VehicleTrackPoints
   ========================= */
ALTER TABLE [VehicleTrackPoints] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix_VehicleTrackPoints CHAR(28) = 'A12F3C9E-0000-5000-8000-000C';
UPDATE [VehicleTrackPoints]
SET [IdGuid] = CAST(@prefix_VehicleTrackPoints + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), [Id]), 2), 8) AS UNIQUEIDENTIFIER)
WHERE [IdGuid] IS NULL;

ALTER TABLE [VehicleTrackPoints] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [VehicleTrackPoints] ADD CONSTRAINT [UQ_VehicleTrackPoints_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [VehicleTrackPoints] ADD CONSTRAINT [DF_VehicleTrackPoints_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* =========================
   TripPoints (bigint → Guid)
   ========================= */
ALTER TABLE [TripPoints] ADD [IdGuid] UNIQUEIDENTIFIER NULL;

DECLARE @prefix3_TripPoints CHAR(19) = 'A12F3C9E-0000-5D00-';

UPDATE TP
SET TP.IdGuid = CAST(
    @prefix3_TripPoints
    + SUBSTRING(H.Hex16, 1, 4) + '-' + SUBSTRING(H.Hex16, 5, 12)
    AS UNIQUEIDENTIFIER)
FROM [TripPoints] TP
CROSS APPLY (
    SELECT RIGHT(CONVERT(VARCHAR(16), CONVERT(VARBINARY(8), CAST(TP.Id AS BIGINT)), 2), 16) AS Hex16
) H
WHERE TP.IdGuid IS NULL;

ALTER TABLE [TripPoints] ALTER COLUMN [IdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [TripPoints] ADD CONSTRAINT [UQ_TripPoints_IdGuid] UNIQUE ([IdGuid]);
ALTER TABLE [TripPoints] ADD CONSTRAINT [DF_TripPoints_IdGuid] DEFAULT NEWSEQUENTIALID() FOR [IdGuid];

/* ==========================================================
   13) Добавляем GUID-FK колонки и бэкофилим значения
   ========================================================== */

/* Vehicles FKs -> BrandModels, Enterprises, Drivers(active) */
ALTER TABLE [Vehicles] 
  ADD [BrandModelIdGuid] UNIQUEIDENTIFIER NULL,
      [EnterpriseIdGuid] UNIQUEIDENTIFIER NULL,
      [ActiveDriverIdGuid] UNIQUEIDENTIFIER NULL;

-- BrandModelId (NOT NULL)
UPDATE V
SET V.BrandModelIdGuid = CAST(@prefix_BrandModels + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), V.BrandModelId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Vehicles] V;

-- EnterpriseId (NOT NULL)
UPDATE V
SET V.EnterpriseIdGuid = CAST(@prefix_Enterprises + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), V.EnterpriseId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Vehicles] V;

-- ActiveDriverId (NULLABLE)
UPDATE V
SET V.ActiveDriverIdGuid = CASE 
    WHEN V.ActiveDriverId IS NULL THEN NULL
    ELSE CAST(@prefix_Drivers + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), V.ActiveDriverId), 2), 8) AS UNIQUEIDENTIFIER)
END
FROM [Vehicles] V;

ALTER TABLE [Vehicles] ALTER COLUMN [BrandModelIdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Vehicles] ALTER COLUMN [EnterpriseIdGuid] UNIQUEIDENTIFIER NOT NULL;
/* ActiveDriverIdGuid остаётся NULLABLE */

/* Drivers FKs -> Users, Vehicles(NULL), Enterprises */
ALTER TABLE [Drivers]
  ADD [UserIdGuid] UNIQUEIDENTIFIER NULL,
      [VehicleIdGuid] UNIQUEIDENTIFIER NULL,
      [EnterpriseIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE D
SET D.UserIdGuid = CAST(@prefix_Users + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), D.UserId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Drivers] D;

UPDATE D
SET D.VehicleIdGuid = CASE
    WHEN D.VehicleId IS NULL THEN NULL
    ELSE CAST(@prefix_Vehicles + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), D.VehicleId), 2), 8) AS UNIQUEIDENTIFIER)
END
FROM [Drivers] D;

UPDATE D
SET D.EnterpriseIdGuid = CAST(@prefix_Enterprises + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), D.EnterpriseId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Drivers] D;

ALTER TABLE [Drivers] ALTER COLUMN [UserIdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [Drivers] ALTER COLUMN [EnterpriseIdGuid] UNIQUEIDENTIFIER NOT NULL;
/* VehicleIdGuid остаётся NULLABLE */

/* Managers FKs -> Users */
ALTER TABLE [Managers]
  ADD [UserIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE M
SET M.UserIdGuid = CAST(@prefix_Users + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), M.UserId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Managers] M;

ALTER TABLE [Managers] ALTER COLUMN [UserIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* ManagerEnterprises FKs -> Managers, Enterprises */
ALTER TABLE [ManagerEnterprises]
  ADD [ManagerIdGuid] UNIQUEIDENTIFIER NULL,
      [EnterpriseIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE ME
SET ME.ManagerIdGuid = CAST(@prefix_Managers + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), ME.ManagerId), 2), 8) AS UNIQUEIDENTIFIER),
    ME.EnterpriseIdGuid = CAST(@prefix_Enterprises + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), ME.EnterpriseId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [ManagerEnterprises] ME;

ALTER TABLE [ManagerEnterprises] ALTER COLUMN [ManagerIdGuid] UNIQUEIDENTIFIER NOT NULL;
ALTER TABLE [ManagerEnterprises] ALTER COLUMN [EnterpriseIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* Credentials FKs -> Users */
ALTER TABLE [Credentials]
  ADD [UserIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE C
SET C.UserIdGuid = CAST(@prefix_Users + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), C.UserId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Credentials] C;

ALTER TABLE [Credentials] ALTER COLUMN [UserIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* UserRoles FKs -> Users */
ALTER TABLE [UserRoles]
  ADD [UserIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE UR
SET UR.UserIdGuid = CAST(@prefix_Users + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), UR.UserId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [UserRoles] UR;

ALTER TABLE [UserRoles] ALTER COLUMN [UserIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* Devices FKs -> Users */
ALTER TABLE [Devices]
  ADD [UserIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE Dv
SET Dv.UserIdGuid = CAST(@prefix_Users + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), Dv.UserId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Devices] Dv;

ALTER TABLE [Devices] ALTER COLUMN [UserIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* Trips FKs -> Vehicles */
ALTER TABLE [Trips]
  ADD [VehicleIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE T
SET T.VehicleIdGuid = CAST(@prefix_Vehicles + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), T.VehicleId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [Trips] T;

ALTER TABLE [Trips] ALTER COLUMN [VehicleIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* VehicleTrackPoints FKs -> Vehicles */
ALTER TABLE [VehicleTrackPoints]
  ADD [VehicleIdGuid] UNIQUEIDENTIFIER NULL;

UPDATE VTP
SET VTP.VehicleIdGuid = CAST(@prefix_Vehicles + RIGHT(CONVERT(VARCHAR(8), CONVERT(VARBINARY(4), VTP.VehicleId), 2), 8) AS UNIQUEIDENTIFIER)
FROM [VehicleTrackPoints] VTP;

ALTER TABLE [VehicleTrackPoints] ALTER COLUMN [VehicleIdGuid] UNIQUEIDENTIFIER NOT NULL;

/* Trips: GUID-ссылки на TripPoints */
ALTER TABLE [Trips]
  ADD [StartPointIdGuid] UNIQUEIDENTIFIER NULL,
      [EndPointIdGuid]   UNIQUEIDENTIFIER NULL;

UPDATE T
SET
  T.StartPointIdGuid = CASE
      WHEN T.StartPointId IS NULL THEN NULL
      ELSE CAST(@prefix3_TripPoints + SUBSTRING(HS.Hex16, 1, 4) + '-' + SUBSTRING(HS.Hex16, 5, 12) AS UNIQUEIDENTIFIER)
  END,
  T.EndPointIdGuid = CASE
      WHEN T.EndPointId IS NULL THEN NULL
      ELSE CAST(@prefix3_TripPoints + SUBSTRING(HE.Hex16, 1, 4) + '-' + SUBSTRING(HE.Hex16, 5, 12) AS UNIQUEIDENTIFIER)
  END
FROM [Trips] T
OUTER APPLY (
    SELECT RIGHT(CONVERT(VARCHAR(16), CONVERT(VARBINARY(8), CAST(T.StartPointId AS BIGINT)), 2), 16) AS Hex16
) HS
OUTER APPLY (
    SELECT RIGHT(CONVERT(VARCHAR(16), CONVERT(VARBINARY(8), CAST(T.EndPointId AS BIGINT)), 2), 16) AS Hex16
) HE;

/* ==========================================================
   14) Вешаем новые GUID-FK (на UQ_* IdGuid)
   ========================================================== */

ALTER TABLE [Vehicles]
  ADD CONSTRAINT [FK_Vehicles_BrandModels_BrandModelIdGuid]
      FOREIGN KEY ([BrandModelIdGuid]) REFERENCES [BrandModels] ([IdGuid]),
      CONSTRAINT [FK_Vehicles_Enterprises_EnterpriseIdGuid]
      FOREIGN KEY ([EnterpriseIdGuid]) REFERENCES [Enterprises] ([IdGuid]),
      CONSTRAINT [FK_Vehicles_Drivers_ActiveDriverIdGuid]
      FOREIGN KEY ([ActiveDriverIdGuid]) REFERENCES [Drivers] ([IdGuid]);

ALTER TABLE [Drivers]
  ADD CONSTRAINT [FK_Drivers_Users_UserIdGuid]
      FOREIGN KEY ([UserIdGuid]) REFERENCES [Users] ([IdGuid]),
      CONSTRAINT [FK_Drivers_Vehicles_VehicleIdGuid]
      FOREIGN KEY ([VehicleIdGuid]) REFERENCES [Vehicles] ([IdGuid]),
      CONSTRAINT [FK_Drivers_Enterprises_EnterpriseIdGuid]
      FOREIGN KEY ([EnterpriseIdGuid]) REFERENCES [Enterprises] ([IdGuid]);

ALTER TABLE [Managers]
  ADD CONSTRAINT [FK_Managers_Users_UserIdGuid]
      FOREIGN KEY ([UserIdGuid]) REFERENCES [Users] ([IdGuid]);

ALTER TABLE [ManagerEnterprises]
  ADD CONSTRAINT [FK_ManagerEnterprises_Managers_ManagerIdGuid]
      FOREIGN KEY ([ManagerIdGuid]) REFERENCES [Managers] ([IdGuid]),
      CONSTRAINT [FK_ManagerEnterprises_Enterprises_EnterpriseIdGuid]
      FOREIGN KEY ([EnterpriseIdGuid]) REFERENCES [Enterprises] ([IdGuid]);

ALTER TABLE [Credentials]
  ADD CONSTRAINT [FK_Credentials_Users_UserIdGuid]
      FOREIGN KEY ([UserIdGuid]) REFERENCES [Users] ([IdGuid]);

ALTER TABLE [UserRoles]
  ADD CONSTRAINT [FK_UserRoles_Users_UserIdGuid]
      FOREIGN KEY ([UserIdGuid]) REFERENCES [Users] ([IdGuid]);

ALTER TABLE [Devices]
  ADD CONSTRAINT [FK_Devices_Users_UserIdGuid]
      FOREIGN KEY ([UserIdGuid]) REFERENCES [Users] ([IdGuid]);

ALTER TABLE [Trips]
  ADD CONSTRAINT [FK_Trips_Vehicles_VehicleIdGuid]
      FOREIGN KEY ([VehicleIdGuid]) REFERENCES [Vehicles] ([IdGuid]),
      CONSTRAINT [FK_Trips_TripPoints_StartPointIdGuid]
      FOREIGN KEY ([StartPointIdGuid]) REFERENCES [TripPoints] ([IdGuid]),
      CONSTRAINT [FK_Trips_TripPoints_EndPointIdGuid]
      FOREIGN KEY ([EndPointIdGuid]) REFERENCES [TripPoints] ([IdGuid]);

ALTER TABLE [VehicleTrackPoints]
  ADD CONSTRAINT [FK_VehicleTrackPoints_Vehicles_VehicleIdGuid]
      FOREIGN KEY ([VehicleIdGuid]) REFERENCES [Vehicles] ([IdGuid]);
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Contract rollback: drop new Guid FKs and columns (does not remove data)
            migrationBuilder.Sql(@"
/* ===== Drop GUID-based FKs ===== */
IF OBJECT_ID('FK_Vehicles_BrandModels_BrandModelIdGuid', 'F') IS NOT NULL ALTER TABLE [Vehicles] DROP CONSTRAINT [FK_Vehicles_BrandModels_BrandModelIdGuid];
IF OBJECT_ID('FK_Vehicles_Enterprises_EnterpriseIdGuid', 'F') IS NOT NULL ALTER TABLE [Vehicles] DROP CONSTRAINT [FK_Vehicles_Enterprises_EnterpriseIdGuid];
IF OBJECT_ID('FK_Vehicles_Drivers_ActiveDriverIdGuid', 'F') IS NOT NULL ALTER TABLE [Vehicles] DROP CONSTRAINT [FK_Vehicles_Drivers_ActiveDriverIdGuid];

IF OBJECT_ID('FK_Drivers_Users_UserIdGuid', 'F') IS NOT NULL ALTER TABLE [Drivers] DROP CONSTRAINT [FK_Drivers_Users_UserIdGuid];
IF OBJECT_ID('FK_Drivers_Vehicles_VehicleIdGuid', 'F') IS NOT NULL ALTER TABLE [Drivers] DROP CONSTRAINT [FK_Drivers_Vehicles_VehicleIdGuid];
IF OBJECT_ID('FK_Drivers_Enterprises_EnterpriseIdGuid', 'F') IS NOT NULL ALTER TABLE [Drivers] DROP CONSTRAINT [FK_Drivers_Enterprises_EnterpriseIdGuid];

IF OBJECT_ID('FK_Managers_Users_UserIdGuid', 'F') IS NOT NULL ALTER TABLE [Managers] DROP CONSTRAINT [FK_Managers_Users_UserIdGuid];

IF OBJECT_ID('FK_ManagerEnterprises_Managers_ManagerIdGuid', 'F') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP CONSTRAINT [FK_ManagerEnterprises_Managers_ManagerIdGuid];
IF OBJECT_ID('FK_ManagerEnterprises_Enterprises_EnterpriseIdGuid', 'F') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP CONSTRAINT [FK_ManagerEnterprises_Enterprises_EnterpriseIdGuid];

IF OBJECT_ID('FK_Credentials_Users_UserIdGuid', 'F') IS NOT NULL ALTER TABLE [Credentials] DROP CONSTRAINT [FK_Credentials_Users_UserIdGuid];
IF OBJECT_ID('FK_UserRoles_Users_UserIdGuid', 'F') IS NOT NULL ALTER TABLE [UserRoles] DROP CONSTRAINT [FK_UserRoles_Users_UserIdGuid];
IF OBJECT_ID('FK_Devices_Users_UserIdGuid', 'F') IS NOT NULL ALTER TABLE [Devices] DROP CONSTRAINT [FK_Devices_Users_UserIdGuid];

IF OBJECT_ID('FK_Trips_Vehicles_VehicleIdGuid', 'F') IS NOT NULL ALTER TABLE [Trips] DROP CONSTRAINT [FK_Trips_Vehicles_VehicleIdGuid];
IF OBJECT_ID('FK_Trips_TripPoints_StartPointIdGuid', 'F') IS NOT NULL ALTER TABLE [Trips] DROP CONSTRAINT [FK_Trips_TripPoints_StartPointIdGuid];
IF OBJECT_ID('FK_Trips_TripPoints_EndPointIdGuid', 'F') IS NOT NULL ALTER TABLE [Trips] DROP CONSTRAINT [FK_Trips_TripPoints_EndPointIdGuid];

IF OBJECT_ID('FK_VehicleTrackPoints_Vehicles_VehicleIdGuid', 'F') IS NOT NULL ALTER TABLE [VehicleTrackPoints] DROP CONSTRAINT [FK_VehicleTrackPoints_Vehicles_VehicleIdGuid];

/* ===== Drop DEFAULT and UNIQUE constraints then columns ===== */
-- Helper to drop default constraint by name (we used fixed names in Up)

-- Vehicles new FK columns
IF COL_LENGTH('Vehicles','BrandModelIdGuid') IS NOT NULL ALTER TABLE [Vehicles] DROP COLUMN [BrandModelIdGuid];
IF COL_LENGTH('Vehicles','EnterpriseIdGuid') IS NOT NULL ALTER TABLE [Vehicles] DROP COLUMN [EnterpriseIdGuid];
IF COL_LENGTH('Vehicles','ActiveDriverIdGuid') IS NOT NULL ALTER TABLE [Vehicles] DROP COLUMN [ActiveDriverIdGuid];

-- Drivers new FK columns
IF COL_LENGTH('Drivers','UserIdGuid') IS NOT NULL ALTER TABLE [Drivers] DROP COLUMN [UserIdGuid];
IF COL_LENGTH('Drivers','VehicleIdGuid') IS NOT NULL ALTER TABLE [Drivers] DROP COLUMN [VehicleIdGuid];
IF COL_LENGTH('Drivers','EnterpriseIdGuid') IS NOT NULL ALTER TABLE [Drivers] DROP COLUMN [EnterpriseIdGuid];

-- Managers
IF COL_LENGTH('Managers','UserIdGuid') IS NOT NULL ALTER TABLE [Managers] DROP COLUMN [UserIdGuid];

-- ManagerEnterprises
IF COL_LENGTH('ManagerEnterprises','ManagerIdGuid') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP COLUMN [ManagerIdGuid];
IF COL_LENGTH('ManagerEnterprises','EnterpriseIdGuid') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP COLUMN [EnterpriseIdGuid];

-- Credentials
IF COL_LENGTH('Credentials','UserIdGuid') IS NOT NULL ALTER TABLE [Credentials] DROP COLUMN [UserIdGuid];

-- UserRoles
IF COL_LENGTH('UserRoles','UserIdGuid') IS NOT NULL ALTER TABLE [UserRoles] DROP COLUMN [UserIdGuid];

-- Devices
IF COL_LENGTH('Devices','UserIdGuid') IS NOT NULL ALTER TABLE [Devices] DROP COLUMN [UserIdGuid];

-- Trips FK columns to Vehicles and TripPoints
IF COL_LENGTH('Trips','VehicleIdGuid') IS NOT NULL ALTER TABLE [Trips] DROP COLUMN [VehicleIdGuid];
IF COL_LENGTH('Trips','StartPointIdGuid') IS NOT NULL ALTER TABLE [Trips] DROP COLUMN [StartPointIdGuid];
IF COL_LENGTH('Trips','EndPointIdGuid') IS NOT NULL ALTER TABLE [Trips] DROP COLUMN [EndPointIdGuid];

-- VehicleTrackPoints FK column
IF COL_LENGTH('VehicleTrackPoints','VehicleIdGuid') IS NOT NULL ALTER TABLE [VehicleTrackPoints] DROP COLUMN [VehicleIdGuid];

-- Drop UNIQUE + DEFAULT + IdGuid columns for each table

-- Users
IF OBJECT_ID('UQ_Users_IdGuid','UQ') IS NOT NULL ALTER TABLE [Users] DROP CONSTRAINT [UQ_Users_IdGuid];
IF OBJECT_ID('DF_Users_IdGuid','D') IS NOT NULL ALTER TABLE [Users] DROP CONSTRAINT [DF_Users_IdGuid];
IF COL_LENGTH('Users','IdGuid') IS NOT NULL ALTER TABLE [Users] DROP COLUMN [IdGuid];

-- Managers
IF OBJECT_ID('UQ_Managers_IdGuid','UQ') IS NOT NULL ALTER TABLE [Managers] DROP CONSTRAINT [UQ_Managers_IdGuid];
IF OBJECT_ID('DF_Managers_IdGuid','D') IS NOT NULL ALTER TABLE [Managers] DROP CONSTRAINT [DF_Managers_IdGuid];
IF COL_LENGTH('Managers','IdGuid') IS NOT NULL ALTER TABLE [Managers] DROP COLUMN [IdGuid];

-- Enterprises
IF OBJECT_ID('UQ_Enterprises_IdGuid','UQ') IS NOT NULL ALTER TABLE [Enterprises] DROP CONSTRAINT [UQ_Enterprises_IdGuid];
IF OBJECT_ID('DF_Enterprises_IdGuid','D') IS NOT NULL ALTER TABLE [Enterprises] DROP CONSTRAINT [DF_Enterprises_IdGuid];
IF COL_LENGTH('Enterprises','IdGuid') IS NOT NULL ALTER TABLE [Enterprises] DROP COLUMN [IdGuid];

-- BrandModels
IF OBJECT_ID('UQ_BrandModels_IdGuid','UQ') IS NOT NULL ALTER TABLE [BrandModels] DROP CONSTRAINT [UQ_BrandModels_IdGuid];
IF OBJECT_ID('DF_BrandModels_IdGuid','D') IS NOT NULL ALTER TABLE [BrandModels] DROP CONSTRAINT [DF_BrandModels_IdGuid];
IF COL_LENGTH('BrandModels','IdGuid') IS NOT NULL ALTER TABLE [BrandModels] DROP COLUMN [IdGuid];

-- Vehicles
IF OBJECT_ID('UQ_Vehicles_IdGuid','UQ') IS NOT NULL ALTER TABLE [Vehicles] DROP CONSTRAINT [UQ_Vehicles_IdGuid];
IF OBJECT_ID('DF_Vehicles_IdGuid','D') IS NOT NULL ALTER TABLE [Vehicles] DROP CONSTRAINT [DF_Vehicles_IdGuid];
IF COL_LENGTH('Vehicles','IdGuid') IS NOT NULL ALTER TABLE [Vehicles] DROP COLUMN [IdGuid];

-- Drivers
IF OBJECT_ID('UQ_Drivers_IdGuid','UQ') IS NOT NULL ALTER TABLE [Drivers] DROP CONSTRAINT [UQ_Drivers_IdGuid];
IF OBJECT_ID('DF_Drivers_IdGuid','D') IS NOT NULL ALTER TABLE [Drivers] DROP CONSTRAINT [DF_Drivers_IdGuid];
IF COL_LENGTH('Drivers','IdGuid') IS NOT NULL ALTER TABLE [Drivers] DROP COLUMN [IdGuid];

-- ManagerEnterprises
IF OBJECT_ID('UQ_ManagerEnterprises_IdGuid','UQ') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP CONSTRAINT [UQ_ManagerEnterprises_IdGuid];
IF OBJECT_ID('DF_ManagerEnterprises_IdGuid','D') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP CONSTRAINT [DF_ManagerEnterprises_IdGuid];
IF COL_LENGTH('ManagerEnterprises','IdGuid') IS NOT NULL ALTER TABLE [ManagerEnterprises] DROP COLUMN [IdGuid];

-- Credentials
IF OBJECT_ID('UQ_Credentials_IdGuid','UQ') IS NOT NULL ALTER TABLE [Credentials] DROP CONSTRAINT [UQ_Credentials_IdGuid];
IF OBJECT_ID('DF_Credentials_IdGuid','D') IS NOT NULL ALTER TABLE [Credentials] DROP CONSTRAINT [DF_Credentials_IdGuid];
IF COL_LENGTH('Credentials','IdGuid') IS NOT NULL ALTER TABLE [Credentials] DROP COLUMN [IdGuid];

-- UserRoles
IF OBJECT_ID('UQ_UserRoles_IdGuid','UQ') IS NOT NULL ALTER TABLE [UserRoles] DROP CONSTRAINT [UQ_UserRoles_IdGuid];
IF OBJECT_ID('DF_UserRoles_IdGuid','D') IS NOT NULL ALTER TABLE [UserRoles] DROP CONSTRAINT [DF_UserRoles_IdGuid];
IF COL_LENGTH('UserRoles','IdGuid') IS NOT NULL ALTER TABLE [UserRoles] DROP COLUMN [IdGuid];

-- Devices
IF OBJECT_ID('UQ_Devices_IdGuid','UQ') IS NOT NULL ALTER TABLE [Devices] DROP CONSTRAINT [UQ_Devices_IdGuid];
IF OBJECT_ID('DF_Devices_IdGuid','D') IS NOT NULL ALTER TABLE [Devices] DROP CONSTRAINT [DF_Devices_IdGuid];
IF COL_LENGTH('Devices','IdGuid') IS NOT NULL ALTER TABLE [Devices] DROP COLUMN [IdGuid];

-- Trips
IF OBJECT_ID('UQ_Trips_IdGuid','UQ') IS NOT NULL ALTER TABLE [Trips] DROP CONSTRAINT [UQ_Trips_IdGuid];
IF OBJECT_ID('DF_Trips_IdGuid','D') IS NOT NULL ALTER TABLE [Trips] DROP CONSTRAINT [DF_Trips_IdGuid];
IF COL_LENGTH('Trips','IdGuid') IS NOT NULL ALTER TABLE [Trips] DROP COLUMN [IdGuid];

-- VehicleTrackPoints
IF OBJECT_ID('UQ_VehicleTrackPoints_IdGuid','UQ') IS NOT NULL ALTER TABLE [VehicleTrackPoints] DROP CONSTRAINT [UQ_VehicleTrackPoints_IdGuid];
IF OBJECT_ID('DF_VehicleTrackPoints_IdGuid','D') IS NOT NULL ALTER TABLE [VehicleTrackPoints] DROP CONSTRAINT [DF_VehicleTrackPoints_IdGuid];
IF COL_LENGTH('VehicleTrackPoints','IdGuid') IS NOT NULL ALTER TABLE [VehicleTrackPoints] DROP COLUMN [IdGuid];

-- TripPoints (bigint based)
IF OBJECT_ID('UQ_TripPoints_IdGuid','UQ') IS NOT NULL ALTER TABLE [TripPoints] DROP CONSTRAINT [UQ_TripPoints_IdGuid];
IF OBJECT_ID('DF_TripPoints_IdGuid','D') IS NOT NULL ALTER TABLE [TripPoints] DROP CONSTRAINT [DF_TripPoints_IdGuid];
IF COL_LENGTH('TripPoints','IdGuid') IS NOT NULL ALTER TABLE [TripPoints] DROP COLUMN [IdGuid];
");
        }
    }
}