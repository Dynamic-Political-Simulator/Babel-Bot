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
                    Establishment = table.Column<float>(type: "real", nullable: false),
                    UpperPartyModifier = table.Column<float>(type: "real", nullable: false),
                    LowerPartyModiifer = table.Column<float>(type: "real", nullable: false),
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
                name: "DiscordUsers",
                columns: table => new
                {
                    DiscordUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    ActiveCharacterId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUsers", x => x.DiscordUserId);
                });

            migrationBuilder.CreateTable(
                name: "Empires",
                columns: table => new
                {
                    EmpireId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "Graveyards",
                columns: table => new
                {
                    ChannelId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServerId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graveyards", x => x.ChannelId);
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
                name: "Militaries",
                columns: table => new
                {
                    RevolutionaryGuardId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MilitaryPoliticisation = table.Column<float>(type: "real", nullable: false),
                    MilitaryGroups = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MilitaryFactions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Militaries", x => x.RevolutionaryGuardId);
                });

            migrationBuilder.CreateTable(
                name: "Parties",
                columns: table => new
                {
                    PartyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PopGroupEnlistment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperPartyMembership = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerPartyMembership = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperPartyAffinity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerPartyAffinity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperPartyPercentage = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Parties", x => x.PartyId);
                });

            migrationBuilder.CreateTable(
                name: "PopsimGlobalEthicGroups",
                columns: table => new
                {
                    PopsimGlobalEthicGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PopsimGlobalEthicGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PartyInvolvementFactor = table.Column<int>(type: "int", nullable: false),
                    Radicalisation = table.Column<float>(type: "real", nullable: false),
                    PartyEnlistmentModifier = table.Column<float>(type: "real", nullable: false),
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
                    table.PrimaryKey("PK_PopsimGlobalEthicGroups", x => x.PopsimGlobalEthicGroupId);
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
                name: "VoteMessages",
                columns: table => new
                {
                    MessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    CreatorId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    ChannelId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    EndTime = table.Column<long>(type: "bigint", nullable: false),
                    TimeSpan = table.Column<long>(type: "bigint", nullable: false),
                    Anonymous = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteMessages", x => x.MessageId);
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
                name: "VoteEntries",
                columns: table => new
                {
                    VoteEntryId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    VoteMessageId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    Vote = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoteEntries", x => x.VoteEntryId);
                    table.ForeignKey(
                        name: "FK_VoteEntries_VoteMessages_VoteMessageId",
                        column: x => x.VoteMessageId,
                        principalTable: "VoteMessages",
                        principalColumn: "MessageId",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_AlignmentSpendings_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_PopsimGlobalEthicGroups_GlobalTargetId",
                        column: x => x.GlobalTargetId,
                        principalTable: "PopsimGlobalEthicGroups",
                        principalColumn: "PopsimGlobalEthicGroupId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlignmentSpendings_PopsimGlobalEthicGroups_PlanetTargetId",
                        column: x => x.PlanetTargetId,
                        principalTable: "PopsimGlobalEthicGroups",
                        principalColumn: "PopsimGlobalEthicGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterDeathTimers",
                columns: table => new
                {
                    CharacterDeathTimerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CharacterId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    YearOfDeath = table.Column<int>(type: "int", nullable: false),
                    DeathTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDeathTimers", x => x.CharacterDeathTimerId);
                    table.ForeignKey(
                        name: "FK_CharacterDeathTimers_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
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
                        name: "FK_CliqueInvites_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_CliqueMemberCharacter_Characters_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_CliqueOfficerCharacter_Characters_OfficerId",
                        column: x => x.OfficerId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_CustomSpendings_Characters_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Characters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomSpendings_Cliques_CliqueId",
                        column: x => x.CliqueId,
                        principalTable: "Cliques",
                        principalColumn: "CliqueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Armies",
                columns: table => new
                {
                    ArmyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    FleetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    ShipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    StarbaseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    GalacticObjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlanetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlanetClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Population = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    ControllerId = table.Column<int>(type: "int", nullable: false),
                    GalacticObjectId = table.Column<int>(type: "int", nullable: false),
                    Output = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EconGmData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PopsimGmData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planets", x => x.PlanetId);
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
                    BuildingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    DistrictId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                name: "Pops",
                columns: table => new
                {
                    PopId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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

            migrationBuilder.CreateTable(
                name: "PopsimPlanetEthicGroup",
                columns: table => new
                {
                    PopsimPlanetEthicGroupId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MembersOnPlanet = table.Column<long>(type: "bigint", nullable: false),
                    Percentage = table.Column<float>(type: "real", nullable: false),
                    PopsimGlobalEthicGroupId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PlanetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PopsimPlanetEthicGroup", x => x.PopsimPlanetEthicGroupId);
                    table.ForeignKey(
                        name: "FK_PopsimPlanetEthicGroup_Planets_PlanetId",
                        column: x => x.PlanetId,
                        principalTable: "Planets",
                        principalColumn: "PlanetId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PopsimPlanetEthicGroup_PopsimGlobalEthicGroups_PopsimGlobalEthicGroupId",
                        column: x => x.PopsimGlobalEthicGroupId,
                        principalTable: "PopsimGlobalEthicGroups",
                        principalColumn: "PopsimGlobalEthicGroupId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "DiscordUsers",
                columns: new[] { "DiscordUserId", "ActiveCharacterId", "IsAdmin", "UserName" },
                values: new object[] { "75968535074967552", null, true, "Obi" });

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
                name: "IX_CharacterDeathTimers_CharacterId",
                table: "CharacterDeathTimers",
                column: "CharacterId",
                unique: true,
                filter: "[CharacterId] IS NOT NULL");

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
                name: "IX_Planets_ControllerId",
                table: "Planets",
                column: "ControllerId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_GalacticObjectId",
                table: "Planets",
                column: "GalacticObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Planets_OwnerId",
                table: "Planets",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pops_PlanetId",
                table: "Pops",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_PopsimPlanetEthicGroup_PlanetId",
                table: "PopsimPlanetEthicGroup",
                column: "PlanetId");

            migrationBuilder.CreateIndex(
                name: "IX_PopsimPlanetEthicGroup_PopsimGlobalEthicGroupId",
                table: "PopsimPlanetEthicGroup",
                column: "PopsimGlobalEthicGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_FleetId",
                table: "Ships",
                column: "FleetId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Starbases_FleetId",
                table: "Starbases",
                column: "FleetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoteEntries_VoteMessageId",
                table: "VoteEntries",
                column: "VoteMessageId");

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
                name: "AlignmentClique");

            migrationBuilder.DropTable(
                name: "AlignmentSpendings");

            migrationBuilder.DropTable(
                name: "Armies");

            migrationBuilder.DropTable(
                name: "Buildings");

            migrationBuilder.DropTable(
                name: "CharacterDeathTimers");

            migrationBuilder.DropTable(
                name: "CliqueInvites");

            migrationBuilder.DropTable(
                name: "CliqueMemberCharacter");

            migrationBuilder.DropTable(
                name: "CliqueOfficerCharacter");

            migrationBuilder.DropTable(
                name: "CustomSpendings");

            migrationBuilder.DropTable(
                name: "Data");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "GameState");

            migrationBuilder.DropTable(
                name: "Graveyards");

            migrationBuilder.DropTable(
                name: "InfrastructureData");

            migrationBuilder.DropTable(
                name: "Militaries");

            migrationBuilder.DropTable(
                name: "Parties");

            migrationBuilder.DropTable(
                name: "PlayerStaffAction");

            migrationBuilder.DropTable(
                name: "Pops");

            migrationBuilder.DropTable(
                name: "PopsimPlanetEthicGroup");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "StaffActionPosts");

            migrationBuilder.DropTable(
                name: "StaffStaffAction");

            migrationBuilder.DropTable(
                name: "VoteEntries");

            migrationBuilder.DropTable(
                name: "Alignments");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "Cliques");

            migrationBuilder.DropTable(
                name: "Planets");

            migrationBuilder.DropTable(
                name: "PopsimGlobalEthicGroups");

            migrationBuilder.DropTable(
                name: "StaffActions");

            migrationBuilder.DropTable(
                name: "VoteMessages");

            migrationBuilder.DropTable(
                name: "Committee");

            migrationBuilder.DropTable(
                name: "Species");

            migrationBuilder.DropTable(
                name: "DiscordUsers");

            migrationBuilder.DropTable(
                name: "Empires");

            migrationBuilder.DropTable(
                name: "GalacticObjects");

            migrationBuilder.DropTable(
                name: "Starbases");

            migrationBuilder.DropTable(
                name: "Fleets");
        }
    }
}
