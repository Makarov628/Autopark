using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Autopark.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultUtcToPurchaseDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Добавляем DEFAULT SYSUTCDATETIME() для PurchaseDate
            migrationBuilder.Sql(@"
                ALTER TABLE Vehicles 
                ADD CONSTRAINT DF_Vehicles_PurchaseDate 
                DEFAULT (SYSUTCDATETIME()) 
                FOR PurchaseDate
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем DEFAULT constraint
            migrationBuilder.Sql(@"
                ALTER TABLE Vehicles 
                DROP CONSTRAINT DF_Vehicles_PurchaseDate
            ");
        }
    }
}