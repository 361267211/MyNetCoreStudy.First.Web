using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreStudy.First.EFCore.Migrations.FondDb
{
    public partial class FxFondv002 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fonds_FondEvents_FxFondEventId",
                table: "Fonds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fonds",
                table: "Fonds");

            migrationBuilder.DropIndex(
                name: "IX_Fonds_FxFondEventId",
                table: "Fonds");

            migrationBuilder.AlterColumn<string>(
                name: "FxFondEventId",
                table: "Fonds",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fonds",
                table: "Fonds",
                columns: new[] { "FxFondEventId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Fonds_FondEvents_FxFondEventId",
                table: "Fonds",
                column: "FxFondEventId",
                principalTable: "FondEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fonds_FondEvents_FxFondEventId",
                table: "Fonds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Fonds",
                table: "Fonds");

            migrationBuilder.AlterColumn<string>(
                name: "FxFondEventId",
                table: "Fonds",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fonds",
                table: "Fonds",
                column: "Id");

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
    }
}
