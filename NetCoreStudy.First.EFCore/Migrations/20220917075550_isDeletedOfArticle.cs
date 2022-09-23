using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreStudy.First.EFCore.Migrations
{
    public partial class isDeletedOfArticle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "T_Article",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "T_Article");
        }
    }
}
