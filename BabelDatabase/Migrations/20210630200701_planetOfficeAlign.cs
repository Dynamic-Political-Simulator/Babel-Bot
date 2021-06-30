using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class planetOfficeAlign : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanetarySystem_PopsimPlanet_PopsimPlanetId",
                table: "PlanetarySystem");

            migrationBuilder.DropTable(
                name: "PopsimPlanet");

            migrationBuilder.DropColumn(
                name: "PopsimPlanetId",
                table: "PlanetarySystem");

            migrationBuilder.RenameTable(
                name: "PlanetarySystem",
                newName: "PlanetarySystems");

            migrationBuilder.AddColumn<int>(
                name: "PlanetId",
                table: "Alignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemId",
                table: "PlanetarySystems",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Colour",
                table: "PlanetarySystems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Lat",
                table: "PlanetarySystems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Lng",
                table: "PlanetarySystems",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "PlanetId",
                table: "PlanetarySystems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlanetarySystems",
                table: "PlanetarySystems",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Alignments_PlanetId",
                table: "Alignments",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanetarySystems_PlanetId",
                table: "PlanetarySystems",
                column: "PlanetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Alignments_Planets_PlanetId",
                table: "Alignments",
                column: "PlanetId",
                principalTable: "Planets",
                principalColumn: "PlanetId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlanetarySystems_Planets_PlanetId",
                table: "PlanetarySystems",
                column: "PlanetId",
                principalTable: "Planets",
                principalColumn: "PlanetId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alignments_Planets_PlanetId",
                table: "Alignments");

            migrationBuilder.DropForeignKey(
                name: "FK_PlanetarySystems_Planets_PlanetId",
                table: "PlanetarySystems");

            migrationBuilder.DropIndex(
                name: "IX_Alignments_PlanetId",
                table: "Alignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlanetarySystems",
                table: "PlanetarySystems");

            migrationBuilder.DropIndex(
                name: "IX_PlanetarySystems_PlanetId",
                table: "PlanetarySystems");

            migrationBuilder.DropColumn(
                name: "PlanetId",
                table: "Alignments");

            migrationBuilder.DropColumn(
                name: "SystemId",
                table: "PlanetarySystems");

            migrationBuilder.DropColumn(
                name: "Colour",
                table: "PlanetarySystems");

            migrationBuilder.DropColumn(
                name: "Lat",
                table: "PlanetarySystems");

            migrationBuilder.DropColumn(
                name: "Lng",
                table: "PlanetarySystems");

            migrationBuilder.DropColumn(
                name: "PlanetId",
                table: "PlanetarySystems");

            migrationBuilder.RenameTable(
                name: "PlanetarySystems",
                newName: "PlanetarySystem");

            migrationBuilder.AddColumn<int>(
                name: "PopsimPlanetId",
                table: "PlanetarySystem",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PopsimPlanet",
                columns: table => new
                {
                    TempId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.UniqueConstraint("AK_PopsimPlanet_TempId", x => x.TempId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PlanetarySystem_PopsimPlanet_PopsimPlanetId",
                table: "PlanetarySystem",
                column: "PopsimPlanetId",
                principalTable: "PopsimPlanet",
                principalColumn: "TempId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
