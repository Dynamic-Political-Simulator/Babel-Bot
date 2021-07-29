using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class IdentityRemoveP2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Party",
                table: "Party");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Military",
                table: "Military");

            migrationBuilder.RenameTable(
                name: "Party",
                newName: "Parties");

            migrationBuilder.RenameTable(
                name: "Military",
                newName: "Militaries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parties",
                table: "Parties",
                column: "PartyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Militaries",
                table: "Militaries",
                column: "RevolutionaryGuardId");

            migrationBuilder.CreateTable(
                name: "Data",
                columns: table => new
                {
                    DataId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Stratas = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BaseGdpPerPop = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Data", x => x.DataId);
                });

            migrationBuilder.CreateTable(
                name: "Empires",
                columns: table => new
                {
                    EmpireId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalOutput = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EconGmData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GeneralAssembly = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PopsimGmData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empires", x => x.EmpireId);
                });

            migrationBuilder.CreateTable(
                name: "InfrastructureData",
                columns: table => new
                {
                    InfraStructureDataId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GdpPerInfrastructure = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Infrastructures = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfrastructureData", x => x.InfraStructureDataId);
                });

            migrationBuilder.CreateTable(
                name: "Armies",
                columns: table => new
                {
                    ArmyId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Armies", x => x.ArmyId);
                    table.ForeignKey(
                        name: "FK_Armies_Empires_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Empires",
                        principalColumn: "EmpireId");
                });

            migrationBuilder.CreateTable(
                name: "Fleets",
                columns: table => new
                {
                    FleetId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OwnerID = table.Column<int>(type: "int", nullable: false),
                    MilitaryPower = table.Column<double>(type: "float", nullable: false),
                    SystemId = table.Column<int>(type: "int", nullable: false),
                    EmpireId = table.Column<int>(type: "int", nullable: true),
                    EmpireId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fleets", x => x.FleetId);
                    table.ForeignKey(
                        name: "FK_Fleets_Empires_EmpireId",
                        column: x => x.EmpireId,
                        principalTable: "Empires",
                        principalColumn: "EmpireId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fleets_Empires_EmpireId1",
                        column: x => x.EmpireId1,
                        principalTable: "Empires",
                        principalColumn: "EmpireId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fleets_Empires_OwnerID",
                        column: x => x.OwnerID,
                        principalTable: "Empires",
                        principalColumn: "EmpireId");
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ShipId = table.Column<int>(type: "int", nullable: false),
                    FleetId = table.Column<int>(type: "int", nullable: false),
                    ShipName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ShipId);
                    table.ForeignKey(
                        name: "FK_Ships_Fleets_FleetId",
                        column: x => x.FleetId,
                        principalTable: "Fleets",
                        principalColumn: "FleetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Starbases",
                columns: table => new
                {
                    StarbaseId = table.Column<int>(type: "int", nullable: false),
                    Owner = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Modules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Buildings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FleetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Starbases", x => x.StarbaseId);
                    table.ForeignKey(
                        name: "FK_Starbases_Fleets_FleetId",
                        column: x => x.FleetId,
                        principalTable: "Fleets",
                        principalColumn: "FleetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GalacticObjects",
                columns: table => new
                {
                    GalacticObjectId = table.Column<int>(type: "int", nullable: false),
                    PosX = table.Column<float>(type: "real", nullable: false),
                    PosY = table.Column<float>(type: "real", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hyperlanes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StarbaseId = table.Column<int>(type: "int", nullable: false),
                    EmpireId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GalacticObjects", x => x.GalacticObjectId);
                    table.ForeignKey(
                        name: "FK_GalacticObjects_Empires_EmpireId",
                        column: x => x.EmpireId,
                        principalTable: "Empires",
                        principalColumn: "EmpireId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GalacticObjects_Starbases_StarbaseId",
                        column: x => x.StarbaseId,
                        principalTable: "Starbases",
                        principalColumn: "StarbaseId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Planets",
                columns: table => new
                {
                    PlanetId = table.Column<int>(type: "int", nullable: false),
                    PlanetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Population = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    ControllerId = table.Column<int>(type: "int", nullable: false),
                    GalacticObjectId = table.Column<int>(type: "int", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EconGmData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PopsimGmData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExecutiveAlignmentAlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    LegislativeAlignmentAlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PartyAlignmentAlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planets", x => x.PlanetId);
                    table.ForeignKey(
                        name: "FK_Planets_Alignments_ExecutiveAlignmentAlignmentId",
                        column: x => x.ExecutiveAlignmentAlignmentId,
                        principalTable: "Alignments",
                        principalColumn: "AlignmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Planets_Alignments_LegislativeAlignmentAlignmentId",
                        column: x => x.LegislativeAlignmentAlignmentId,
                        principalTable: "Alignments",
                        principalColumn: "AlignmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Planets_Alignments_PartyAlignmentAlignmentId",
                        column: x => x.PartyAlignmentAlignmentId,
                        principalTable: "Alignments",
                        principalColumn: "AlignmentId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Planets_Empires_ControllerId",
                        column: x => x.ControllerId,
                        principalTable: "Empires",
                        principalColumn: "EmpireId");
                    table.ForeignKey(
                        name: "FK_Planets_Empires_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Empires",
                        principalColumn: "EmpireId");
                    table.ForeignKey(
                        name: "FK_Planets_GalacticObjects_GalacticObjectId",
                        column: x => x.GalacticObjectId,
                        principalTable: "GalacticObjects",
                        principalColumn: "GalacticObjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    BuildingId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ruined = table.Column<bool>(type: "bit", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.BuildingId);
                    table.ForeignKey(
                        name: "FK_Buildings_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "PlanetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_Districts_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "PlanetId",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "Pops",
                columns: table => new
                {
                    PopId = table.Column<int>(type: "int", nullable: false),
                    PlanetId = table.Column<int>(type: "int", nullable: false),
                    Job = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Strata = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Power = table.Column<float>(type: "real", nullable: false),
                    Happiness = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pops", x => x.PopId);
                    table.ForeignKey(
                        name: "FK_Pops_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "PlanetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PopsimPlanetEthicGroup_PlanetId",
                table: "PopsimPlanetEthicGroup",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Armies_OwnerId",
                table: "Armies",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Armies_PlanetId",
                table: "Armies",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_PlanetId",
                table: "Buildings",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_PlanetId",
                table: "Districts",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_EmpireId",
                table: "Fleets",
                column: "EmpireId");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_EmpireId1",
                table: "Fleets",
                column: "EmpireId1");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_OwnerID",
                table: "Fleets",
                column: "OwnerID");

            migrationBuilder.CreateIndex(
                name: "IX_Fleets_SystemId",
                table: "Fleets",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_GalacticObjects_EmpireId",
                table: "GalacticObjects",
                column: "EmpireId");

            migrationBuilder.CreateIndex(
                name: "IX_GalacticObjects_StarbaseId",
                table: "GalacticObjects",
                column: "StarbaseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanetarySystems_PlanetId",
                table: "PlanetarySystems",
                column: "PlanetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Planets_ControllerId",
                table: "Planets",
                column: "ControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_ExecutiveAlignmentAlignmentId",
                table: "Planets",
                column: "ExecutiveAlignmentAlignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_GalacticObjectId",
                table: "Planets",
                column: "GalacticObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_LegislativeAlignmentAlignmentId",
                table: "Planets",
                column: "LegislativeAlignmentAlignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_OwnerId",
                table: "Planets",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_PartyAlignmentAlignmentId",
                table: "Planets",
                column: "PartyAlignmentAlignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Pops_PlanetId",
                table: "Pops",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_FleetId",
                table: "Ships",
                column: "FleetId");

            migrationBuilder.CreateIndex(
                name: "IX_Starbases_FleetId",
                table: "Starbases",
                column: "FleetId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PopsimPlanetEthicGroup_Planets_PlanetId",
                table: "PopsimPlanetEthicGroup",
                column: "PlanetId",
                principalTable: "Planets",
                principalColumn: "PlanetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Armies_Planets_PlanetId",
                table: "Armies",
                column: "PlanetId",
                principalTable: "Planets",
                principalColumn: "PlanetId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Fleets_GalacticObjects_SystemId",
                table: "Fleets",
                column: "SystemId",
                principalTable: "GalacticObjects",
                principalColumn: "GalacticObjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PopsimPlanetEthicGroup_Planets_PlanetId",
                table: "PopsimPlanetEthicGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Empires_EmpireId",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Empires_EmpireId1",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_Empires_OwnerID",
                table: "Fleets");

            migrationBuilder.DropForeignKey(
                name: "FK_GalacticObjects_Empires_EmpireId",
                table: "GalacticObjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Fleets_GalacticObjects_SystemId",
                table: "Fleets");

            migrationBuilder.DropTable(
                name: "Armies");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "InfrastructureData");

            migrationBuilder.DropTable(
                name: "PlanetarySystems");

            migrationBuilder.DropTable(
                name: "Pops");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Planets");

            migrationBuilder.DropTable(
                name: "Empires");

            migrationBuilder.DropTable(
                name: "GalacticObjects");

            migrationBuilder.DropTable(
                name: "Starbases");

            migrationBuilder.DropTable(
                name: "Fleets");

            migrationBuilder.DropIndex(
                name: "IX_PopsimPlanetEthicGroup_PlanetId",
                table: "PopsimPlanetEthicGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parties",
                table: "Parties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Militaries",
                table: "Militaries");

            migrationBuilder.RenameTable(
                name: "Parties",
                newName: "Party");

            migrationBuilder.RenameTable(
                name: "Militaries",
                newName: "Military");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Party",
                table: "Party",
                column: "PartyId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Military",
                table: "Military",
                column: "RevolutionaryGuardId");
        }
    }
}
