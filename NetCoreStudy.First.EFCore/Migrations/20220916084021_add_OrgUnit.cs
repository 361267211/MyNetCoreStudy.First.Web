using Microsoft.EntityFrameworkCore.Migrations;

namespace NetCoreStudy.First.EFCore.Migrations
{
    public partial class add_OrgUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_OrgUnit",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ParentId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_OrgUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_T_OrgUnit_T_OrgUnit_ParentId",
                        column: x => x.ParentId,
                        principalTable: "T_OrgUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_T_OrgUnit_ParentId",
                table: "T_OrgUnit",
                column: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_OrgUnit");
        }
    }
}
