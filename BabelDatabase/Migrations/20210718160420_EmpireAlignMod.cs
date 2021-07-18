using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class EmpireAlignMod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GlobalAlignment",
                table: "Empires",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GlobalAlignment",
                table: "Empires");
        }
    }
}
