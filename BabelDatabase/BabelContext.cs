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
			//optionsBuilder.UseSqlServer("");
			optionsBuilder
			.UseLazyLoadingProxies()
			.UseInMemoryDatabase("test");
		}

		public DbSet<Character> Characters { get; set; }
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<PopsimParty> Parties { get; set; }
		public DbSet<Species> Species { get; set; }
		public DbSet<Year> Year { get; set; }

		public DbSet<PopsimReport> PopsimReports { get; set; }

		public DbSet<PopsimGlobalEthicGroup> PopsimGlobalEthicGroups { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DiscordUser>().HasData(new DiscordUser()
			{
				DiscordUserId = "75968535074967552"
			});
		}
	}
}
