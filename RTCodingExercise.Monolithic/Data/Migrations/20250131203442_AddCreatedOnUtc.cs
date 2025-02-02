using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTCodingExercise.Monolithic.Migrations
{
    public partial class AddCreatedOnUtc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOnUtc",
                table: "Plates",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOnUtc",
                table: "Plates");
        }
    }
}
