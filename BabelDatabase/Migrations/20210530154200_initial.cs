using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BabelDatabase.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alignments",
                columns: table => new
                {
                    AlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FederalismCentralism = table.Column<int>(type: "int", nullable: false),
                    DemocracyAuthority = table.Column<int>(type: "int", nullable: false),
                    GlobalismIsolationism = table.Column<int>(type: "int", nullable: false),
                    MilitarismPacifism = table.Column<int>(type: "int", nullable: false),
                    SecurityFreedom = table.Column<int>(type: "int", nullable: false),
                    CooperationCompetition = table.Column<int>(type: "int", nullable: false),
                    SecularismSpiritualism = table.Column<int>(type: "int", nullable: false),
                    ProgressivismTraditionalism = table.Column<int>(type: "int", nullable: false),
                    MonoculturalismMulticulturalism = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alignments", x => x.AlignmentId);
                });

            migrationBuilder.CreateTable(
                name: "Cliques",
                columns: table => new
                {
                    CliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CliqueName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Money = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliques", x => x.CliqueId);
                });

            migrationBuilder.CreateTable(
                name: "Committee",
                columns: table => new
                {
                    CommitteeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommitteeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Money = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Committee", x => x.CommitteeId);
                });

            migrationBuilder.CreateTable(
                name: "GameState",
                columns: table => new
                {
                    GameStateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrentYear = table.Column<int>(type: "int", nullable: false),
                    SecondsToMidnight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameState", x => x.GameStateId);
                });

            migrationBuilder.CreateTable(
                name: "PopsimGlobalEthicGroup",
                columns: table => new
                {
                    PopsimGlobalEthicGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PopsimGlobalEthicGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FederalismCentralism = table.Column<int>(type: "int", nullable: false),
                    DemocracyAuthority = table.Column<int>(type: "int", nullable: false),
                    GlobalismIsolationism = table.Column<int>(type: "int", nullable: false),
                    MilitarismPacifism = table.Column<int>(type: "int", nullable: false),
                    SecurityFreedom = table.Column<int>(type: "int", nullable: false),
                    CooperationCompetition = table.Column<int>(type: "int", nullable: false),
                    SecularismSpiritualism = table.Column<int>(type: "int", nullable: false),
                    ProgressivismTraditionalism = table.Column<int>(type: "int", nullable: false),
                    MonoculturalismMulticulturalism = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopsimGlobalEthicGroup", x => x.PopsimGlobalEthicGroupId);
                });

            migrationBuilder.CreateTable(
                name: "PopsimPlanet",
                columns: table => new
                {
                    PopsimPlanetId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopsimPlanet", x => x.PopsimPlanetId);
                });

            migrationBuilder.CreateTable(
                name: "Species",
                columns: table => new
                {
                    SpeciesId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpeciesName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Species", x => x.SpeciesId);
                });

            migrationBuilder.CreateTable(
                name: "AlignmentClique",
                columns: table => new
                {
                    AlignmentsAlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CliquesCliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlignmentClique", x => new { x.AlignmentsAlignmentId, x.CliquesCliqueId });
                    table.ForeignKey(
                        name: "FK_AlignmentClique_Alignments_AlignmentsAlignmentId",
                        column: x => x.AlignmentsAlignmentId,
                        principalTable: "Alignments",
                        principalColumn: "AlignmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlignmentClique_Cliques_CliquesCliqueId",
                        column: x => x.CliquesCliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PopsimPlanetEthicGroup",
                columns: table => new
                {
                    PopsimPlanetEthicGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MembersOnPlanet = table.Column<long>(type: "bigint", nullable: false),
                    PopsimGlobalEthicGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PopsimPlanetId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopsimPlanetEthicGroup", x => x.PopsimPlanetEthicGroupId);
                    table.ForeignKey(
                        name: "FK_PopsimPlanetEthicGroup_PopsimGlobalEthicGroup_PopsimGlobalEthicGroupId",
                        column: x => x.PopsimGlobalEthicGroupId,
                        principalTable: "PopsimGlobalEthicGroup",
                        principalColumn: "PopsimGlobalEthicGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PopsimPlanetEthicGroup_PopsimPlanet_PopsimPlanetId",
                        column: x => x.PopsimPlanetId,
                        principalTable: "PopsimPlanet",
                        principalColumn: "PopsimPlanetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AlignmentSpendings",
                columns: table => new
                {
                    AlignmentSpendingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AlignmentId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PlanetTargetId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GlobalTargetId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlignmentSpendings", x => x.AlignmentSpendingId);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_Alignments_AlignmentId",
                        column: x => x.AlignmentId,
                        principalTable: "Alignments",
                        principalColumn: "AlignmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_PopsimGlobalEthicGroup_GlobalTargetId",
                        column: x => x.GlobalTargetId,
                        principalTable: "PopsimGlobalEthicGroup",
                        principalColumn: "PopsimGlobalEthicGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_PopsimGlobalEthicGroup_PlanetTargetId",
                        column: x => x.PlanetTargetId,
                        principalTable: "PopsimGlobalEthicGroup",
                        principalColumn: "PopsimGlobalEthicGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CliqueInvites",
                columns: table => new
                {
                    CliqueInviteId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CliqueInvites", x => x.CliqueInviteId);
                    table.ForeignKey(
                        name: "FK_CliqueInvites_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CliqueMemberCharacter",
                columns: table => new
                {
                    CliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CliqueMemberCharacter", x => new { x.CliqueId, x.MemberId });
                    table.ForeignKey(
                        name: "FK_CliqueMemberCharacter_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CliqueOfficerCharacter",
                columns: table => new
                {
                    CliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OfficerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CliqueOfficerCharacter", x => new { x.CliqueId, x.OfficerId });
                    table.ForeignKey(
                        name: "FK_CliqueOfficerCharacter_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomSpendings",
                columns: table => new
                {
                    CustomSpendingId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SpendingDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CliqueId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomSpendings", x => x.CustomSpendingId);
                    table.ForeignKey(
                        name: "FK_CustomSpendings_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscordUsers",
                columns: table => new
                {
                    DiscordUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    ActiveCharacterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActiveCharacterCharacterId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUsers", x => x.DiscordUserId);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharacterName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearOfBirth = table.Column<int>(type: "int", nullable: false),
                    YearOfDeath = table.Column<int>(type: "int", nullable: false),
                    CauseOfDeath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CharacterBio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpeciesId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DiscordUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CommitteeId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_Characters_Committee_CommitteeId",
                        column: x => x.CommitteeId,
                        principalTable: "Committee",
                        principalColumn: "CommitteeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Characters_DiscordUsers_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Characters_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "SpeciesId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffActions",
                columns: table => new
                {
                    StaffActionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStarted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffActions", x => x.StaffActionId);
                    table.ForeignKey(
                        name: "FK_StaffActions_DiscordUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId");
                });

            migrationBuilder.CreateTable(
                name: "PlayerStaffAction",
                columns: table => new
                {
                    PlayerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StaffActionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStaffAction", x => new { x.PlayerId, x.StaffActionId });
                    table.ForeignKey(
                        name: "FK_PlayerStaffAction_DiscordUsers_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlayerStaffAction_StaffActions_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "StaffActions",
                        principalColumn: "StaffActionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffActionPosts",
                columns: table => new
                {
                    StaffActionPostId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimePosted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StaffActionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffActionPosts", x => x.StaffActionPostId);
                    table.ForeignKey(
                        name: "FK_StaffActionPosts_DiscordUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffActionPosts_StaffActions_StaffActionId",
                        column: x => x.StaffActionId,
                        principalTable: "StaffActions",
                        principalColumn: "StaffActionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffStaffAction",
                columns: table => new
                {
                    StaffId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StaffActionId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffStaffAction", x => new { x.StaffId, x.StaffActionId });
                    table.ForeignKey(
                        name: "FK_StaffStaffAction_DiscordUsers_StaffId",
                        column: x => x.StaffId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordUserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StaffStaffAction_StaffActions_StaffActionId",
                        column: x => x.StaffActionId,
                        principalTable: "StaffActions",
                        principalColumn: "StaffActionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DiscordUsers",
                columns: new[] { "DiscordUserId", "ActiveCharacterCharacterId", "ActiveCharacterId", "IsAdmin", "UserName" },
                values: new object[] { "75968535074967552", null, null, true, "Obi" });

            migrationBuilder.InsertData(
                table: "GameState",
                columns: new[] { "GameStateId", "CurrentYear", "SecondsToMidnight" },
                values: new object[] { 1, 2500, 10800 });

            migrationBuilder.InsertData(
                table: "Species",
                columns: new[] { "SpeciesId", "SpeciesName" },
                values: new object[,]
                {
                    { "1", "Human" },
                    { "2", "Zelvan" },
                    { "3", "Liaran" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlignmentClique_CliquesCliqueId",
                table: "AlignmentClique",
                column: "CliquesCliqueId");

            migrationBuilder.CreateIndex(
                name: "IX_AlignmentSpendings_AlignmentId",
                table: "AlignmentSpendings",
                column: "AlignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AlignmentSpendings_CharacterId",
                table: "AlignmentSpendings",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_AlignmentSpendings_CliqueId",
                table: "AlignmentSpendings",
                column: "CliqueId");

            migrationBuilder.CreateIndex(
                name: "IX_AlignmentSpendings_GlobalTargetId",
                table: "AlignmentSpendings",
                column: "GlobalTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_AlignmentSpendings_PlanetTargetId",
                table: "AlignmentSpendings",
                column: "PlanetTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_CommitteeId",
                table: "Characters",
                column: "CommitteeId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_DiscordUserId",
                table: "Characters",
                column: "DiscordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Characters_SpeciesId",
                table: "Characters",
                column: "SpeciesId");

            migrationBuilder.CreateIndex(
                name: "IX_CliqueInvites_CharacterId",
                table: "CliqueInvites",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CliqueInvites_CliqueId",
                table: "CliqueInvites",
                column: "CliqueId");

            migrationBuilder.CreateIndex(
                name: "IX_CliqueMemberCharacter_MemberId",
                table: "CliqueMemberCharacter",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_CliqueOfficerCharacter_OfficerId",
                table: "CliqueOfficerCharacter",
                column: "OfficerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomSpendings_CharacterId",
                table: "CustomSpendings",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomSpendings_CliqueId",
                table: "CustomSpendings",
                column: "CliqueId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscordUsers_ActiveCharacterCharacterId",
                table: "DiscordUsers",
                column: "ActiveCharacterCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_PopsimPlanetEthicGroup_PopsimGlobalEthicGroupId",
                table: "PopsimPlanetEthicGroup",
                column: "PopsimGlobalEthicGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PopsimPlanetEthicGroup_PopsimPlanetId",
                table: "PopsimPlanetEthicGroup",
                column: "PopsimPlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffActionPosts_AuthorId",
                table: "StaffActionPosts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffActionPosts_StaffActionId",
                table: "StaffActionPosts",
                column: "StaffActionId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffActions_OwnerId",
                table: "StaffActions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffStaffAction_StaffActionId",
                table: "StaffStaffAction",
                column: "StaffActionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlignmentSpendings_Characters_CharacterId",
                table: "AlignmentSpendings",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CliqueInvites_Characters_CharacterId",
                table: "CliqueInvites",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CliqueMemberCharacter_Characters_MemberId",
                table: "CliqueMemberCharacter",
                column: "MemberId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CliqueOfficerCharacter_Characters_OfficerId",
                table: "CliqueOfficerCharacter",
                column: "OfficerId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomSpendings_Characters_CharacterId",
                table: "CustomSpendings",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiscordUsers_Characters_ActiveCharacterCharacterId",
                table: "DiscordUsers",
                column: "ActiveCharacterCharacterId",
                principalTable: "Characters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DiscordUsers_Characters_ActiveCharacterCharacterId",
                table: "DiscordUsers");

            migrationBuilder.DropTable(
                name: "AlignmentClique");

            migrationBuilder.DropTable(
                name: "AlignmentSpendings");

            migrationBuilder.DropTable(
                name: "CliqueInvites");

            migrationBuilder.DropTable(
                name: "CliqueMemberCharacter");

            migrationBuilder.DropTable(
                name: "CliqueOfficerCharacter");

            migrationBuilder.DropTable(
                name: "CustomSpendings");

            migrationBuilder.DropTable(
                name: "GameState");

            migrationBuilder.DropTable(
                name: "PlayerStaffAction");

            migrationBuilder.DropTable(
                name: "PopsimPlanetEthicGroup");

            migrationBuilder.DropTable(
                name: "StaffActionPosts");

            migrationBuilder.DropTable(
                name: "StaffStaffAction");

            migrationBuilder.DropTable(
                name: "Alignments");

            migrationBuilder.DropTable(
                name: "Cliques");

            migrationBuilder.DropTable(
                name: "PopsimGlobalEthicGroup");

            migrationBuilder.DropTable(
                name: "PopsimPlanet");

            migrationBuilder.DropTable(
                name: "StaffActions");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Committee");

            migrationBuilder.DropTable(
                name: "DiscordUsers");

            migrationBuilder.DropTable(
                name: "Species");
        }
    }
}
