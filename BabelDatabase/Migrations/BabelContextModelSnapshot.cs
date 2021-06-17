﻿// <auto-generated />
using System;
using BabelDatabase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BabelDatabase.Migrations
{
    [DbContext(typeof(BabelContext))]
    partial class BabelContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.5")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AlignmentClique", b =>
                {
                    b.Property<string>("AlignmentsAlignmentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CliquesCliqueId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AlignmentsAlignmentId", "CliquesCliqueId");

                    b.HasIndex("CliquesCliqueId");

                    b.ToTable("AlignmentClique");
                });

            modelBuilder.Entity("BabelDatabase.Alignment", b =>
                {
                    b.Property<string>("AlignmentId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CooperationCompetition")
                        .HasColumnType("int");

                    b.Property<int>("DemocracyAuthority")
                        .HasColumnType("int");

                    b.Property<int>("FederalismCentralism")
                        .HasColumnType("int");

                    b.Property<int>("GlobalismIsolationism")
                        .HasColumnType("int");

                    b.Property<int>("MilitarismPacifism")
                        .HasColumnType("int");

                    b.Property<int>("MonoculturalismMulticulturalism")
                        .HasColumnType("int");

                    b.Property<int>("ProgressivismTraditionalism")
                        .HasColumnType("int");

                    b.Property<int>("SecularismSpiritualism")
                        .HasColumnType("int");

                    b.Property<int>("SecurityFreedom")
                        .HasColumnType("int");

                    b.HasKey("AlignmentId");

                    b.ToTable("Alignments");
                });

            modelBuilder.Entity("BabelDatabase.AlignmentSpending", b =>
                {
                    b.Property<string>("AlignmentSpendingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AlignmentId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("CharacterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CliqueId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("GlobalTargetId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PlanetTargetId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AlignmentSpendingId");

                    b.HasIndex("AlignmentId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("CliqueId");

                    b.HasIndex("GlobalTargetId");

                    b.HasIndex("PlanetTargetId");

                    b.ToTable("AlignmentSpendings");
                });

            modelBuilder.Entity("BabelDatabase.Character", b =>
                {
                    b.Property<string>("CharacterId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CauseOfDeath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CharacterBio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CharacterName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CommitteeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("DiscordUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SpeciesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("YearOfBirth")
                        .HasColumnType("int");

                    b.Property<int>("YearOfDeath")
                        .HasColumnType("int");

                    b.HasKey("CharacterId");

                    b.HasIndex("CommitteeId");

                    b.HasIndex("DiscordUserId");

                    b.HasIndex("SpeciesId");

                    b.ToTable("Characters");
                });

            modelBuilder.Entity("BabelDatabase.Clique", b =>
                {
                    b.Property<string>("CliqueId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CliqueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Money")
                        .HasColumnType("decimal(20,0)");

                    b.HasKey("CliqueId");

                    b.ToTable("Cliques");
                });

            modelBuilder.Entity("BabelDatabase.CliqueInvite", b =>
                {
                    b.Property<string>("CliqueInviteId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CharacterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CliqueId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CliqueInviteId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("CliqueId");

                    b.ToTable("CliqueInvites");
                });

            modelBuilder.Entity("BabelDatabase.CliqueMemberCharacter", b =>
                {
                    b.Property<string>("CliqueId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("MemberId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CliqueId", "MemberId");

                    b.HasIndex("MemberId");

                    b.ToTable("CliqueMemberCharacter");
                });

            modelBuilder.Entity("BabelDatabase.CliqueOfficerCharacter", b =>
                {
                    b.Property<string>("CliqueId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OfficerId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("CliqueId", "OfficerId");

                    b.HasIndex("OfficerId");

                    b.ToTable("CliqueOfficerCharacter");
                });

            modelBuilder.Entity("BabelDatabase.Committee", b =>
                {
                    b.Property<string>("CommitteeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CommitteeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Money")
                        .HasColumnType("bigint");

                    b.HasKey("CommitteeId");

                    b.ToTable("Committee");
                });

            modelBuilder.Entity("BabelDatabase.CustomSpending", b =>
                {
                    b.Property<string>("CustomSpendingId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(20,0)");

                    b.Property<string>("CharacterId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CliqueId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SpendingDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CustomSpendingId");

                    b.HasIndex("CharacterId");

                    b.HasIndex("CliqueId");

                    b.ToTable("CustomSpendings");
                });

            modelBuilder.Entity("BabelDatabase.DiscordUser", b =>
                {
                    b.Property<string>("DiscordUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ActiveCharacterCharacterId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ActiveCharacterId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("DiscordUserId");

                    b.HasIndex("ActiveCharacterCharacterId");

                    b.ToTable("DiscordUsers");

                    b.HasData(
                        new
                        {
                            DiscordUserId = "75968535074967552",
                            IsAdmin = true,
                            UserName = "Obi"
                        });
                });

            modelBuilder.Entity("BabelDatabase.GameState", b =>
                {
                    b.Property<int>("GameStateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CurrentYear")
                        .HasColumnType("int");

                    b.Property<int>("SecondsToMidnight")
                        .HasColumnType("int");

                    b.HasKey("GameStateId");

                    b.ToTable("GameState");

                    b.HasData(
                        new
                        {
                            GameStateId = 1,
                            CurrentYear = 2500,
                            SecondsToMidnight = 10800
                        });
                });

            modelBuilder.Entity("BabelDatabase.PlayerStaffAction", b =>
                {
                    b.Property<string>("PlayerId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("StaffActionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PlayerId", "StaffActionId");

                    b.ToTable("PlayerStaffAction");
                });

            modelBuilder.Entity("BabelDatabase.PopsimGlobalEthicGroup", b =>
                {
                    b.Property<string>("PopsimGlobalEthicGroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("CooperationCompetition")
                        .HasColumnType("int");

                    b.Property<int>("DemocracyAuthority")
                        .HasColumnType("int");

                    b.Property<int>("FederalismCentralism")
                        .HasColumnType("int");

                    b.Property<int>("GlobalismIsolationism")
                        .HasColumnType("int");

                    b.Property<int>("MilitarismPacifism")
                        .HasColumnType("int");

                    b.Property<int>("MonoculturalismMulticulturalism")
                        .HasColumnType("int");

                    b.Property<string>("PopsimGlobalEthicGroupName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProgressivismTraditionalism")
                        .HasColumnType("int");

                    b.Property<int>("SecularismSpiritualism")
                        .HasColumnType("int");

                    b.Property<int>("SecurityFreedom")
                        .HasColumnType("int");

                    b.HasKey("PopsimGlobalEthicGroupId");

                    b.ToTable("PopsimGlobalEthicGroup");
                });

            modelBuilder.Entity("BabelDatabase.PopsimPlanet", b =>
                {
                    b.Property<string>("PopsimPlanetId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PlanetDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlanetName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PopsimPlanetId");

                    b.ToTable("PopsimPlanet");
                });

            modelBuilder.Entity("BabelDatabase.PopsimPlanetEthicGroup", b =>
                {
                    b.Property<string>("PopsimPlanetEthicGroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<long>("MembersOnPlanet")
                        .HasColumnType("bigint");

                    b.Property<string>("PopsimGlobalEthicGroupId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PopsimPlanetId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PopsimPlanetEthicGroupId");

                    b.HasIndex("PopsimGlobalEthicGroupId");

                    b.HasIndex("PopsimPlanetId");

                    b.ToTable("PopsimPlanetEthicGroup");
                });

            modelBuilder.Entity("BabelDatabase.Species", b =>
                {
                    b.Property<string>("SpeciesId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("SpeciesName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SpeciesId");

                    b.ToTable("Species");

                    b.HasData(
                        new
                        {
                            SpeciesId = "1",
                            SpeciesName = "Human"
                        },
                        new
                        {
                            SpeciesId = "2",
                            SpeciesName = "Zelvan"
                        },
                        new
                        {
                            SpeciesId = "3",
                            SpeciesName = "Liaran"
                        });
                });

            modelBuilder.Entity("BabelDatabase.StaffAction", b =>
                {
                    b.Property<string>("StaffActionId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimeStarted")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StaffActionId");

                    b.HasIndex("OwnerId");

                    b.ToTable("StaffActions");
                });

            modelBuilder.Entity("BabelDatabase.StaffActionPost", b =>
                {
                    b.Property<string>("StaffActionPostId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("AuthorId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StaffActionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TimePosted")
                        .HasColumnType("datetime2");

                    b.HasKey("StaffActionPostId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("StaffActionId");

                    b.ToTable("StaffActionPosts");
                });

            modelBuilder.Entity("BabelDatabase.StaffStaffAction", b =>
                {
                    b.Property<string>("StaffId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("StaffActionId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("StaffId", "StaffActionId");

                    b.HasIndex("StaffActionId");

                    b.ToTable("StaffStaffAction");
                });

            modelBuilder.Entity("BabelDatabase.VoteMessage", b =>
                {
                    b.Property<decimal>("MessageId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("decimal(20,0)")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.None);

                    b.Property<decimal>("ChannelId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<decimal>("CreatorId")
                        .HasColumnType("decimal(20,0)");

                    b.Property<long>("EndTime")
                        .HasColumnType("bigint");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("MessageId");

                    b.ToTable("VoteMessages");
                });

            modelBuilder.Entity("AlignmentClique", b =>
                {
                    b.HasOne("BabelDatabase.Alignment", null)
                        .WithMany()
                        .HasForeignKey("AlignmentsAlignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Clique", null)
                        .WithMany()
                        .HasForeignKey("CliquesCliqueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BabelDatabase.AlignmentSpending", b =>
                {
                    b.HasOne("BabelDatabase.Alignment", "Alignment")
                        .WithMany()
                        .HasForeignKey("AlignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Clique", "Clique")
                        .WithMany()
                        .HasForeignKey("CliqueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.PopsimGlobalEthicGroup", "GlobalTarget")
                        .WithMany()
                        .HasForeignKey("GlobalTargetId");

                    b.HasOne("BabelDatabase.PopsimGlobalEthicGroup", "PlanetTarget")
                        .WithMany()
                        .HasForeignKey("PlanetTargetId");

                    b.Navigation("Alignment");

                    b.Navigation("Character");

                    b.Navigation("Clique");

                    b.Navigation("GlobalTarget");

                    b.Navigation("PlanetTarget");
                });

            modelBuilder.Entity("BabelDatabase.Character", b =>
                {
                    b.HasOne("BabelDatabase.Committee", null)
                        .WithMany("CommitteeMembers")
                        .HasForeignKey("CommitteeId");

                    b.HasOne("BabelDatabase.DiscordUser", "DiscordUser")
                        .WithMany("Characters")
                        .HasForeignKey("DiscordUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Species", "Species")
                        .WithMany()
                        .HasForeignKey("SpeciesId");

                    b.Navigation("DiscordUser");

                    b.Navigation("Species");
                });

            modelBuilder.Entity("BabelDatabase.CliqueInvite", b =>
                {
                    b.HasOne("BabelDatabase.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Clique", "Clique")
                        .WithMany()
                        .HasForeignKey("CliqueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Clique");
                });

            modelBuilder.Entity("BabelDatabase.CliqueMemberCharacter", b =>
                {
                    b.HasOne("BabelDatabase.Clique", "Clique")
                        .WithMany("CliqueMembers")
                        .HasForeignKey("CliqueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Character", "Member")
                        .WithMany("Cliques")
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clique");

                    b.Navigation("Member");
                });

            modelBuilder.Entity("BabelDatabase.CliqueOfficerCharacter", b =>
                {
                    b.HasOne("BabelDatabase.Clique", "Clique")
                        .WithMany("CliqueOfficers")
                        .HasForeignKey("CliqueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Character", "Officer")
                        .WithMany()
                        .HasForeignKey("OfficerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Clique");

                    b.Navigation("Officer");
                });

            modelBuilder.Entity("BabelDatabase.CustomSpending", b =>
                {
                    b.HasOne("BabelDatabase.Character", "Character")
                        .WithMany()
                        .HasForeignKey("CharacterId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.Clique", "Clique")
                        .WithMany()
                        .HasForeignKey("CliqueId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Character");

                    b.Navigation("Clique");
                });

            modelBuilder.Entity("BabelDatabase.DiscordUser", b =>
                {
                    b.HasOne("BabelDatabase.Character", "ActiveCharacter")
                        .WithMany()
                        .HasForeignKey("ActiveCharacterCharacterId");

                    b.Navigation("ActiveCharacter");
                });

            modelBuilder.Entity("BabelDatabase.PlayerStaffAction", b =>
                {
                    b.HasOne("BabelDatabase.DiscordUser", "Player")
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.StaffAction", "StaffAction")
                        .WithMany("Players")
                        .HasForeignKey("PlayerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player");

                    b.Navigation("StaffAction");
                });

            modelBuilder.Entity("BabelDatabase.PopsimPlanetEthicGroup", b =>
                {
                    b.HasOne("BabelDatabase.PopsimGlobalEthicGroup", "PopsimGlobalEthicGroup")
                        .WithMany("PlanetaryEthicGroups")
                        .HasForeignKey("PopsimGlobalEthicGroupId");

                    b.HasOne("BabelDatabase.PopsimPlanet", "PopsimPlanet")
                        .WithMany()
                        .HasForeignKey("PopsimPlanetId");

                    b.Navigation("PopsimGlobalEthicGroup");

                    b.Navigation("PopsimPlanet");
                });

            modelBuilder.Entity("BabelDatabase.StaffAction", b =>
                {
                    b.HasOne("BabelDatabase.DiscordUser", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("BabelDatabase.StaffActionPost", b =>
                {
                    b.HasOne("BabelDatabase.DiscordUser", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.StaffAction", "StaffAction")
                        .WithMany("StaffActionPosts")
                        .HasForeignKey("StaffActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("StaffAction");
                });

            modelBuilder.Entity("BabelDatabase.StaffStaffAction", b =>
                {
                    b.HasOne("BabelDatabase.StaffAction", "StaffAction")
                        .WithMany("Staff")
                        .HasForeignKey("StaffActionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BabelDatabase.DiscordUser", "Staff")
                        .WithMany()
                        .HasForeignKey("StaffId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Staff");

                    b.Navigation("StaffAction");
                });

            modelBuilder.Entity("BabelDatabase.Character", b =>
                {
                    b.Navigation("Cliques");
                });

            modelBuilder.Entity("BabelDatabase.Clique", b =>
                {
                    b.Navigation("CliqueMembers");

                    b.Navigation("CliqueOfficers");
                });

            modelBuilder.Entity("BabelDatabase.Committee", b =>
                {
                    b.Navigation("CommitteeMembers");
                });

            modelBuilder.Entity("BabelDatabase.DiscordUser", b =>
                {
                    b.Navigation("Characters");
                });

            modelBuilder.Entity("BabelDatabase.PopsimGlobalEthicGroup", b =>
                {
                    b.Navigation("PlanetaryEthicGroups");
                });

            modelBuilder.Entity("BabelDatabase.StaffAction", b =>
                {
                    b.Navigation("Players");

                    b.Navigation("Staff");

                    b.Navigation("StaffActionPosts");
                });
#pragma warning restore 612, 618
        }
    }
}
