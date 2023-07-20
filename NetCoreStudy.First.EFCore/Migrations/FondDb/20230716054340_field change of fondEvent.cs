using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreStudy.First.EFCore.Migrations.FondDb
{
    public partial class fieldchangeoffondEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "FondEvents",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "FondEvents",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "FondEvents");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "FondEvents");
        }
    }
}
