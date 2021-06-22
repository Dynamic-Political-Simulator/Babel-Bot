using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using DPSSimulation.Classes;

namespace BabelDatabase
{
	public class BabelContext : DbContext
	{
		public IConfiguration Configuration { get; }

		public BabelContext(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer(Configuration.GetValue<string>("Database:ConnectionString"));
			//optionsBuilder.UseInMemoryDatabase("test");
			optionsBuilder.UseLazyLoadingProxies();			
		}

		public DbSet<Alignment> Alignments { get; set; }
		public DbSet<AlignmentSpending> AlignmentSpendings { get; set; }
		public DbSet<Character> Characters { get; set; }
		public DbSet<Clique> Cliques { get; set; }
		public DbSet<CliqueInvite> CliqueInvites { get; set; }
		public DbSet<CustomSpending> CustomSpendings { get; set; }
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<Species> Species { get; set; }
		public DbSet<StaffAction> StaffActions { get; set; }
		public DbSet<StaffActionPost> StaffActionPosts { get; set; }
		public DbSet<GameState> GameState { get; set; }

		//public DbSet<PopsimReport> PopsimReports { get; set; }

		//public DbSet<PopsimGlobalEthicGroup> PopsimGlobalEthicGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DiscordUser>().HasData(new DiscordUser()
			{
				DiscordUserId = "75968535074967552",
				UserName = "Obi",
				IsAdmin = true
			});

			modelBuilder.Entity<Species>().HasData(
				new Species
				{
					SpeciesId = "1",
					SpeciesName = "Human"
				},
				new Species
				{
					SpeciesId = "2",
					SpeciesName = "Zelvan"
				},
				new Species
				{
					SpeciesId = "3",
					SpeciesName = "Liaran"
				}
			);

			modelBuilder.Entity<GameState>().HasData(
				new GameState()
			);

			modelBuilder.Entity<DiscordUser>()
				.HasMany(du => du.Characters)
				.WithOne(c => c.DiscordUser)
				.HasForeignKey(c => c.DiscordUserId);

			modelBuilder.Entity<Character>()
				.HasOne(c => c.Species)
				.WithMany()
				.HasForeignKey(c => c.SpeciesId);

			modelBuilder.Entity<CliqueMemberCharacter>().HasKey(cmc => new { cmc.CliqueId, cmc.MemberId });

			modelBuilder.Entity<CliqueMemberCharacter>()
				.HasOne(cmc => cmc.Clique)
				.WithMany(c => c.CliqueMembers);

			modelBuilder.Entity<CliqueMemberCharacter>()
				.HasOne(cmc => cmc.Member)
				.WithMany(c => c.Cliques);

			modelBuilder.Entity<CliqueOfficerCharacter>().HasKey(cmc => new { cmc.CliqueId, cmc.OfficerId });

			modelBuilder.Entity<CliqueOfficerCharacter>()
				.HasOne(cmc => cmc.Clique)
				.WithMany(c => c.CliqueOfficers);

			modelBuilder.Entity<CliqueOfficerCharacter>()
				.HasOne(cmc => cmc.Officer)
				.WithMany();

			// Staff action stuff

			modelBuilder.Entity<PlayerStaffAction>().HasKey(psa => new { psa.PlayerId, psa.StaffActionId });

			modelBuilder.Entity<PlayerStaffAction>()
				.HasOne(psa => psa.Player)
				.WithMany()
				.HasForeignKey(psa => psa.PlayerId);

			modelBuilder.Entity<PlayerStaffAction>()
				.HasOne(psa => psa.StaffAction)
				.WithMany(sa => sa.Players)
				.HasForeignKey(psa => psa.PlayerId);

			modelBuilder.Entity<StaffStaffAction>().HasKey(ssa => new { ssa.StaffId, ssa.StaffActionId });

			modelBuilder.Entity<StaffStaffAction>()
				.HasOne(ssa => ssa.Staff)
				.WithMany()
				.HasForeignKey(ssa => ssa.StaffId);

			modelBuilder.Entity<StaffStaffAction>()
				.HasOne(ssa => ssa.StaffAction)
				.WithMany(sa => sa.Staff)
				.HasForeignKey(ssa => ssa.StaffActionId);

			modelBuilder.Entity<StaffAction>()
				.HasMany(sa => sa.StaffActionPosts)
				.WithOne(sap => sap.StaffAction)
				.HasForeignKey(sap => sap.StaffActionId);

			modelBuilder.Entity<StaffAction>()
				.HasOne(sa => sa.Owner)
				.WithMany()
				.HasForeignKey(sa => sa.OwnerId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<StaffActionPost>()
				.HasOne(sap => sap.Author)
				.WithMany()
				.HasForeignKey(sap => sap.AuthorId);

			modelBuilder.Entity<Committee>()
				.HasMany(c => c.CommitteeMembers);

			modelBuilder.Entity<Clique>()
				.HasMany(c => c.Alignments)
				.WithMany(a => a.Cliques);

			modelBuilder.Entity<CliqueInvite>()
				.HasOne(ci => ci.Clique)
				.WithMany()
				.HasForeignKey(ci => ci.CliqueId);

			modelBuilder.Entity<AlignmentSpending>()
				.HasOne(alsp => alsp.Clique)
				.WithMany()
				.HasForeignKey(alsp => alsp.CliqueId);

			modelBuilder.Entity<AlignmentSpending>()
				.HasOne(alsp => alsp.Character)
				.WithMany()
				.HasForeignKey(alsp => alsp.CharacterId);

			modelBuilder.Entity<AlignmentSpending>()
				.HasOne(alsp => alsp.Alignment)
				.WithMany()
				.HasForeignKey(alsp => alsp.AlignmentId);

			modelBuilder.Entity<AlignmentSpending>()
				.HasOne(alsp => alsp.PlanetTarget)
				.WithMany()
				.HasForeignKey(alsp => alsp.PlanetTargetId);

			modelBuilder.Entity<AlignmentSpending>()
				.HasOne(alsp => alsp.GlobalTarget)
				.WithMany()
				.HasForeignKey(alsp => alsp.GlobalTargetId);

			modelBuilder.Entity<PopsimGlobalEthicGroup>()
				.HasMany(pgeg => pgeg.PlanetaryEthicGroups)
				.WithOne(ppeg => ppeg.PopsimGlobalEthicGroup)
				.HasForeignKey(ppeg => ppeg.PopsimGlobalEthicGroupId);

			modelBuilder.Entity<PopsimPlanetEthicGroup>()
				.HasOne(ppeg => ppeg.PopsimPlanet)
				.WithMany()
				.HasForeignKey(ppeg => ppeg.PopsimPlanetId);

			modelBuilder.Entity<InfraStructureData>()
				.Property(b => b.Infrastructures)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<string, Infrastructure>>(v));
			modelBuilder.Entity<Data>()
				.Property(b => b.Stratas)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<List<Strata>>(v));
			modelBuilder.Entity<Empire>()
				.Property(b => b.NationalOutput)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<string, ulong>>(v));
			modelBuilder.Entity<Empire>()
				.Property(b => b.EconGmData)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<string, float>>(v));
			modelBuilder.Entity<Empire>()
				.Property(b => b.PopsimGmData)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment, float>>>(v));
			modelBuilder.Entity<Empire>()
				.Property(b => b.GeneralAssembly)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<Alignment, int>>(v));
			modelBuilder.Entity<Planet>()
				.Property(b => b.Output)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<string, ulong>>(v));
			modelBuilder.Entity<Planet>()
				.Property(b => b.PlanetGroups)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimPlanetEthicGroup, float>>(v));
			modelBuilder.Entity<Planet>()
				.Property(b => b.EconGmData)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<string, float>>(v));
			modelBuilder.Entity<Planet>()
				.Property(b => b.PopsimGmData)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimPlanetEthicGroup, Dictionary<Alignment,float>>>(v));
			modelBuilder.Entity<Party>()
				.Property(b => b.PopGroupEnlistment)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));
			modelBuilder.Entity<Party>()
				.Property(b => b.UpperPartyMembership)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));
			modelBuilder.Entity<Party>()
				.Property(b => b.LowerPartyMembership)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));
			modelBuilder.Entity<Party>()
				.Property(b => b.UpperPartyAffinity)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<Alignment, float>>(v));
			modelBuilder.Entity<Party>()
				.Property(b => b.LowerPartyAffinity)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<Alignment, float>>(v));
			modelBuilder.Entity<Military>()
				.Property(b => b.MilitaryGroups)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<PopsimGlobalEthicGroup, float>>(v));
			modelBuilder.Entity<Military>()
				.Property(b => b.MilitaryFactions)
				.HasConversion(
					v => JsonConvert.SerializeObject(v),
					v => JsonConvert.DeserializeObject<Dictionary<Alignment, float>>(v));
		}
	}
}
