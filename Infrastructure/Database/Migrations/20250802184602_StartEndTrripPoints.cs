using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Autopark.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class StartEndTrripPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Trips",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Trips",
                type: "datetime2(7)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<long>(
                name: "EndPointId",
                table: "Trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "StartPointId",
                table: "Trips",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TripPoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Location = table.Column<Point>(type: "geography", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AddressResolvedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripPoints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTrackPoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Location = table.Column<Point>(type: "geography", nullable: false),
                    Speed = table.Column<float>(type: "real", nullable: false),
                    Rpm = table.Column<int>(type: "int", nullable: false),
                    FuelLevel = table.Column<byte>(type: "tinyint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTrackPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleTrackPoints_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_EndPointId",
                table: "Trips",
                column: "EndPointId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartPointId",
                table: "Trips",
                column: "StartPointId");

            migrationBuilder.CreateIndex(
                name: "IX_TripPoints_CreatedAt",
                table: "TripPoints",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTrackPoints_TimestampUtc",
                table: "VehicleTrackPoints",
                column: "TimestampUtc");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTrackPoints_VehicleId",
                table: "VehicleTrackPoints",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTrackPoints_VehicleId_TimestampUtc",
                table: "VehicleTrackPoints",
                columns: new[] { "VehicleId", "TimestampUtc" });

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_TripPoints_EndPointId",
                table: "Trips",
                column: "EndPointId",
                principalTable: "TripPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_TripPoints_StartPointId",
                table: "Trips",
                column: "StartPointId",
                principalTable: "TripPoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_TripPoints_EndPointId",
                table: "Trips");

            migrationBuilder.DropForeignKey(
                name: "FK_Trips_TripPoints_StartPointId",
                table: "Trips");

            migrationBuilder.DropTable(
                name: "TripPoints");

            migrationBuilder.DropTable(
                name: "VehicleTrackPoints");

            migrationBuilder.DropIndex(
                name: "IX_Trips_EndPointId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_StartPointId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "EndPointId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StartPointId",
                table: "Trips");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Trips",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2(7)");
        }
    }
}
