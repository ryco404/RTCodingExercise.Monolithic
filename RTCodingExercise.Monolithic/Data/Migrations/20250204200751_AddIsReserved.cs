﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTCodingExercise.Monolithic.Migrations
{
    public partial class AddIsReserved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsReserved",
                table: "Plates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsReserved",
                table: "Plates");
        }
    }
}
