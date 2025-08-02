using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autopark.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    StartUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    DistanceKm = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_EndUtc",
                table: "Trips",
                column: "EndUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StartUtc",
                table: "Trips",
                column: "StartUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_VehicleId_StartUtc_EndUtc",
                table: "Trips",
                columns: new[] { "VehicleId", "StartUtc", "EndUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trips");
        }
    }
}
