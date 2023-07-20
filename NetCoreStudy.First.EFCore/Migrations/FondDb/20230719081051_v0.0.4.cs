using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreStudy.First.EFCore.Migrations.FondDb
{
    public partial class v004 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FondEvents",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Fonds",
                table: "Fonds",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Fonds_FxFondEventId",
                table: "Fonds",
                column: "FxFondEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Fonds",
                table: "Fonds");

            migrationBuilder.DropIndex(
                name: "IX_Fonds_FxFondEventId",
                table: "Fonds");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "FondEvents");

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
        }
    }
}
