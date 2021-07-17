using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class PlanetaryAlignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GlobalAlignment",
                table: "Planets",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "DiscordUsers",
                columns: new[] { "DiscordUserId", "ActiveCharacterId", "IsAdmin", "UserName" },
                values: new object[] { "222825184887963648", null, true, "Grindor" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "DiscordUsers",
                keyColumn: "DiscordUserId",
                keyValue: "222825184887963648");

            migrationBuilder.DropColumn(
                name: "GlobalAlignment",
                table: "Planets");
        }
    }
}
