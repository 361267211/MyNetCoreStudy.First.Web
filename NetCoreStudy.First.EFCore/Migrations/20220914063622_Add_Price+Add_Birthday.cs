using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreStudy.First.EFCore.Migrations
{
    public partial class Add_PriceAdd_Birthday : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MyProperty",
                table: "T_Books",
                newName: "Price");

            migrationBuilder.AddColumn<double>(
                name: "Birthday",
                table: "T_Books",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Birthday",
                table: "T_Books");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "T_Books",
                newName: "MyProperty");
        }
    }
}
