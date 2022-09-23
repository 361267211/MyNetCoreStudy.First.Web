using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreStudy.First.EFCore.Migrations
{
    public partial class index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_T_Books_Title_Price",
                table: "T_Books",
                columns: new[] { "Title", "Price" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_T_Books_Title_Price",
                table: "T_Books");
        }
    }
}
