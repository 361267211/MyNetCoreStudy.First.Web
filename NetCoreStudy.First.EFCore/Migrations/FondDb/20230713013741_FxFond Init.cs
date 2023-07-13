using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreStudy.First.EFCore.Migrations.FondDb
{
    public partial class FxFondInit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FondContacts",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondContacts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FondEvents",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    EventInitiator = table.Column<string>(type: "text", nullable: true),
                    Keyword = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FondEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Fonds",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ExContactId = table.Column<string>(type: "text", nullable: true),
                    InContactId = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    EventId = table.Column<string>(type: "text", nullable: true),
                    ContactId = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fonds", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FondContacts");

            migrationBuilder.DropTable(
                name: "FondEvents");

            migrationBuilder.DropTable(
                name: "Fonds");
        }
    }
}
