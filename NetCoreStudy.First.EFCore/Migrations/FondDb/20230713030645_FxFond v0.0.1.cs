using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreStudy.First.EFCore.Migrations.FondDb
{
    public partial class FxFondv001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactId",
                table: "Fonds");

            migrationBuilder.RenameColumn(
                name: "EventId",
                table: "Fonds",
                newName: "FxFondEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Fonds_FxFondEventId",
                table: "Fonds",
                column: "FxFondEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fonds_FondEvents_FxFondEventId",
                table: "Fonds",
                column: "FxFondEventId",
                principalTable: "FondEvents",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fonds_FondEvents_FxFondEventId",
                table: "Fonds");

            migrationBuilder.DropIndex(
                name: "IX_Fonds_FxFondEventId",
                table: "Fonds");

            migrationBuilder.RenameColumn(
                name: "FxFondEventId",
                table: "Fonds",
                newName: "EventId");

            migrationBuilder.AddColumn<string>(
                name: "ContactId",
                table: "Fonds",
                type: "text",
                nullable: true);
        }
    }
}
