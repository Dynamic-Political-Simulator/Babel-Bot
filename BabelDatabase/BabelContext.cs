using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace BabelDatabase
{
	public class BabelContext : DbContext
	{
		private readonly BabelDatabaseConfig _config;

		public BabelContext(BabelDatabaseConfig config)
		{
			_config = config;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			//optionsBuilder.UseSqlServer("");
			optionsBuilder
			.UseLazyLoadingProxies()
			.UseInMemoryDatabase("test");
		}

		public DbSet<Character> Characters { get; set; }
		public DbSet<DiscordUser> DiscordUsers { get; set; }
		public DbSet<Party> Parties { get; set; }
		public DbSet<Species> Species { get; set; }
		public DbSet<Year> Year { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

		}
	}
}
