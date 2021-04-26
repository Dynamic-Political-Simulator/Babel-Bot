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

		public DbSet<Character> Characters { get; set; }
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<PopsimParty> Parties { get; set; }
		public DbSet<Species> Species { get; set; }
		public DbSet<Year> Year { get; set; }

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

			modelBuilder.Entity<DiscordUser>()
				.HasMany(du => du.Characters)
				.WithOne(c => c.DiscordUser)
				.HasForeignKey(c => c.DiscordUserId);

			modelBuilder.Entity<Character>()
				.HasOne(c => c.Species)
				.WithMany()
				.HasForeignKey(c => c.SpeciesId);
		}
	}
}
