using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class AlignFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alignments_Planets_PlanetId",
                table: "Alignments");

            migrationBuilder.DropIndex(
                name: "IX_Alignments_PlanetId",
                table: "Alignments");

            migrationBuilder.DropColumn(
                name: "PlanetId",
                table: "Alignments");

            migrationBuilder.AddColumn<string>(
                name: "ExecutiveAlignmentAlignmentId",
                table: "Planets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LegislativeAlignmentAlignmentId",
                table: "Planets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartyAlignmentAlignmentId",
                table: "Planets",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AlignmentName",
                table: "Alignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_ExecutiveAlignmentAlignmentId",
                table: "Planets",
                column: "ExecutiveAlignmentAlignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_LegislativeAlignmentAlignmentId",
                table: "Planets",
                column: "LegislativeAlignmentAlignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_PartyAlignmentAlignmentId",
                table: "Planets",
                column: "PartyAlignmentAlignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Planets_Alignments_ExecutiveAlignmentAlignmentId",
                table: "Planets",
                column: "ExecutiveAlignmentAlignmentId",
                principalTable: "Alignments",
                principalColumn: "AlignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Planets_Alignments_LegislativeAlignmentAlignmentId",
                table: "Planets",
                column: "LegislativeAlignmentAlignmentId",
                principalTable: "Alignments",
                principalColumn: "AlignmentId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Planets_Alignments_PartyAlignmentAlignmentId",
                table: "Planets",
                column: "PartyAlignmentAlignmentId",
                principalTable: "Alignments",
                principalColumn: "AlignmentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Planets_Alignments_ExecutiveAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropForeignKey(
                name: "FK_Planets_Alignments_LegislativeAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropForeignKey(
                name: "FK_Planets_Alignments_PartyAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropIndex(
                name: "IX_Planets_ExecutiveAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropIndex(
                name: "IX_Planets_LegislativeAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropIndex(
                name: "IX_Planets_PartyAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "ExecutiveAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "LegislativeAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "PartyAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "AlignmentName",
                table: "Alignments");

            migrationBuilder.AddColumn<int>(
                name: "PlanetId",
                table: "Alignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Alignments_PlanetId",
                table: "Alignments",
                column: "PlanetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alignments_Planets_PlanetId",
                table: "Alignments",
                column: "PlanetId",
                principalTable: "Planets",
                principalColumn: "PlanetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
