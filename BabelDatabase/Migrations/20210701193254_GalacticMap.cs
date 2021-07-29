using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class GalacticMap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlignmentClique_Alignments_AlignmentsAlignmentId",
                table: "AlignmentClique");

            migrationBuilder.DropForeignKey(
                name: "FK_AlignmentClique_Cliques_CliquesCliqueId",
                table: "AlignmentClique");

            migrationBuilder.RenameColumn(
                name: "CliquesCliqueId",
                table: "AlignmentClique",
                newName: "AlignmentId");

            migrationBuilder.RenameColumn(
                name: "AlignmentsAlignmentId",
                table: "AlignmentClique",
                newName: "CliqueId");

            migrationBuilder.RenameIndex(
                name: "IX_AlignmentClique_CliquesCliqueId",
                table: "AlignmentClique",
                newName: "IX_AlignmentClique_AlignmentId");

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

            migrationBuilder.AlterColumn<string>(
                name: "AlignmentName",
                table: "Alignments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PlanetarySystems",
                columns: table => new
                {
                    SystemId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Lat = table.Column<float>(type: "real", nullable: false),
                    Lng = table.Column<float>(type: "real", nullable: false),
                    Colour = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanetarySystems", x => x.SystemId);
                    table.ForeignKey(
                        name: "FK_PlanetarySystems_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "PlanetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Alignments",
                columns: new[] { "AlignmentId", "AlignmentName", "CooperationCompetition", "DemocracyAuthority", "Establishment", "FederalismCentralism", "GlobalismIsolationism", "LowerPartyModiifer", "MilitarismPacifism", "MonoculturalismMulticulturalism", "ProgressivismTraditionalism", "SecularismSpiritualism", "SecurityFreedom", "UpperPartyModifier" },
                values: new object[,]
                {
                    { "123", "ULTRA COMMIE LIBERTARIANS", 3, 1, 0f, 5, 0, 0f, 8, 4, 9, 7, 4, 0f },
                    { "124", "ULTRA LIBERTARIAN COMMIES", 1, 3, 0f, 2, 4, 0f, 2, 3, 4, 0, 6, 0f }
                });

            migrationBuilder.InsertData(
                table: "DiscordUsers",
                columns: new[] { "DiscordUserId", "ActiveCharacterId", "IsAdmin", "UserName" },
                values: new object[] { "75968535074967552", null, true, "Obi" });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "SpeciesId", "SpeciesDescription", "SpeciesName" },
                values: new object[,]
                {
                    { "1", null, "Human" },
                    { "2", null, "Zelvan" },
                    { "3", null, "Liaran" }
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PlanetarySystems_PlanetId",
                table: "PlanetarySystems",
                column: "PlanetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AlignmentClique_Alignments_AlignmentId",
                table: "AlignmentClique",
                column: "AlignmentId",
                principalTable: "Alignments",
                principalColumn: "AlignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlignmentClique_Cliques_CliqueId",
                table: "AlignmentClique",
                column: "CliqueId",
                principalTable: "Cliques",
                principalColumn: "CliqueId",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_AlignmentClique_Alignments_AlignmentId",
                table: "AlignmentClique");

            migrationBuilder.DropForeignKey(
                name: "FK_AlignmentClique_Cliques_CliqueId",
                table: "AlignmentClique");

            migrationBuilder.DropForeignKey(
                name: "FK_Planets_Alignments_ExecutiveAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropForeignKey(
                name: "FK_Planets_Alignments_LegislativeAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropForeignKey(
                name: "FK_Planets_Alignments_PartyAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropTable(
                name: "PlanetarySystems");

            migrationBuilder.DropIndex(
                name: "IX_Planets_ExecutiveAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropIndex(
                name: "IX_Planets_LegislativeAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropIndex(
                name: "IX_Planets_PartyAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DeleteData(
                table: "Alignments",
                keyColumn: "AlignmentId",
                keyValue: "123");

            migrationBuilder.DeleteData(
                table: "Alignments",
                keyColumn: "AlignmentId",
                keyValue: "124");

            migrationBuilder.DeleteData(
                table: "DiscordUsers",
                keyColumn: "DiscordUserId",
                keyValue: "75968535074967552");

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "SpeciesId",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "SpeciesId",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "Species",
                keyColumn: "SpeciesId",
                keyValue: "3");

            migrationBuilder.DropColumn(
                name: "ExecutiveAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "LegislativeAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.DropColumn(
                name: "PartyAlignmentAlignmentId",
                table: "Planets");

            migrationBuilder.RenameColumn(
                name: "AlignmentId",
                table: "AlignmentClique",
                newName: "CliquesCliqueId");

            migrationBuilder.RenameColumn(
                name: "CliqueId",
                table: "AlignmentClique",
                newName: "AlignmentsAlignmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AlignmentClique_AlignmentId",
                table: "AlignmentClique",
                newName: "IX_AlignmentClique_CliquesCliqueId");

            migrationBuilder.AlterColumn<string>(
                name: "AlignmentName",
                table: "Alignments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_AlignmentClique_Alignments_AlignmentsAlignmentId",
                table: "AlignmentClique",
                column: "AlignmentsAlignmentId",
                principalTable: "Alignments",
                principalColumn: "AlignmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AlignmentClique_Cliques_CliquesCliqueId",
                table: "AlignmentClique",
                column: "CliquesCliqueId",
                principalTable: "Cliques",
                principalColumn: "CliqueId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
