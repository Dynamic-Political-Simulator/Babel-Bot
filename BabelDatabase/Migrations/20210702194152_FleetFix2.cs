using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class FleetFix2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_GalacticObjects_SystemId",
                table: "Fleets");

            migrationBuilder.DropIndex(
                name: "IX_Fleets_SystemId",
                table: "Fleets");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Fleets_SystemId",
                table: "Fleets",
                column: "SystemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_GalacticObjects_SystemId",
                table: "Fleets",
                column: "SystemId",
                principalTable: "GalacticObjects",
                principalColumn: "GalacticObjectId");
        }
    }
}
