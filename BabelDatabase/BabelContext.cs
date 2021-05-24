using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BabelDatabase
{
	public class BabelContext : DbContext
	{
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlServer("Server=192.168.178.34;Database=wopr;User ID=SA;Password=gstenaTI22!!;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;");
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
		public DbSet<Year> Year { get; set; }
		public DbSet<TimeToMidnight> TimeToMidnight { get; set; }

		//public DbSet<PopsimReport> PopsimReports { get; set; }

		//public DbSet<PopsimGlobalEthicGroup> PopsimGlobalEthicGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			//modelBuilder.Entity<DiscordUser>().HasData(new DiscordUser()
			//{
			//	DiscordUserId = "75968535074967552"
			//});

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
				}
			);

			modelBuilder.Entity<TimeToMidnight>().HasData(
				new TimeToMidnight()
			);

			modelBuilder.Entity<DiscordUser>()
				.HasMany(du => du.Characters)
				.WithOne(c => c.DiscordUser)
				.HasForeignKey(c => c.DiscordUserId);

			modelBuilder.Entity<Character>()
				.HasOne(c => c.Species)
				.WithMany()
				.HasForeignKey(c => c.SpeciesId);

			modelBuilder.Entity<Character>()
				.HasMany(c => c.Cliques)
				.WithMany(c => c.CliqueMembers);

			modelBuilder.Entity<Character>()
				.HasMany(c => c.Cliques)
				.WithMany(c => c.CliqueOfficers);

			modelBuilder.Entity<StaffAction>()
				.HasMany(sa => sa.Players);

			modelBuilder.Entity<StaffAction>()
				.HasMany(sa => sa.Staff);

			modelBuilder.Entity<StaffAction>()
				.HasMany(sa => sa.StaffActionPosts)
				.WithOne(sap => sap.StaffAction)
				.HasForeignKey(sap => sap.StaffActionId);

			modelBuilder.Entity<StaffAction>()
				.HasOne(sa => sa.Owner)
				.WithMany()
				.HasForeignKey(sa => sa.OwnerId);

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
		}
	}
}
